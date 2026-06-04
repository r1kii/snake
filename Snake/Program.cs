using System;
using System.Threading; 

namespace Snake
{
    internal class Program
    {
        static void Main(string[] args)
        {
          
            Had had = new Had();

            Console.CursorVisible = false; 

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo klavesa = Console.ReadKey(true);

                    if (klavesa.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    switch (klavesa.Key)
                    {
                        case ConsoleKey.UpArrow:
                            had.AktualniSmer = Smer.Up;
                            break;
                        case ConsoleKey.DownArrow:
                            had.AktualniSmer = Smer.Down;
                            break;
                        case ConsoleKey.LeftArrow:
                            had.AktualniSmer = Smer.Left;
                            break;
                        case ConsoleKey.RightArrow:
                            had.AktualniSmer = Smer.Right;
                            break;
                    }
                }

                had.Pohyb();
                
                Thread.Sleep(100);
            }
        }
    }
}