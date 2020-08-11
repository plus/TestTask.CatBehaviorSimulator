using System;

namespace Plus.CatSimulator
{

    public class CatBehaviour : ICatBehaviour
    {
        public string Name { get; private set; }

        public CatMood MoodCondition { get; private set; }

        public ICatAction CatAction { get; private set; }

        public string BehaviourDescription { get; }

        public CatMood MoodResult { get; private set; }

        public CatBehaviour(string name, CatMood moodCondition, ICatAction catAction, string behaviourDescription, CatMood moodResult)
        {
            Name = name;
            MoodCondition = moodCondition;
            CatAction = catAction;
            BehaviourDescription = behaviourDescription;
            MoodResult = moodResult;
        }
    }
}