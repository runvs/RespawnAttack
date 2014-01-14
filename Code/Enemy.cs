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

        private float _bombTimer;
        private bool _playerIsInBombRange;
        


        public Enemy (World world, Vector2f position)
        {
            Health = HealthMax = 100;
            _world = world;
            Position = position;
            IsDying = false;
            IsDead = false;
            _bombTimer = 0.0f;
            
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
            if (IsDying)
            {
                _dyingTimer -= deltaT;
                Velocity = new Vector2f(Velocity.X, Velocity.Y + GameProperties.GravityFactor);

                Explosion expl = new Explosion(_world, 
                    Position + RandomGenerator.GetRandomVector2f(
                        new Vector2f(0, _sprite.Sprite.GetGlobalBounds().Width), 
                        new Vector2f(_sprite.Sprite.GetGlobalBounds().Height/1.5f, _sprite.Sprite.GetGlobalBounds().Height)),
                    GameProperties.ExplosionPlayerRange, GameProperties.ExplosionPlayerTotalTime/3.0f, 
                    false);
                _world.AddExplosion(expl);
                if (_dyingTimer <= 0.0f)
                {
                    IsDead = true;
                    IsDying = false;
                }
            }
            else
            {
                DoEnemyMovement(deltaT);

                if (_bombTimer >= 0)
                {
                    _bombTimer -= deltaT;
                }
                else
                {
                    if (_playerIsInBombRange)
                    {
                        DropBomb();
                    }
                }

            }

            Position += deltaT * Velocity;
            _sprite.Update(deltaT);
        }

        private void DropBomb()
        {
            _bombTimer += GameProperties.EnemyBombTimer;

            Bomb newBomb = new Bomb(_world, new Vector2f(Position.X + _sprite.Sprite.GetLocalBounds().Width / 2.0f, Position.Y + _sprite.Sprite.GetLocalBounds().Width));
            _world.AddBomb(newBomb);
        }

        private void DoEnemyMovement(float deltaT)
        {
            if (!_world._player.IsDead)
            {
                float playerPosX = _world._player.Position.X - 25;  // the offset is for the fact that the enemy is larger than the pklayer but must be central over him
                float enemyPosX = Position.X;
                _playerIsInBombRange = false;
                if (playerPosX > enemyPosX + GameProperties.EnemyBombRange)
                {
                    MoveRight();
                }
                else if (playerPosX < enemyPosX - GameProperties.EnemyBombRange)
                {
                    MoveLeft();
                }
                else
                {
                    _playerIsInBombRange = true;
                }
            }
            else
            {
                _playerIsInBombRange = false;

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
            if (!IsDying && !IsDead)
            {
                _sprite.Flash(GameProperties.Color9, 0.2f);
                _sprite.Shake(0.2f, 0.015f, 2.5f);

                Health -= GameProperties.PlayerDamage;

                CheckIfDead();
            }

        }

        private void CheckIfDead()
        {
            if (!IsDying && !IsDead)
            {
                if (Health <= 0)
                {
                    IsDying = true;
                    _dyingTimer += 3.5f;
                    _world.EnemyKilled();
                }
            }
        }


        public bool IsDead { get; set; }

        public bool IsDying { get; set; }
    }
}
