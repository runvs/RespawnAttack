using SFML.Graphics;
using System;
using SFML.Window;
using SFMLCollision;
using JamUtilities;

namespace JamTemplate
{
	class World
	{

		#region Fields

		private System.Collections.Generic.List<Tile> _tileList;
		private System.Collections.Generic.List<Shot> _shotList;
		private System.Collections.Generic.List<Bomb> _bombList;
		private System.Collections.Generic.List<Explosion> _explosionList;

		public Player _player;

		private Enemy _enemy;

		private Background _background;

		public float TotalTime { get; private set; }
		public int NumberOfHits { get; private set; }
		public int NumberOfKills { get; private set; }
		#endregion Fields

		#region Methods

		public World()
		{
			_background = new Background();
			InitGame();
		}

		public void GetInput()
		{
			_player.GetInput();
		}

		public void Update(float deltaT)
		{

			TotalTime += deltaT;
			_background.Update(deltaT);
			ScreenEffects.Update(deltaT);

			foreach (var t in _tileList)
			{
				t.Update(deltaT);
			}


			System.Collections.Generic.List<Shot> newShotList = new System.Collections.Generic.List<Shot>();

			foreach (var s in _shotList)
			{
				s.Update(deltaT);

				if (!_enemy.IsDying)
				{
					if (SFMLCollision.Collision.BoundingBoxTest(s.GetSprite(), _enemy.GetSprite()))
					{
						_explosionList.Add(
							new Explosion(this, s.Position, GameProperties.ExplosionPlayerRange, GameProperties.ExplosionPlayerTotalTime, false)
							);
						s.IsAlive = false;
						NumberOfHits += 1;
						_enemy.TakeDamage();
					}
				}

				if (s.IsAlive)
				{
					newShotList.Add(s);
				}

			}
			_shotList = newShotList;

			System.Collections.Generic.List<Bomb> newBombList = new System.Collections.Generic.List<Bomb>();
			foreach (var b in _bombList)
			{
				b.Update(deltaT);
				if (b.Position.Y >= 550 - b._sprite.Sprite.GetGlobalBounds().Height)
				{
					b.IsAlive = false;
					Vector2f explosionPosition = new Vector2f(b.Position.X +b._sprite.Sprite.GetGlobalBounds().Width/2.0f, b.Position.Y + b._sprite.Sprite.GetGlobalBounds().Height/2.0f);
					Explosion expl = new Explosion(this, explosionPosition , GameProperties.ExplosionEnemyRange, GameProperties.ExplosionEnemyTotalTime);
					_explosionList.Add(expl);
					b.Explode();
					ScreenEffects.ShakeScreen (0.75f, 0.001f, 3, ShakeDirection.UpDown);

					

					foreach (var t in _tileList)
					{

						float tilePositionX = t.TilePosition.X * GameProperties.TileSizeInPixels;
						float distanceTileToExplosion = Math.Abs(tilePositionX - explosionPosition.X);


						if (distanceTileToExplosion <= 150.0f)
						{
							t._sprite.Shake(0.5f, 0.0025f, 10.0f * (float)Math.Pow((1 - distanceTileToExplosion/150.0f),2.0), ShakeDirection.UpDown);
							ParticleManager.SpawnSmokeCloud(new Vector2f(t.TilePosition.X, t.TilePosition.Y - 0.25f) * GameProperties.TileSizeInPixels, GameProperties.TileSizeInPixels / 2.5f, 7, GameProperties.Color9, 2.6f);
							if (distanceTileToExplosion <= 75.0f)
							{
								ParticleManager.SpawnSmokeCloud(new Vector2f(t.TilePosition.X, t.TilePosition.Y - 0.25f) * GameProperties.TileSizeInPixels, GameProperties.TileSizeInPixels / 2.5f, 7, GameProperties.Color8, 3.0f);
								ParticleManager.SpawnSmokeCloud(new Vector2f(t.TilePosition.X, t.TilePosition.Y - 0.25f) * GameProperties.TileSizeInPixels, GameProperties.TileSizeInPixels / 2.5f, 7, GameProperties.Color8, 3.5f);
							}
							
						}
					}


				}
				if (b.IsAlive)
				{
					newBombList.Add(b);
				}
			}
			_bombList = newBombList;

			_enemy.Update(deltaT);

			if (_enemy.IsDead)
			{
				NumberOfKills++;
				RespawnEnemy();
			}

			_player.Update(deltaT);

			if (_player._godModeTimer >= 0.0f)  // player Is Respawning
			{
				if (Collision.BoundingBoxTest(_enemy.GetSprite(), _player._sprite.Sprite))
				{
					NumberOfHits += 1;
					while (!_enemy.IsDying)
					{
						_enemy.TakeDamage();
					}
				}

			}
				


			System.Collections.Generic.List<Explosion> newExplosionList = new System.Collections.Generic.List<Explosion>();
			foreach (var e in _explosionList)
			{
				e.Update(deltaT);

				if (e.IsAlive)
				{
					newExplosionList.Add(e);
					CheckIfPlayerIsInExplosion(e);
				}

			}
			_explosionList = newExplosionList;

			

			ParticleManager.Update(deltaT);

		}

