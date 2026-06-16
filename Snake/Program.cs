using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame 
{
    internal class Program
    {
        
        private const int SirkaPlochy = 40;
        private const int VyskaPlochy = 20;

        static void Main(string[] args)
        {
            
            ScoreManager scoreManager = new ScoreManager();
            Renderer renderer = new Renderer(SirkaPlochy, VyskaPlochy);
            FoodGenerator foodGenerator = new FoodGenerator(SirkaPlochy, VyskaPlochy);
            CollisionDetector collisionDetector = new CollisionDetector(SirkaPlochy, VyskaPlochy);
            GameOver gameOverScreen = new GameOver(scoreManager);

            bool hratZnovu = true;

            while (hratZnovu)
            {
                
                scoreManager.Reset();
                Had had = new Had(); 

                
                List<(int X, int Y)> obsazeneBunky = PrevedTeloHada(had.Telo);
                Food? jidlo = foodGenerator.GenerateFood(obsazeneBunky);

                if (jidlo == null) return; 

                
                renderer.DrawAll(obsazeneBunky, jidlo, scoreManager);

                bool konecHry = false;
                CollisionType duvodKonce = CollisionType.None;

                
                while (!konecHry)
                {
                    
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo klavesa = Console.ReadKey(true);

                        if (klavesa.Key == ConsoleKey.Escape)
                        {
                            konecHry = true;
                            hratZnovu = false;
                            break;
                        }

                        
                        switch (klavesa.Key)
                        {
                            case ConsoleKey.UpArrow:
                                if (had.AktualniSmer != Smer.Down) had.AktualniSmer = Smer.Up;
                                break;
                            case ConsoleKey.DownArrow:
                                if (had.AktualniSmer != Smer.Up) had.AktualniSmer = Smer.Down;
                                break;
                            case ConsoleKey.LeftArrow:
                                if (had.AktualniSmer != Smer.Right) had.AktualniSmer = Smer.Left;
                                break;
                            case ConsoleKey.RightArrow:
                                if (had.AktualniSmer != Smer.Left) had.AktualniSmer = Smer.Right;
                                break;
                        }
                    }

                   
                    (int pristiX, int pristiY) = VypocitejPristiHlavu(had.Telo[0], had.AktualniSmer);
                    obsazeneBunky = PrevedTeloHada(had.Telo);

                    
                    CollisionType kolize = collisionDetector.Check((pristiX, pristiY), obsazeneBunky, jidlo);

                    if (kolize == CollisionType.Wall || kolize == CollisionType.Self)
                    {
                        konecHry = true;
                        duvodKonce = kolize;
                    }
                    else if (kolize == CollisionType.Food)
                    {
                        
                        had.Rust();

                        
                        bool levelUp = scoreManager.AddScore(jidlo.Points);

                        
                        obsazeneBunky = PrevedTeloHada(had.Telo);
                        jidlo = foodGenerator.GenerateFood(obsazeneBunky);

                        if (jidlo == null)
                        {
                            Console.Clear();
                            Console.WriteLine("GRATULACE! Vyhrál/a jsi, celá plocha je plná hada!");
                            konecHry = true;
                            hratZnovu = false;
                            break;
                        }

                      
                        if (levelUp)
                        {
                            renderer.DrawAll(obsazeneBunky, jidlo, scoreManager);
                        }
                        else
                        {
                            renderer.DrawHud(scoreManager);
                            renderer.DrawFood(jidlo);
                            renderer.DrawSnake(obsazeneBunky);
                        }
                    }
                    else 
                    {
                        
                        var staryOcas = (had.Telo[had.Telo.Count - 1].X, had.Telo[had.Telo.Count - 1].Y);
                        var staryKrk = (had.Telo[0].X, had.Telo[0].Y);

                      
                        had.Pohyb();

                        var novaHlava = (had.Telo[0].X, had.Telo[0].Y);

                      
                        renderer.UpdateSnake(novaHlava, staryKrk, staryOcas);
                    }

                    
                    Thread.Sleep(scoreManager.GetSpeedMs());
                }

               
                if (duvodKonce != CollisionType.None)
                {
                    hratZnovu = gameOverScreen.Show(duvodKonce);
                }
            }

            Console.Clear();
            Console.WriteLine("Děkujeme za hraní!");
        }

        private static List<(int X, int Y)> PrevedTeloHada(List<System.Drawing.Point> telo)
        {
            var seznam = new List<(int X, int Y)>();
            foreach (var bod in telo)
            {
                seznam.Add((bod.X, bod.Y));
            }
            return seznam;
        }

        
        private static (int X, int Y) VypocitejPristiHlavu(System.Drawing.Point hlava, Smer smer)
        {
            return smer switch
            {
                Smer.Up => (hlava.X, hlava.Y - 1),
                Smer.Down => (hlava.X, hlava.Y + 1),
                Smer.Left => (hlava.X - 1, hlava.Y),
                Smer.Right => (hlava.X + 1, hlava.Y),
                _ => (hlava.X, hlava.Y)
            };
        }
    }
}
