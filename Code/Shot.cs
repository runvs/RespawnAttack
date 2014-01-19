using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Shot
    {
        private World _world;
        private SmartSprite _sprite;
        private static  SmartSprite _glowSprite;
        public bool IsAlive;
        public Sprite GetSprite() { return _sprite.Sprite;}

        // this is used to determine when the shot will disappear
        private float _totalTime;

        public Shot (World world, Vector2f position, Vector2f direction)
        {
            _world = world;
            Position = position;
            Direction = direction;
            try
            {
                _sprite = new SmartSprite("../GFX/shot.png");
                if (_glowSprite == null)
                {
                    Texture glowTexture;
                    uint glowsize = 24;
                    GlowSpriteCreator.CreateRadialGlow(out glowTexture, glowsize, GameProperties.Color3, 0.4f, PennerDoubleAnimation.EquationType.CubicEaseInOut);
                    _glowSprite = new SmartSprite(glowTexture);
                    _glowSprite.Sprite.Origin = new Vector2f(glowsize / 2.0f - 2.0f, glowsize / 2.0f - 2.0f);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            IsAlive = true;
            _totalTime = 0.0f;
        }

        public void Draw (RenderWindow rw)
        {
            _sprite.Position = Position;
            _glowSprite.Position = Position;
            _glowSprite.Draw(rw);
            _sprite.Draw(rw);
        }
        public void Update (float deltaT)
        {
            _totalTime += deltaT;
            _glowSprite.Alpha = (byte)(255 * (0.5+  0.5 * RandomGenerator.Random.NextDouble()));
            DoBulletMovement(deltaT);
            
            
        }

        private void DoBulletMovement(float deltaT)
        {
            Position += Direction * deltaT * GameProperties.BulletMovement;

        }

        public Vector2f Direction { get; private set; }
        public Vector2f Position { get; private set; }
    }
}
