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
		private float _totalTime;

        private static Sound _explosionSound;
        private static SoundBuffer _explosionSoundBuffer;

        private static Sound _hitSound;
        private static SoundBuffer _hitSoundBuffer;

        private static Sound _bombDropSound;
        private static SoundBuffer _bombDropSoundBuffer;

        private static Sound _heliRespawnSound;
        private static SoundBuffer _heliRespawnSoundBuffer;

		public Enemy (World world, Vector2f position)
		{

            Health = HealthMax = GameProperties.EnemyBaseHealth;
			_world = world;
			Position = position;
			IsDying = false;
			IsDead = false;
			_bombTimer = 0.0f;
			_totalTime = 0.0f;
			
			try
			{
				_sprite = new SmartSprite("../GFX/enemy.png");
                LoadSounds();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

            _heliRespawnSound.Play();
		}

        private void LoadSounds()
        {
            if (_explosionSoundBuffer == null)
            {
                _explosionSoundBuffer = new SoundBuffer("../SFX/HeliExplosionSound.wav");
                _hitSoundBuffer = new SoundBuffer("../SFX/Hit.wav");
                _bombDropSoundBuffer = new SoundBuffer("../SFX/BombDrop.wav");
                _heliRespawnSoundBuffer = new SoundBuffer("../SFX/HeliRespawn.wav");
            }
            if (_explosionSound == null)
            {
                _explosionSound = new Sound(_explosionSoundBuffer);
                _explosionSound.Volume = 25.0f;

                _hitSound = new Sound(_hitSoundBuffer);
                _hitSound.Volume = 25.0f;

                _bombDropSound = new Sound(_bombDropSoundBuffer);
                _bombDropSound.Volume = 25.0f;

                _heliRespawnSound = new Sound(_heliRespawnSoundBuffer);
                _heliRespawnSound.Volume = 15.0f;
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
				Velocity = new Vector2f(Velocity.X, Velocity.Y + GameProperties.GravityFactor * 1.25f); // accellerate downwards

                //spawn some little explosions every frame
				Explosion expl = new Explosion(_world, 
					Position + RandomGenerator.GetRandomVector2f(
						new Vector2f(0, _sprite.Sprite.GetGlobalBounds().Width), 
						new Vector2f(_sprite.Sprite.GetGlobalBounds().Height/1.5f, _sprite.Sprite.GetGlobalBounds().Height)),
					GameProperties.ExplosionPlayerRange, GameProperties.ExplosionPlayerTotalTime/3.0f, 
					false);
				_world.AddExplosion(expl);

                // spawn some smoke Clouds 
                ParticleManager.SpawnSmokeCloud(Position + RandomGenerator.GetRandomVector2f(
                        new Vector2f(0, _sprite.Sprite.GetGlobalBounds().Width),
                        new Vector2f(_sprite.Sprite.GetGlobalBounds().Height / 1.5f, _sprite.Sprite.GetGlobalBounds().Height)), 10, 7.0f, GameProperties.Color8);

				if (_dyingTimer <= 0.0f)
				{
					IsDead = true;
					IsDying = false;
				}
			}
			else
			{
				_totalTime += deltaT;
				DoEnemyMovement(deltaT);

				if (_bombTimer >= 0)
				{
					_bombTimer -= deltaT * (1.0f + GameProperties.EnemyLearingFactor*(_world.NumberOfKills+1));
				}
				else
				{
					if (_playerIsInBombRange)
					{
						DropBomb();
					}
				}

			}

			Position += deltaT * Velocity * (1.0f + GameProperties.EnemyLearingFactor*(_world.NumberOfKills+1));
			_sprite.Update(deltaT);
		}

		private void DropBomb()
		{
			_bombTimer += GameProperties.EnemyBombTimer;
            _bombDropSound.Play();
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



			// Y Movement
			Position = new Vector2f(Position.X, Position.Y +  0.75f* (float)Math.Sin(_totalTime * GameProperties.EnemyYWobbleFrequency ));

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

                _hitSound.Play();

				CheckIfDead();

                ParticleManager.SpawnSmokeCloud(Position + RandomGenerator.GetRandomVector2f(
                        new Vector2f(0, _sprite.Sprite.GetGlobalBounds().Width),
                        new Vector2f(_sprite.Sprite.GetGlobalBounds().Height / 1.5f, _sprite.Sprite.GetGlobalBounds().Height)),10, 7.0f, GameProperties.Color8);
			}

		}

		private void CheckIfDead()
		{
			if (!IsDying && !IsDead)
			{
				if (Health <= 0)
				{
                    _explosionSound.Play();
					ScreenEffects.ShakeScreen (0.75f, 0.01f, 4);
					IsDying = true;
					_dyingTimer += 3.5f;
					_world.EnemyKilled();

                    Explosion expl = new Explosion(_world, Position + new Vector2f(_sprite.Sprite.GetGlobalBounds().Width / 2.0f, _sprite.Sprite.GetGlobalBounds().Height / 2.0f), GameProperties.ExplosionEnemyRange, GameProperties.ExplosionEnemyTotalTime / 2.0f);
                    _world.AddExplosion(expl);

				}
			}
		}


		public bool IsDead { get; set; }

		public bool IsDying { get; set; }
	}
}
