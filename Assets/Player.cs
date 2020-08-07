using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour
    {
        private Camera cachedCamera;
        private NavMeshAgent navMeshAgent;

        private void Start()
        {
            cachedCamera = Camera.main;
            navMeshAgent = transform.GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = cachedCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    navMeshAgent.SetDestination(hit.point);
                }
            }
        }

    }
}