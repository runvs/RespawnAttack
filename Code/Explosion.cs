using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Explosion
    {
        private World _world;
        public bool CanHitPlayer { get; private set; }
        private static SmartSprite _glowSprite;

        public Explosion(World world, Vector2f position, float explosionRange, float explosionTotalTime, bool canHitPlayer = true)
        {
            _world = world;
            _explostionTotalRange = explosionRange;
            _explosionTotalTime = explosionTotalTime;
            Position = position;

            _timeSinceExplosion = 0.0f;
            IsAlive = true;
            CanHitPlayer = canHitPlayer;

            CreateCircleList();
            
            if (_glowSprite == null)
            {
                Texture glowTexture;
                uint glowsize = (uint)(GameProperties.ExplosionEnemyRange * 1.75f);
                GlowSpriteCreator.CreateGlow(out glowTexture, glowsize, GameProperties.Color2, 0.25f);
                _glowSprite = new SmartSprite(glowTexture);
                _glowSprite.Sprite.Origin = new Vector2f(glowsize / 2.0f - 2.0f, glowsize / 2.0f - 2.0f);
            }

        }

        private void CreateCircleList()
        {

            _listCircles = new List<CircleShape>();

            CircleShape circ = new CircleShape(_explostionTotalRange);
            circ.Origin = new Vector2f(_explostionTotalRange, _explostionTotalRange * 1.25f);
            circ.Position = Position;

            Color explosionColor = GameProperties.Color1;
            explosionColor.A = 100;
            circ.FillColor = explosionColor;

            _listCircles.Add(circ);

            circ = new CircleShape(_explostionTotalRange * 0.75f);
            circ.Origin = new Vector2f(_explostionTotalRange*0.75f, _explostionTotalRange * 0.75f * 1.25f);
            circ.Position = Position;

            explosionColor = GameProperties.Color2;
            explosionColor.A = 175;
            circ.FillColor = explosionColor;

            _listCircles.Add(circ);

            circ = new CircleShape(_explostionTotalRange * 0.5f);
            circ.Origin = new Vector2f(_explostionTotalRange * 0.5f, _explostionTotalRange * 0.5f * 1.25f);
            circ.Position = Position;

            explosionColor = GameProperties.Color9;
            explosionColor.A = 200;
            circ.FillColor = explosionColor;

            _listCircles.Add(circ);
        }

        System.Collections.Generic.List<CircleShape> _listCircles;
        private float _timeSinceExplosion;
        public float _explostionTotalRange;
        private float _explosionTotalTime;

        public void Draw (RenderWindow rw)
        {
            if (CanHitPlayer)
            {
                _glowSprite.Position = Position;
                _glowSprite.Scale(_scalingOffset);
                _glowSprite.Draw(rw);
            }
            foreach (var c in _listCircles)
            {
                rw.Draw(c);
            }
        }

        public float _explosionRadius;
        private float _scalingOffset;


        public void Update (float deltaT)
        {
            _timeSinceExplosion += deltaT;
            if (_timeSinceExplosion >= _explosionTotalTime * 1.25f)
            {
                IsAlive = false;
            }
            foreach (var c in _listCircles)
            {
                float x = _timeSinceExplosion / _explosionTotalTime * 1.5f;
                _scalingOffset = 1.0f +  1.05f - (x - 0.475f) * (x - 0.475f);
                _explosionRadius =  _scalingOffset;
                c.Scale = new Vector2f(_explosionRadius, _explosionRadius);
            }
        }


        public Vector2f Position { get; set; }

        public bool IsAlive { get; set; }
    }
}
