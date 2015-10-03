/* 
* FILE : PowerUp.cs
* PROJECT : PROG3105 - Assignment #1
* PROGRAMMER : Sunny Mangat & Dylan Sawchuk
* FIRST VERSION : 2014-10-03
* DESCRIPTION : Contains the methods used to draw and update the positions of the power ups
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SET_Breakout
{
    class PowerUp
    {
        public static Texture2D tunnelTexture;
        public static Texture2D sizeUpTexture;
        public static Texture2D unstoppableTexture;
        public static Random rand = new Random ();

        public Vector2 location;
        Texture2D texture;
        public powerUpType type;

        public enum powerUpType { tunnel = 0, sizeUp = 1, unstoppable = 2 };


        /* 
        * CONSTRUCTOR : PowerUp
        * DESCRIPTION : This is a construtor that will set the location and randoml generate the type of the power up
        * PARAMETERS : Vector2 location: the location of the power up
        * RETURNS : 
        */
        public PowerUp (Vector2 location)
        {
            this.location = location;

            type = (powerUpType)rand.Next (3);

            if (type == powerUpType.sizeUp)
            {
                texture = sizeUpTexture;
            }
            else if (type == powerUpType.tunnel)
            {
                texture = tunnelTexture;
            }
            else if (type == powerUpType.unstoppable)
            {
                texture = unstoppableTexture;
            }
        }

        /* 
        * FUNCTION : Draw
        * DESCRIPTION : Draw the power up
        * PARAMETERS : SpriteBatch : spriteBatch : SpriteBatch used to draw the power up
        * RETURNS : 
        */
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (texture, location, Color.White);
        }

        /* 
        * FUNCTION : Update
        * DESCRIPTION : Update the power up's position
        * PARAMETERS : 
        * RETURNS : 
        */
        public void Update ()
        {
            location.Y += 2.5f;
        }

        /* 
        * FUNCTION : getBounds
        * DESCRIPTION : gets the rectangular boundaries of the power up
        * PARAMETERS : 
        * RETURNS : Rectangle: the boundaries of the power up within the view space
        */
        public Rectangle getBounds ()
        {
            return new Rectangle ((int)location.X, (int)location.Y, texture.Width, texture.Height);
        }
    }
}
