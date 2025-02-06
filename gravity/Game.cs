// Include the namespaces (code libraries) you need below.
using System;
using System.Numerics;

// The namespace your code is in.
namespace MohawkGame2D
{
    /// <summary>
    ///     Your game code goes inside this class!
    /// </summary>
    public class Game
    {
        // Place your variables here:
        Vector2 position;
        Vector2 velocity;
        float radius = 25;
        float speed = 10; // units of speed per second
        float fricition= 0.7f;
        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetTitle("Gravity");
            Window.SetSize(400, 400);

            position = new Vector2(Window.Width / 2, radius);
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.White);

            //Accelerate velocity over time
            velocity.Y += Time.DeltaTime * speed;
            //update position based on velocity
            position += velocity;

            //Detect that ball hits bottom of screen
            bool isTouchingBottomEdge = position.Y + radius >= Window.Height;
            if (isTouchingBottomEdge)
            {
                //reposition to touch edge of screen
                position.Y = Window.Height - radius;
                //Reflect velocity 
                velocity.Y = -velocity.Y * fricition;

            }

            Draw.FillColor = Color.Red;
            Draw.Circle(position, radius);

            Draw.FillColor = Color.Green;
            Draw.Rectangle(275, 250, 75, 350);
            Draw.FillColor = Color.Green;
            Draw.Rectangle(275, 0, 75, 100);



            //
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space)) 
            {
                velocity.Y = -5;
            }

        }
    }

}
