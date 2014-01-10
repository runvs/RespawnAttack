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

        public Vector2f Position { get; private set; }
        public Vector2f Velocity { get; private set; }

        #endregion Fields

        #region Methods

        public Player(World world, int number)
        {
            _world = world;
            playerNumber = number;
            
            SetupActionMap();

            Velocity = new Vector2f(0.0f, 0.0f);
            Position = new Vector2f(0.0f, 500.0f);

            Points = 0;
            _scoreMultiplier = 1.0f;
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
            if (movementTimer <= 0.0f)
            {
                MapInputToActions();
            }
        }


        public void Update(float deltaT)
        {
            DoPlayerMovement(deltaT);
			_sprite.Update(deltaT);

            if (_shootTimer >= 0.0f)
            {
                _shootTimer -= deltaT;
            }
            

            //Console.WriteLine(Velocity);
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
            _sprite.Position = Position;
            _sprite.Draw(rw);

            DrawScore(rw);
        }

        private void DrawScore(RenderWindow rw)
        {
            SmartText.DrawText(Points.ToString(), TextAlignment.RIGHT, new Vector2f(750, 20), GameProperties.Color1, rw);
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

        #endregion Methods


        internal void AddKill()
        {
            Points += 100.0f * _scoreMultiplier;
            _scoreMultiplier += 0.05f;
        }

        public float Points { get; set; }

        internal void Die()
        {
            IsDead = true;
            Console.WriteLine("You Die!");
        }

        public bool IsDead { get; private set; }
    }
}
