using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{   
    [RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
    public class Cat : MonoBehaviour, ICat
    {
        public CatMood Mood {
            get
            {
                return mood;
            }
            private set
            {
                if (value != mood)
                {
                    mood = value;
                    MoodChange?.Invoke(this, new CatMoodArgs(value));
                }                
            }
        }

        public string CurrentBehaviourDescription => currentBehaviour.BehaviourDescription;

        public Vector3 Position => transform.position;

        public event EventHandler<CatMoodArgs> MoodChange;
        public event EventHandler<CatBehaviourArgs> BehaviourUpdate;

        private CatMood mood;
        private List<ICatBehaviour> behaviours = new List<ICatBehaviour>();
        private ICatBehaviour currentBehaviour;

        private NavMeshAgent navMeshAgent;
        private ICarpet[] carpets;
        private IBall ball;
        private IRoom[] rooms;        
        private IPlayer player;

        private Queue<IFood> food = new Queue<IFood>();
        
        private bool behaviourWasStarted;
        private bool behaviourWasFinished;
        private bool destinationWasReached;
        private Vector3 destinationPosition;
        private ICarpet currentCarpet;
        private System.Random random;

        private readonly float distancePlayerIsClose = 1.5f;

        [SerializeField] private Animator animator;
        [SerializeField] private AudioClip clipPurr;
        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            behaviours.Add(new CatBehaviour("Play", CatMood.Bad, BehaviourSitting, "Сидит на месте", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Play", CatMood.Good, BehaviourRunSlowlyToTheBall, "Медленно бегает за мячиком", CatMood.Great));
            behaviours.Add(new CatBehaviour("Play", CatMood.Great, BehaviourRunLikeForestGump, "Носится как угорелая", CatMood.Great));

            behaviours.Add(new CatBehaviour("Feed", CatMood.Bad, BehaviourEatAllAggressive, "все съедает, но если в это время подойти - поцарапает", CatMood.Good));
            behaviours.Add(new CatBehaviour("Feed", CatMood.Good, BehaviourEatAllQuickly, "быстро все съедает", CatMood.Great));
            behaviours.Add(new CatBehaviour("Feed", CatMood.Great, BehaviourEatAllQuickly, "быстро все съедает", CatMood.Great));

            behaviours.Add(new CatBehaviour("Stroke", CatMood.Bad, BehaviourScratch, "царапает", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Stroke", CatMood.Good, BehaviourPurr, "мурлычет", CatMood.Great));
            behaviours.Add(new CatBehaviour("Stroke", CatMood.Great, BehaviourPurrAndWagTail, "мурлычет и виляет хвостом", CatMood.Great));

            behaviours.Add(new CatBehaviour("Kick", CatMood.Bad, BehaviourJumpAndBiteRightEar, "прыгает и кусает за правое ухо", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Kick", CatMood.Good, BehaviourRunAndPiss, "убегает на ковёр и писает", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Kick", CatMood.Great, BehaviourRunAnotherRoom, "убегает в другую комнату", CatMood.Bad));

            Mood = behaviours.FirstOrDefault().MoodCondition;
            currentBehaviour = behaviours.First();

            random = new System.Random();
        }

        private void Start()
        {
            navMeshAgent = transform.GetComponent<NavMeshAgent>();

            carpets = GameObject.FindObjectsOfType<Carpet>();
            if (carpets.Length == 0) throw new Exception("<Carpet> no found in scene");

            ball = GameObject.FindObjectOfType<Ball>();
            if (ball is null) throw new Exception("<Ball> no found in scene");

            rooms = GameObject.FindObjectsOfType<Room>();
            if (rooms.Length < 2) throw new Exception("<Room> count < 2 in scene");

            player = GameObject.FindObjectOfType<Player>();
            if (player is null) throw new Exception("<Player> no found in scene");
        }

        private void Update()
        {
            if (currentBehaviour is object)
                currentBehaviour.Behaviour();
        }

        public void TakeAction(string actionName)
        {
            if (!behaviourWasFinished) return;

            try
            {
                StopAllCoroutines();

                var behaviour = behaviours.Where(i => i.Name == actionName && i.MoodCondition == Mood).Single();
                
                currentBehaviour = behaviour;
                behaviourWasStarted = false;
                SetSpeed(CatSpeed.Default);

                animator.SetBool("Eat", false);
                animator.SetBool("Walk", false);
                animator.SetBool("Jump", false);
                animator.SetBool("Meow", false);
                animator.SetBool("Sit", false);
                animator.SetBool("Tail", false);
                animator.SetBool("Scratch", false);
                animator.speed = 1;

                audioSource.Stop();

                Mood = behaviour.MoodResult;

                BehaviourUpdate?.Invoke(this, new CatBehaviourArgs(behaviour.BehaviourDescription));
            }
            catch (Exception)
            {
                Debug.Log($"action: \"{actionName}\" and MoodCondition: \"{Mood}\" not found in cat's behaviours list. Or your list of behaviours is incorrect");
                throw;
            }  
        }

        public void TakeFood(IFood[] food)
        {
            foreach (var item in food)
            {
                this.food.Enqueue(item);
            }
        }

        private void SetSpeed(CatSpeed speed)
        {
            switch (speed)
            {
                case CatSpeed.Default:
                    navMeshAgent.acceleration = 8f;
                    navMeshAgent.speed = 3.5f;
                    navMeshAgent.angularSpeed = 300f;
                    break;
                case CatSpeed.Fast:
                    navMeshAgent.acceleration = 50f;
                    navMeshAgent.speed = 25f;
                    navMeshAgent.angularSpeed = 600f;
                    break;
                case CatSpeed.SuperFast:
                    navMeshAgent.acceleration = 200f;
                    navMeshAgent.speed = 50f;
                    navMeshAgent.angularSpeed = 1000f;
                    break;
            }
        }

        private void EatFood(bool isAggressive)
        {
            if (!behaviourWasStarted)
            {
                behaviourWasFinished = false;
                behaviourWasStarted = true;
                StartCoroutine(GoAndEat());
            }

            IEnumerator GoAndEat()
            {
                while (food.Count != 0)
                {
                    var firstFood = food.Peek();
                    navMeshAgent.SetDestination(firstFood.Position);
                    animator.SetBool("Walk", true);

                    if (isAggressive && player.IsWalking &&  ((transform.position - player.Position).magnitude < 1.5f))
                    {
                        navMeshAgent.SetDestination(transform.position);
                        animator.SetBool("Walk", false);
                        yield return StartCoroutine(Scratching());
                        navMeshAgent.SetDestination(firstFood.Position);
                        animator.SetBool("Walk", true);
                    }
                    else if ((transform.position - firstFood.Position).magnitude < 1.5f)
                    {
                        animator.SetBool("Walk", false);
                        animator.SetBool("Eat", true);

                        yield return StartCoroutine(Eating());
                        firstFood.EatMe();
                        food.Dequeue();

                        animator.SetBool("Eat", false);
                    }
                    else
                    {
                        yield return null;
                    }
                }

                if (food.Count == 0)
                {
                    behaviourWasFinished = true;
                }                
            }

            IEnumerator Eating()
            {
                var time = Time.realtimeSinceStartup;

                while (Time.realtimeSinceStartup - time < 3f)
                {
                    if (isAggressive && ((transform.position - player.Position).magnitude < 1.5f))
                    {
                        yield return StartCoroutine(Scratching());
                    }
                        
                    yield return null;
                }                
            }

            IEnumerator Scratching()
            {           
                var eatBefore = animator.GetBool("Eat");
                
                animator.SetBool("Eat", false);
                animator.SetBool("Scratch", true);

                yield return StartCoroutine(WaitAndRotate(3f));

                animator.SetBool("Scratch", false);
                animator.SetBool("Eat", eatBefore);
            }

            IEnumerator WaitAndRotate(float duration)
            {
                var time = Time.realtimeSinceStartup;

                while (Time.realtimeSinceStartup - time < duration)
                {
                    navMeshAgent.updateRotation = false;

                    var targetRotation = Quaternion.LookRotation(player.Position - transform.position, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3f);

                    yield return null;                    
                }

                navMeshAgent.updateRotation = true;
            }
        }

        #region Behaviours
        private void BehaviourSitting()
        {
            behaviourWasFinished = true;
            animator.SetBool("Sit", true);
        }

        private void BehaviourRunSlowlyToTheBall()
        {
            behaviourWasFinished = true;

            animator.SetBool("Walk", true);
            navMeshAgent.SetDestination(ball.Position);
        }

        private void BehaviourRunLikeForestGump()
        {
            behaviourWasFinished = true;
            SetSpeed(CatSpeed.SuperFast);

            if (!behaviourWasStarted)
            {
                animator.SetBool("Walk", true);
                animator.speed = 3;
                StartCoroutine(Running());
                behaviourWasStarted = true;
            }            

            IEnumerator Running()
            {
                while (true)
                {
                    float walkDistance = 10f;
                    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkDistance;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, walkDistance, NavMesh.AllAreas);
                    Vector3 finalPosition = hit.position;
                    
                    navMeshAgent.SetDestination(finalPosition);

                    yield return new WaitForSeconds(0.3f);
                }                
            }
        }

        private void BehaviourEatAllAggressive()
        {
            SetSpeed(CatSpeed.Default);            
            EatFood(true);
        }

        private void BehaviourEatAllQuickly()
        {
            SetSpeed(CatSpeed.Fast);
            animator.speed = 3f;
            EatFood(false);
        }

        private void BehaviourScratch()
        {
            behaviourWasFinished = true;

            navMeshAgent.updateRotation = false;
            var targetRotation = Quaternion.LookRotation(player.Position - transform.position, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3f);
            animator.SetBool("Scratch", true);            
            navMeshAgent.updateRotation = true;
        }

        private void BehaviourPurr()
        {
            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                behaviourWasFinished = true;
                audioSource.clip = clipPurr;
                audioSource.Play();
            }
        }

        private void BehaviourPurrAndWagTail()
        {
            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                behaviourWasFinished = true;
                audioSource.clip = clipPurr;
                audioSource.Play();

                animator.SetBool("Tail", true);
                SetSpeed(CatSpeed.Fast);
            }
        }

        private void BehaviourJumpAndBiteRightEar()
        {
            if (!behaviourWasStarted)
            {
                behaviourWasFinished = true;
                behaviourWasStarted = true;
                animator.SetBool("Jump", true);

                if ((transform.position - player.Position).magnitude >= 1.5f)
                {
                    navMeshAgent.updateRotation = true;
                    navMeshAgent.SetDestination(player.Position);
                    animator.SetBool("Jump", true);
                }       
                else
                {
                    animator.Play("Base Layer.Jump", 0, 0.25f);
                }
            }           
        }

        private void BehaviourRunAndPiss()
        {
            behaviourWasFinished = true;            

            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                destinationWasReached = false;

                currentCarpet = (carpets.Count() > 0) ? carpets.ElementAt(random.Next(carpets.Count())) : null;
                if (currentCarpet is object)
                {
                    animator.SetBool("Walk", true);
                    SetSpeed(CatSpeed.Fast);

                    destinationPosition = currentCarpet.Position;
                    navMeshAgent.SetDestination(destinationPosition);
                }
                else
                {
                    destinationWasReached = true;
                }
            }

            if (behaviourWasStarted && !destinationWasReached)
            {
                if ((destinationPosition - transform.position).magnitude < 1.5f) // TODO: 1f magic
                {
                    destinationWasReached = true;
                    animator.SetBool("Walk", false);

                    if (currentCarpet is object)
                    {
                        currentCarpet.PissOnMe();
                    }
                }
            }
        }

        private void BehaviourRunAnotherRoom()
        {
            behaviourWasFinished = true;
            if (!behaviourWasStarted)
            {                
                behaviourWasStarted = true;
                destinationWasReached = false;

                NavMeshHit navMeshHit;
                navMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0f, out navMeshHit);

                var anotherRoom = rooms.Where(i => i.NavMeshAreaMask != navMeshHit.mask).FirstOrDefault();

                if (anotherRoom is object)
                {
                    animator.SetBool("Walk", true);
                    SetSpeed(CatSpeed.Fast);

                    destinationPosition = anotherRoom.Center;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(destinationPosition, out hit, 1f, anotherRoom.NavMeshAreaMask);

                    navMeshAgent.SetDestination(hit.position);
                }
            }

            if (behaviourWasStarted && !destinationWasReached)
            {
                if ((destinationPosition - transform.position).magnitude <  1.5f) // TODO: 1f magic
                {
                    destinationWasReached = true;
                    animator.SetBool("Walk", false);
                }
            }
        }
        #endregion
    }
}