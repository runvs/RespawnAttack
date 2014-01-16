using JamUtilities;
using SFML.Graphics;

namespace JamTemplate
{
    class Score
    {

        #region Fields

        #endregion Fields

        private int _playerPoints;
        private int _playerShots;
        private int _playerHits;
        private float _playerAccuracy;
        private int _survivedTime;

        #region Methods

        public Score(World world)
        {
            _survivedTime = (int)world.TotalTime;
            _playerPoints = world._player.Points;
            _playerShots = world._player.NumberOfShots;
            _playerHits = world.NumberOfHits;
        }

        public void Draw(RenderWindow rw)
        {
            SmartText.DrawText("Time: " + _survivedTime, TextAlignment.MID, new SFML.Window.Vector2f(400, 100), rw);
            SmartText.DrawText("Points: " + _playerPoints, TextAlignment.MID, new SFML.Window.Vector2f(400, 140), rw);
            SmartText.DrawText("Shots Fired: " + _playerShots, TextAlignment.MID, new SFML.Window.Vector2f(400, 180), rw);
            SmartText.DrawText("Hits: " + _playerHits, TextAlignment.MID, new SFML.Window.Vector2f(400, 240), rw);
            
        }

        #endregion Methods

    }
}
