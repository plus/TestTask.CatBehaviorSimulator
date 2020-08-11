namespace Plus.CatSimulator
{
    public class CatActionPurr : AbstractCatAction
    {
        private bool behaviourWasStarted;

        public CatActionPurr(CatParams catParams) : base(catParams)
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
                audioSource.clip = clipPurr;
                audioSource.Play();
            }
        }
    }
}