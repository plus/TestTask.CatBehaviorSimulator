using UnityEngine;

namespace Plus.CatSimulator
{
    public class CatActionEatAll : AbstractCatAction
    {
        private enum EatStatement
        {
            Idle = 1,
            GoToFood = 2,
            GoToScratching = 3,
            Eat = 4,
            EatScratching = 5,
            Exit = 6,
        }

        private bool behaviourWasStarted;
        private bool isAggressive;

        private EatStatement statement;
        private IFood firstFood;        

        private readonly float timeScratching = 3f;
        private readonly float timeFeedEating = 3f;

        private System.Diagnostics.Stopwatch lastEatingTimeStart;
        private System.Diagnostics.Stopwatch lastScratchingTimeStart;

        public CatActionEatAll(CatParams catParams, bool isAggressive = false) : base(catParams)
        {
            this.isAggressive = isAggressive;           
        }

        public override void Reset()
        {
            behaviourWasStarted = false;
        }

        public override void Run()
        {
            if (!behaviourWasStarted)
            {
                BehaviourWasFinished = false;
                behaviourWasStarted = true;

                statement = EatStatement.Idle;

                if (isAggressive)
                {
                    SetSpeed(CatSpeed.Default);
                }
                else
                {
                    SetSpeed(CatSpeed.Fast);
                    animator.speed = 3f;
                }
            }
            
            if (behaviourWasStarted)
            {
                if (statement == EatStatement.Idle)
                {
                    if (food.Count == 0)
                    {
                        statement = EatStatement.Exit;
                    }
                    else
                    {
                        statement = EatStatement.GoToFood;

                        firstFood = food.Peek();
                        navMeshAgent.SetDestination(firstFood.Position);
                        animator.SetBool("Walk", true);                        
                    }
                }

                if (statement == EatStatement.GoToFood)
                {
                    if (isAggressive && player.IsWalking && transform.position.IsClose(player.Position, CloseType.Action))
                    {
                        navMeshAgent.SetDestination(transform.position);
                        animator.SetBool("Walk", false);

                        statement = EatStatement.GoToScratching;
                        lastScratchingTimeStart = System.Diagnostics.Stopwatch.StartNew();
                        animator.SetBool("Eat", false);
                        animator.SetBool("Scratch", true);
                    }
                    else if (transform.position.IsClose(firstFood.Position, CloseType.VeryClose))
                    {
                        animator.SetBool("Walk", false);
                        animator.SetBool("Eat", true);

                        statement = EatStatement.Eat;
                        lastEatingTimeStart = System.Diagnostics.Stopwatch.StartNew();
                    }
                }

                if (statement == EatStatement.GoToScratching)
                {
                    navMeshAgent.updateRotation = false;
                    var targetRotation = Quaternion.LookRotation(player.Position - transform.position, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3f);

                    if (lastScratchingTimeStart.ElapsedMilliseconds < timeScratching * 1000f)
                    {

                    }
                    else
                    {
                        if (!transform.position.IsClose(player.Position, CloseType.Action))
                        {
                            statement = EatStatement.GoToFood;
                            navMeshAgent.SetDestination(firstFood.Position);
                            animator.SetBool("Walk", true);
                            animator.SetBool("Scratch", false);
                            navMeshAgent.updateRotation = true;
                        }
                    }
                }

                if (statement == EatStatement.Eat)
                {
                    if (lastEatingTimeStart.ElapsedMilliseconds < timeFeedEating * 1000f)
                    {
                        if (isAggressive && player.IsWalking && transform.position.IsClose(player.Position, CloseType.Action))
                        {
                            statement = EatStatement.EatScratching;
                            lastScratchingTimeStart = System.Diagnostics.Stopwatch.StartNew();
                            animator.SetBool("Eat", false);
                            animator.SetBool("Scratch", true);
                        }
                    }
                    else
                    {
                        firstFood.EatMe();
                        food.Dequeue();
                        animator.SetBool("Eat", false);

                        statement = EatStatement.Idle;
                    }
                }

                if (statement == EatStatement.EatScratching)
                {
                    navMeshAgent.updateRotation = false;
                    var targetRotation = Quaternion.LookRotation(player.Position - transform.position, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 3f);

                    if (lastScratchingTimeStart.ElapsedMilliseconds < timeScratching * 1000f)
                    {

                    }
                    else
                    {
                        if (!transform.position.IsClose(player.Position, CloseType.Action))
                        {
                            statement = EatStatement.Eat;                            
                            lastEatingTimeStart = System.Diagnostics.Stopwatch.StartNew();

                            animator.SetBool("Scratch", false);
                            animator.SetBool("Eat", true);
                            navMeshAgent.updateRotation = true;
                        }
                    }
                }

                if (statement == EatStatement.Exit)
                {
                    BehaviourWasFinished = true;
                }
            }
        }
    }
}