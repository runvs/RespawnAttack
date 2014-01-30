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
        SmartSprite _star2;
        SmartSprite _star3;

        System.Collections.Generic.List<Vector2f> _starLayerPositions;
        System.Collections.Generic.List<uint> _starLayerType;
        System.Collections.Generic.List<float> _starLayerAlphaFrequency;


        //System.Collections.Generic.List<Vector2f> _cloudLayerPositions;
        //System.Collections.Generic.List<Vector2f> _cloudLayerScale;
        System.Collections.Generic.List<Vector2f> _cloudLayerIndividualMovementFrequencies;
        //CircleShape _cloudShape;
        System.Collections.Generic.List<JamUtilities.AreatricCloud> _cloudList;
        Color _cloudColor;
        Vector2f _cloudMovementVector;



        float _totalTimePassed;

        public Background()
        {
            try
            {
                _star1 = new SmartSprite("../GFX/star1.png");
                _star2 = new SmartSprite("../GFX/star2.png");
                _star3 = new SmartSprite("../GFX/star3.png");
            }
            catch (SFML.LoadingFailedException e)
            {
                Console.WriteLine(e);
            }
            _star1.GlobalOffsetFactor = 0.5f;
            _star2.GlobalOffsetFactor = 0.5f;
            _star3.GlobalOffsetFactor = 0.5f;
            _totalTimePassed = 0.0f;
            _cloudMovementVector = new Vector2f(4, 0);
            CreateBackground();

        }

        private void CreateBackground()
        {
            _starLayerPositions = new List<Vector2f>();
            _starLayerAlphaFrequency = new List<float>();
            _starLayerType = new List<uint>();
            for (int i = 0; i <= GameProperties.BackgroundNumberOfStars; i++)
            {
                if (RandomGenerator.Random.Next(2) == 0)
                {
                    _starLayerType.Add((uint)RandomGenerator.Random.Next(2, 4));
                }
                else
                {
                    _starLayerType.Add((uint)RandomGenerator.Random.Next(1, 4));
                }
                _starLayerPositions.Add(RandomGenerator.GetRandomVector2f(new Vector2f(-0, 800.0f), new Vector2f(0, 600)));
                float alphaFrequency = (float)(JamUtilities.RandomGenerator.Random.NextDouble() + 0.5) * GameProperties.BackgroundAlphaBaseFrequency;
                _starLayerAlphaFrequency.Add(alphaFrequency);
            }

            _cloudLayerIndividualMovementFrequencies = new List<Vector2f>();

            _cloudColor = GameProperties.Color8;
            _cloudColor.A = 20;
            _cloudList = new List<AreatricCloud>();
            for (int i = 0; i <= GameProperties.BackgroundNumberOfClouds; i++)
            {
                _cloudList.Add(new AreatricCloud(RandomGenerator.GetRandomVector2f(new Vector2f(-50, 750.0f), new Vector2f(0, 600)), _cloudColor, 8, 50));
                _cloudLayerIndividualMovementFrequencies.Add(new Vector2f((float)((RandomGenerator.Random.NextDouble() + 0.5f) * GameProperties.BackgroundCloudBaseFrequency), (float)((RandomGenerator.Random.NextDouble() + 0.5f) * GameProperties.BackgroundCloudBaseFrequency)));
            }

        }

        public void Update (float deltaT)
        {
            _totalTimePassed += deltaT;

            DoCloudMovement(deltaT);

        }

        private void DoCloudMovement(float deltaT)
        {
            //for (int i = 0; i != _cloudLayerPositions.Count; i++ )
            int i = 0;
            foreach(var c in _cloudList)
            {
                Vector2f IndividualVelocity = new Vector2f(1.5f * (float)Math.Sin(_totalTimePassed * _cloudLayerIndividualMovementFrequencies[i].X), 2.5f * (float)Math.Sin(_totalTimePassed * _cloudLayerIndividualMovementFrequencies[i].Y));
                c.Position += deltaT * (_cloudMovementVector + IndividualVelocity) * GameProperties.BackgroundCloudMovementSpeed;
                if (c.Position.X >= 810)
                {
                    c.Position = new Vector2f(-2.0f * GameProperties.BackgroundCloudRadius, c.Position.Y);
                }
                if (c.Position.Y >= 1000)
                {
                    c.Position = new Vector2f(c.Position.X, -350);
                }
                if (c.Position.Y <= -400)
                {
                    c.Position = new Vector2f(c.Position.X, 900);
                }
                i++;
            }
        }

        public void Draw(RenderWindow rw)
        {
            rw.Clear(GameProperties.Color9);



            int i = 0;
            foreach (var v in _starLayerPositions)
            {
                SmartSprite spr = null;
                if (_starLayerType[i] == 1)
                {
                    spr = _star1;
                }
                else if (_starLayerType[i] == 2)
                {
                    spr = _star2;
                }
                else
                {
                    spr = _star3;
                }
                spr.Position = v;
                spr.Alpha = (byte)(200 + 50*Math.Cos(_starLayerAlphaFrequency[i] * _totalTimePassed));
                spr.Draw(rw);
                i++;
            }
            ScreenEffects.DrawFadeUp(rw);
            i = 0;
            foreach (var c in _cloudList)
            {
                //_cloudShape = new CircleShape(GameProperties.BackgroundCloudRadius);
                //_cloudShape.FillColor = _cloudColor;
                //_cloudShape.Position = v + ScreenEffects.GlobalSpriteOffset *0.75f;
                //_cloudShape.Scale = _cloudLayerScale[i];

                //rw.Draw(_cloudShape);
                //i++;
                c.Draw(rw);
            }

            
            ScreenEffects.DrawFadeRadial(rw);

        }



    }
}
