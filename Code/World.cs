using SFML.Graphics;
using System;
using SFML.Window;

namespace JamTemplate
{
    class World
    {

        #region Fields

        private System.Collections.Generic.List<Tile> _tileList;
        private Player _player;

        #endregion Fields

        #region Methods

        public World()
        {
            InitGame();
        }

        public void GetInput()
        {
            _player.GetInput();
        }

        public void Update(float deltaT)
        {
            foreach (var t in _tileList)
            {
                t.Update(deltaT);
            }
            _player.Update(deltaT);
        }

        public void Draw(RenderWindow rw)
        {
            foreach (var t in _tileList)
            {
                t.Draw(rw);
            }

            _player.Draw(rw);

        }

        private void InitGame()
        {
            _tileList = new System.Collections.Generic.List<Tile>();
            CreateWorld();

            _player = new Player(this, 0);
            
        }

        private void CreateWorld()
        {
            for (uint i = 0; i != 32; i++)
            {
                Tile tile = new Tile(new Vector2u(i, 11));
                _tileList.Add(tile);
            }
        }
        public System.Collections.Generic.List<Tile> GetTileList ()
        {
            return _tileList;
        }

        #endregion Methods

    }
}
