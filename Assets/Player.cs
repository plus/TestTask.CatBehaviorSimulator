using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public static class NavMeshExtensions
    {
        public static Vector3 RandomPosition(this NavMeshAgent agent, Vector3 basePosition, float radius)
        {
            var randDirection = Random.insideUnitSphere * radius;
            randDirection += basePosition;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randDirection, out navHit, radius, -1))
            {
                return navHit.position;
            }
            else
            {
                return basePosition;
            }
            
        }
    }

    public interface IPlayer
    {
        void SpawnSomeFood();
        Vector3 Position { get; }
        void DoAction(string actionName);
    }


    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour, IPlayer
    {
        private Camera cachedCamera;
        private NavMeshAgent navMeshAgent;
        private IFoodPool foodPool;
        private ICat cat;

        public Vector3 Position => transform.position;

        private bool isGoingToCat = false;
        private string currentAction = "";

        private void Start()
        {
            cachedCamera = Camera.main;
            navMeshAgent = transform.GetComponent<NavMeshAgent>();
            foodPool = GameObject.FindObjectOfType<FoodPool>();
            cat = GameObject.FindObjectOfType<Cat>();
        }

        private void Update()
        {
            if (isGoingToCat)
            {
                CheckCatReached();
            }
            else
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

        public void SpawnSomeFood()
        {            
            var foods = foodPool.GetFoods();

            foreach (var item in foods)
            {
                var randomPoint = navMeshAgent.RandomPosition(cat.Transform.position, 5f);
                item.Transform.position = randomPoint;
            }            
            cat.TakeFood(foods);
        }

        public void DoAction(string actionName)
        {
            isGoingToCat = true;
            currentAction = actionName;
            navMeshAgent.SetDestination(cat.Transform.position);

            CheckCatReached();
        }

        private void CheckCatReached()
        {
            if ((cat.Transform.position - transform.position).magnitude < 3f)
            {
                isGoingToCat = false;
                cat.TakeAction(currentAction);
                navMeshAgent.SetDestination(transform.position);

                navMeshAgent.updateRotation = false;
                transform.LookAt(cat.Transform.position, Vector3.up);
                navMeshAgent.updateRotation = true;
            }
        }
    }
}