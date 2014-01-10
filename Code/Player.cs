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
        private SmartSprite _sprite;

        Dictionary<Keyboard.Key, Action> _actionMap;
        private float movementTimer = 0.0f; // time between two successive movement commands
        private World _world;
        private bool _noCollision;

        public Vector2f Position { get; private set; }
        public Vector2f Velocity { get; private set; }

        #endregion Fields

        #region Methods

        public Player(World world, int number)
        {
            _world = world;
            playerNumber = number;
            
            SetupActionMap();

            Velocity = new Vector2f(0.0f, 100.0f);
            Position = new Vector2f(0.0f, 0.0f);

            

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

            Console.WriteLine(Velocity);
        }

        private void DoPlayerMovement(float deltaT)
        {
            if (_noCollision)
            {
                Velocity = new Vector2f(Velocity.X, Velocity.Y + GameProperties.GravityFactor);
            }
            Velocity *= GameProperties.VelocityDampingFactor;

            Vector2f movementVector = deltaT * Velocity;
            Vector2f newPos = Position + movementVector;

            _noCollision = true;
            foreach (var t in _world.GetTileList())
            {
                if (Collision.BoundingBoxTest(_sprite.Sprite, t._sprite.Sprite))
                {
                    Console.WriteLine("Collision");
                    Velocity = new Vector2f(Velocity.X, 0);
                    newPos.Y -= movementVector.Y;
                    _noCollision = false;

                }
            }

            
            Position = newPos;
        }

        private void MoveRightAction ()
        {
            Velocity = new Vector2f(Velocity.X + GameProperties.VelocityMovementAdd, Velocity.Y);
        }
        private void MoveLeftAction()
        {
            Velocity = new Vector2f(Velocity.X - GameProperties.VelocityMovementAdd, Velocity.Y);
        }

        public void Draw(SFML.Graphics.RenderWindow rw)
        {
            _sprite.Position = Position;
            _sprite.Draw(rw);
        }

        private void SetupActionMap()
        {
            _actionMap = new Dictionary<Keyboard.Key, Action>();
            // e.g. _actionMap.Add(Keyboard.Key.Escape, ResetActionMap);
            _actionMap.Add(Keyboard.Key.Left, MoveLeftAction);
            _actionMap.Add(Keyboard.Key.A, MoveLeftAction);

            _actionMap.Add(Keyboard.Key.Right, MoveRightAction);
            _actionMap.Add(Keyboard.Key.D, MoveRightAction);
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

    }
}
