using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class GameProperties
    {

        // Colors

        public static Color Color1 { get { return new Color(255, 204, 51); } }
        public static Color Color2 { get { return new Color(238, 187, 57); } }
        public static Color Color3 { get { return new Color(221, 170, 62); } }
        public static Color Color4 { get { return new Color(204, 153, 68); } }
        public static Color Color5 { get { return new Color(187, 136, 74); } }
        public static Color Color6 { get { return new Color(170, 119, 79); } }
        public static Color Color7 { get { return new Color(153, 102, 85); } }
        public static Color Color8 { get { return new Color(136, 85, 91); } }
        public static Color Color9 { get { return new Color(119, 68, 96); } }
        public static Color Color10 { get { return new Color(102, 51, 102); } }


        public static float TileSizeInPixels { get { return 50; } }

        public static float VelocityDampingFactor { get {return 0.85f;}  }
        public static float PlayerAcceleration { get { return 22.0f; } }
        public static float GravityFactor { get { return 45.0f; } }

        public static Vector2f MousePosition { get; set; }

        public static float BulletMovement { get { return 250.0f; } }

        public static float PlayerShootTime { get { return 0.5f; } }


        public static float EnemyBombRange { get { return 25.0f; } }

        public static float EnemyAcceleration { get { return 12.0f; } }

        public static int PlayerDamage { get { return 10 + RandomGenerator.Random.Next(1, 6); } }
    }
}
