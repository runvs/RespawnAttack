using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Enemy
    {
        private World _world;
        public Vector2f Position { get; private set; }
        public Vector2f Velocity { get; private set; }

        private SmartSprite _sprite;
        private float _dyingTimer;
        public Sprite GetSprite() { return _sprite.Sprite; }

        public int Health {get; private set;}
        public int HealthMax {get; private set;}
        


        public Enemy (World world, Vector2f position)
        {
            Health = HealthMax = 100;
            _world = world;
            Position = position;
            IsDiyin = false;
            IsDead = false;
            
            try
            {
                _sprite = new SmartSprite("../GFX/enemy.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Draw (RenderWindow rw)
        {
            _sprite.Position = Position;
            _sprite.Draw(rw);
        }

        public void Update (float deltaT)
        {
            if (IsDiyin)
            {
                _dyingTimer -= deltaT;
                Velocity = new Vector2f(Velocity.X, Velocity.Y + 9.0f);
                if (_dyingTimer <= 0.0f)
                {
                    IsDead = true;
                }
            }
            else
            {
                DoEnemyMovement(deltaT);
            }

            Position += deltaT * Velocity;
            _sprite.Update(deltaT);
        }

        private void DoEnemyMovement(float deltaT)
        {
            float playerPosX = _world._player.Position.X - 25;  // the offset is for the fact that the enemy is larger than the pklayer but must be central over him
            float enemyPosX = Position.X;
            if (playerPosX > enemyPosX + GameProperties.EnemyBombRange)
            {
                MoveRight();
            }
            else if (playerPosX < enemyPosX - GameProperties.EnemyBombRange)
            {
                MoveLeft();
            }

            //Velocity = new Vector2f(Velocity.X, )

            Velocity *= GameProperties.VelocityDampingFactor;
            
        }

        private void MoveLeft()
        {
            Velocity = new Vector2f(Velocity.X - GameProperties.EnemyAcceleration, Velocity.Y);
        }

        private void MoveRight()
        {
            Velocity = new Vector2f(Velocity.X + GameProperties.EnemyAcceleration, Velocity.Y);
        }

        public void TakeDamage ()
        {
            if (!IsDiyin && !IsDead)
            {
                _sprite.Flash(GameProperties.Color9, 0.2f);
                _sprite.Shake(0.2f, 0.015f, 2.5f);

                Health -= GameProperties.PlayerDamage;

                CheckIfDead();
            }

        }

        private void CheckIfDead()
        {
            if (!IsDiyin && !IsDead)
            {
                if (Health <= 0)
                {
                    IsDiyin = true;
                    _dyingTimer += 3.5f;
                    _world.EnemyKilled();
                }
            }
        }


        public bool IsDead { get; set; }

        public bool IsDiyin { get; set; }
    }
}
