using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Background
    {

        SmartSprite _star1;

        System.Collections.Generic.List<Vector2f> _starLayer;
        System.Collections.Generic.List<float> _starLayerAlphaFrequency;


        System.Collections.Generic.List<Vector2f> _cloudLayerPositions;
        System.Collections.Generic.List<Vector2f> _cloudLayerScale;
        System.Collections.Generic.List<Vector2f> _cloudLayerIndividualMovementFrequencies;
        CircleShape _cloudShape;
        Color _cloudColor;
        Vector2f _cloudMovementVector;



        float _totalTimePassed;

        public Background()
        {
            _star1 = new SmartSprite("../GFX/star1.png");
            _star1.GlobalOffsetFactor = 0.5f;
            _totalTimePassed = 0.0f;
            _cloudMovementVector = new Vector2f(4, 0);
            CreateBackground();

        }

        private void CreateBackground()
        {
            _starLayer = new List<Vector2f>();
            _starLayerAlphaFrequency = new List<float>();
            for (int i = 0; i <= GameProperties.BackgroundNumberOfStars; i++)
            {
                _starLayer.Add(RandomGenerator.GetRandomVector2f(new Vector2f(-0, 800.0f), new Vector2f(0, 600)));
                float alphaFrequency = (float)(JamUtilities.RandomGenerator.Random.NextDouble() + 0.5) * GameProperties.BackgroundAlphaBaseFrequency;
                _starLayerAlphaFrequency.Add(alphaFrequency);
            }

            _cloudLayerPositions = new List<Vector2f>();
            _cloudLayerScale = new List<Vector2f>();
            _cloudLayerIndividualMovementFrequencies = new List<Vector2f>();
            for (int i = 0; i <= GameProperties.BackgroundNumberOfClouds; i++)
            {
                _cloudLayerPositions.Add(RandomGenerator.GetRandomVector2f(new Vector2f(-50, 750.0f), new Vector2f(0,600)));
                _cloudLayerScale.Add(RandomGenerator.GetRandomVector2f(new Vector2f(1.0f, 1.1f), new Vector2f(0.30f, 0.35f)));

                _cloudLayerIndividualMovementFrequencies.Add(new Vector2f((float)((RandomGenerator.Random.NextDouble() + 0.5f) * GameProperties.BackgroundCloudBaseFrequency), (float)((RandomGenerator.Random.NextDouble() + 0.5f) * GameProperties.BackgroundCloudBaseFrequency)));
            }
            _cloudColor = GameProperties.Color8;
            _cloudColor.A = 125;
            
        }

        public void Update (float deltaT)
        {
            _totalTimePassed += deltaT;

            DoCloudMovement(deltaT);

        }

        private void DoCloudMovement(float deltaT)
        {
            for (int i = 0; i != _cloudLayerPositions.Count; i++ )
            {
                Vector2f IndividualVelocity = new Vector2f(1.5f * (float)Math.Sin(_totalTimePassed * _cloudLayerIndividualMovementFrequencies[i].X), 2.5f * (float)Math.Sin(_totalTimePassed * _cloudLayerIndividualMovementFrequencies[i].Y));
                _cloudLayerPositions[i] += deltaT * (_cloudMovementVector + IndividualVelocity) * GameProperties.BackgroundCloudMovementSpeed;
                if (_cloudLayerPositions[i].X >= 810)
                {
                    _cloudLayerPositions[i] = new Vector2f(- 2.0f* GameProperties.BackgroundCloudRadius, _cloudLayerPositions[i].Y);
                }
                if (_cloudLayerPositions[i].Y >= 1000)
                {
                    _cloudLayerPositions[i] = new Vector2f(_cloudLayerPositions[i].X, -350);
                }
                if (_cloudLayerPositions[i].Y <= -400)
                {
                    _cloudLayerPositions[i] = new Vector2f(_cloudLayerPositions[i].X, 900);
                }
            }
        }

        public void Draw(RenderWindow rw)
        {
            rw.Clear(GameProperties.Color9);

            ScreenEffects.DrawFadeUp(rw);
            ScreenEffects.DrawFadeRadial(rw);

            int i = 0;
            foreach (var v in _starLayer)
            {
                _star1.Position = v;
                _star1.Alpha = (byte)(200 + 50*Math.Cos(_starLayerAlphaFrequency[i] * _totalTimePassed));
                _star1.Draw(rw);
                i++;
            }

            i = 0;
            foreach (var v in _cloudLayerPositions)
            {
                _cloudShape = new CircleShape(GameProperties.BackgroundCloudRadius);
                _cloudShape.FillColor = _cloudColor;
                _cloudShape.Position = v + ScreenEffects.GlobalSpriteOffset *0.75f;
                _cloudShape.Scale = _cloudLayerScale[i];

                rw.Draw(_cloudShape);
                i++;
            }

        }



    }
}
