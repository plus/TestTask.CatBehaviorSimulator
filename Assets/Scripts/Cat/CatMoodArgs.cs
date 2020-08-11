using System;

namespace Plus.CatSimulator
{
    public class CatMoodArgs : EventArgs
    {
        public CatMood Mood { get; private set; }

        public CatMoodArgs(CatMood mood)
        {
            Mood = mood;
        }
    }
}