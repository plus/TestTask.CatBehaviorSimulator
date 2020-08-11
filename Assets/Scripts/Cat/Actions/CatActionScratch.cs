using UnityEngine;

namespace Plus.CatSimulator
{
    public class CatActionScratch : AbstractCatAction
    {
        private bool behaviourWasStarted;
        private readonly float timeScratching = 3f;

        private System.Diagnostics.Stopwatch watch;

        public CatActionScratch(CatParams catParams) : base(catParams)
        {
            BehaviourWasFinished = true;
        }

        public override void Reset()
        {
            behaviourWasStarted = false;
        }

        public override void Run()
        {
            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                watch = System.Diagnostics.Stopwatch.StartNew();
            }

            if (behaviourWasStarted)
            {
                if (watch.ElapsedMilliseconds < timeScratching * 1000f)
                {                  
                    navMeshAgent.updateRotation = false;
                    var targetRotation = Quaternion.LookRotation(player.Position - transform.position, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3f);
                    animator.SetBool("Scratch", true);
                    navMeshAgent.updateRotation = true;
                } 
                else
                {
                    watch.Stop();
                    animator.SetBool("Scratch", false);
                }
            }
        }
    }
}