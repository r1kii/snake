using System;
using System.Collections.Generic;

namespace SnakeGame
{

    public class Renderer
    {
        private readonly int _boardWidth;
        private readonly int _boardHeight;

       private const char SnakeHead = 'O';
        private const char SnakeBody = 'o';
        private const char WallH = '─';
        private const char WallV = '│';
        private const char CornerTL = '╔';
        private const char CornerTR = '╗';
        private const char CornerBL = '╚';
        private const char CornerBR = '╝';

        private const int HudLines = 2;

        public Renderer(int boardWidth, int boardHeight)
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;

            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

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

        public void DrawHud(ScoreManager scoreManager)
        {
            SetCursor(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(new string(' ', Console.WindowWidth - 1)); 
            SetCursor(0, 0);
            scoreManager.PrintCurrentScore();
            Console.ResetColor();
        }
      public void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            SetCursor(0, HudLines);
            Console.Write(CornerTL);
            Console.Write(new string(WallH, _boardWidth));
            Console.Write(CornerTR);


            for (int y = 0; y < _boardHeight; y++)
            {
                SetCursor(0, HudLines + 1 + y);
                Console.Write(WallV);
                SetCursor(_boardWidth + 1, HudLines + 1 + y);
                Console.Write(WallV);
            }

            SetCursor(0, HudLines + _boardHeight + 1);
            Console.Write(CornerBL);
            Console.Write(new string(WallH, _boardWidth));
            Console.Write(CornerBR);

            Console.ResetColor();
        }

        public void DrawSnake(List<(int X, int Y)> body)
        {
            for (int i = 0; i < body.Count; i++)
            {
                DrawSnakeSegment(body[i], isHead: i == 0);
            }
        }

          public void UpdateSnake(
            (int X, int Y) newHead,
            (int X, int Y) oldNeck,  
            (int X, int Y) removedTail)
        {
            
            ClearCell(removedTail);

            DrawSnakeSegment(oldNeck, isHead: false);

            DrawSnakeSegment(newHead, isHead: true);
        }

        private void DrawSnakeSegment((int X, int Y) pos, bool isHead)
        {
            SetBoardCursor(pos.X, pos.Y);
            Console.ForegroundColor = isHead ? ConsoleColor.Green : ConsoleColor.DarkGreen;
            Console.Write(isHead ? SnakeHead : SnakeBody);
            Console.ResetColor();
        }

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

        public void ClearFood(Food food)
        {
            ClearCell((food.X, food.Y));
        }

        private void SetBoardCursor(int x, int y)
        {
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
