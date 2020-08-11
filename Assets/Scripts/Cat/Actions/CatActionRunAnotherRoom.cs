using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public class CatActionRunAnotherRoom : AbstractCatAction
    {
        private bool behaviourWasStarted;
        private bool destinationWasReached;
        private System.Random random;
        private Vector3 destinationPosition;
        private ICarpet currentCarpet;

        public CatActionRunAnotherRoom(CatParams catParams) : base(catParams)
        {            
            random = new System.Random();
        }

        public override void Reset()
        {
            behaviourWasStarted = false;
            BehaviourWasFinished = false;
        }
        public override void Run()
        {
            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                destinationWasReached = false;

                NavMeshHit navMeshHit;
                navMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0f, out navMeshHit);

                var anotherRoom = rooms.Where(i => i.NavMeshAreaMask != navMeshHit.mask).FirstOrDefault();

                if (anotherRoom is object)
                {
                    animator.SetBool("Walk", true);
                    SetSpeed(CatSpeed.Fast);

                    destinationPosition = anotherRoom.Center;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(destinationPosition, out hit, 1f, anotherRoom.NavMeshAreaMask);

                    navMeshAgent.SetDestination(hit.position);
                }
            }

            if (behaviourWasStarted && !destinationWasReached)
            {

                if (transform.position.IsClose(destinationPosition, CloseType.VeryClose))
                {
                    destinationWasReached = true;
                    animator.SetBool("Walk", false);
                    BehaviourWasFinished = true;
                }
            }
        }
    }
}