using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SnakeGame
{
    public class ScoreEntry
    {
        public string PlayerName { get; set; } = "Player";
        public int Score { get; set; }
        public int Level { get; set; }
        public DateTime Date { get; set; }

        public ScoreEntry(string playerName, int score, int level)
        {
            PlayerName = playerName;
            Score = score;
            Level = level;
            Date = DateTime.Now;
        }

        public ScoreEntry() { }

        public override string ToString()
        {
            return $"{PlayerName,-15} {Score,6} bodů   Level {Level}   ({Date:dd.MM.yyyy HH:mm})";
        }
    }

    public class ScoreManager
    {
        public int CurrentScore { get; private set; } = 0;
        public int Level { get; private set; } = 1;
        public int FoodEaten { get; private set; } = 0;

        private const int FoodPerLevel = 5;

        public double Multiplier => 1.0 + (Level - 1) * 0.25;

        private List<ScoreEntry> _highScores = new List<ScoreEntry>();
        private const int MaxHighScores = 10;
        private const string SaveFile = "highscores.json";

        public ScoreManager()
        {
            LoadHighScores();
        }


        public bool AddScore(int basePoints)
        {
            int earned = (int)Math.Round(basePoints * Multiplier);
            CurrentScore += earned;
            FoodEaten++;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"+{earned} bodů  (x{Multiplier:F2})");
            Console.ResetColor();

        if (FoodEaten % FoodPerLevel == 0)
            {
                LevelUp();
                return true;
            }

            return false;
        }
        private void LevelUp()
        {
            Level++;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"*** LEVEL UP! Jsi na levelu {Level} — had se zrychlí! ***");
            Console.ResetColor();
        }
        public int GetSpeedMs()
        {
            int speed = 200 - (Level - 1) * 20;
            return Math.Max(speed, 50);
        }
      public bool IsHighScore()
        {
            if (_highScores.Count < MaxHighScores) return true;
            return CurrentScore > _highScores[^1].Score;
        }
      public void SaveScore(string playerName)
        {
            var entry = new ScoreEntry(playerName, CurrentScore, Level);
            _highScores.Add(entry);
            _highScores.Sort((a, b) => b.Score.CompareTo(a.Score));

            if (_highScores.Count > MaxHighScores)
                _highScores.RemoveAt(_highScores.Count - 1);

            SaveHighScores();
            Console.WriteLine($"Skóre uloženo pro hráče: {playerName}");
        }

        public void PrintHighScores()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n══════════════ ŽEBŘÍČEK ══════════════");
            Console.ResetColor();

            if (_highScores.Count == 0)
            {
                Console.WriteLine("Zatím žádné záznamy.");
            }
            else
            {
                for (int i = 0; i < _highScores.Count; i++)
                {
                    Console.ForegroundColor = i == 0 ? ConsoleColor.Yellow : ConsoleColor.White;
                    Console.WriteLine($"  {i + 1,2}. {_highScores[i]}");
                }
            }

            Console.ResetColor();
            Console.WriteLine("══════════════════════════════════════\n");
        }

        public void PrintCurrentScore()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Skóre: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{CurrentScore,6}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"   Level: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{Level}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"   Jídlo: {FoodEaten}");
            Console.ResetColor();
        }

        

        private void SaveHighScores()
        {
            try
            {
                string json = JsonSerializer.Serialize(_highScores, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SaveFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání skóre: {ex.Message}");
            }
        }

        private void LoadHighScores()
        {
            try
            {
                if (!File.Exists(SaveFile)) return;

                string json = File.ReadAllText(SaveFile);
                _highScores = JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání skóre: {ex.Message}");
                _highScores = new List<ScoreEntry>();
            }
        }

        /// <summary>
        /// Resets the current game state (but keeps high scores).
        /// </summary>
        public void Reset()
        {
            CurrentScore = 0;
            Level = 1;
            FoodEaten = 0;
        }
    }
}
