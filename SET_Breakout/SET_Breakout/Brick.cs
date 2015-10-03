/* 
* FILE : Brick.cs
* PROJECT : PROG3105 - Assignment #1
* PROGRAMMER : Sunny Mangat & Dylan Sawchuk
* FIRST VERSION : 2014-10-01
* DESCRIPTION :The functions in this file are used set up the bricks for the game
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SET_Breakout
{
    class Brick
    {
        Texture2D texture;
        Rectangle location_Of_Brick;
        Color brick_Color;
        public bool broken;

        public Rectangle Location
        {
            get { return location_Of_Brick; }
        }

        /* 
        * FUNCTION : Brick
        * DESCRIPTION : This is a construtor that will set the texture, location and colour of the bricks
        * PARAMETERS : Texture2D : texture : texture of the brick
        *              Rectangle : location_Of_Brick : brick placement
        *              Color : brick_Color : colour of the brick
        * RETURNS : 
        */
        public Brick(Texture2D texture, Rectangle location_Of_Brick, Color brick_Color)
        {
            this.texture = texture;
            this.location_Of_Brick = location_Of_Brick;
            this.brick_Color = brick_Color;
            this.broken = false;
        }

        /* 
        * FUNCTION : Draw
        * DESCRIPTION : Draw the brick
        * PARAMETERS : SpriteBatch : spriteBatch : SpriteBatch used to draw the brick
        * RETURNS : 
        */
        public void Draw(SpriteBatch spriteBatch)
        {
            if(!broken)
            {
                spriteBatch.Draw(texture, location_Of_Brick, brick_Color);
            }
        }

    }
}
