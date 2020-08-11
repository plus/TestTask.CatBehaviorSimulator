using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    [RequireComponent(typeof(NavMeshModifierVolume))]
    public class Room : MonoBehaviour, IRoom
    {
        public Vector3 Center => transform.position;

        public int NavMeshAreaMask { get; private set; }

        private NavMeshModifierVolume modifierVolume;

        private void Awake()
        {
            modifierVolume = transform.GetComponent<NavMeshModifierVolume>();
            NavMeshAreaMask = 1 << modifierVolume.area;
        }
    }
}