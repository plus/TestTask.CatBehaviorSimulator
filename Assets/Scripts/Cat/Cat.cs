using System;
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
        
        #pragma warning disable 0649
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClip clipPurr;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private CatSpeedConfigure navMeshSpeedConfigure;
        #pragma warning restore 0649
       

        private void Start()
        {
            Initialize();
            InitializeCatBehaviours();
        }

        private void Update()
        {
            if (currentBehaviour is object)
                currentBehaviour.CatAction.Run();
        }

        public void TakeAction(string actionName)
        {
            if (!currentBehaviour.CatAction.BehaviourWasFinished) return;

            try
            {
                var behaviour = behaviours.Where(i => i.Name == actionName && i.MoodCondition == Mood).Single();
                behaviour.CatAction.Reset();

                StopAllCoroutines();
                currentBehaviour = behaviour;
                SetSpeed(CatSpeed.Default);
                SetAnimationsDefault();
                audioSource.Stop();

                Mood = behaviour.MoodResult;

                BehaviourUpdate?.Invoke(this, new CatBehaviourArgs(behaviour.BehaviourDescription));

                if (currentBehaviour is object)
                    currentBehaviour.CatAction.Run();
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

        private void Initialize()
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

        private void InitializeCatBehaviours()
        {
            var catParams = new CatParams(animator, navMeshAgent, ball, transform, player, clipPurr, audioSource, navMeshSpeedConfigure, carpets, rooms, food);

            behaviours.Add(new CatBehaviour("Play", CatMood.Bad, new CatActionSit(catParams), "Сидит на месте", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Play", CatMood.Good, new CatActionRunSlowlyToTheBall(catParams), "Медленно бегает за мячиком", CatMood.Great));
            behaviours.Add(new CatBehaviour("Play", CatMood.Great, new CatActionRunLikeForestGump(catParams), "Носится как угорелая", CatMood.Great));

            behaviours.Add(new CatBehaviour("Feed", CatMood.Bad, new CatActionEatAll(catParams, true), "Все съедает, но если в это время подойти, — поцарапает", CatMood.Good));
            behaviours.Add(new CatBehaviour("Feed", CatMood.Good, new CatActionEatAll(catParams), "Быстро все съедает", CatMood.Great));
            behaviours.Add(new CatBehaviour("Feed", CatMood.Great, new CatActionEatAll(catParams), "Быстро все съедает", CatMood.Great));

            behaviours.Add(new CatBehaviour("Stroke", CatMood.Bad, new CatActionScratch(catParams), "Царапает", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Stroke", CatMood.Good, new CatActionPurr(catParams), "Мурлычет", CatMood.Great));
            behaviours.Add(new CatBehaviour("Stroke", CatMood.Great, new CatActionPurrAndWagTail(catParams), "Мурлычет и виляет хвостом", CatMood.Great));

            behaviours.Add(new CatBehaviour("Kick", CatMood.Bad, new CatActionJumpAndBiteRightEar(catParams), "Прыгает и кусает за правое ухо", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Kick", CatMood.Good, new CatActionRunAndPiss(catParams), "Убегает на ковёр и писает", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Kick", CatMood.Great, new CatActionRunAnotherRoom(catParams), "Убегает в другую комнату", CatMood.Bad));

            currentBehaviour = behaviours.First();
            Mood = currentBehaviour.MoodCondition;
        }

        private void SetAnimationsDefault()
        {
            animator.SetBool("Eat", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Meow", false);
            animator.SetBool("Sit", false);
            animator.SetBool("Tail", false);
            animator.SetBool("Scratch", false);
            animator.speed = 1;
        }

        private void SetSpeed(CatSpeed speed)
        {
            switch (speed)
            {
                case CatSpeed.Default:
                    navMeshAgent.acceleration = navMeshSpeedConfigure.DefaultAcceleration;
                    navMeshAgent.speed = navMeshSpeedConfigure.DefaultSpeed;
                    navMeshAgent.angularSpeed = navMeshSpeedConfigure.DefaultAngularSpeed;
                    break;
                case CatSpeed.Fast:
                    navMeshAgent.acceleration = navMeshSpeedConfigure.FastAcceleration;
                    navMeshAgent.speed = navMeshSpeedConfigure.FastSpeed;
                    navMeshAgent.angularSpeed = navMeshSpeedConfigure.FastAngularSpeed;
                    break;
                case CatSpeed.SuperFast:
                    navMeshAgent.acceleration = navMeshSpeedConfigure.SuperFastAcceleration;
                    navMeshAgent.speed = navMeshSpeedConfigure.SuperFastSpeed;
                    navMeshAgent.angularSpeed = navMeshSpeedConfigure.SuperFastAngularSpeed;
                    break;
            }
        }
    }
}