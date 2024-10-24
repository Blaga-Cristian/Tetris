using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Cell
    {
        public Cell()
        {
            Color = Color.Transparent;
        }

        public Cell(int x, int y, Color color, bool inMiscare)
        {
            X = x;
            Y = y;
            Color = color;
            InMiscare = inMiscare;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
        public bool InMiscare { get; set; }
    }
}
