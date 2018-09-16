using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuParser
{
    class CircleObject : HitObject
    {
        public CircleObject(int x, int y, float startTime)
        {
            Position = new Point(x, y);
            StartTime = startTime;
        }
    }
}
