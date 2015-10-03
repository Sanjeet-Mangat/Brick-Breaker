/* 
* FILE : Paddle.cs
* PROJECT : PROG3105 - Assignment #1
* PROGRAMMER : Sunny Mangat & Dylan Sawchuk
* FIRST VERSION : 2014-10-01
* DESCRIPTION :The functions in this file are used to draw the paddle on screen and update it's position
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SET_Breakout
{
    class Paddle
    {
        Vector2 paddle_Position; // position of the paddle in 2D space
        Vector2 paddle_Motion; // direction the paddle will move
        float paddle_speed = 6f; // speed of the paddle

        KeyboardState key_Board_State;
        GamePadState game_Pad_State;

        float paddleWidth;
        public float PaddleWidth
        {
            get
            {
                return paddleWidth;
            }
            set// setter used to update the width of the paddle without chaning the center location of the paddle
            {
                paddle_Position.X += (paddleWidth - value) * texture.Width / 2;
                paddleWidth = value;
            }
        }
        public float baseWidth = 1;
        bool baseWidthReduced = false;

        Texture2D texture;
        Rectangle screen_Bounds; // size of screen to make sure it can't wrap around

        public bool paddle_tunnel;

        /* 
        * CONSTRUCTOR : Paddle
        * DESCRIPTION : This is a construtor that will set the texture,screen bound so the paddle wont move past the screen
        *               and the setPaddleInStartPosition function to set the paddle in position. 
        * PARAMETERS : Texture2D : texture : texture of the paddle
        *              Rectangle : screenBounds : area the paddle operates in
        * RETURNS : 
        */
        public Paddle(Texture2D texture, Rectangle screenBounds)
        {
            this.texture = texture;
            this.screen_Bounds = screenBounds;
            // have a function that sets the paddle in a start position 
            SetPaddleInStartPosition();
        }

        /* 
        * FUNCTION : SetPaddleInStartPosition
        * DESCRIPTION : Will return the paddle to where it starts
        * PARAMETERS : 
        * RETURNS : 
        */
        public void SetPaddleInStartPosition()
        {
            paddle_Position.X = (screen_Bounds.Width - texture.Width * baseWidth) / 2;
            paddle_Position.Y = screen_Bounds.Height - texture.Height - 2;
            paddleWidth = baseWidth;
            paddle_tunnel = false;
        }


        /* 
        * FUNCTION : resetPaddle
        * DESCRIPTION : Will return the paddle to where it starts and reset the base width based on the value of the parameter
        * PARAMETERS : bool baseWidthReduced: if its true base width will be half of the default value
        * RETURNS : 
        */
        public void resetPaddle(bool baseWidthReduced)
        {
            this.baseWidthReduced = baseWidthReduced;
            baseWidth = 1.5f;
            if (baseWidthReduced) baseWidth /= 2;

            SetPaddleInStartPosition ();
        }

        /* 
        * FUNCTION : Update
        * DESCRIPTION : Will update the paddles position
        * PARAMETERS : 
        * RETURNS : 
        */
        public void Update()
        {
            paddle_Motion = Vector2.Zero;

            key_Board_State = Keyboard.GetState();
            game_Pad_State = GamePad.GetState(PlayerIndex.One);

            if(key_Board_State.IsKeyDown(Keys.Left) || game_Pad_State.IsButtonDown(Buttons.LeftThumbstickLeft) || game_Pad_State.IsButtonDown(Buttons.DPadLeft))
            {
                paddle_Motion.X -= 1;
            }

            if(key_Board_State.IsKeyDown(Keys.Right) || game_Pad_State.IsButtonDown(Buttons.LeftThumbstickRight) || game_Pad_State.IsButtonDown(Buttons.DPadRight))
            {
                paddle_Motion.X += 1;
            }

            //if the paddle is too far left
            if (paddle_Position.X < 0)
            {
                if (paddle_tunnel)//and it is set to tunnel, move it to the right side of the screen
                {
                    paddle_Position.X += screen_Bounds.Width;
                }
                else//if its not set to tunnel, prevent it from moving off screen
                {
                    paddle_Position.X = 0;
                }
            }
            else if (paddle_Position.X > screen_Bounds.Width - texture.Width * paddleWidth)//if the right side of the paddle is beyond the right side of the screen
            {
                if (!paddle_tunnel)//if the paddle is not set to tunnel, prevent it from moving off screen
                {
                    paddle_Position.X = screen_Bounds.Width - texture.Width * paddleWidth;
                }
                else if (paddle_Position.X > screen_Bounds.Width)//if the paddle is set to tunnel and the left side of the paddle is off the righthand side of the screen
                {
                    paddle_Position.X -= screen_Bounds.Width;//make the paddle wrap to the left side of the screen
                }
            }

            //move the paddle -> this is done after the boundar checking so theres a little animation of the player trying to push the paddle through the wall when not tunneling
            paddle_Motion *= paddle_speed;
            paddle_Position += paddle_Motion;
        }

        /* 
        * FUNCTION : Draw
        * DESCRIPTION : Draws the paddle
        * PARAMETERS : SpriteBatch : spriteBatch : SpriteBatch used to draw the paddle
        * RETURNS :
        */
        public void Draw(SpriteBatch spriteBatch)
        {
            drawPaddleAtPosition (paddle_Position, spriteBatch);//draw the paddle in its regular position

            if (paddle_Position.X > screen_Bounds.Width - texture.Width * paddleWidth && paddle_tunnel)//if the paddle is tunneling and the right hand side of the paddle is offscreen
            {
                drawPaddleAtPosition (new Vector2 (paddle_Position.X - screen_Bounds.Width, paddle_Position.Y), spriteBatch);//draw a second paddle on the lefthand side of the screen
            }
        }

        /* 
        * FUNCTION : drawPaddleAtPosition
        * DESCRIPTION : Draws the paddle at a specific position using a custom drawing algorithm for stretch to prevent the curved ends of the paddle from stretching
        *               the disadvantage to this is that when the paddle is small it looks tiny because the curved ends have not changed in size
        * PARAMETERS : SpriteBatch : spriteBatch : SpriteBatch used to draw the paddle
        * RETURNS :
        */
        private void drawPaddleAtPosition (Vector2 position, SpriteBatch spriteBatch)
        {
            /*
             * the drawing of the paddle, to prevent the curved ends from growing/shrinking with the paddle, is done using segments of the paddle
             * 
             * the drawing method uses the following assumptions
             * -each curved end is < 1/8 width of the texture
             * -the middle 3/4 of the texture is flat
             * 
             */

            /*
             * When the paddle is smaller than the length of 2 end pieces (1/8 ea) and 2 middle pieces (3/4 ea) -> 1 and 3/4 of the texture size
             * the paddle is drawn using 2 equal segments that meet in the middle. 
             * 
             * This allows the paddle to dynamically resize without having the middle of the segments extend past the rounded sections of the paddle
             * when the paddle shrinks
             * 
             */

            if (paddleWidth <= 1.75)
            {
                spriteBatch.Draw (texture, position, new Rectangle (0, 0, (int)(texture.Width * paddleWidth / 2) + 1, texture.Height), Color.White);
                spriteBatch.Draw (texture, new Vector2 (position.X + texture.Width * paddleWidth / 2 - 1, position.Y), new Rectangle ((int)(texture.Width * (1 - paddleWidth / 2)) - 1, 0, (int)(texture.Width * paddleWidth / 2 + 1), texture.Height), Color.White);
            }

            /*
             * When the paddle is larger than the length of 2 end pieces (1/8 ea) and 2 middle pieces (3/4 ea) -> 1 and 3/4 of the texture size
             * the paddle is drawn using 2 segments the length of an endpiece (1/8) and a middle piece (3/4)-> 7 / 8 of the texture size
             * as well as a bunch of 3/4 long middle segments
             * 
             * the number of middle segments is determined to be n = Math.Ceiling((width - 1.75)/0.75) by the for loop
             * 
             * The last middle segment on the right side is shifted slightly to the left if necessary to prevent the corners of the segment extending past the 
             * curved endpiece
             * 
             */
            else
            {
                spriteBatch.Draw (texture, position, new Rectangle (0, 0, texture.Width * 7 / 8, texture.Height), Color.White);

                for (float i = 0.875f; i < paddleWidth; i += 0.75f)
                {
                    spriteBatch.Draw (texture,
                                      new Vector2 (position.X + texture.Width * Math.Min (i, paddleWidth - 0.875f) - 1, position.Y), 
                                      new Rectangle (texture.Width / 8 - 1, 0, texture.Width * 3 / 4 + 1, texture.Height),
                                      Color.White);
                }

                spriteBatch.Draw (texture, new Vector2 (position.X + texture.Width * (paddleWidth - 0.875f) - 1, position.Y), new Rectangle (texture.Width / 8 - 1, 0, texture.Width * 7 / 8 + 1, texture.Height), Color.White);
            }
        }

        /* 
        * FUNCTION : GetBounds
        * DESCRIPTION : Returns a Rectangle that decribes the paddle
        * PARAMETERS : 
        * RETURNS : Rectangle that decribes the paddle
        */
        public Rectangle GetBounds()
        {
            return new Rectangle((int)paddle_Position.X,(int)paddle_Position.Y,(int)(texture.Width * paddleWidth),texture.Height);
        }

        /* 
        * FUNCTION : GetBoundsWithWrap
        * DESCRIPTION : Returns a Rectangle array that decribes the paddle with tunneling
        * PARAMETERS : 
        * RETURNS : Rectangle[] that decribes the paddle(s)
        */
        public Rectangle[] GetBoundsWithWrap ()
        {
            Rectangle[] returnValue = new Rectangle[1];

            if (paddle_Position.X > screen_Bounds.Width - texture.Width && paddle_tunnel)
            {
                returnValue = new Rectangle[2];
                returnValue[1] = new Rectangle ((int)paddle_Position.X - screen_Bounds.Width, (int)paddle_Position.Y, (int)(texture.Width * paddleWidth), texture.Height);
            }

            returnValue[0] = new Rectangle ((int)paddle_Position.X, (int)paddle_Position.Y, (int)(texture.Width * paddleWidth), texture.Height);

            return returnValue;
        }


        /* 
        * FUNCTION : reduceBaseWidth
        * DESCRIPTION : Reduces the base width of the paddle to half it's normal size if it hasnt already been reduced
        * PARAMETERS : 
        * RETURNS : 
        */
        public void reduceBaseWidth ()
        {
            if (!baseWidthReduced)
            {
                baseWidth /= 2;
                paddleWidth /= 2;
                baseWidthReduced = true;
            }
        }
    }
}
