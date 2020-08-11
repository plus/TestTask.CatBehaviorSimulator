namespace Plus.CatSimulator
{
    public class CatActionJumpAndBiteRightEar : AbstractCatAction
    {
        private bool behaviourWasStarted;

        public CatActionJumpAndBiteRightEar(CatParams catParams) : base(catParams)
        {

        }

        public override void Reset()
        {
            behaviourWasStarted = false;
        }

        public override void Run()
        {
            if (!behaviourWasStarted)
            {
                BehaviourWasFinished = true;
                behaviourWasStarted = true;
                animator.SetBool("Jump", true);

                if (!transform.position.IsClose(player.Position, CloseType.Action))
                {
                    navMeshAgent.updateRotation = true;
                    navMeshAgent.SetDestination(player.Position);
                    animator.SetBool("Jump", true);
                }
                else
                {
                    animator.Play("Base Layer.Jump", 0, 0);
                }
            }
        }
    }
}