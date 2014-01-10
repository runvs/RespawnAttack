using SFML.Graphics;
using System;
using SFML.Window;
using SFMLCollision;

namespace JamTemplate
{
    class World
    {

        #region Fields

        private System.Collections.Generic.List<Tile> _tileList;
        private System.Collections.Generic.List<Shot> _shotList;
        private System.Collections.Generic.List<Bomb> _bombList;

        public Player _player;

        private Enemy _enemy;

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


            System.Collections.Generic.List<Shot> newShotList = new System.Collections.Generic.List<Shot>();
            foreach (var s in _shotList)
            {
                s.Update(deltaT);

                if (SFMLCollision.Collision.BoundingBoxTest(s.GetSprite(), _enemy.GetSprite()))
                {
                    s.IsAlive = false;
                    _enemy.TakeDamage();
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
                RespawnEnemy();
            }

            _player.Update(deltaT);
        }

        private void RespawnEnemy()
        {
            _enemy = new Enemy(this, new Vector2f(100, 100));
        }

        public void Draw(RenderWindow rw)
        {

            rw.Clear(GameProperties.Color7);

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

        }

        private void InitGame()
        {
            _shotList = new System.Collections.Generic.List<Shot>();
            _bombList = new System.Collections.Generic.List<Bomb>();
            _tileList = new System.Collections.Generic.List<Tile>();
            CreateWorld();

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

        #endregion Methods


    }
}
