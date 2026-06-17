using System;
using System.Collections.Generic;

namespace SnakeGame
{
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
    
    public class FoodGenerator
    {
        private readonly int _boardWidth;
        private readonly int _boardHeight;
        private readonly Random _random;

            private readonly Dictionary<char, int> _foodTypes = new Dictionary<char, int>
        {
            { '*', 10 },  
            { '$', 25 },   
            { '@', 50 },   
        };

        public FoodGenerator(int boardWidth, int boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
            _random = new Random();
        }

        
        public Food? GenerateFood(List<(int X, int Y)> occupiedCells)
        {
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
                return null;

            var chosen = freeCells[_random.Next(freeCells.Count)];

         
            char symbol = PickFoodType();
            int points = _foodTypes[symbol];

            return new Food(chosen.X, chosen.Y, symbol, points);
        }
        private char PickFoodType()
        {
            int roll = _random.Next(100);

            if (roll < 70) return '*';   
            if (roll < 90) return '$';   
            return '@';                  
        }
    }
}
