namespace Plus.CatSimulator
{
    public class CatActionPurrAndWagTail : AbstractCatAction
    {
        private bool behaviourWasStarted;

        public CatActionPurrAndWagTail(CatParams catParams) : base(catParams)
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
                behaviourWasStarted = true;
                BehaviourWasFinished = true;
                audioSource.clip = clipPurr;
                audioSource.Play();

                animator.SetBool("Tail", true);
                SetSpeed(CatSpeed.Fast);
            }
        }
    }
}