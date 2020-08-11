namespace Plus.CatSimulator
{
    public class CatActionSit : AbstractCatAction
    {
        public CatActionSit(CatParams catParams) : base(catParams)
        {
            BehaviourWasFinished = true;
        }

        public override void Reset()
        {
         
        }

        public override void Run()
        {
            animator.SetBool("Sit", true);
        }
    }
}