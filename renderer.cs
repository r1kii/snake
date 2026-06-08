using System;
using System.Collections.Generic;

namespace SnakeGame
{
    /// <summary>
    /// Handles all rendering for the Snake game.
    /// </summary>
    public class Renderer
    {
        private readonly int _boardWidth;
        private readonly int _boardHeight;

        // Symboly
        private const char SnakeHead = 'O';
        private const char SnakeBody = 'o';
        private const char WallH = '─';
        private const char WallV = '│';
        private const char CornerTL = '╔';
        private const char CornerTR = '╗';
        private const char CornerBL = '╚';
        private const char CornerBR = '╝';

        // HUD je nad hrací plochou — offset pro správné pozicování
        private const int HudLines = 2;

        public Renderer(int boardWidth, int boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;

            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        // ── Celá obrazovka ───────────────────────────────────────────────

        /// <summary>
        /// Vykreslí vše od nuly — volej na začátku hry nebo po restartu.
        /// </summary>
        public void DrawAll(
            List<(int X, int Y)> snakeBody,
            Food food,
            ScoreManager scoreManager)
        {
            Console.Clear();
            DrawHud(scoreManager);
            DrawBorder();
            DrawFood(food);
            DrawSnake(snakeBody);
        }

        // ── HUD (skóre nahoře) ───────────────────────────────────────────

        /// <summary>
        /// Překreslí HUD řádek (skóre, level, jídlo).
        /// Volej po každé změně skóre.
        /// </summary>
        public void DrawHud(ScoreManager scoreManager)
        {
            SetCursor(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(new string(' ', Console.WindowWidth - 1)); // Vymaž řádek
            SetCursor(0, 0);
            scoreManager.PrintCurrentScore();
            Console.ResetColor();
        }

        // ── Hranice hrací plochy ─────────────────────────────────────────

        /// <summary>
        /// Vykreslí rámeček kolem hrací plochy. Volej jen jednou.
        /// </summary>
        public void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            // Horní hrana
            SetCursor(0, HudLines);
            Console.Write(CornerTL);
            Console.Write(new string(WallH, _boardWidth));
            Console.Write(CornerTR);

            // Boční hrany
            for (int y = 0; y < _boardHeight; y++)
            {
                SetCursor(0, HudLines + 1 + y);
                Console.Write(WallV);
                SetCursor(_boardWidth + 1, HudLines + 1 + y);
                Console.Write(WallV);
            }

            // Dolní hrana
            SetCursor(0, HudLines + _boardHeight + 1);
            Console.Write(CornerBL);
            Console.Write(new string(WallH, _boardWidth));
            Console.Write(CornerBR);

            Console.ResetColor();
        }

        // ── Had ──────────────────────────────────────────────────────────

        /// <summary>
        /// Vykreslí celého hada. Volej jen při prvním renderu.
        /// Pro pohyb používej UpdateSnake().
        /// </summary>
        public void DrawSnake(List<(int X, int Y)> body)
        {
            for (int i = 0; i < body.Count; i++)
            {
                DrawSnakeSegment(body[i], isHead: i == 0);
            }
        }

        /// <summary>
        /// Efektivní update hada — vykreslí novou hlavu a smaže starý ocas.
        /// Volej každý herní tick místo DrawSnake().
        /// </summary>
        public void UpdateSnake(
            (int X, int Y) newHead,
            (int X, int Y) oldNeck,   // druhý článek — přebarví se z hlavy na tělo
            (int X, int Y) removedTail)
        {
            // Smaž starý ocas
            ClearCell(removedTail);

            // Přebarvi starý neck (byl hlava, teď je tělo)
            DrawSnakeSegment(oldNeck, isHead: false);

            // Nakresli novou hlavu
            DrawSnakeSegment(newHead, isHead: true);
        }

        private void DrawSnakeSegment((int X, int Y) pos, bool isHead)
        {
            SetBoardCursor(pos.X, pos.Y);
            Console.ForegroundColor = isHead ? ConsoleColor.Green : ConsoleColor.DarkGreen;
            Console.Write(isHead ? SnakeHead : SnakeBody);
            Console.ResetColor();
        }

        // ── Jídlo ────────────────────────────────────────────────────────

        /// <summary>
        /// Vykreslí jídlo na hrací ploše.
        /// </summary>
        public void DrawFood(Food food)
        {
            SetBoardCursor(food.X, food.Y);
            Console.ForegroundColor = food.Symbol switch
            {
                '$' => ConsoleColor.Yellow,
                '@' => ConsoleColor.Magenta,
                _ => ConsoleColor.Red
            };
            Console.Write(food.Symbol);
            Console.ResetColor();
        }

        /// <summary>
        /// Smaže jídlo z obrazovky (použij před vygenerováním nového).
        /// </summary>
        public void ClearFood(Food food)
        {
            ClearCell((food.X, food.Y));
        }

        // ── Pomocné metody ───────────────────────────────────────────────

        /// <summary>
        /// Přepočítá souřadnice herní plochy na konzolové souřadnice (bere v úvahu rámeček a HUD).
        /// </summary>
        private void SetBoardCursor(int x, int y)
        {
            // +1 kvůli levé stěně rámečku, HudLines + 1 kvůli HUDu a horní stěně
            SetCursor(x + 1, y + HudLines + 1);
        }

        private static void SetCursor(int x, int y)
        {
            Console.SetCursorPosition(x, y);
        }

        private void ClearCell((int X, int Y) pos)
        {
            SetBoardCursor(pos.X, pos.Y);
            Console.Write(' ');
        }
    }
}