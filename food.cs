using System;
using System.Collections.Generic;

namespace SnakeGame
{
    /// <summary>
    /// Represents a food item on the game board.
    /// </summary>
    public class Food
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
        public int Points { get; set; }

        public Food(int x, int y, char symbol = '*', int points = 10)
        {
            X = x;
            Y = y;
            Symbol = symbol;
            Points = points;
        }
    }

    /// <summary>
    /// Handles food generation and placement on the game board.
    /// </summary>
    public class FoodGenerator
    {
        private readonly int _boardWidth;
        private readonly int _boardHeight;
        private readonly Random _random;

        // Special food types: symbol -> points value
        private readonly Dictionary<char, int> _foodTypes = new Dictionary<char, int>
        {
            { '*', 10 },   // Normal food
            { '$', 25 },   // Bonus food
            { '@', 50 },   // Super food (rare)
        };

        public FoodGenerator(int boardWidth, int boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
            _random = new Random();
        }

        /// <summary>
        /// Generates a new food item at a random position not occupied by the snake.
        /// Returns null if no free cell is available.
        /// </summary>
        /// <param name="occupiedCells">List of (X, Y) positions occupied by the snake.</param>
        public Food? GenerateFood(List<(int X, int Y)> occupiedCells)
        {
            // Build a list of all free cells
            var freeCells = new List<(int X, int Y)>();

            for (int x = 0; x < _boardWidth; x++)
            {
                for (int y = 0; y < _boardHeight; y++)
                {
                    if (!occupiedCells.Contains((x, y)))
                        freeCells.Add((x, y));
                }
            }

            if (freeCells.Count == 0)
                return null; // Board is full — player wins!

            // Pick a random free cell
            var chosen = freeCells[_random.Next(freeCells.Count)];

            // Pick food type (weighted: normal 70%, bonus 20%, super 10%)
            char symbol = PickFoodType();
            int points = _foodTypes[symbol];

            return new Food(chosen.X, chosen.Y, symbol, points);
        }

        /// <summary>
        /// Picks a food type based on weighted probability.
        /// </summary>
        private char PickFoodType()
        {
            int roll = _random.Next(100); // 0–99

            if (roll < 70) return '*';   // 70% — normal
            if (roll < 90) return '$';   // 20% — bonus
            return '@';                  // 10% — super
        }
    }
}
