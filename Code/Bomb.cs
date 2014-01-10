using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Bomb
    {
        private World _world;
        public SmartSprite _sprite;
        private float _timeSinceDrop;
        public Bomb (World world, Vector2f position)
        {
            _world = world;
            Position = position;
            Velocity = new Vector2f(0.0f, 0.0f);
            IsAlive = true;
            _timeSinceDrop = 0.0f;

            try
            {
                _sprite = new SmartSprite("../GFX/bomb.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Vector2f Position { get; private set; }
        public Vector2f Velocity { get; private set; }

        public void Draw (RenderWindow rw)
        {
            _sprite.Position = Position;
            _sprite.Draw(rw);
        }

        public void Update (float deltaT)
        {
            _timeSinceDrop += deltaT;
            DoBombMovement();

            Position += Velocity * deltaT;

            //Console.WriteLine(Position);

            _sprite.Update(deltaT);

        }

        private void DoBombMovement()
        {
            Velocity = new Vector2f(Velocity.X, Velocity.Y + GameProperties.GravityFactor);
            _sprite.Scale(1.0f + 0.15f*_timeSinceDrop);
        }

        public bool IsAlive { get; set; }
    }
}
