using System;
using UnityEngine;
using UnityEngine.AI;

namespace Plus.CatSimulator
{
    public class CatActionRunLikeForestGump : AbstractCatAction
    {
        private bool behaviourWasStarted;
        private System.Diagnostics.Stopwatch lastDirectionChangeTime;

        private float timeRunLikeForestGumpChangeDirection = 0.5f;

        public CatActionRunLikeForestGump(CatParams catParams) : base(catParams)
        {
            BehaviourWasFinished = true;
        }

        public override void Reset()
        {
            behaviourWasStarted = false;
        }

        public override void Run()
        {            
            SetSpeed(CatSpeed.SuperFast);

            if (!behaviourWasStarted)
            {
                behaviourWasStarted = true;
                animator.SetBool("Walk", true);
                animator.speed = 3;

                Vector3 finalPosition;
                navMeshAgent.RandomPosition(transform.position, 10f, out finalPosition);
                navMeshAgent.SetDestination(finalPosition);
                lastDirectionChangeTime = System.Diagnostics.Stopwatch.StartNew();
            }
            else
            {
                if (lastDirectionChangeTime.ElapsedMilliseconds >= timeRunLikeForestGumpChangeDirection * 1000f)
                {
                    Vector3 finalPosition;
                    navMeshAgent.RandomPosition(transform.position, 10f, out finalPosition);
                    navMeshAgent.SetDestination(finalPosition);
                    lastDirectionChangeTime = System.Diagnostics.Stopwatch.StartNew();
                }
            }
        }
    }
}