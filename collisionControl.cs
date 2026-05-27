using System;
using System.Collections.Generic;

namespace SnakeGame
{
    /// <summary>
    /// Výsledek kontroly kolize.
    /// </summary>
    public enum CollisionType
    {
        None,       // Žádná kolize
        Wall,       // Had narazil do zdi
        Self,       // Had narazil sám do sebe
        Food        // Had snědl jídlo
    }

    /// <summary>
    /// Handles all collision detection for the Snake game.
    /// </summary>
    public class CollisionDetector
    {
        private readonly int _boardWidth;
        private readonly int _boardHeight;

        public CollisionDetector(int boardWidth, int boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
        }

        // ── Hlavní metoda ────────────────────────────────────────────────

        /// <summary>
        /// Zkontroluje všechny typy kolizí pro příští pozici hlavy hada.
        /// Volej PŘED tím, než had udělá krok.
        /// </summary>
        /// <param name="nextHead">Příští pozice hlavy hada.</param>
        /// <param name="snakeBody">Celé tělo hada (včetně současné hlavy).</param>
        /// <param name="food">Aktuální jídlo na hrací ploše.</param>
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

        // ── Jednotlivé kontroly ──────────────────────────────────────────

        /// <summary>
        /// Vrátí true pokud had vyleze mimo hrací plochu.
        /// </summary>
        public bool IsWallCollision((int X, int Y) head)
        {
            return head.X < 0
                || head.X >= _boardWidth
                || head.Y < 0
                || head.Y >= _boardHeight;
        }

        /// <summary>
        /// Vrátí true pokud hlava hada narazí do jeho těla.
        /// Poslední článek těla ignorujeme — ten se v tomto kroku posune.
        /// </summary>
        public bool IsSelfCollision(
            (int X, int Y) head,
            List<(int X, int Y)> snakeBody)
        {
            // Ignorujeme poslední článek (ocas), ten se posune pryč
            int checkUntil = snakeBody.Count - 1;

            for (int i = 0; i < checkUntil; i++)
            {
                if (snakeBody[i] == head)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Vrátí true pokud hlava hada je na stejné pozici jako jídlo.
        /// </summary>
        public bool IsFoodCollision((int X, int Y) head, Food food)
        {
            return head.X == food.X && head.Y == food.Y;
        }

        // ── Pomocné metody ───────────────────────────────────────────────

        /// <summary>
        /// Vypočítá příští pozici hlavy hada podle směru pohybu.
        /// Směr: 0 = nahoru, 1 = doprava, 2 = dolů, 3 = doleva
        /// </summary>
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

        /// <summary>
        /// Zabrání hadovi otočit se o 180° (zpět do sebe).
        /// </summary>
        public static bool IsOppositeDirection(int current, int next)
        {
            return (current == 0 && next == 2)
                || (current == 2 && next == 0)
                || (current == 1 && next == 3)
                || (current == 3 && next == 1);
        }

        /// <summary>
        /// Vypíše do konzole co se stalo (pro debug nebo game over screen).
        /// </summary>
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