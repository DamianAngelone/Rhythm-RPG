namespace OsuParser
{
    public abstract class HitObject
    {
        public struct Point
        {
            public int X { get; private set; }
            public int Y { get; private set;  }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        protected Point Position;
        protected float StartTime { get; set; }
        protected HitObjectType HitObjectType { get; set; }

        public float StartTimeInMs()
        {
            return StartTime;
        }

        public float StartTimeInBeats(float msPerBeat)
        {
            return StartTime/msPerBeat;
        }

        public HitObjectType GetHitObjectType()
        {
            return HitObjectType;
        }
    }
}