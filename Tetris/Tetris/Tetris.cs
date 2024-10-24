using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Tetris : Form
    {
        static string respath = $@"{Directory.GetCurrentDirectory()}\..\..\..\Resources\";

        SoundPlayer sp = new SoundPlayer(respath + $@"clear.wav");
        SoundPlayer background = new SoundPlayer(respath + $@"33466531_game-music-techno-tetris-emotional-_by_theloki_preview.wav");
        int level = 1;
        int scor;
        Cell urcell;
        int currTetrominoAngle;
        int GAMESPEED = 500;
        int cellwidth = 20, cellheight = 20;
        int offsetx = 3, offsety = 0;
        bool collision_up = false, collision_down = false, collision_left = false, collision_right = false;
        Random rand = new Random();
        string currTetromino = "";
        Timer gameTimer = new Timer();
        List<Cell> currTetrominoCells = new List<Cell>();
        string nextTetromino = "";
        List<Cell> nextTetrominoCells = new List<Cell>();


        private static Color O_TetrominoColor = Color.GreenYellow;
        private static Color I_TetrominoColor = Color.Red;
        private static Color T_TetrominoColor = Color.Gold;
        private static Color S_TetrominoColor = Color.Violet;
        private static Color Z_TetrominoColor = Color.DeepSkyBlue;
        private static Color J_TetrominoColor = Color.Cyan;
        private static Color L_TetrominoColor = Color.LightSeaGreen;

        Color[] shapeColor =
        {
            O_TetrominoColor,
            I_TetrominoColor,
            T_TetrominoColor,
            S_TetrominoColor,
            Z_TetrominoColor,
            J_TetrominoColor,
            L_TetrominoColor
        };

        string[] arrayTetrominos = {"",
        "O_Tetromino", "I_Tetromino_0", "T_Tetromino_0", "S_Tetromino_0", "Z_Tetromino_0",
         "J_Tetromino_0", "L_Tetromino_0"};

        #region Tetrominos
        public int[,] O_Tetromino = new int[2, 2]
        {
            { 1, 1 },
            { 1, 1 }
        };

        public int[,] I_Tetromino_0 = new int[1, 4]
        { { 1, 1, 1, 1 } };
        public int[,] I_Tetromino_90 = new int[4, 1]
        {
            {1 },
            {1 },
            {1 },
            {1 }
        };

        public int[,] T_Tetromino_0 = new int[2, 3]
        {
            {0 , 1, 0 },
            {1, 1, 1 }
        };
        public int[,] T_Tetromino_90 = new int[3, 2]
        {
            {1, 0 },
            {1, 1 },
            {1, 0 }
        };
        public int[,] T_Tetromino_180 = new int[2, 3]
        {
            {1, 1, 1 },
            {0, 1, 0 }
        };
        public int[,] T_Tetromino_270 = new int[3, 2]
        {
            {0, 1 },
            {1, 1 },
            {0, 1 }
        };

        public int[,] S_Tetromino_0 = new int[2, 3]
        {
            {0, 1, 1 },
            {1, 1, 0 }
        };
        public int[,] S_Tetromino_90 = new int[3, 2]
        {
            {1, 0 },
            {1, 1 },
            {0, 1 }
        };

        public int[,] Z_Tetromino_0 = new int[2, 3]
        {
            {1, 1, 0 },
            {0, 1, 1 }
        };
        public int[,] Z_Tetromino_90 = new int[3, 2]
        {
            {0, 1 },
            {1, 1 },
            {1, 0 }
        };

        public int[,] J_Tetromino_0 = new int[2, 3]
        {
            {1, 0, 0 },
            {1, 1, 1 }
        };
        public int[,] J_Tetromino_90 = new int[3, 2]
        {
            {1, 1 },
            {1, 0 },
            {1, 0 }
        };
        public int[,] J_Tetromino_180 = new int[2, 3]
        {
            {1, 1, 1 },
            {0, 0, 1 }
        };
        public int[,] J_Tetromino_270 = new int[3, 2]
        {
            {0, 1 },
            {0, 1 },
            {1, 1 }
        };

        public int[,] L_Tetromino_0 = new int[2, 3]
        {
            {0, 0 ,1 },
            {1, 1, 1 }
        };
        public int[,] L_Tetromino_90 = new int[3, 2]
        {
            {1, 0 },
            {1, 0 },
            {1, 1 }
        };
        public int[,] L_Tetromino_180 = new int[2, 3]
        {
            {1, 1, 1 },
            {1, 0, 0 }
        };
        public int[,] L_Tetromino_270 = new int[3, 2]
        {
            {1, 1 },
            {0, 1 },
            {0, 1 }
        };
        #endregion

        Cell[,] stationary_cells = new Cell[10, 20];

        public Tetris()
        {
            InitCells();
            InitializeComponent();
            this.KeyPreview = true;
            background.PlayLooping();
        }

        private void InitCells()
        {
            for (int x = 0; x < 10; ++x)
                for (int y = 0; y < 20; ++y)
                    stationary_cells[x, y] = new Cell();
        }

        private void GameEvent(object sender, EventArgs e)
        {
            if (currTetromino == "")
            {
                SwapCurrandNext();
                GetNewTetromino();

                bool Ok = false;
                foreach(Cell c in currTetrominoCells)
                    if (stationary_cells[c.X, c.Y].Color != Color.Transparent)
                        Ok = true;

                if(Ok)
                {
                    GameOver();
                    return;
                }
            }

            bool ok = false;
            foreach (Cell c in currTetrominoCells)
            {
                CheckCollisions(c.X, c.Y);
                if (collision_down == true)
                    ok = true;
            }

            if (ok)
            {
                foreach (Cell c in currTetrominoCells)
                    stationary_cells[c.X, c.Y] = new Cell(c.X, c.Y, c.Color, false);

                currTetromino = "";
                currTetrominoCells = new List<Cell>();
            }
            else
            {
                foreach (Cell c in currTetrominoCells)
                    c.Y = c.Y + 1;
                urcell.Y++;
            }

            for (int y = 0; y < 20; ++y)
                if (RowCount(y) == 10)
                {
                    //sp.Play();
                    scor += 10;
                    EliminareRow(y);
                }


            lblScor.Text = "" + scor;
            level = 1 + scor / 20;
            lblLevel.Text = "Level " + level;
            gameTimer.Interval = GAMESPEED - (level - 1) * 50;

            pb.Invalidate();
        }

        private void SwapCurrandNext()
        {
            currTetromino = nextTetromino;
            currTetrominoCells = nextTetrominoCells;
            urcell = new Cell(offsetx, offsety, Color.Transparent, true);
            currTetrominoAngle = 0;
        }

        private void GameOver()
        {
            btnStart.Enabled = true;
            gameTimer.Stop();
            gameTimer.Tick -= GameEvent;
            MessageBox.Show("Jocul s-a terminat");
        }

        private void pb_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < 10; ++x)
                for (int y = 0; y < 20; ++y)
                    DrawCell(stationary_cells[x, y], e.Graphics);

            foreach (Cell c in currTetrominoCells)
                DrawCell(c, e.Graphics);
        }

        private void DrawCell(Cell c, Graphics g)
        {
            if (c.Color == Color.Transparent) return;

            Pen pen = new Pen(Color.White, 2f);
            LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(c.X * cellwidth, c.Y * cellheight, cellwidth, cellheight), Color.Black, c.Color, 45);
            ColorBlend cb = new ColorBlend();
            cb.Positions = new float[]{ 0f, 0.6f,  1f};
            cb.Colors = new Color[] { Color.Black, c.Color, c.Color };
            lgb.InterpolationColors = cb;

            g.FillRectangle(lgb,
                        new Rectangle(c.X * cellwidth, c.Y * cellheight, cellwidth, cellheight));
            g.DrawRectangle(pen,
                        new Rectangle(c.X * cellwidth, c.Y * cellheight, cellwidth, cellheight));

            lgb.Dispose();
            pen.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            InitCells();
            urcell = new Cell();
            currTetrominoAngle = 0;
            currTetromino = "";
            currTetrominoCells = new List<Cell>();
            scor = 0;
            lblScor.Text = "" + scor;

            btnStart.Enabled = false;
            gameTimer.Interval = GAMESPEED;
            gameTimer.Tick += GameEvent;
            gameTimer.Start();
        }

        private void Tetris_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;


            if (currTetromino == "") return;

            if(e.KeyCode == Keys.A)
            {
                bool ok = true;
                foreach(Cell c in currTetrominoCells)
                {
                    CheckCollisions(c.X, c.Y);
                    if (collision_left == true)
                        ok = false;
                }

                if (ok)
                {
                    foreach (Cell c in currTetrominoCells)
                        c.X -= 1;
                    urcell.X--;
                }
            }
            else if(e.KeyCode == Keys.D)
            {
                bool ok = true;
                foreach (Cell c in currTetrominoCells)
                {
                    CheckCollisions(c.X, c.Y);
                    if (collision_right == true)
                        ok = false;
                }

                if (ok)
                {
                    foreach (Cell c in currTetrominoCells)
                        c.X += 1;
                    urcell.X++;
                }
            }
            else if(e.KeyCode == Keys.S)
            {
                bool ok = true;
                foreach (Cell c in currTetrominoCells)
                {
                    CheckCollisions(c.X, c.Y);
                    if (collision_down == true)
                        ok = false;
                }

                if (ok)
                {
                    foreach (Cell c in currTetrominoCells)
                        c.Y += 1;
                    urcell.Y++;
                }
            }

            pb.Invalidate();
        }

        private void EliminareRow(int y)
        {
            for (int i = y; i > 0; --i)
                for (int x = 0; x < 10; ++x)
                {
                    stationary_cells[x, i] = stationary_cells[x, i - 1];
                    stationary_cells[x, i].Y++;
                }

            for (int x = 0; x < 10; ++x)
                stationary_cells[x, 0] = new Cell(x, 0, Color.Transparent, false);
        }

        private void pbNext_Paint(object sender, PaintEventArgs e)
        {
            foreach (Cell c in nextTetrominoCells)
                DrawCell(new Cell(c.X - offsetx, c.Y - offsety, c.Color, c.InMiscare), e.Graphics);
        }

        private int RowCount(int y)
        {
            int cnt = 0;
            for (int x = 0; x < 10; ++x)
                if (stationary_cells[x, y].Color != Color.Transparent)
                    cnt++;
            return cnt;
        }

        private void GetNewTetromino()
        {
            int v = rand.Next(1, 8);
            nextTetrominoCells = new List<Cell>();
            nextTetromino = arrayTetrominos[v];
            int[,] arr = (int[,])this.GetType().GetField(nextTetromino).GetValue(this);

            for (int y = 0; y < arr.GetLength(0); ++y)
                for (int x = 0; x < arr.GetLength(1); ++x)
                    if(arr[y, x] == 1)
                        nextTetrominoCells.Add(new Cell(offsetx + x, offsety + y, shapeColor[v - 1], true));

            pbNext.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, 0), new Point(0, ClientRectangle.Height), Color.DarkBlue, Color.Blue);
            e.Graphics.FillRectangle(lgb, ClientRectangle);
        }

        private void CheckCollisions(int x, int y)
        {
            collision_down = collision_left = collision_right = collision_up = false;

            if (y >= 19 || stationary_cells[x, y + 1].Color != Color.Transparent)
                collision_down = true;
            if (x <= 0 || stationary_cells[x - 1, y].Color != Color.Transparent)
                collision_left = true;
            if (x >= 9 || stationary_cells[x + 1, y].Color != Color.Transparent)
                collision_right = true;
            if (y <= 0 || stationary_cells[x, y - 1].Color != Color.Transparent)
                collision_up = true;
        }

        protected override void OnMouseWheel(MouseEventArgs mea)
        {
            if (currTetromino == "") return;

            int unit = mea.Delta / SystemInformation.MouseWheelScrollDelta;
            int angle = currTetrominoAngle;

            if (currTetromino == "O_Tetromino") return;

            if(unit == 1)
            {
                angle += 90;
                if (currTetromino[0] == 'I' && angle > 90) angle = 0;
                if (currTetromino[0] == 'T' && angle > 270) angle = 0;
                if (currTetromino[0] == 'S' && angle > 90) angle = 0;
                if (currTetromino[0] == 'Z' && angle > 90) angle = 0;
                if (currTetromino[0] == 'J' && angle > 270) angle = 0;
                if (currTetromino[0] == 'L' && angle > 270) angle = 0;
            }
            else
            {
                angle -= 90;
                if (currTetromino[0] == 'I' && angle < 0) angle = 90;
                if (currTetromino[0] == 'T' && angle < 0) angle = 270;
                if (currTetromino[0] == 'S' && angle < 0) angle = 90;
                if (currTetromino[0] == 'Z' && angle < 0) angle = 90;
                if (currTetromino[0] == 'J' && angle < 0) angle = 270;
                if (currTetromino[0] == 'L' && angle < 0) angle = 270;
            }


            string[] s = currTetromino.Split('_');
            string line = s[0] + '_' + s[1] + '_' + angle.ToString();

            int[,] arr = this.GetType().GetField(line).GetValue(this) as int[,];


            bool ok = true;
            for (int y = 0; y < arr.GetLength(0); ++y)
                for (int x = 0; x < arr.GetLength(1); ++x)
                    if (arr[y, x] == 1 && (urcell.X + x >= 10 || urcell.Y + y >= 20 || 
                        stationary_cells[urcell.X + x, urcell.Y].Color != Color.Transparent ||
                        stationary_cells[urcell.X, urcell.Y + y].Color != Color.Transparent))
                        ok = false;

            if(!ok)return;

            Color color = currTetrominoCells[0].Color;

            currTetrominoAngle = angle;
            currTetrominoCells.Clear();

            for (int y = 0; y < arr.GetLength(0); ++y)
                for (int x = 0; x < arr.GetLength(1); ++x)
                    if(arr[y, x] == 1)
                        currTetrominoCells.Add(new Cell(urcell.X + x, urcell.Y + y, color, true));

            pb.Invalidate();
        }
    }
}
