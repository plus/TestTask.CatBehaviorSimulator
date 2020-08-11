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
        
        [SerializeField] private Animator animator;

        private bool isGoingToCat = false;
        private string currentAction = "";
        private Vector3 destinationPoint;

        private void Start()
        {
            cachedCamera = Camera.main;
            navMeshAgent = transform.GetComponent<NavMeshAgent>();
            foodPool = GameObject.FindObjectOfType<FoodPool>();
            cat = GameObject.FindObjectOfType<Cat>();

            destinationPoint = transform.position;
        }

        private void Update()
        {
            if (!isGoingToCat)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var ray = cachedCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        destinationPoint = hit.point;
                        navMeshAgent.SetDestination(destinationPoint);

                        animator.SetBool("Walk", true);
                    }
                }

                if ((destinationPoint - transform.position).magnitude < 1f)
                {
                    animator.SetBool("Walk", false);
                }
            }
        }

        public void DoAction(string actionName)
        {
            StartCoroutine(GoToCatAndDoAction(actionName));
        }

        IEnumerator GoToCatAndDoAction(string actionName)
        {
            isGoingToCat = true;
            currentAction = actionName;
            navMeshAgent.SetDestination(cat.Transform.position);
            animator.SetBool("Walk", true);

            while (!CheckCatReached())
            {
                yield return null;
            }

            if (actionName == "Feed")
            {
                SpawnSomeFood();
            }

            cat.TakeAction(currentAction);
            isGoingToCat = false;
            navMeshAgent.SetDestination(transform.position);
            animator.SetBool("Walk", false);

            navMeshAgent.updateRotation = false;
            transform.LookAt(cat.Transform.position, Vector3.up);
            navMeshAgent.updateRotation = true;
        }

        private bool CheckCatReached()
        {
            if ((cat.Transform.position - transform.position).magnitude < 3f)
            {
                return true;
            }
            else
            {
                navMeshAgent.SetDestination(cat.Transform.position);
            }

            return false;
        }

        private void SpawnSomeFood()
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