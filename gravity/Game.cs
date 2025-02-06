using System;
using System.Numerics;

namespace MohawkGame2D
{
    public class Game
    {
        // Variables
        Vector2 position;
        Vector2 velocity;
        float radius = 25;
        float speed = 10; // units of speed per second
        float friction = 0.7f;
        float backgroundX = 0; // Horizontal scroll position for the background
        int colorIndex = 0; // Tracks the current color index

        // Color array (fixing array initialization)
        Color[] playerColor = { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Cyan, Color.Magenta, Color.White };

        // Pipe-related variables
        const int MaxPipes = 5; // Maximum number of pipes in the game at once
        Pipe[] pipes = new Pipe[MaxPipes]; // Array to hold the pipes
        int pipeIndex = 0; // Tracks the next available pipe slot

        // Score-related variables
        int score = 0; // The player's score
        float previousPipeX = 0; // Keeps track of the last pipe X position the bird passed

        public const float PipeWidth = 50; // Width of the pipe
        public const float GapHeight = 100; // The height of the gap between the pipes

        public class Pipe
        {
            public float X; // X position of the pipe
            public float TopHeight; // Random height for the top pipe

            public Pipe(float x)
            {
                X = x;
                TopHeight = Random.Float(50, 250); // Random height for the top pipe between 50 and 250
            }
        }

        /// <summary>
        ///     Setup runs once before the game loop begins.
        /// </summary>
        public void Setup()
        {
            Window.SetTitle("Jump Thing");
            Window.SetSize(400, 400);

            position = new Vector2(100, Window.Height / 2); // Start bird in the center vertically

            // Create initial pipes
            pipes[pipeIndex] = new Pipe(Window.Width + 100); // Add a pipe at the far right
            pipeIndex = (pipeIndex + 1) % MaxPipes; // Move to next pipe slot
        }

        /// <summary>
        ///     Update runs every frame.
        /// </summary>
        public void Update()
        {
            Window.ClearBackground(Color.Blue); // Set background color to sky blue

            // Scroll the background
            backgroundX -= speed * Time.DeltaTime; // Move the background to the left

            if (backgroundX <= -Window.Width) // Reset when background moves offscreen
                backgroundX = 0;

            // Update velocity and position of the bird
            velocity.Y += Time.DeltaTime * speed;
            position += velocity;

            // Detect collision with bottom of screen
            bool isTouchingBottomEdge = position.Y + radius >= Window.Height;
            if (isTouchingBottomEdge)
            {
                position.Y = Window.Height - radius;
                velocity.Y = -velocity.Y * friction;
            }

            // Move and draw the pipes
            for (int i = 0; i < MaxPipes; i++)
            {
                Pipe pipe = pipes[i];

                if (pipe == null) continue;

                // Move pipe to the left
                pipe.X -= 200 * Time.DeltaTime;

                // If pipe goes offscreen, remove it and add a new pipe
                if (pipe.X + PipeWidth < 0)
                {
                    pipes[i] = new Pipe(Window.Width + 25); // Add new pipe at the far right
                }

                // Check if the bird has passed the pipe (to increase the score)
                if (position.X > pipe.X + PipeWidth && previousPipeX != pipe.X)
                {
                    score++; // Increment score when passing a pipe
                    previousPipeX = pipe.X; // Update previousPipeX to the current pipe's X
                }

                // Draw the pipes
                Draw.FillColor = Color.Green;

                // Top pipe
                Draw.Rectangle(pipe.X, 0, PipeWidth, pipe.TopHeight);

                // Bottom pipe (calculated based on the gap height)
                Draw.Rectangle(pipe.X, pipe.TopHeight + GapHeight, PipeWidth, Window.Height - (pipe.TopHeight + GapHeight));
            }

            // Draw the bird with the color from the playerColor array
            Draw.FillColor = playerColor[colorIndex]; // Set the bird color
            Draw.Circle(position, radius);

            // Handle Circle jump on spacebar press
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space))
            {
                velocity.Y = -4; // Jump upward
            }

            // Change Color of Circle when Enter is pressed
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Enter))
            {
                colorIndex = (colorIndex + 1) % playerColor.Length; // Cycle through the colors
            }

            // Display the score
            Draw.FillColor = Color.White; // Set color for the score text
            Text.Draw($"Score: {score}", 10, 10); // Display score in the top-left corner
        }
    }
}
