using UnityEngine;

namespace Plus.CatSimulator
{
    public interface IPlayer
    {
        Vector3 Position { get; }
        void DoAction(string actionName);
    }
}