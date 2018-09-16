using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsuParser
{
    [Flags]
    public enum HitObjectType
    {
        None = 0,
        Circle = (1 << 0),
        Slider = (1 << 1),
        NewCombo = (1 << 2),
        Spinner = (1 << 3)
    }

    public enum SliderType
    {
        Linear = 0,
        PSpline = 1,
        Bezier = 2,
        CSpline = 3
    }
}
