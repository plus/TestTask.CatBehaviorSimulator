using System;

namespace Plus.CatSimulator
{
    public class CatBehaviourArgs : EventArgs
    {
        public string BehaviourDescription { get; private set; }

        public CatBehaviourArgs(string behaviourDescription)
        {
            BehaviourDescription = behaviourDescription;
        }
    }
}