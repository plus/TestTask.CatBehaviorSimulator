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

        private CatMood mood;

        private List<CatBehaviour> behaviours = new List<CatBehaviour>();
        private ICatBehaviour currentBehaviour;

        public event EventHandler<CatMoodArgs> MoodChange;
        public event EventHandler<CatBehaviourArgs> BehaviourUpdate;

        private void Awake()
        {
            behaviours.Add(new CatBehaviour("Play", CatMood.Bad, () => { }, "Сидит на месте", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Play", CatMood.Good, () => { }, "Медленно бегает за мячиком", CatMood.Great));
            behaviours.Add(new CatBehaviour("Play", CatMood.Great, () => { }, "Носится как угарелая", CatMood.Great));

            behaviours.Add(new CatBehaviour("Feed", CatMood.Bad, () => { }, "все съедает, но если в это время подойти - поцарапает", CatMood.Good));
            behaviours.Add(new CatBehaviour("Feed", CatMood.Good, () => { }, "быстро все съедает", CatMood.Great));
            behaviours.Add(new CatBehaviour("Feed", CatMood.Great, () => { }, "быстро все съедает", CatMood.Great));

            behaviours.Add(new CatBehaviour("Stroke", CatMood.Bad, () => { }, "царапает", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Stroke", CatMood.Good, () => { }, "мурлычет", CatMood.Great));
            behaviours.Add(new CatBehaviour("Stroke", CatMood.Great, () => { }, "мурлычет и виляет хвостом", CatMood.Great));

            behaviours.Add(new CatBehaviour("Kick", CatMood.Bad, () => { }, "прыгает и кусает за правое ухо", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Kick", CatMood.Good, () => { }, "убегает на ковёр и писает", CatMood.Bad));
            behaviours.Add(new CatBehaviour("Kick", CatMood.Great, () => { }, "убегает в другую комнату", CatMood.Bad));

            // TODO: Default.
            Mood = behaviours.First().MoodCondition;
            currentBehaviour = behaviours.First();            
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
                Mood = behaviour.MoodResult;

                BehaviourUpdate?.Invoke(this, new CatBehaviourArgs(behaviour.BehaviourDescription));
            }
            catch (Exception)
            {
                Debug.Log($"action: \"{actionName}\" and MoodCondition: \"{Mood}\" not found in cat's behaviours list");
                throw;
            }  
        }
    }
}