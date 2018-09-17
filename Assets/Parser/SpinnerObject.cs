using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuParser
{
    class SpinnerObject : HitObject
    {
        public float EndTime;

        public SpinnerObject(int x, int y, float startTime, float endTime)
        {
            Position = new Point(x, y);
            StartTime = startTime;
            EndTime = endTime;
            HitObjectType = HitObjectType.Spinner;
        }

        public float EndTimeInMs(float beatLength, float sliderMultiplier)
        {
            return EndTime;
        }

        public float EndTimeInBeats(float beatLength)
        {
            return EndTime / beatLength;
        }
    }
}