		private void CheckIfPlayerIsInExplosion(Explosion e)
		{
			if (e.CanHitPlayer)
			{
				if (e.Position.Y >= 450)
				{
					float explosionX = e.Position.X;
					float playerX = _player.Position.X + _player._sprite.Sprite.GetGlobalBounds().Width / 2.0f;

					if (Math.Abs(explosionX - playerX) <= e._explostionTotalRange * e._explosionRadius)
					{
						_player.Die();
					}
				}
			}
		}

		private void RespawnEnemy()
		{
			_enemy = new Enemy(this, new Vector2f(100 + JamUtilities.RandomGenerator.Random.Next(0,601), 100));
		}

		public void Draw(RenderWindow rw)
		{

			//rw.Clear(GameProperties.Color7);
			_background.Draw(rw);

			foreach (var t in _tileList)
			{
				t.Draw(rw);
			}
			foreach (var s in _shotList)
			{
				s.Draw(rw);
			}
			foreach (var b in _bombList)
			{
				b.Draw(rw);
			}

			_enemy.Draw(rw);

			_player.Draw(rw);

			ParticleManager.Draw(rw);

			foreach (var e in _explosionList)
			{
				e.Draw(rw);
			}
			ScreenEffects.Draw(rw);

			_player.DrawScore(rw);

		}

		private void InitGame()
		{
			TotalTime = 0.0f;
			NumberOfHits = 0;
			_shotList = new System.Collections.Generic.List<Shot>();
			_bombList = new System.Collections.Generic.List<Bomb>();
			_explosionList = new System.Collections.Generic.List<Explosion>();
			_tileList = new System.Collections.Generic.List<Tile>();
			CreateWorld();
            ParticleManager.ResetParticleSystem();

			NumberOfKills = 0;
			_player = new Player(this, 0);
		}

		public void AddShot (Shot shot)
		{
			_shotList.Add(shot);
		}

		private void CreateWorld()
		{
			for (uint i = 0; i != 32; i++)
			{
				Tile tile = new Tile(new Vector2u(i, 11));
				_tileList.Add(tile);
			}

			RespawnEnemy();
		}

		public System.Collections.Generic.List<Tile> GetTileList ()
		{
			return _tileList;
		}


		internal void EnemyKilled()
		{
			_player.AddKill();
		}

		internal void AddBomb(Bomb newBomb)
		{
			_bombList.Add(newBomb);
		}

		internal void AddExplosion(Explosion newExplosion)
		{
			_explosionList.Add(newExplosion);
		}

		#endregion Methods


		internal Score GetStats()
		{
			Score score = new Score(this);

			return score;
		}

		
	}
}
