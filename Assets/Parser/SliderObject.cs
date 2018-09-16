using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuParser
{
    class SliderObject : HitObject
    {
        public float PixelLength { get; private set; }

        public SliderObject(int x, int y, float startTime, float pixelLength)
        {
            Position = new Point(x, y);
            StartTime = startTime;
            PixelLength = pixelLength;
            
        }

        public float EndTimeInMs(float beatLength, float sliderMultiplier)
        {
            return (PixelLength / (100 * sliderMultiplier)) * beatLength;
        }

        public float EndTimeInBeats(float beatLength, float sliderMultiplier)
        {
            return PixelLength/(100*sliderMultiplier);
        }
    }
}
