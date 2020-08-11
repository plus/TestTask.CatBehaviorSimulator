using System.Linq;
using UnityEngine;

namespace Plus.CatSimulator
{
    public class CatActionRunAndPiss : AbstractCatAction
    {
        private bool behaviourWasStarted;
        private bool destinationWasReached;
        private System.Random random;
        private Vector3 destinationPosition;
        private ICarpet currentCarpet;

        public CatActionRunAndPiss(CatParams catParams) : base(catParams)
        {            
            random = new System.Random();
        }

        public override void Reset()
        {
            behaviourWasStarted = false;
            destinationWasReached = false;
            BehaviourWasFinished = false;
        }

        public override void Run()
        {            
            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                destinationWasReached = false;

                currentCarpet = (carpets.Count() > 0) ? carpets.ElementAt(random.Next(carpets.Count())) : null;
                if (currentCarpet is object)
                {
                    animator.SetBool("Walk", true);
                    SetSpeed(CatSpeed.Fast);

                    destinationPosition = currentCarpet.Position;
                    navMeshAgent.SetDestination(destinationPosition);
                }
                else
                {
                    destinationWasReached = true;
                }
            }

            if (behaviourWasStarted && !destinationWasReached)
            {

                if (transform.position.IsClose(destinationPosition, CloseType.VeryClose))
                {
                    destinationWasReached = true;
                    animator.SetBool("Walk", false);
                    navMeshAgent.SetDestination(transform.position);

                    if (currentCarpet is object)
                    {
                        currentCarpet.PissOnMe();
                        BehaviourWasFinished = true;
                    }
                }
            }
        }
    }
}