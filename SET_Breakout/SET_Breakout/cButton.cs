/* 
* FILE : cButton.cs
* PROJECT : PROG3105 - Assignment #1
* PROGRAMMER : Sunny Mangat & Dylan Sawchuk
* FIRST VERSION : 2014-10-01
* DESCRIPTION :The functions in this file are used to manage the state of menu buttons
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SET_Breakout
{
    class cButton
    {
        public Texture2D texture;
        Vector2 position;
        Rectangle rectangle;

        Color colour = new Color(255, 255, 255, 255);// colour of button

        public Vector2 size;

        /* 
        * CONSTRUCTOR : cButton
        * DESCRIPTION : Contructor that holds the button properties
        * PARAMETERS : Texture2D : newTexture
        *              GraphicsDevice : graphics
        * RETURNS : 
        */
        public cButton(Texture2D newTexture, GraphicsDevice graphics, Vector2? size = null)
        {
            texture = newTexture;
            if (size != null)
            {
                this.size = (Vector2)size;
            }
            else
            {
                this.size = new Vector2 (texture.Width, texture.Height);
            }
        }

        bool down;
        public bool isClicked; // hold calue to see if button is clicked

        /* 
        * FUNCTION : Update
        * DESCRIPTION : Allow the user to interact with the buttons on screen and allow them to click the button
        * PARAMETERS : MouseState : mouse : state of the mouse on the screen
        * RETURNS : 
        */
        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            Rectangle MouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if(MouseRectangle.Intersects(rectangle))//if the mouse is overtop of the button
            {
                //animate the button colour to show it as clickable
                if (colour.A == 255) down = false;
                if (colour.A == 0) down = true;
                if (down) colour.A += 5; else colour.A -= 5;

                //if the button is pressed, change the isClicked state to true;
                if (mouse.LeftButton == ButtonState.Pressed) isClicked = true; else isClicked = false;
            }
            else//if the button is not being hovered over, reset its animation and set isclicked to false
            {
                colour.A  = 255;
                isClicked = false;
            }
        }

        /* 
        * FUNCTION : setPosition
        * DESCRIPTION : positions the button
        * PARAMETERS : Vector2 : newPosition : position of where the button will be
        * RETURNS : 
        */
        public void setPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        /* 
        * FUNCTION : Draw
        * DESCRIPTION : Draw the button
        * PARAMETERS : SpriteBatch : spriteBatch : object that has the button
        * RETURNS : 
        */
        public void Draw (SpriteBatch spriteBatch, Color colour)
        {
            spriteBatch.Draw (texture, rectangle, colour);
        }

        /* 
        * FUNCTION : Draw
        * DESCRIPTION : Draw the button
        * PARAMETERS : SpriteBatch : spriteBatch : object that has the button
        * RETURNS : 
        */
        public void Draw (SpriteBatch spriteBatch)
        {
            Draw (spriteBatch, colour);
        }
    }
}
