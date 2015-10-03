/* 
* FILE : Ball.cs
* PROJECT : PROG3105 - Assignment #1
* PROGRAMMER : Sunny Mangat & Dylan Sawchuk
* FIRST VERSION : 2014-10-01
* DESCRIPTION :The functions in this file are used to draw the ball on screen and detect collisions between the ball and other objects
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SET_Breakout
{
    class Ball
    {
        Vector2 motion; //direction the ball moves
        Vector2 position; //position of the ball in 2D space
        float ball_speed; //speed the ball moves
        public float ball_speed_modifier = 1;
        Texture2D texture;
        Rectangle screen_Bounds;

        int hitCount;
        bool hitMiddleRow, hitTopRow;

        bool unstoppable = false;
        int unstoppableTime = 0;

        /* 
        * FUNCTION : Ball
        * DESCRIPTION : The constructor for the ball 
        * PARAMETERS : Texture2D : texture : texture of the ball
        *              Rectangle : screenBounds : area the ball operates in
        * RETURNS : 
        */
        public Ball(Texture2D texture, Rectangle screen_Bounds)
        {
            this.texture = texture;
            this.screen_Bounds = screen_Bounds;
        }


        /* 
        * FUNCTION : SetBallInStartPosition
        * DESCRIPTION : sets the balls position to start above the paddle
        * PARAMETERS : Rectangle : paddle_Location : paddles start location on screen
        * RETURNS : 
        */
        public void SetBallInStartPosition(Rectangle paddle_Location)
        {
            motion = new Vector2(1, -1);
            position.Y = paddle_Location.Y - texture.Height;
            position.X = paddle_Location.X + (paddle_Location.Width - texture.Width / 2);

            ball_speed = 4;
            hitCount = 0;
            hitMiddleRow = hitTopRow = false;

            unstoppable = false;
            unstoppableTime = 0;
        }

        /* 
        * FUNCTION : Update
        * DESCRIPTION : Update the balls position
        * PARAMETERS : 
        * RETURNS : bool: true if the ball has collided with the top wall
        */
        public bool Update()
        {
            position += motion * ball_speed * ball_speed_modifier;

            if (unstoppable)
            {
                --unstoppableTime;
                if (unstoppableTime == 0)
                {
                    unstoppable = false;
                }
            }

            return CheckWallCollision();
        }

        /* 
        * FUNCTION : Off_Bottom
        * DESCRIPTION :  Checks to see if the ball has fallen off the bottom of the screen
        * PARAMETERS : 
        * RETURNS : true : if ball has fallen off screen
        *           false : if it hasn't 
        */
        public bool Off_Bottom()
        {
            if (position.Y > screen_Bounds.Height)
                return true;
            return false;
        }

        /* 
        * FUNCTION : PaddleCollision
        * DESCRIPTION :  Checks to see if the ball collides with the paddle
        * PARAMETERS : Rectangle[] : paddleLocations : location of the paddle
        * RETURNS : bool: true if there was a collision
         * 
         * NOTE* currently there is a bug in here somewhere which involves large paddles and the paddle tunnel
         * TODO - fix bug
        */
        public bool PaddleCollision(Rectangle[] paddleLocations)
        {
            float radius = texture.Width / 2;
            float ballCenterX = position.X + radius;
            float ballCenterY = position.Y + radius;

            float diffXposition;
            float diffYposition;
            float halfPaddleWidth;
            float halfPaddleHeight;

            bool returnValue = false;

            foreach (Rectangle paddleLocation in paddleLocations)
            {
                halfPaddleHeight = paddleLocation.Height / 2;
                halfPaddleWidth = paddleLocation.Width / 2;

                diffXposition = Math.Abs (ballCenterX - (paddleLocation.X + halfPaddleWidth));
                diffYposition = Math.Abs (ballCenterY - (paddleLocation.Y + halfPaddleHeight));

                if ((diffXposition <= halfPaddleWidth - halfPaddleHeight && diffYposition <= halfPaddleHeight + radius) ||//detects top & bottom collisions
                    Math.Sqrt (Math.Pow (diffXposition - halfPaddleWidth + halfPaddleHeight, 2) + Math.Pow (diffYposition, 2)) <= halfPaddleHeight + radius)// detects collisions on rounded edges of paddles
                {
                    motion.Y = Math.Abs (motion.Y);
                    if (ballCenterY < paddleLocation.Y + halfPaddleHeight)
                    {
                        motion.Y = -motion.Y;
                    }

                    motion.X = (float)(diffXposition / halfPaddleWidth * 1.5);
                    if (ballCenterX < paddleLocation.X + halfPaddleWidth)
                    {
                        motion.X = -motion.X;
                    }

                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        /* 
        * FUNCTION : Draw
        * DESCRIPTION : Draw the ball
        * PARAMETERS : SpriteBatch : spriteBatch : SpriteBatch used to draw the ball
        */
        public void Draw(SpriteBatch spriteBatch)
        {
            if (unstoppable)
            {
                spriteBatch.Draw (texture, position, Color.Red);
            }
            else
            {
                spriteBatch.Draw (texture, position, Color.Black);
            }
        }

        /* 
        * FUNCTION : CheckWallCollision
        * DESCRIPTION : Checks to see if the ball has moved off the left side of the screen by
        *               checking if the X coordinate of the balls position is less than zero 
        *               and vice versa. 
        * PARAMETERS : 
        * RETURNS : bool: true if the ball has collided with the top of the screen
        */
        private bool CheckWallCollision()
        {
            if (position.X < 0)
            {
                position.X = 0; //set x coordinate to 0
                motion.X *= -1;// change direction to move left to right
            }
            if (position.X + texture.Width > screen_Bounds.Width)
            {
                position.X = screen_Bounds.Width - texture.Width;
                motion.X *= -1;
            }
            if (position.Y < 0)
            {
                position.Y = 0;//set y coordinate to 0
                motion.Y *= -1;//change directions fromup to down

                return true;
            }
            return false;
        }

        /* 
        * FUNCTION : checkBrickCollisions
        * DESCRIPTION : Checks for collisions with bricks and spawns power ups at a random chance on collision
        * PARAMETERS : Brick[,] bricks: the array of bricks to test for collision with
        *              ref List <PowerUp> powerUps: the list of powerUps to add to if a collision creates a power up
        *              out bool allBricksBroken: set to true if all of the bricks are broken
        * RETURNS : int: the increase in score caused by the brick collision(s);
        */
        public int checkBrickCollisions(Brick[,] bricks, ref List<PowerUp> powerUps, out bool allBricksBroken)
        {
            float radius = texture.Width / 2;
            float ballCenterX = position.X + radius;
            float ballCenterY = position.Y + radius;
            int retValue = 0;
            float diffXposition;
            float diffYposition;
            float halfBrickWidth;
            float halfBrickHeight;

            allBricksBroken = true;

            //foreach (Brick brick in bricks)
            for(int i = 0; i < 6; ++i)
            {
                for(int j = 0; j < 10; ++j)
                {
                    Brick brick = bricks[j, i];

                    if (brick.broken) continue;//if the brick is broken, ignore it for collision detection

                    halfBrickHeight = brick.Location.Height / 2;
                    halfBrickWidth = brick.Location.Width / 2;
                    diffXposition = Math.Abs(ballCenterX - (brick.Location.X + halfBrickWidth));
                    diffYposition = Math.Abs(ballCenterY - (brick.Location.Y + halfBrickHeight));

                    if (diffYposition <= halfBrickHeight && diffXposition <= halfBrickWidth + radius)//detects left and right side collisions
                    {
                        if (!unstoppable)//only reflect the ball if it isnt unstoppable
                        {
                            motion.X = Math.Abs (motion.X);
                            if (ballCenterX < (brick.Location.X + halfBrickWidth))
                            {
                                motion.X = -motion.X;
                            }
                        }
                        brick.broken = true;
                    }
                    else if (diffXposition <= halfBrickWidth && diffYposition <= halfBrickHeight + radius)//detects top and bottom collisions
                    {
                        if (!unstoppable)//only reflect the ball if it isnt unstoppable
                        {
                            motion.Y = Math.Abs(motion.Y);
                            if (ballCenterY < brick.Location.Y + halfBrickHeight)
                            {
                                motion.Y = -motion.Y;
                            }
                        }
                        brick.broken = true;
                    }
                    else if (Math.Sqrt(Math.Pow(diffXposition - halfBrickWidth, 2) + Math.Pow(diffYposition - halfBrickHeight, 2)) <= radius)//detects corner collisions
                    {
                        if (!unstoppable)//only reflect the ball if it isnt unstoppable
                        {
                            motion.X = Math.Abs (motion.X);
                            if (ballCenterX < (brick.Location.X + halfBrickWidth))
                            {
                                motion.X = -motion.X;
                            }

                            motion.Y = Math.Abs (motion.Y);
                            if (ballCenterY < brick.Location.Y + halfBrickHeight)
                            {
                                motion.Y = -motion.Y;
                            }
                        }
                        brick.broken = true;
                    }
                    
                    if (brick.broken)// if the brick was broken
                    {
                        switch(i)//increase the score and the ball speed depending on which ball was hit
                        {
                            case 0:
                                retValue += 5;
                                if (!hitTopRow)
                                {
                                    hitTopRow = true;
                                    ball_speed += 0.75f;
                                }
                                break;
                            case 1:
                                retValue += 5;
                                break;
                            case 2:
                                retValue += 3;
                                break;
                            case 3:
                                retValue += 3;
                                if (!hitMiddleRow)
                                {
                                    hitMiddleRow = true;
                                    ball_speed += 1;
                                }
                                break;
                            case 4:
                                retValue += 1;
                                break;
                            case 5:
                                retValue += 1;
                                break;
                        }

                        if (++hitCount == 4 || hitCount == 12) ball_speed += 0.75f;//increase the ball speed after 4 or 12 collisions

                        //spawn a power up at a random chance
                        if (PowerUp.rand.Next(10) == 1) powerUps.Add (new PowerUp (new Vector2 (brick.Location.X + halfBrickWidth, brick.Location.Y + halfBrickHeight)));
                    }  
                    else//if there is an unbroken brick
                    {
                        allBricksBroken = false;
                    }
                }
            }

             return retValue;
        }//public int checkBrickCollisions (Brick[,] bricks)

        /* 
        * FUNCTION : makeUnstoppable
        * DESCRIPTION : Causes the ball to be not reflected by bricks for a short duration
        * PARAMETERS : 
        * RETURNS : 
        */
        public void makeUnstoppable ()
        {
            unstoppable = true;
            unstoppableTime += 300;
        }
    }
}
