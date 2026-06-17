using System;

namespace SnakeGame
{
    public class GameOver
    {
        private readonly ScoreManager _scoreManager;

        public GameOver(ScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
        }

            public bool Show(CollisionType reason)
        {
            Console.Clear();
            PrintReason(reason);
            PrintStats();
            AskForHighScore();
            _scoreManager.PrintHighScores();
            return AskPlayAgain();
        }

            private void PrintReason(CollisionType reason)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            string msg = reason switch
            {
                CollisionType.Wall => "💥 Had narazil do zdi!",
                CollisionType.Self => "🐍 Had narazil sám do sebe!",
                _ => "Hra skončila."
            };

            Console.WriteLine($"  {msg}");
            Console.ResetColor();
            Console.WriteLine();
        }

            private void PrintStats()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ───────────── VÝSLEDKY ─────────────");
            Console.ResetColor();

            Console.Write("  Skóre:        ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(_scoreManager.CurrentScore);
            Console.ResetColor();

            Console.Write("  Level:        ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(_scoreManager.Level);
            Console.ResetColor();

            Console.Write("  Snědené jídlo: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(_scoreManager.FoodEaten);
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ─────────────────────────────────────");
            Console.ResetColor();
            Console.WriteLine();
        }

    
        private void AskForHighScore()
        {
            if (!_scoreManager.IsHighScore()) return;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  🏆 Dostals/a se do žebříčku!");
            Console.ResetColor();

            Console.Write("  Zadej své jméno: ");
            string name = Console.ReadLine()?.Trim() ?? "Hráč";

            if (string.IsNullOrWhiteSpace(name))
                name = "Hráč";

            _scoreManager.SaveScore(name);
            Console.WriteLine();
        }

      private bool AskPlayAgain()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("  Chceš hrát znovu? ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[A]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" ano  /  ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[N]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ne");
            Console.ResetColor();

            while (true)
            {
                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.A) return true;
                if (key == ConsoleKey.N) return false;

                if (key == ConsoleKey.Enter) return true;
            }
        }
    }
}
