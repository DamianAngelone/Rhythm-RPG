using System;

namespace OsuParser
{
    public class TimingPoint
    {
        public float Time;
        public float TimePerBeat;
        public int TimeSignature = 4;
        public bool FrenzyMode;

        public TimingPoint(float time, float timePerBeat, int timeSignature, bool frenzyMode)
        {
            Time = time;
            TimePerBeat = timePerBeat;
            TimeSignature = timeSignature;
            FrenzyMode = frenzyMode;
        }
    }
}