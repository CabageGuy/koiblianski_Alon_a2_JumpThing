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

        // Flavor Element-related variables (e.g., clouds)
        const int MaxClouds = 3; // Number of clouds (flavor elements)
        Cloud[] clouds = new Cloud[MaxClouds]; // Array to hold the clouds

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

        // Flavor Element Class (Clouds)
        public class Cloud
        {
            public float X; // X position of the cloud
            public float Y; // Y position of the cloud
            public float Speed; // Speed at which the cloud moves
            public float Width; // Width of the cloud (ellipse)
            public float Height; // Height of the cloud (ellipse)

            public Cloud(float x, float y, float speed, float width, float height)
            {
                X = x;
                Y = y;
                Speed = speed;
                Width = width;
                Height = height;
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

            // Create initial clouds (flavor elements)
            for (int i = 0; i < MaxClouds; i++)
            {
                float cloudY = Random.Float(50, Window.Height / 2); // Random Y position
                float cloudSpeed = Random.Float(30, 50); // Random speed for each cloud
                float cloudWidth = Random.Float(50, 150); // Random width for cloud ellipse
                float cloudHeight = Random.Float(20, 60); // Random height for cloud ellipse

                clouds[i] = new Cloud(Window.Width + 100 + (i * 100), cloudY, cloudSpeed, cloudWidth, cloudHeight); // Spread clouds out
            }
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


            // Move and draw the clouds (flavor elements)
            for (int i = 0; i < MaxClouds; i++)
            {
                Cloud cloud = clouds[i];

                // Move cloud to the left
                cloud.X -= cloud.Speed * Time.DeltaTime;

                // If cloud goes offscreen, reset its position
                if (cloud.X + cloud.Width < 0)
                {
                    cloud.X = Window.Width + 100;
                    cloud.Y = Random.Float(50, Window.Height / 2); // Randomize the Y position again
                }

                // Draw the cloud as an ellipse
                Draw.FillColor = Color.White;
                Draw.Ellipse(cloud.X, cloud.Y, cloud.Width, cloud.Height); // Draw as ellipse
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
