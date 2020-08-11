using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public class Ball : MonoBehaviour, IBall
    {
        public Vector3 Position => transform.position;

        private void Start()
        {
            transform.GetComponent<NavMeshAgent>().updateRotation = true;
        }
    }
}