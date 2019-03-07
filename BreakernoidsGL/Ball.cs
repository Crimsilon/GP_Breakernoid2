using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BreakernoidsGL
{
    public class Ball : GameObject
    {
        public Vector2 direction = new Vector2(0.707f, -0.707f);
        public float speed = 500;
        public bool caught = false;
        public Vector2 tempBallPaddleRatio;
        public int balltimer = 0;


        public Ball(Game myGame) :
        base(myGame)
        {
            textureName = "ball";
        }




        public override void Update(float deltaTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Space))
            {
                caught = false;
                tempBallPaddleRatio = new Vector2(0, 0);
            }

            if (caught == false)
            {
                position += direction * speed * deltaTime;
            }

            base.Update(deltaTime);

        }




        public void SetRatio(Vector2 paddlePos)
        {

            if (tempBallPaddleRatio == new Vector2(0, 0) && caught)
            {
                tempBallPaddleRatio = paddlePos - this.position;
                if(0 > tempBallPaddleRatio.X)
                    {
                    direction = new Vector2(.707f, -.707f);
                }
                if (0 < tempBallPaddleRatio.X)
                {
                    direction = new Vector2(-.707f, -.707f);
                }
            }
        }
    }


}