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
    }


    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour, IPlayer
    {
        private Camera cachedCamera;
        private NavMeshAgent navMeshAgent;
        private IFoodPool foodPool;
        private ICat cat;

        public Vector3 Position => transform.position;

        private void Start()
        {
            cachedCamera = Camera.main;
            navMeshAgent = transform.GetComponent<NavMeshAgent>();
            foodPool = GameObject.FindObjectOfType<FoodPool>();
            cat = GameObject.FindObjectOfType<Cat>();
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
    }
}