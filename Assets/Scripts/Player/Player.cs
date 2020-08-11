using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Player : MonoBehaviour, IPlayer
    {
        private Camera cachedCamera;
        private NavMeshAgent navMeshAgent;
        private IFoodPool foodPool;
        private ICat cat;

        public Vector3 Position => transform.position;
        public bool IsWalking { get; private set; }

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
                        IsWalking = true;
                    }
                }

                if ((destinationPoint - transform.position).magnitude < 1f)
                {
                    animator.SetBool("Walk", false);
                    IsWalking = false;
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
            navMeshAgent.SetDestination(cat.Position);
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
            transform.LookAt(cat.Position, Vector3.up);
            navMeshAgent.updateRotation = true;
        }

        private bool CheckCatReached()
        {
            if ((cat.Position - transform.position).magnitude < 3f)
            {
                return true;
            }
            else
            {
                navMeshAgent.SetDestination(cat.Position);
            }

            return false;
        }

        private void SpawnSomeFood()
        {
            var foods = foodPool.GetFoods();

            foreach (var item in foods)
            {
                var randomPoint = navMeshAgent.RandomPosition(cat.Position, 5f);
                item.Position = randomPoint;
            }
            cat.TakeFood(foods);
        }
    }
}