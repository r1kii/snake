using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public enum CollisionType
    {
        None,       
        Wall,       
        Self,       
        Food        
    }

       public class CollisionDetector
    {
        private readonly int _boardWidth;
        private readonly int _boardHeight;

        public CollisionDetector(int boardWidth, int boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
        }

    
        public CollisionType Check(
            (int X, int Y) nextHead,
            List<(int X, int Y)> snakeBody,
            Food food)
        {
            if (IsWallCollision(nextHead))
                return CollisionType.Wall;

            if (IsSelfCollision(nextHead, snakeBody))
                return CollisionType.Self;

            if (IsFoodCollision(nextHead, food))
                return CollisionType.Food;

            return CollisionType.None;
        }

            public bool IsWallCollision((int X, int Y) head)
        {
            return head.X < 0
                || head.X >= _boardWidth
                || head.Y < 0
                || head.Y >= _boardHeight;
        }

            public bool IsSelfCollision(
            (int X, int Y) head,
            List<(int X, int Y)> snakeBody)
        {
    
            int checkUntil = snakeBody.Count - 1;

            for (int i = 0; i < checkUntil; i++)
            {
                if (snakeBody[i] == head)
                    return true;
            }

            return false;
        }

    
        public bool IsFoodCollision((int X, int Y) head, Food food)
        {
            return head.X == food.X && head.Y == food.Y;
        }

            public static (int X, int Y) GetNextHead((int X, int Y) currentHead, int direction)
        {
            return direction switch
            {
                0 => (currentHead.X, currentHead.Y - 1), // nahoru
                1 => (currentHead.X + 1, currentHead.Y), // doprava
                2 => (currentHead.X, currentHead.Y + 1), // dolů
                3 => (currentHead.X - 1, currentHead.Y), // doleva
                _ => throw new ArgumentException($"Neplatný směr: {direction}")
            };
        }

            public static bool IsOppositeDirection(int current, int next)
        {
            return (current == 0 && next == 2)
                || (current == 2 && next == 0)
                || (current == 1 && next == 3)
                || (current == 3 && next == 1);
        }

     public static void PrintCollisionMessage(CollisionType type)
        {
            Console.ForegroundColor = type switch
            {
                CollisionType.Wall => ConsoleColor.Red,
                CollisionType.Self => ConsoleColor.Magenta,
                CollisionType.Food => ConsoleColor.Green,
                _ => ConsoleColor.White
            };

            string message = type switch
            {
                CollisionType.Wall => "GAME OVER — Had narazil do zdi!",
                CollisionType.Self => "GAME OVER — Had narazil sám do sebe!",
                CollisionType.Food => "Nom nom! Had snědl jídlo.",
                CollisionType.None => "Žádná kolize.",
                _ => "Neznámá kolize."
            };

            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
