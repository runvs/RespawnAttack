using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Game
    {

        #region Fields

        private State _gameState;

        World _myWorld;
        Score _score;
        float _timeTilNextInput = 0.0f;
        SmartSprite _glowSprite;
        SFML.Audio.Music _gameMusic;

        #endregion Fields

        #region Methods

        public Game()
        {
            // Predefine game state to menu
            _gameState = State.Menu;

            //TODO  Default values, replace with correct ones !
            SmartSprite._scaleVector = new Vector2f(2.0f, 2.0f);
            SmartText._font = new Font("../GFX/font.ttf");
            SmartText._lineLengthInChars = 18;
            SmartText._lineSpread = 1.2f;
            //ScreenEffects._fadeColor = GameProperties.Color1;
            ScreenEffects.Init(new Vector2u(800,600));
            ParticleManager.SetPositionRect(new FloatRect(0, 0, 800, 600));

            Texture glowTexture;
            uint glowsize = 30;
            GlowSpriteCreator.CreateRadialGlow(out glowTexture, glowsize, GameProperties.Color3, 0.3f);
            _glowSprite = new SmartSprite(glowTexture);
            _glowSprite.Origin = new Vector2f(glowsize / 2.0f + 2.0f, glowsize / 2.0f + 2.0f);
            _glowSprite.Scale(5.5f, ShakeDirection.LeftRight);
            CanBeQuit = true;

            _gameMusic = new SFML.Audio.Music("../SFX/RespawnAttack_OST2.ogg");
            _gameMusic.Loop = true;
            _gameMusic.Volume = 18.0f;
            _gameMusic.Play();

        }

        public void GetInput()
        {
            if (_timeTilNextInput < 0.0f)
            {
                if (_gameState == State.Menu)
                {
                    GetInputMenu();
                }
                else if (_gameState == State.Game)
                {
                    _myWorld.GetInput();
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                    {
                        ChangeGameState(State.Menu);
                    }
                }
                else if (_gameState == State.Credits || _gameState == State.Score)
                {
                    GetInputCreditsScore();
                }
            }
        }

        private void GetInputMenu()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
            {
                StartGame();
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
            {
                ChangeGameState(State.Credits);
            }

        }

        private void GetInputCreditsScore()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) || Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                ChangeGameState(State.Menu, 1.0f);
            }
        }

        public void Update(float deltaT)
        {
            if (_timeTilNextInput >= 0.0f)
            {
                _timeTilNextInput -= deltaT;
            }
            CanBeQuit = false;
            if (_gameState == State.Game)
            {
                _myWorld.Update(deltaT);

                if (_myWorld._player.IsDeadFinal)
                {
                    _score = _myWorld.GetStats();
                    ChangeGameState(State.Score);
                }
            }
            else if (_gameState == State.Menu && this._timeTilNextInput <= 0.0f) 
            {
                CanBeQuit = true;
            }


        }

        public void Draw(RenderWindow rw)
        {
            rw.Clear();
            if (_gameState == State.Menu)
            {
                DrawMenu(rw);
            }
            else if (_gameState == State.Game)
            {
                _myWorld.Draw(rw);
            }
            else if (_gameState == State.Credits)
            {
                DrawCredits(rw);
            }
            else if (_gameState == State.Score)
            {
                _score.Draw(rw);
            }
        }

        private void DrawMenu(RenderWindow rw)
        {
            _glowSprite.Position = new Vector2f(410.0f, 175.0f);
            _glowSprite.Draw(rw);
            SmartText.DrawText("Respawn, Attack!", TextAlignment.MID, new Vector2f(400.0f, 139.0f), new Vector2f(1.25f, 1.25f), GameProperties.Color2, rw);

            SmartText.DrawText("Start [Return]", TextAlignment.MID, new Vector2f(400.0f, 250.0f), GameProperties.Color1, rw);
            SmartText.DrawText("A D ", TextAlignment.LEFT, new Vector2f(200.0f, 340.0f), GameProperties.Color2, rw);
            SmartText.DrawText("Move", TextAlignment.RIGHT, new Vector2f(600.0f, 340.0f), GameProperties.Color2, rw);
            SmartText.DrawText("Space, LMB", TextAlignment.LEFT, new Vector2f(200, 390.0f), GameProperties.Color2, rw);
            SmartText.DrawText("Shoot", TextAlignment.RIGHT, new Vector2f(600, 390.0f), GameProperties.Color2, rw);
            SmartText.DrawText("RMB", TextAlignment.LEFT, new Vector2f(200, 440.0f), GameProperties.Color2, rw);
            SmartText.DrawText("Respawn", TextAlignment.RIGHT, new Vector2f(600, 440.0f), GameProperties.Color2, rw);

            SmartText.DrawText("[C]redits", TextAlignment.LEFT, new Vector2f(30.0f, 550.0f), GameProperties.Color4, rw);

        }

        private void DrawCredits(RenderWindow rw)
        {

            SmartText.DrawText("Respawn, Attack!", TextAlignment.MID, new Vector2f(400.0f, 20.0f), new Vector2f(1.25f, 1.25f), GameProperties.Color2, rw);

            SmartText.DrawText("A Game by", TextAlignment.MID, new Vector2f(400.0f, 100.0f), 0.75f, rw);
            SmartText.DrawText("Simon Weis @Laguna999", TextAlignment.MID, new Vector2f(400.0f, 135.0f), rw);

            SmartText.DrawText("Visual Studio 2012 \t C#", TextAlignment.MID, new Vector2f(400, 170), 0.75f, rw);
            SmartText.DrawText("aseprite \t SFML.NET 2.1", TextAlignment.MID, new Vector2f(400, 200), 0.75f, rw);
            SmartText.DrawText("SFXR \t Cubase",TextAlignment.MID, new Vector2f(400, 230), 0.75f, rw);


            SmartText.DrawText("Thanks to", TextAlignment.MID, new Vector2f(400, 350), 0.75f, rw);
            SmartText.DrawText("MasterGFXer Thunraz", TextAlignment.MID, new Vector2f(400, 375), 0.75f, rw);
            SmartText.DrawText("Families & Friends for their great support", TextAlignment.MID, new Vector2f(400, 400), 0.75f, rw);

            SmartText.DrawText("Created Jan 2014", TextAlignment.MID, new Vector2f(400.0f, 500.0f), 0.75f, rw);

        }

        private void ChangeGameState(State newState, float inputdeadTime = 0.5f)
        {
            this._gameState = newState;
            _timeTilNextInput = inputdeadTime;
        }


        private void StartGame()
        {
            _myWorld = new World();
            ChangeGameState(State.Game, 0.1f);
        }


        #endregion Methods

        #region Subclasses/Enums

        private enum State
        {
            Menu,
            Game,
            Score,
            Credits
        }

        #endregion Subclasses/Enums


        public bool CanBeQuit { get; private set; }
    }
}
