using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public interface IBall
    {
        Transform Transform { get; }
    }

    public class Ball : MonoBehaviour, IBall
    {
        public Transform Transform => transform;

        private void Start()
        {
            transform.GetComponent<NavMeshAgent>().updateRotation = true;
        }
    }
}