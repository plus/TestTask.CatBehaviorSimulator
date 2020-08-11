using System;

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
}