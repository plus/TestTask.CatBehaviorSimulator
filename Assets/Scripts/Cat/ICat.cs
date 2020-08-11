using System;
using UnityEngine;

namespace Plus.CatSimulator
{
    public interface ICat
    {
        CatMood Mood { get; }
        string CurrentBehaviourDescription { get; }
        Vector3 Position { get; }

        event EventHandler<CatMoodArgs> MoodChange;
        event EventHandler<CatBehaviourArgs> BehaviourUpdate;

        void TakeAction(string actionName);
        void TakeFood(IFood[] food);
    }
}