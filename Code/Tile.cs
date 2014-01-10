using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Tile
    {
        public Tile (Vector2u position)
        {
            TilePosition = position;

            try
            {
                _sprite = new SmartSprite("../GFX/tile.png");
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Vector2u TilePosition { get; private set; }

        public void Update (float deltaT)
        {
            _sprite.Position = new Vector2f((float)(TilePosition.X * GameProperties.TileSizeInPixels), (float)(TilePosition.Y * GameProperties.TileSizeInPixels));
            _sprite.Update(deltaT);
        }

        public void Draw (RenderWindow rw)
        {
            _sprite.Draw(rw);
        }

        public SmartSprite _sprite;
    }
}
