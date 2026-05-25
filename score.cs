using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SnakeGame
{
    /// <summary>
    /// Represents a single high score entry.
    /// </summary>
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

        // Parameterless constructor needed for JSON deserialization
        public ScoreEntry() { }

        public override string ToString()
        {
            return $"{PlayerName,-15} {Score,6} bodů   Level {Level}   ({Date:dd.MM.yyyy HH:mm})";
        }
    }

    /// <summary>
    /// Manages current score, levels, and high score leaderboard.
    /// </summary>
    public class ScoreManager
    {
        // ── Current game state ──────────────────────────────────────────
        public int CurrentScore { get; private set; } = 0;
        public int Level { get; private set; } = 1;
        public int FoodEaten { get; private set; } = 0;

        // How many foods to eat before leveling up
        private const int FoodPerLevel = 5;

        // Score multiplier increases with level
        public double Multiplier => 1.0 + (Level - 1) * 0.25;

        // ── High scores ─────────────────────────────────────────────────
        private List<ScoreEntry> _highScores = new List<ScoreEntry>();
        private const int MaxHighScores = 10;
        private const string SaveFile = "highscores.json";

        public ScoreManager()
        {
            LoadHighScores();
        }

        // ── Scoring ─────────────────────────────────────────────────────

        /// <summary>
        /// Adds points for eating food. Applies level multiplier.
        /// Returns true if the player leveled up.
        /// </summary>
        public bool AddScore(int basePoints)
        {
            int earned = (int)Math.Round(basePoints * Multiplier);
            CurrentScore += earned;
            FoodEaten++;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"+{earned} bodů  (x{Multiplier:F2})");
            Console.ResetColor();

            // Check level up
            if (FoodEaten % FoodPerLevel == 0)
            {
                LevelUp();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Increases the level and notifies the player.
        /// </summary>
        private void LevelUp()
        {
            Level++;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"*** LEVEL UP! Jsi na levelu {Level} — had se zrychlí! ***");
            Console.ResetColor();
        }

        /// <summary>
        /// Returns the game speed delay in ms based on current level.
        /// Minimum 50ms so the game stays playable.
        /// </summary>
        public int GetSpeedMs()
        {
            int speed = 200 - (Level - 1) * 20;
            return Math.Max(speed, 50);
        }

        // ── High score table ─────────────────────────────────────────────

        /// <summary>
        /// Checks if the current score qualifies for the leaderboard.
        /// </summary>
        public bool IsHighScore()
        {
            if (_highScores.Count < MaxHighScores) return true;
            return CurrentScore > _highScores[^1].Score;
        }

        /// <summary>
        /// Saves the current score to the leaderboard and persists to disk.
        /// </summary>
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

        /// <summary>
        /// Prints the leaderboard to the console.
        /// </summary>
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

        /// <summary>
        /// Prints the current score and level (for the HUD).
        /// </summary>
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

        // ── Persistence ──────────────────────────────────────────────────

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