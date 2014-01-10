using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamTemplate
{
    class GameProperties
    {
        public static float TileSizeInPixels { get { return 50; } }

        public static float VelocityDampingFactor { get {return 0.85f;}  }
        public static float VelocityMovementAdd { get { return 14.0f; } }
        public static float GravityFactor { get { return 45.0f; } }
    }
}
