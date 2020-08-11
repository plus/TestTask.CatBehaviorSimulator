namespace Plus.CatSimulator
{
    public class CatActionRunSlowlyToTheBall : AbstractCatAction
    {
        public CatActionRunSlowlyToTheBall(CatParams catParams) : base(catParams)
        {
            BehaviourWasFinished = true;
        }

        public override void Reset()
        {

        }

        public override void Run()
        {
            animator.SetBool("Walk", true);
            navMeshAgent.SetDestination(ball.Position);
        }
    }
}