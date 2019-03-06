﻿using System;
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
    public class Ball : GameObject {
        public Vector2 direction = new Vector2(0.707f, -0.707f);
        public float speed = 500;
        public bool caught = false;
        public Ball(Game myGame) :
        base(myGame)
        {
            textureName = "ball";
        }
        public override void Update(float deltaTime)
        {
            if (caught == false)
            {
                position += direction * speed * deltaTime;
            }

            base.Update(deltaTime);

        }
    }


}