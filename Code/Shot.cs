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
        public bool IsAlive;
        public Sprite GetSprite() { return _sprite.Sprite;}

        public Shot (World world, Vector2f position, Vector2f direction)
        {
            _world = world;
            Position = position;
            Direction = direction;
            _sprite = new SmartSprite("../GFX/shot.png");
            IsAlive = true;

        }

        public void Draw (RenderWindow rw)
        {
            _sprite.Draw(rw);
        }
        public void Update (float deltaT)
        {
            DoBulletMovement(deltaT);
            _sprite.Position = Position;
        }

        private void DoBulletMovement(float deltaT)
        {
            Position += Direction * deltaT * GameProperties.BulletMovement;

        }

        public Vector2f Direction { get; private set; }
        public Vector2f Position { get; private set; }
    }
}
