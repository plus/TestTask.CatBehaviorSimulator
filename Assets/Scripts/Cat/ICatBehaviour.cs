using System;

namespace Plus.CatSimulator
{
    public interface ICatBehaviour
    {
        string Name { get; }
        CatMood MoodCondition { get; }
        ICatAction CatAction { get; }
        string BehaviourDescription { get; }
        CatMood MoodResult { get; }
    }
}