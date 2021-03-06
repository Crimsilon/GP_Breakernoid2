﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BreakernoidsGL
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SoundEffect ballBounceSFX, ballHitSFX, deathSFX, powerUpSFX;
        Random random = new Random();

        // Background texture
        Texture2D bgTexture;
        // Paddle object
        Paddle paddle;
        // Ball object
        Ball ball;
        //Ball catch
        bool ballCatch = false;


        // If 0, the ball can collide with the paddle
        int ballWithPaddle = 0;


        //List of powerups
        List<PowerUp> powerups = new List<PowerUp>();

        //Powerup percentage
        Double spawnChance = 1.0;
        bool destroyPowerUp = false;

        // List of all the blocks
        List<Block> blocks = new List<Block>();
        int[,] blockLayout = new int[,]{
            {5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
            {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4},
        };

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Background
            bgTexture = Content.Load<Texture2D>("bg");

            // Paddle
            paddle = new Paddle(this);
            paddle.LoadContent();
            paddle.position = new Vector2(512, 740);

            // Ball
            ball = new Ball(this);
            ball.LoadContent();
            ball.position = paddle.position;
            ball.position.Y -= ball.Height + paddle.Height;

            //Sounds
            ballBounceSFX = Content.Load<SoundEffect>("ball_bounce");
            ballHitSFX = Content.Load<SoundEffect>("ball_hit");
            powerUpSFX = Content.Load<SoundEffect>("powerup");
            deathSFX = Content.Load<SoundEffect>("death");

            // Blocks
            // For now, just create a row of blocks
            for (int i = 0; i != blockLayout.GetLength(0); i++)
            {
                for (int l = 0; l != blockLayout.GetLength(1); l++)
                {
                    Block tempBlock = new Block((Block.BlockColor)blockLayout[i, l], this);
                    tempBlock.LoadContent();
                    tempBlock.position = new Vector2(64 + l * 64, 100 + i * 32);
                    blocks.Add(tempBlock);
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            paddle.Update(deltaTime);
            ball.Update(deltaTime);

            foreach (PowerUp powerup in powerups)
            {
                CheckForPowerUps(powerup);
                powerup.Update(deltaTime);
            }
            CheckCollisions();
            base.Update(gameTime);
        }

        private void CheckForPowerUps(PowerUp powerup)
        {
            if (paddle.BoundingRect.Intersects(powerup.BoundingRect))
            {
                ActivatePowerUp(powerup);
            }

        }
        private void ActivatePowerUp(PowerUp powerup)
        {
            for (int i = powerups.Count - 1; i >= 0; i--)
            {
                if (powerups[i] == powerup)
                {
                    powerups[i].readyToDestroy = true;
                    destroyPowerUp = true;
                }
            }
            switch (powerup.powerUpType)
            {
                case (PowerUp.PowerUpType)0:
                    ballCatch = true;
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Red);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // Draw all sprites here
            spriteBatch.Draw(bgTexture, new Vector2(0, 0), Color.White);
            paddle.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            // Draw bricks
            foreach (Block b in blocks)
            {
                b.Draw(spriteBatch);
            }

            // Draw powerups
            foreach (PowerUp powerup in powerups)
            {
                powerup.Draw(spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void CheckCollisions()
        {
            float radius = ball.Width / 2;
                if (ball.caught)
                {
                    if (ball.tempBallPaddleRatio == new Vector2(0, 0))
                    {
                        ball.SetRatio(paddle.position);

                    }
                    ball.position = paddle.position + ball.tempBallPaddleRatio;

                }

            // Check for paddle
            if (ballWithPaddle == 0 &&
                (ball.position.X > (paddle.position.X - radius - paddle.Width / 2)) &&
                (ball.position.X < (paddle.position.X + radius + paddle.Width / 2)) &&
                (ball.position.Y < paddle.position.Y) &&
                (ball.position.Y > (paddle.position.Y - radius - paddle.Height / 2)) &&
                !ball.caught)
            {




                // Reflect based on which part of the paddle is hit

                // By default, set the normal to "up"
                Vector2 normal = -1.0f * Vector2.UnitY;

                // Distance from the leftmost to rightmost part of the paddle
                float dist = paddle.Width + radius * 2;
                // Where within this distance the ball is at
                float ballLocation = ball.position.X -
                    (paddle.position.X - radius - paddle.Width / 2);
                // Percent between leftmost and rightmost part of paddle
                float pct = ballLocation / dist;

                if (pct < 0.33f)
                {
                    normal = new Vector2(-0.196f, -0.981f);
                }
                else if (pct > 0.66f)
                {
                    normal = new Vector2(0.196f, -0.981f);
                }

                ball.direction = Vector2.Reflect(ball.direction, normal);
                // No collisions between ball and paddle for 20 frames
                ballWithPaddle = 20;
            }

            else if (ballWithPaddle > 0)
            {
                ballWithPaddle--;
            }

            // if ball catch is active
            if (ballWithPaddle == 20 && ballCatch && !ball.caught)
            {
                ball.caught = true;
                ball.tempBallPaddleRatio = ball.position - paddle.position;
            }

            // Check for blocks
            // First, let's see if we collided with any block
            Block collidedBlock = null;
            foreach (Block b in blocks)
            {
                if ((ball.position.X > (b.position.X - b.Width / 2 - radius)) &&
                    (ball.position.X < (b.position.X + b.Width / 2 + radius)) &&
                    (ball.position.Y > (b.position.Y - b.Height / 2 - radius)) &&
                    (ball.position.Y < (b.position.Y + b.Height / 2 + radius)))
                {
                    collidedBlock = b;
                    ballBounceSFX.Play();
                    break;
                }
            }

            // Now figure out how to reflect the ball
            if (collidedBlock != null)
            {
                // Assume that if our Y is close to the top or bottom of the block,
                // we're colliding with the top or bottom
                if ((ball.position.Y <
                    (collidedBlock.position.Y - collidedBlock.Height / 2)) ||
                    (ball.position.Y >
                    (collidedBlock.position.Y + collidedBlock.Height / 2)))
                {
                    ball.direction.Y = -1.0f * ball.direction.Y;
                }
                else // otherwise, we have to be colliding from the sides
                {
                    ball.direction.X = -1.0f * ball.direction.X;
                }

                // Now remove this block from the list
                blocks.Remove(collidedBlock);
                if (random.NextDouble() % 1 < spawnChance)
                {
                    SpawnPowerUp(collidedBlock.position);
                }
            }

            // Check walls
            if (Math.Abs(ball.position.X - 32) < radius)
            {
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.X - 992) < radius)
            {
                ball.direction.X = -1.0f * ball.direction.X;
            }
            else if (Math.Abs(ball.position.Y - 32) < radius)
            {
                ball.direction.Y = -1.0f * ball.direction.Y;
            }
            else if (ball.position.Y > (768 + radius))
            {
                LoseLife();
            }
            //Destroying powerup when it hits the paddle
            if (destroyPowerUp)
            {
                for (int i = powerups.Count - 1; i >= 0; i--)
                {
                    if (powerups[i].readyToDestroy)
                    {
                        PowerUp temppowerup = powerups[i];
                        powerups.Remove(temppowerup);
                    }
                }
                destroyPowerUp = false;
            }
        }

        //make a random type of powerup and spawn it at the position a block is broken
        private void SpawnPowerUp(Vector2 position)
        {
            int type = random.Next(3);
            PowerUp newpowerup = new PowerUp((PowerUp.PowerUpType)type, this);
            newpowerup.position = position;
            newpowerup.LoadContent();
            powerups.Add(newpowerup);
        }

        protected void LoseLife()
        {
            // Reset paddle and ball
            paddle.position = new Vector2(512, 740);
            ball.position = paddle.position;
            ball.position.Y -= ball.Height + paddle.Height;
            ball.direction = new Vector2(0.707f, -0.707f);
            deathSFX.Play();
        }
    }
}
