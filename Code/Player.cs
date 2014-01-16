using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using JamUtilities;
using SFMLCollision;

namespace JamTemplate
{
	class Player
	{

		#region Fields

		public int playerNumber;
		public string PlayerName { get; private set; }
		public SmartSprite _sprite;

		Dictionary<Keyboard.Key, Action> _actionMap;
		private float movementTimer = 0.0f; // time between two successive movement commands
		private World _world;
		
		private float _shootTimer;

		private float _scoreMultiplier;
		public int Points { get; set; }

		public Vector2f Position { get; private set; }
		public Vector2f Velocity { get; private set; }


        public bool IsDying { get; private set; }
		public bool IsDead { get; private set; }
		public bool IsDeadFinal { get; private set; }
		private int _remainingLives;
		private float _godModeTimer;
        private float _dyingTimer;
        private float _timeSinceDeath;

        public int NumberOfShots { get; private set; }

		#endregion Fields

		#region Methods

		public Player(World world, int number)
		{
			_world = world;
			playerNumber = number;
			_remainingLives = 3;

			IsDead = IsDeadFinal = IsDying = false;

            NumberOfShots = 0;

            Velocity = new Vector2f(0.0f, 0.0f);
            Position = new Vector2f(0.0f, 500.0f);

            Points = 0;
            _scoreMultiplier = 1.0f;

			SetupActionMap();

			try
			{
				LoadGraphics();
			}
			catch (SFML.LoadingFailedException e)
			{
				System.Console.Out.WriteLine("Error loading player Graphics.");
				System.Console.Out.WriteLine(e.ToString());
			}
		}

		private void SetPlayerNumberDependendProperties()
		{
			PlayerName = "Player" + playerNumber.ToString();
		}

		public void GetInput()
		{
			if (!IsDead && !IsDying)
			{
				if (movementTimer <= 0.0f)
				{
					MapInputToActions();
				}
			}
			else
			{
				if (Mouse.IsButtonPressed(Mouse.Button.Left))
				{
					RespawnPlayer();
				}
			}
		}

		private void RespawnPlayer()
		{
			IsDead = false;
			_remainingLives--;
			//CheckFinalDead();
			Position = new Vector2f(GameProperties.MousePosition.X, 500);
			ScreenEffects.ScreenFlash(GameProperties.Color1, 0.5f);
			_godModeTimer += 1.5f;
		}


		public void Update(float deltaT)
		{
			DoPlayerMovement(deltaT);
			_sprite.Update(deltaT);

			if (_shootTimer >= 0.0f)
			{
				_shootTimer -= deltaT;
			}
			if (_godModeTimer >= 0.0f)
			{
				_godModeTimer -= deltaT;
			}
            if (IsDying)
            {
                _dyingTimer -= deltaT;
                _timeSinceDeath += deltaT;
                if (_dyingTimer <= 0.0f)
                {
                    IsDead = true;
                    IsDying = false;
                    Position = new Vector2f(-100, 500);
                }
                Position = new Vector2f(Position.X + 55.0f * deltaT, Position.Y  -8.0f*(_timeSinceDeath/1.5f) + 25.0f * (float)Math.Pow((_timeSinceDeath/1.5f),2.0));
            }
		}

		private void DoPlayerMovement(float deltaT)
		{

			Velocity *= GameProperties.VelocityDampingFactor;

			Vector2f movementVector = deltaT * Velocity;
			Vector2f newPos = Position + movementVector;

			Position = newPos;
		}

		private void MoveRightAction ()
		{
			Velocity = new Vector2f(Velocity.X + GameProperties.PlayerAcceleration, Velocity.Y);
		}
		private void MoveLeftAction()
		{
			Velocity = new Vector2f(Velocity.X - GameProperties.PlayerAcceleration, Velocity.Y);
		}

		private void ShootAction()
		{
			if (_shootTimer <= 0.0f)
			{
				Shoot();
			}
		}

		private void Shoot()
		{
            NumberOfShots += 1;
			Vector2f shotDirection = GameProperties.MousePosition -  Position;
			float shotLength = (float)(Math.Sqrt(shotDirection.X*shotDirection.X + shotDirection.Y*shotDirection.Y));
			shotDirection/=shotLength;

			Vector2f shotPosition = new Vector2f(Position.X + 25, Position.Y);

			Shot newShot = new Shot(_world, shotPosition, shotDirection);
			_world.AddShot(newShot);
			_shootTimer += GameProperties.PlayerShootTime;
		}

		public void Draw(SFML.Graphics.RenderWindow rw)
		{
			if (!IsDead)
			{
				_sprite.Position = Position;
				_sprite.Draw(rw);
			}
            else if (IsDying)
            {
                Color col = GameProperties.Color5;
                _sprite.Position = Position;
                
                _sprite.Draw(rw);
            }
            else if (IsDead)
            {
                Vector2f pos = GameProperties.MousePosition;
                pos.Y = 150;
                SmartText.DrawText("Click For Respawn", pos, rw);

            }
			//DrawScore(rw); // this will be done in the World class
		}

		public void DrawScore(RenderWindow rw)
		{
			SmartText.DrawText(Points.ToString(), TextAlignment.RIGHT, new Vector2f(750, 20), GameProperties.Color1, rw);
			SmartText.DrawText("Lives: " + _remainingLives.ToString(), TextAlignment.RIGHT, new Vector2f(750, 50), GameProperties.Color1, rw);
		}

		private void SetupActionMap()
		{
			_actionMap = new Dictionary<Keyboard.Key, Action>();
			// e.g. _actionMap.Add(Keyboard.Key.Escape, ResetActionMap);
			_actionMap.Add(Keyboard.Key.Left, MoveLeftAction);
			_actionMap.Add(Keyboard.Key.A, MoveLeftAction);

			_actionMap.Add(Keyboard.Key.Right, MoveRightAction);
			_actionMap.Add(Keyboard.Key.D, MoveRightAction);

			_actionMap.Add(Keyboard.Key.Space, ShootAction);
		}

		private void MapInputToActions()
		{
			foreach (var kvp in _actionMap)
			{
				if (Keyboard.IsKeyPressed(kvp.Key))
				{
					// Execute the saved callback
					kvp.Value();
				}
			}
		}

		private void LoadGraphics()
		{

			_sprite = new SmartSprite("../GFX/player.png");
		}


		internal void AddKill()
		{
			Points += (int)(100.0f * _scoreMultiplier);
			_scoreMultiplier += 0.05f;
		}

		internal void Die()
		{
			if (_godModeTimer <= 0.0f && _dyingTimer <= 0.0f)
			{
				IsDying = true;
                _sprite.Flash(GameProperties.Color5, 1.5f);
                _dyingTimer = 1.5f;
                _timeSinceDeath = 0.0f;
				CheckFinalDead();
			}

		}

		private void CheckFinalDead()
		{
			if (_remainingLives <= 0)
			{
				IsDeadFinal = true;
			}
		}

		#endregion Methods



        
    }
}
