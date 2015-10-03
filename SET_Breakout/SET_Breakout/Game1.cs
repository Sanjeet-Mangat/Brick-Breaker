/* 
* FILE : Game1.cs
* PROJECT : PROG3105 - Assignment #1
* PROGRAMMER : Sunny Mangat & Dylan Sawchuk
* FIRST VERSION : 2014-10-01
* DESCRIPTION :The functions in this file are used to draw the ball on screen and and allow the player o play the game
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SET_Breakout
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        //objects used for display of objects
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Rectangle screen_Rectangle; // playing area of the game

        Paddle paddle; // paddle object
        Ball ball;

        //settnigs for changing the difficulty
        bool hardMode = false;
        int pointsToWin = 360;

        //variables for managing the array of bricks
        int bricksPerRow = 10;
        int rowsOfBricks = 6;
        Texture2D brick_Image;
        Brick[,] bricks;
        bool allBricksBroken;

        List<PowerUp> powerUps = new List<PowerUp> ();

        //GameState is used to track the progress of the user through differences in game usage amounts
        // Mainmenu basically means they havent played yet,
        // paused means theyre in the middle of a game
        // gameOver means they've just won or lost
        enum GameState
        {
            MainMenu,Quit,Playing,GameOver, Paused
        }
        GameState gameState = GameState.MainMenu;

        //determines which part of the menu to show when the game is not playing
        //same menu system is used everywhere
        enum MenuState
        {
            Main, Settings, Help, About
        }
        MenuState menuState = MenuState.Main;

        Texture2D pauseMenu, settingsMenu, helpMenu, aboutMenu, mainMenu, win, lose, backGroundTexture;//large textures for headings and backgrounds on the game & menus

        //variables used for tracking and displaying lives and score
        SpriteFont Arial;
        int score;
        Vector2 score_position;
        int lives;
        Vector2 lives_position;

        cButton btnPlay, btnSettings, btnQuit, aboutButton, helpButton, resumeButton, playAgainButton, //main menu buttons
                ballSpeed_Slow, ballSpeed_Med, ballSpeed_Fast, diffHard, diffNormal, Two_Screen, Five_Screen, Unlimited_Screen, //settings menu buttons
                backButton; //sub menu button for returning to main menu

        /* 
        * CONSTRUCTOR : Game1()
        * DESCRIPTION : Sets up the desired window size and content directories
        * PARAMETERS :
        * RETURNS : 
        */
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 400;

            screen_Rectangle = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // load up prefered values and objects to display a 400 * 500 playing area
            spriteBatch = new SpriteBatch (GraphicsDevice);
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 400;
            graphics.ApplyChanges ();
            IsMouseVisible = true;

            //load up the textures for the mai game objects
            Texture2D temp_Texture = Content.Load<Texture2D>("SET_Breakout_Paddle");
            paddle = new Paddle(temp_Texture, screen_Rectangle);
            Texture2D tempTexture = Content.Load<Texture2D>("SET_Breakout_Ball");
            ball = new Ball(tempTexture, screen_Rectangle);
            brick_Image = Content.Load<Texture2D> ("SET_Breakout_Brick");
            //load up the power up sorites
            PowerUp.sizeUpTexture = Content.Load<Texture2D> ("sizeUpTexture");
            PowerUp.tunnelTexture = Content.Load<Texture2D> ("tunnelTexture");
            PowerUp.unstoppableTexture = Content.Load<Texture2D> ("unstoppableTexture");

            //load the main menu buttons
            btnPlay = new cButton (Content.Load<Texture2D> ("button"), graphics.GraphicsDevice);
            playAgainButton = new cButton (Content.Load<Texture2D> ("playAgainButton"), graphics.GraphicsDevice);
            resumeButton = new cButton (Content.Load<Texture2D> ("resumeButton"), graphics.GraphicsDevice);
            btnSettings = new cButton(Content.Load<Texture2D>("settingBTN"), graphics.GraphicsDevice);
            helpButton = new cButton (Content.Load<Texture2D> ("helpButton"), graphics.GraphicsDevice);
            aboutButton = new cButton (Content.Load<Texture2D> ("aboutButton"), graphics.GraphicsDevice);
            btnQuit = new cButton (Content.Load<Texture2D> ("btnQuit"), graphics.GraphicsDevice);

            //locate the main menu buttons
            btnPlay.setPosition (new Vector2 (250 - btnPlay.texture.Width / 2, 150 - btnPlay.texture.Height));
            playAgainButton.setPosition (new Vector2 (250 - playAgainButton.texture.Width / 2, 150 - playAgainButton.texture.Height));
            resumeButton.setPosition (new Vector2 (250 - resumeButton.texture.Width / 2, 150 - resumeButton.texture.Height));
            btnSettings.setPosition (new Vector2 (250 - btnSettings.texture.Width / 2, 200 - btnSettings.texture.Height));
            helpButton.setPosition (new Vector2 (250 - helpButton.texture.Width / 2, 250 - helpButton.texture.Height));
            aboutButton.setPosition (new Vector2 (250 - aboutButton.texture.Width / 2, 300 - aboutButton.texture.Height));
            btnQuit.setPosition (new Vector2 (250 - btnQuit.texture.Width / 2, 350 - btnQuit.texture.Height));

            //load up the backButton for the about, help & settings menu
            backButton = new cButton (Content.Load<Texture2D> ("backButton"), graphics.GraphicsDevice);
            backButton.setPosition (new Vector2 (5, 5));

            Vector2 buttonSize = new Vector2 (100, 20);//makes all of the options in the settings menu the same size for more consistent looking layout

            //import and set the locations of all of the settings menu buttons
            ballSpeed_Slow = new cButton (Content.Load<Texture2D> ("ballSpeedSlow"), graphics.GraphicsDevice, buttonSize);
            ballSpeed_Slow.setPosition (new Vector2 (42, 139));

            ballSpeed_Med = new cButton (Content.Load<Texture2D> ("ballSpeedMed"), graphics.GraphicsDevice, buttonSize);
            ballSpeed_Med.setPosition (new Vector2 (200, 139));

            ballSpeed_Fast = new cButton (Content.Load<Texture2D> ("ballSpeedFast"), graphics.GraphicsDevice, buttonSize);
            ballSpeed_Fast.setPosition (new Vector2 (358, 139));

            diffNormal = new cButton (Content.Load<Texture2D> ("diffNormal"), graphics.GraphicsDevice, buttonSize);
            diffNormal.setPosition (new Vector2 (121, 235));

            diffHard = new cButton (Content.Load<Texture2D> ("diffHard"), graphics.GraphicsDevice, buttonSize);
            diffHard.setPosition (new Vector2 (279, 235));

            Two_Screen = new cButton (Content.Load<Texture2D> ("pointsToWin_2"), graphics.GraphicsDevice, buttonSize);
            Two_Screen.setPosition (new Vector2 (42, 331));

            Five_Screen = new cButton (Content.Load<Texture2D> ("pointsToWin_5"), graphics.GraphicsDevice, buttonSize);
            Five_Screen.setPosition (new Vector2 (200, 331));

            Unlimited_Screen = new cButton (Content.Load<Texture2D> ("pointsToWin_Unlimited"), graphics.GraphicsDevice, buttonSize);
            Unlimited_Screen.setPosition (new Vector2 (358, 331));

            //load the background images
            pauseMenu = Content.Load<Texture2D> ("paused");
            settingsMenu = Content.Load<Texture2D> ("settings");
            mainMenu = Content.Load<Texture2D> ("MainMenu");
            lose = Content.Load<Texture2D> ("lose");
            win = Content.Load<Texture2D> ("win");
            helpMenu = Content.Load<Texture2D> ("help");
            aboutMenu = Content.Load<Texture2D> ("about");
            backGroundTexture = Content.Load<Texture2D> ("GameBackground");

            //variables for managing score and lives display
            Arial = Content.Load<SpriteFont> ("Arial");
            score_position.X = 5;
            score_position.Y = 5;
            lives_position.Y = 5;
            lives_position.X = screen_Rectangle.Width * 0.85f;

            //initialises the gamestate to a new game
            Start_Breakout_Game();
        }

        /* 
        * FUNCTION : Start_Breakout_Game
        * DESCRIPTION : Resets the gamestate to be a new game
        * PARAMETERS :
        * RETURNS : 
        */
        private void Start_Breakout_Game()
        {
            //resets ball, paddle and powerup positions to default
            paddle.resetPaddle(hardMode);
            ball.SetBallInStartPosition(paddle.GetBounds());
            powerUps.Clear ();

            lives = 3;
            score = 0;


            //initialisation of the bricks
            allBricksBroken = false;
            bricks = new Brick[bricksPerRow, rowsOfBricks];
            Color brick_Color = Color.Red;

            //sets teh location and color of bricks based on their array index
            for(int y = 0; y < rowsOfBricks; y++)
            {
                switch(y)
                {
                    case 5:
                        brick_Color = Color.Red;
                        break;
                    case 4:
                        brick_Color = Color.Orange;
                        break;
                    case 3:
                        brick_Color = Color.Yellow;
                        break;
                    case 2:
                        brick_Color = Color.Green;
                        break;
                    case 1:
                        brick_Color = Color.Blue;
                        break;
                    case 0:
                        brick_Color = Color.Purple;
                        break;
                }

                for (int x = 0; x < bricksPerRow; x++)
                {
                    bricks[x, y] = new Brick(
                    brick_Image,
                    new Rectangle(
                    x * brick_Image.Width,
                    (y + 3) * brick_Image.Height,
                    brick_Image.Width,
                    brick_Image.Height),
                    brick_Color);
                }

            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //no content to be unloaded
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //gets the state of the mouse and the back button(s)
            MouseState mouse = Mouse.GetState ();
            bool backIsPressed = false;
            if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
            {
                backIsPressed = true;
            }

            if (gameState == GameState.Playing)//while the game is running
            {
                if (backIsPressed)//if they pause the game
                {
                    //display the pause menu
                    gameState = GameState.Paused;
                    menuState = MenuState.Main;
                    updateMainMenu (mouse);
                }
                else//if they don't pause it, update the gamestate
                {
                    updateGameState ();
                }
            }
            else if (menuState == MenuState.Main)//if they are at the main menu, update the main menu
            {
                updateMainMenu (mouse);
            }
            else//if the game isnt at the main menu
            {
                //if they press the back button return to the main menu
                backButton.Update (mouse);
                if (backIsPressed || backButton.isClicked) menuState = MenuState.Main;

                //update the settings menu if they are on the settings menu
                if (menuState == MenuState.Settings)
                {
                    updateSettingsMenu (mouse);
                }
            }

            if (gameState == GameState.Quit) Exit ();//if the user clicks quit, exit the game

            base.Update (gameTime);
        }

        /* 
        * FUNCTION : updateSettingsMenu
        * DESCRIPTION : updates the layout and clicked state of the settings menu buttons,
        *               also updates the settings based on the clicked state of the settings menu buttons
        * PARAMETERS : MouseState mouse: the state of the mouse
        * RETURNS : 
        */
        private void updateSettingsMenu (MouseState mouse)
        {
            //update the slow ball speed button, if it is clicked, set the ball speed to slow
            ballSpeed_Slow.Update (mouse);
            if (ballSpeed_Slow.isClicked) ball.ball_speed_modifier = 0.75f;

            //update the medium ball speed button, if it is clicked, set the ball speed to medium
            ballSpeed_Med.Update (mouse);
            if (ballSpeed_Med.isClicked) ball.ball_speed_modifier = 1;

            //update the fast ball speed button, if it is clicked, set the ball speed to fast
            ballSpeed_Fast.Update (mouse);
            if (ballSpeed_Fast.isClicked) ball.ball_speed_modifier = 1.25f;

            //update the normal difficulty button, if it is clicked, turn off hard mode
            diffNormal.Update (mouse);
            if (diffNormal.isClicked) hardMode = false;

            //update the hard difficulty button, if it is clicked, turn on hard mode
            diffHard.Update (mouse);
            if (diffHard.isClicked) hardMode = true;

            //update the two screens to win button, if it is clicked, set the points to win = double a screen's worth (180)
            Two_Screen.Update (mouse);
            if (Two_Screen.isClicked) pointsToWin = 360;

            //update the five screens to win button, if it is clicked, set the points to win = 5 times a screen's worth (180)
            Five_Screen.Update (mouse);
            if (Five_Screen.isClicked) pointsToWin = 900;

            //update the unlimited play button, if it is clicked, set the points to win = 0;
            Unlimited_Screen.Update (mouse);
            if (Unlimited_Screen.isClicked) pointsToWin = 0;
        }

        /* 
        * FUNCTION : updateMainMenu
        * DESCRIPTION : updates the layout and clicked state of the main menu buttons,
        *               also handles the navigation from the main menu back to the game and to the other menus
        * PARAMETERS : MouseState mouse: the state of the mouse
        * RETURNS : 
        */
        private void updateMainMenu (MouseState mouse)
        {
            //update the 3 buttons that start/resume gameplay
            btnPlay.Update (mouse);
            playAgainButton.Update (mouse);
            resumeButton.Update (mouse);

            //if the start/resume button corresponding to the current gamestate is clicked
            if ((btnPlay.isClicked && gameState == GameState.MainMenu) ||
                (playAgainButton.isClicked && gameState == GameState.GameOver) ||
                (resumeButton.isClicked && gameState == GameState.Paused))
            {
                //if the game is not paused, reset the gameboard to normal starting positions
                if (gameState != GameState.Paused)
                {
                    Start_Breakout_Game ();
                }

                //resume play
                gameState = GameState.Playing;
                btnPlay.isClicked = false;
                resumeButton.isClicked = false;
                playAgainButton.isClicked = false;
            }

            btnSettings.Update (mouse);
            if (btnSettings.isClicked)
            {
                menuState = MenuState.Settings;
                btnSettings.isClicked = false;
            }

            btnQuit.Update (mouse);
            if (btnQuit.isClicked)
            {
                gameState = GameState.Quit;
                btnQuit.isClicked = false;
            }

            helpButton.Update (mouse);
            if (helpButton.isClicked)
            {
                menuState = MenuState.Help;
                helpButton.isClicked = false;
            }

            aboutButton.Update (mouse);
            if (aboutButton.isClicked)
            {
                menuState = MenuState.About;
                aboutButton.isClicked = false;
            }
        }

        /* 
        * FUNCTION : updateGameState
        * DESCRIPTION : updates the running state of the game
        * PARAMETERS : 
        * RETURNS : 
        */
        public void updateGameState ()
        {
            paddle.Update ();// move the paddle according to what buttons the user is pressing
            if (ball.Update ()) paddle.reduceBaseWidth ();//update the ball position, if it collides with the top, try to make the paddle smaller

            //check for paddle collision and redirect the ball as necessary
            Rectangle[] paddleBounds = paddle.GetBoundsWithWrap ();
            if (ball.PaddleCollision (paddleBounds) && allBricksBroken)//if when the ball hits the paddle there are no bricks on screen
            {
                //redisplay all the bricks
                foreach (Brick brick in bricks) brick.broken = false;

                allBricksBroken = false;
            }

            for (int i = 0; i < powerUps.Count; ++i)
            {
                powerUps[i].Update ();//make the power ups fall

                if (powerUps[i].location.Y > screen_Rectangle.Height + 16)//if the power up is offscreen, delete it
                {
                    powerUps.RemoveAt (i--);
                    continue;
                }

                foreach (Rectangle paddleBound in paddleBounds)
                {
                    if (paddleBound.Intersects (powerUps[i].getBounds ()))//if the powerup is caught by the paddle
                    {
                        switch (powerUps[i].type)
                        {
                            case PowerUp.powerUpType.sizeUp:///increase the paddle size by half of the base paddle size, until the next time the player loses a life
                                paddle.PaddleWidth += paddle.baseWidth / 2;
                                break;
                            case PowerUp.powerUpType.tunnel://toggle the paddle's ability to tunnel around the screen, reset to false on losing a life
                                paddle.paddle_tunnel = !paddle.paddle_tunnel;
                                break;
                            case PowerUp.powerUpType.unstoppable://make the ball temporarily unstoppable, ended early by losing a life
                                ball.makeUnstoppable ();
                                break;
                        }

                        powerUps.RemoveAt (i--);//delete the power up
                        break;
                    }
                }
            }

            //if there are unbroken bricks, check if the ball has hit any bricks
            if (!allBricksBroken) score += ball.checkBrickCollisions (bricks, ref powerUps, out allBricksBroken);

            if (ball.Off_Bottom ())//if the player drops the ball
            {
                --lives;//take away a life

                //reset the ball, paddle and powerups to default
                paddle.SetPaddleInStartPosition ();
                ball.SetBallInStartPosition (paddle.GetBounds ());
                powerUps.Clear ();

                //if all of the bricks have been broken, redisplay them
                if (allBricksBroken)
                {
                    foreach (Brick brick in bricks) brick.broken = false;

                    allBricksBroken = false;
                }
            }

            //if the player has won or run out of lives, end the game
            if (score >= pointsToWin && pointsToWin != 0 || lives == 0)
            {
                gameState = GameState.GameOver;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin ();

            spriteBatch.Draw (backGroundTexture, new Rectangle (0, 0, 500, 400), Color.White);//draw the backgroun image

            if (gameState == GameState.Playing)
            {
                drawGame (spriteBatch);
            }
            else if (menuState == MenuState.Main)
            {
                drawMainMenu (spriteBatch);
            }
            else if (menuState == MenuState.About)
            {
                drawAboutMenu (spriteBatch);
            }
            else if (menuState == MenuState.Help)
            {
                drawHelpMenu (spriteBatch);
            }
            else if (menuState == MenuState.Settings)
            {
                drawSettingsMenu (spriteBatch);
            }

            spriteBatch.End ();
            base.Draw(gameTime);
        }

        /* 
        * FUNCTION : drawSettingsMenu
        * DESCRIPTION : draws the settings menu
        * PARAMETERS : Spritebatch spriteBatch: The SpriteBatch object to draw with
        * RETURNS : 
        */
        private void drawSettingsMenu (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw (settingsMenu, new Rectangle (0, 0, 500, 400), Color.White);//draw the setings menu (excluding buttons)

            if (ball.ball_speed_modifier < 1)//if the ball speed is slow, draw the slow button selected
            {
                ballSpeed_Slow.Draw (spriteBatch, Color.Red);
                ballSpeed_Med.Draw (spriteBatch);
                ballSpeed_Fast.Draw (spriteBatch);
            }
            else if (ball.ball_speed_modifier == 1)//if the ball speed is medium, draw the medium button selected
            {
                ballSpeed_Slow.Draw (spriteBatch);
                ballSpeed_Med.Draw (spriteBatch, Color.Red);
                ballSpeed_Fast.Draw (spriteBatch);
            }
            else//if the ball speed is fast, draw the fast button selected
            {
                ballSpeed_Slow.Draw (spriteBatch);
                ballSpeed_Med.Draw (spriteBatch);
                ballSpeed_Fast.Draw (spriteBatch, Color.Red);
            }

            if (hardMode)//if the difficulty is hard, draw the hard button selected
            {
                diffNormal.Draw (spriteBatch);
                diffHard.Draw (spriteBatch, Color.Red);
            }
            else//if the difficulty is normal, draw the normal button selected
            {
                diffNormal.Draw (spriteBatch, Color.Red);
                diffHard.Draw (spriteBatch);
            }

            if (pointsToWin == 360)//if it takes 2 screens worth of points to win, draw the 2 screens to win button selected
            {
                Two_Screen.Draw (spriteBatch, Color.Red);
                Five_Screen.Draw (spriteBatch);
                Unlimited_Screen.Draw (spriteBatch);
            }
            else if (pointsToWin == 900)//if it takes 5 screens worth of points to win, draw the 5 screens to win button selected
            {
                Two_Screen.Draw (spriteBatch);
                Five_Screen.Draw (spriteBatch, Color.Red);
                Unlimited_Screen.Draw (spriteBatch);
            }
            else//if you can't win, draw the unlimited play button selected
            {
                Two_Screen.Draw (spriteBatch);
                Five_Screen.Draw (spriteBatch);
                Unlimited_Screen.Draw (spriteBatch, Color.Red);
            }

            backButton.Draw (spriteBatch);
        }

        /* 
        * FUNCTION : drawHelpMenu
        * DESCRIPTION : draws the help menu
        * PARAMETERS : Spritebatch spriteBatch: The SpriteBatch object to draw with
        * RETURNS : 
        */
        private void drawHelpMenu (SpriteBatch spriteBatch)
        {
            //draw the help menu and the back button (the help menu isnt interactive)
            spriteBatch.Draw (helpMenu, new Rectangle (0, 0, 500, 400), Color.White);
            backButton.Draw (spriteBatch);
        }

        /* 
        * FUNCTION : drawAboutMenu
        * DESCRIPTION : draws the about menu
        * PARAMETERS : Spritebatch spriteBatch: The SpriteBatch object to draw with
        * RETURNS : 
        */
        private void drawAboutMenu (SpriteBatch spriteBatch)
        {
            //draw the about menu and the back button (the about menu isnt interactive)
            spriteBatch.Draw (aboutMenu, new Rectangle (0, 0, 500, 400), Color.White);
            backButton.Draw (spriteBatch);
        }

        /* 
        * FUNCTION : drawMainMenu
        * DESCRIPTION : draws the about menu
        * PARAMETERS : Spritebatch spriteBatch: The SpriteBatch object to draw with
        * RETURNS : 
        */
        private void drawMainMenu (SpriteBatch spriteBatch)
        {
            switch (gameState)
            { 
                case GameState.MainMenu://if the user hasnt played a game since launcing the application, draw the application title and the play button
                    spriteBatch.Draw (mainMenu, new Rectangle (0, 0, 500, 400), Color.White);
                    btnPlay.Draw (spriteBatch);
                break;
                case GameState.Paused://if the game is paused, display the paused title and the resume button
                    spriteBatch.Draw (pauseMenu, new Rectangle (0, 0, 500, 400), Color.White);
                    resumeButton.Draw (spriteBatch);
                break;
                case GameState.GameOver://if the game is over
                    if (score >= pointsToWin &&pointsToWin != 0)//if they won, tell them they won
                    {
                        spriteBatch.Draw (win, new Rectangle (0, 0, 500, 400), Color.White);
                    }
                    else//if not, tell them they lost
                    {
                        spriteBatch.Draw (lose, new Rectangle (0, 0, 500, 400), Color.White);
                    }
                    playAgainButton.Draw (spriteBatch);//draw the play agai button
                break;

            }
            if (gameState != GameState.MainMenu)// if they have started a game
            {
                drawScore (spriteBatch);
            }

            //draw all of the buttons that do things other than start gameplay
            btnSettings.Draw (spriteBatch);
            btnQuit.Draw (spriteBatch);
            aboutButton.Draw (spriteBatch);
            helpButton.Draw (spriteBatch);
        }

        /* 
        * FUNCTION : drawGame
        * DESCRIPTION : draws the gameplay screen
        * PARAMETERS : Spritebatch spriteBatch: The SpriteBatch object to draw with
        * RETURNS : 
        */
        private void drawGame (SpriteBatch spriteBatch)
        {
            paddle.Draw (spriteBatch);
            ball.Draw (spriteBatch);

            foreach (Brick brick in bricks)
                brick.Draw (spriteBatch);

            foreach (PowerUp powerUp in powerUps)
                powerUp.Draw (spriteBatch);

            drawScore (spriteBatch);
        }

        /* 
        * FUNCTION : drawScore
        * DESCRIPTION : draws the score and lives of the player
        * PARAMETERS : Spritebatch spriteBatch: The SpriteBatch object to draw with
        * RETURNS : 
        */
        private void drawScore (SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString (Arial, "Score: " + score, score_position, Color.Red);
            spriteBatch.DrawString (Arial, "Lives: " + lives, lives_position, Color.Red);
        }
    }
}
