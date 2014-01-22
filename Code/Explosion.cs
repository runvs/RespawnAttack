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
        private static SmartSprite _radialFlareSprite;
        private float _flareAngleOffset;

        System.Collections.Generic.List<CircleShape> _listCircles;
        private float _timeSinceExplosion;
        public float _explostionTotalRange;
        private float _explosionTotalTime;
        public float _explosionRadius;
        private float _scalingOffset;
        private float _flareScaleOffset;
        private Color _screenFlashColor;

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

            CreateGlowTexture();

            CreateRadialFlares();

            CreateSmokeClouds();

            _screenFlashColor = GameProperties.Color2;
            _screenFlashColor.A = 75;

            if (CanHitPlayer)
            {
                ScreenEffects.ScreenFlash(_screenFlashColor, 0.25f);
            }
        }

        private void CreateSmokeClouds()
        {
            if (CanHitPlayer)
            {
                for (uint i = 0; i != ParticleManager.NumberOfSmokeCloudParticles*9; i++)
                {
                    Vector2f puffPosition = Position + RandomGenerator.GetRandomVector2fSquare(_explostionTotalRange * 2.5f);
                    Vector2f puffvelocity = -1.5f * (Position - puffPosition);
                    ParticleManager.SpawnSmokePuff(puffPosition, puffvelocity, GameProperties.Color8, 7, 3.5f);
                }
            }
            
        }

        private void CreateRadialFlares()
        {
            if (_radialFlareSprite == null)
            {
                Texture flareTexture;
                uint glowsizeX = (uint)(GameProperties.ExplosionEnemyRange * 3.0f);
                uint glowsizeY = (uint)(GameProperties.ExplosionEnemyRange * 0.1f);
                GlowSpriteCreator.CreateLinearGlowInOut(out flareTexture, glowsizeX, glowsizeY, GameProperties.Color2, 0.25f, PennerDoubleAnimation.EquationType.Linear, ShakeDirection.LeftRight);
                _radialFlareSprite = new SmartSprite(flareTexture);
                _radialFlareSprite.Sprite.Origin = new Vector2f(glowsizeX/2.0f, glowsizeY / 2.0f - 2.0f);
            }
            _flareAngleOffset = (float)RandomGenerator.Random.Next(0, 90);
            //_flareScaleOffset = (float)( RandomGenerator.Random.NextDouble() + 0.5);
        }

        private void CreateGlowTexture()
        {
            if (_glowSprite == null)
            {
                Texture glowTexture;
                uint glowsize = (uint)(GameProperties.ExplosionEnemyRange * 4.35f);
                GlowSpriteCreator.CreateRadialGlow(out glowTexture, glowsize, GameProperties.Color2, 0.35f, PennerDoubleAnimation.EquationType.CubicEaseOut);
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




        public void Draw (RenderWindow rw)
        {
            foreach (var c in _listCircles)
            {
                rw.Draw(c);
            }
            if (CanHitPlayer)
            {
                for (uint i = 0; i != GameProperties.ExplosionNumberOfRadialFlares; i++)
                {
                    _radialFlareSprite.Position = Position;
                    _radialFlareSprite.Scale(_scalingOffset + _flareScaleOffset, ShakeDirection.LeftRight);
                    _radialFlareSprite.Rotation = _flareAngleOffset +  i * 360.0f / GameProperties.ExplosionNumberOfRadialFlares;
                    _radialFlareSprite.Draw(rw);
                }

                _glowSprite.Position = Position;
                _glowSprite.Scale(_scalingOffset);
                _glowSprite.Draw(rw);

                
            }

        }



        public void Update (float deltaT)
        {
            _timeSinceExplosion += deltaT;

            //_flareAngleOffset += 15.0f * deltaT;
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
