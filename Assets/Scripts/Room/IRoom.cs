using UnityEngine;

namespace Plus.CatSimulator
{
    public interface IRoom
    {
        Vector3 Center { get; }
        int NavMeshAreaMask { get; }
    }
}