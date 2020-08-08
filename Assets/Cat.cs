using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public interface ICatBehaviour
    {
        string Name { get; }
        CatMood MoodCondition { get; }
        Action Behaviour { get; }
        string BehaviourDescription { get; }
        CatMood MoodResult { get; }
    }

    public class CatBehaviour : ICatBehaviour
    {
        public string Name { get; private set; }

        public CatMood MoodCondition { get; private set; }

        public Action Behaviour { get; private set; }

        public string BehaviourDescription { get; }

        public CatMood MoodResult { get; private set; }

        public CatBehaviour(string name, CatMood moodCondition, Action behaviour, string behaviourDescription, CatMood moodResult)
        {
            Name = name;
            MoodCondition = moodCondition;
            Behaviour = behaviour;
            BehaviourDescription = behaviourDescription;
            MoodResult = moodResult;
        }
    }

    public enum CatMood
    {
        Bad = 1,
        Good = 2,
        Great = 3
    }

    public class CatMoodArgs : EventArgs
    {
        public CatMood Mood { get; private set; }

        public CatMoodArgs(CatMood mood)
        {
            Mood = mood;
        }
    }

    public class CatBehaviourArgs : EventArgs
    {
        public string BehaviourDescription { get; private set; }

        public CatBehaviourArgs(string behaviourDescription)
        {
            BehaviourDescription = behaviourDescription;
        }
    }

    public interface ICat
    {
        CatMood Mood { get; }
        string CurrentBehaviourDescription { get; }

        event EventHandler<CatMoodArgs> MoodChange;
        event EventHandler<CatBehaviourArgs> BehaviourUpdate;

        void TakeAction(string actionName);
    }

    public enum CatSpeed
    {
        Default = 0,
        Fast = 1,
        SuperFast = 2
    }

    [RequireComponent(typeof(NavMeshAgent))]
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
        public event EventHandler<CatMoodArgs> MoodChange;
        public event EventHandler<CatBehaviourArgs> BehaviourUpdate;

        private CatMood mood;
        private List<CatBehaviour> behaviours = new List<CatBehaviour>();
        private ICatBehaviour currentBehaviour;

        private NavMeshAgent navMeshAgent;
        private ICarpet[] carpets;
        private IBall ball;
        private IRoom[] rooms;        

        private bool behaviourWasStarted;

        private void Awake()
        {
            // TODO: move to dictionary?
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

            // TODO: Default.
            Mood = behaviours.First().MoodCondition;
            currentBehaviour = behaviours.First();            
        }

        private void Start()
        {
            navMeshAgent = transform.GetComponent<NavMeshAgent>();
            carpets = GameObject.FindObjectsOfType<Carpet>();
            ball = GameObject.FindObjectOfType<Ball>();
            rooms = GameObject.FindObjectsOfType<Room>();
        }

        private void Update()
        {
            currentBehaviour.Behaviour();
        }

        public void TakeAction(string actionName)
        {
            // Search.
            try
            {
                var behaviour = behaviours.Where(i => i.Name == actionName && i.MoodCondition == Mood).Single();
                
                currentBehaviour = behaviour;
                behaviourWasStarted = false;
                SetSpeed(CatSpeed.Default);

                Mood = behaviour.MoodResult;

                BehaviourUpdate?.Invoke(this, new CatBehaviourArgs(behaviour.BehaviourDescription));
            }
            catch (Exception)
            {
                Debug.Log($"action: \"{actionName}\" and MoodCondition: \"{Mood}\" not found in cat's behaviours list");
                throw;
            }  
        }

        private void SetSpeed(CatSpeed speed)
        {
            switch (speed)
            {
                case CatSpeed.Default:
                    navMeshAgent.acceleration = 8f;
                    navMeshAgent.speed = 3.5f;
                    navMeshAgent.angularSpeed = 120f;
                    break;
                case CatSpeed.Fast:
                    navMeshAgent.acceleration = 50f;
                    navMeshAgent.speed = 25f;
                    navMeshAgent.angularSpeed = 150f;
                    break;
                case CatSpeed.SuperFast:
                    navMeshAgent.acceleration = 200f;
                    navMeshAgent.speed = 50f;
                    navMeshAgent.angularSpeed = 200f;
                    break;
            }
        }

        private void BehaviourSitting()
        {
            // TODO: Run sitting animation.
        }

        private void BehaviourRunSlowlyToTheBall()
        {
            navMeshAgent.SetDestination(ball.Transform.position);
        }

        private void BehaviourRunLikeForestGump()
        {
            SetSpeed(CatSpeed.SuperFast);

            if (!behaviourWasStarted)
            {
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
            // TODO: move to next food, eat and be aggressive.
        }

        private void BehaviourEatAllQuickly()
        {
            // TODO: move to next food and eat.
        }

        private void BehaviourScratch()
        {
            // TODO: scratch animation
        }

        private void BehaviourPurr()
        {
            // TODO: purr animation/sound.
        }

        private void BehaviourPurrAndWagTail()
        {
            // TODO: call BehaviourPurr(); and run animation of tail.
        }

        private void BehaviourJumpAndBiteRightEar()
        {
            // TODO: go to player, run jump animation?
        }

        private void BehaviourRunAndPiss()
        {
            SetSpeed(CatSpeed.Fast);
            try
            {
                navMeshAgent.SetDestination(carpets.First().Transform.position);
            }
            finally
            {

            }            
        }

        private void BehaviourRunAnotherRoom()
        {
            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                SetSpeed(CatSpeed.Fast);

                NavMeshHit navMeshHit;
                navMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0f, out navMeshHit);

                try
                {
                    var anotherRoom = rooms.Where(i => i.NavMeshAreaMask != navMeshHit.mask).First();

                    NavMeshHit hit;
                    NavMesh.SamplePosition(anotherRoom.Center, out hit, 1f, anotherRoom.NavMeshAreaMask);

                    navMeshAgent.SetDestination(hit.position);
                }
                finally
                {

                }
            }
        }
    }
}