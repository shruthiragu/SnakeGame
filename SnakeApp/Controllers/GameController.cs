﻿

using SnakeApp.Models;
using SnakeApp.Views;

namespace SnakeApp.Controllers
{
    public class GameController  // This manages and controls the game flow, handling user inputs, and updating the model
    {
        int speedInput;
        private string? input;
        char[] DirectionChars = ['^', 'v', '<', '>'];
        Game game;
        Direction? direction = null;
        private int gameWidth;
        private int gameHeight;
        private Coordinate? foodCoordinate;
        private Coordinate? currentPosition;
        ConsoleView consoleView;
        public GameState GameState { get; private set; }
        Tile[,] map;
        string speedInputQuestion = "Please enter your desired speed (1-->Low, 2-->Medium, 3-->High";
        public void Start()
        {
            // Implement the game loop here: process input, update game state, render view, etc.
            InitGame();
            var cursorPos5 = Console.GetCursorPosition();
            while (true)
            {
                if (GameState == GameState.InProgress)
                {
                    StartSnakeGame();
                }
                if (GameState == GameState.EndGame)
                {
                    Console.SetCursorPosition(1, 1);
                    Console.WriteLine($"Game Over!! Your score is {game.Snake.SnakeQueue.Count}!");
                    break;
                }

            }
        }

        private void StartSnakeGame()
        {
            var cursorPos4 = Console.GetCursorPosition();

            while (!direction.HasValue && GameState == GameState.InProgress)
            {
                GetDirection();
            }
            while (GameState == GameState.InProgress)
            {
                var cursorPos1 = Console.GetCursorPosition();
                if (Console.WindowWidth != gameWidth || Console.WindowHeight != gameHeight)
                {
                    Console.Clear();
                    Console.WriteLine("Console was resized. Game Over!");
                    GameState = GameState.EndGame;
                    break;
                }
                var currentPos = new Coordinate(currentPosition!.X,currentPosition.Y);
                var cursorPos2 = Console.GetCursorPosition();
                //Update snake position (coordinates) based on user input
                if (currentPosition is null || currentPos is null) // JOSH: attempting to fix null reference exception
                {
                    throw new System.NullReferenceException("currentPosition is null");
                }
                else
                {
                    UpdateSnakePosition(currentPos);
                }
                var cursorPos3 = Console.GetCursorPosition();
                if (!CheckIfPositionIsValid(currentPos))
                {
                    break;
                }

                //Set Cursor position
                Console.SetCursorPosition(currentPos.X,currentPos.Y);
                //var cursorPos = Console.GetCursorPosition();
                //Console.Write(DirectionChars[(int)direction!]);
                Console.Write("*");
                //consoleView.DrawSnake(game.Snake);

                //Save snake's position
                game.Snake.SnakeQueue.Enqueue(currentPos);


                if (map[currentPos.X, currentPos.Y] == Tile.Food)
                {
                    //If snake eats food successfully, update food position
                    foodCoordinate = game.Food.CalculateNexFoodPosition();
                    consoleView.DrawFood(game.Food);
                }
                else
                {
                    //Update old position so that the snake moves within the console along its path.
                    UpdateOldPositionOfSnake();
                }

                if (Console.KeyAvailable)
                {
                    GetDirection();
                }
                Thread.Sleep(game.Snake.GetSnakeSleepDuration());
                currentPosition = currentPos;
            }
        }

        private void UpdateOldPositionOfSnake()
        {
            var oldPosition = game.Snake.SnakeQueue.Dequeue();
            map[oldPosition.X, oldPosition.Y] = Tile.Empty;
            Console.SetCursorPosition(oldPosition.X, oldPosition.Y);
            var currCursorPosition = Console.GetCursorPosition();
            Console.Write(' ');
            var currCursorPosition2 = Console.GetCursorPosition();
        }

        private bool CheckIfPositionIsValid(Coordinate currentPosition)
        {
            if (currentPosition.X < 0 || currentPosition.Y < 0 ||
                currentPosition.X >= game.Board.width ||
                currentPosition.Y >= game.Board.height ||
                //game.Board.map is Tile.Snake) // JOSH: replaced this line with below code to fix syntax error
                map[currentPosition.X, currentPosition.Y] == Tile.Snake)
            {
                Console.Clear();
                //Console.WriteLine($"Game Over!! Your score is {game.Snake.SnakeQueue.Count}");
                GameState = GameState.EndGame;
                return false;
            }
            return true;
        }

        private void UpdateSnakePosition(Coordinate currentPos)
        {
            switch (direction)
            {
                case Direction.Left:
                    currentPos.X--; break;
                case Direction.Right:
                    currentPos.X++; break;
                case Direction.Up:
                    currentPos.Y--; break;
                case Direction.Down:
                    currentPos.Y++; break;
            }
        }

        private void GetDirection()
        {
            //Takes direction from user

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    direction = Direction.Up; break;
                case ConsoleKey.DownArrow:
                    direction = Direction.Down; break;
                case ConsoleKey.LeftArrow:
                    direction = Direction.Left; break;
                case ConsoleKey.RightArrow:
                    direction = Direction.Right; break;
                case ConsoleKey.Escape:
                    GameState = GameState.EndGame; break;
            }
        }

        private void InitGame()
        {
            

            //Game model constructs the Snake, Food and Board objects
            //game = new Game(speedInput); // JOSH: moved to constructor

            //Get Console Dimensions
            UpdateConsoleDimensions();
            
            //Get initial position of snake
            currentPosition = game.Snake.GetCurrentPosition();

            //Get the initial position of food
            foodCoordinate = game.Food.GetCurrentPosition();

            var cursorPos2 = Console.GetCursorPosition();
            //Save the initial position of snake and set its tile to snake
            game.Snake.SnakeQueue.Enqueue(currentPosition);
            map[game.Snake.SnakeCoordinate.X, game.Snake.SnakeCoordinate.Y] = Tile.Snake;

            ////Display the starting position of snake
            //Console.SetCursorPosition(currentPosition.X, currentPosition.Y);
            //Console.Write('*');
            var cursorPos1 = Console.GetCursorPosition();
            // Render game view
            consoleView = new ConsoleView();
            var cursorPos4 = Console.GetCursorPosition();
            consoleView.Render(game);
            var cursorPos3 = Console.GetCursorPosition();
            GameState = GameState.InProgress;
        }

        private void UpdateConsoleDimensions()
        {
            gameWidth = game.Board.width;
            gameHeight = game.Board.height;
            map = game.Board.map;
        }

        private void GetUserSpeed()
        {
            while (!int.TryParse(input = Console.ReadLine(), out speedInput) || speedInput < 1 || speedInput > 3)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    speedInput = 1;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input, please try again...");
                    Console.Write(speedInputQuestion);
                }
            }
        }

        public GameController()
        {
            GameState = GameState.WaitingToStart;
            //Welcome Message
            Console.WriteLine("Welcome to the Game of Snake!");

            //Get user speed
            GameState = GameState.WaitingForUserInput;
            Console.WriteLine(speedInputQuestion);
            GetUserSpeed();

            game = new Game(speedInput); // JOSH: moved from InitGame()

            //game = new Game(); // JOSH: attempting to fix null reference exception
            map = game.Board.map; // JOSH: attempting to fix null reference exception
            currentPosition = game.Snake.GetCurrentPosition(); // JOSH: attempting to fix null reference exception
        }
    }
}
