using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Snake
{
    internal class Had
    {
        public List<Point> Telo { get; set; }

        
        public Had()
        {
            Telo = new List<Point>();

           
            Telo.Add(new Point(10, 10)); 
            Telo.Add(new Point(9, 10));  
            Telo.Add(new Point(8, 10)); 
        }
    }
}
