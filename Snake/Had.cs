using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Snake

{
    public enum Smer { Up, Down, Left, Right }
    internal class Had
    {
        public List<Point> Telo { get; set;}
        public Smer AktualniSmer { get; set; }

        public Had()

        {
            Telo = new List<Point>();

            Telo.Add(new Point(10, 10));
            Telo.Add(new Point(9, 10));
            Telo.Add(new Point(8, 10));

            AktualniSmer = Smer.Right;
        }
        public void Pohyb()
        {
            
            Point novaHlava = Telo[0];

            
            switch (AktualniSmer)
            {
                case Smer.Up:
                    novaHlava.Y--; 
                    break;
                case Smer.Down:
                    novaHlava.Y++; 
                    break;
                case Smer.Left:
                    novaHlava.X--; 
                    break;
                case Smer.Right:
                    novaHlava.X++; 
                    break;
            }

            
            Telo.Insert(0, novaHlava);

            
            Telo.RemoveAt(Telo.Count - 1);
        }
    }

}
