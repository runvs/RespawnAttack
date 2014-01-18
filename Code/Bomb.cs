using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Bomb
    {
        private World _world;
        public SmartSprite _sprite;
        private float _timeSinceDrop;

        private SoundBuffer _bombExplosionSoundBuffer;
        private Sound _bombExplosionSound;

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

                _bombExplosionSoundBuffer = new SoundBuffer("../SFX/BombExplosion.wav");
                _bombExplosionSound = new Sound(_bombExplosionSoundBuffer);
                _bombExplosionSound.Volume = 25.0f;

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

            Position += Velocity * deltaT * (1.0f + GameProperties.EnemyLearingFactor * (_world.NumberOfKills + 1));

            //Console.WriteLine(Position);

            _sprite.Update(deltaT);

        }

        private void DoBombMovement()
        {
            Velocity = new Vector2f(Velocity.X, Velocity.Y + GameProperties.GravityFactor * (1.0f + GameProperties.EnemyLearingFactor * (_world.NumberOfKills +1)));
            _sprite.Scale(1.0f + 0.4f*_timeSinceDrop, ShakeDirection.UpDown);
        }

        public bool IsAlive { get; set; }

        internal void Explode()
        {
            _bombExplosionSound.Play();
        }
    }
}
