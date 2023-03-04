using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tetris
{
    public partial class Tetris : Form
    {
        private const int BLOCK = 40;
        private const int HEIGHT = 17;
        private const int WIDTH = 9;
        private int rowsFinished = 0;
        private int[,] grid = new int[10, 18];

        private int[,] shape0 = {
                { 0,0,0,0},
                { 0,0,0,0},
                { 1,1,1,1},
                { 0,0,0,0},
            };
        private int[,] shape1 = {
                { 0,0,0,0},
                { 0,1,0,0},
                { 0,1,1,1},
                { 0,0,0,0},
            };
        private int[,] shape2 = {
                { 0,0,0,0},
                { 0,0,1,0},
                { 1,1,1,0},
                { 0,0,0,0},
            };
        private int[,] shape3 ={
                { 0,0,0,0},
                { 0,1,1,0},
                { 1,1,0,0},
                { 0,0,0,0},
            };
        private int[,] shape4 ={
                { 0,0,0,0},
                { 1,1,0,0},
                { 0,1,1,0},
                { 0,0,0,0},
            };
        private int[,] shape5 = {
                { 0,0,0,0},
                { 0,1,1,0},
                { 0,1,1,0},
                { 0,0,0,0},
            };
        private int[,] shape6 = {
                { 0,0,0,0},
                { 1,1,1,0},
                { 0,1,0,0},
                { 0,0,0,0},
            };

        private List<int[,]> shapes = new List<int[,]>();

        private int[,] figure,nextFigure;
        private bool activeFigure = false;
        private int currentX = 0;
        private int currentY = 0;
        private Random rand = new Random();
        private List<int> prevPos = new List<int>();
        private bool paused = true;
        FileInfo fi = new FileInfo("highScore.txt");
        System.Media.SoundPlayer player;
        private bool music = true;
        public Tetris()
        {
            InitializeComponent();
            timer1.Interval = 500;
            timer1.Start();
            shapes.Add(shape0);
            shapes.Add(shape1);
            shapes.Add(shape2);
            shapes.Add(shape3);
            shapes.Add(shape4);
            shapes.Add(shape5);
            shapes.Add(shape6);
            nextFigure = shapes[rand.Next(7)];
            figure = new int[4, 4];
            nextFigure = new int[4, 4];
            player = new System.Media.SoundPlayer("soundtrack.wav");

            string text;
            if (fi.Exists)
            {
                text = System.IO.File.ReadAllText("highScore.txt");
            }
            else
            {
                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("0");
                    text = "";
                }
            }
            toolStripMenuItem2.Text = text;
    }

        public void drawGrid()
        {
            using (Graphics myGraphics = base.CreateGraphics())
            using (SolidBrush blueBrush = new SolidBrush(Color.Blue))
            using (SolidBrush whiteBrush = new SolidBrush(Color.Gray))
            using (Pen myPen = new Pen(Color.Black))
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        //clear space before drawing
                        myGraphics.FillRectangle(whiteBrush, x * BLOCK,24+ y * BLOCK, BLOCK, BLOCK);
                        if (grid[x, y] == 1)
                            myGraphics.FillRectangle(blueBrush, x * BLOCK, 24+y * BLOCK, BLOCK, BLOCK);
                        else
                            myGraphics.DrawRectangle(myPen, x * BLOCK,24+ y * BLOCK, BLOCK, BLOCK);
                    }
                }
                for(int x = 0; x < 4; x++)
                {
                    for(int y = 0; y < 4; y++)
                    {
                        myGraphics.FillRectangle(whiteBrush, 460 + x * 25, 95 + y * 25, 25, 25);
                        if (nextFigure[x,y] == 1)
                        {
                            myGraphics.FillRectangle(blueBrush, 460 + x * 25, 95 + y * 25, 25, 25);

                        }
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            drawGrid();
        }
        private void label1_Click(object sender, EventArgs e) { }
        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (!paused)
            {
                if (e.KeyCode.ToString() == "Space")
                {
                    while (activeFigure)
                    {
                        moveFigure(0, 1);
                        updateGrid();
                        removePrev();
                    }

                    drawGrid();
                }
                else if (e.KeyCode.ToString() == "Right")
                {
                    moveFigure(1, 0);
                    removePrev();
                    updateGrid();
                    drawGrid();
                }
                else if (e.KeyCode.ToString() == "Left")
                {
                    moveFigure(-1, 0);
                    removePrev();
                    updateGrid();
                    drawGrid();
                }
                else if (e.KeyCode.ToString() == "Up")
                {
                    rotate();
                    removePrev();
                    updateGrid();
                    drawGrid();
                }
                else if(e.KeyCode == Keys.Home)
                {
                    label5.Text = (int.Parse(label5.Text) + 1000).ToString();
                    label6.Text = (int.Parse(label6.Text) + 10).ToString();
                    label7.Text = (int.Parse(label7.Text) + 1).ToString();
                    timer1.Interval -= timer1.Interval / 4;
                }
            }
            if(e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
            {
                paused = true;
            }else if(e.Modifiers == Keys.Control && e.KeyCode == Keys.G)
            {
                paused = false;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                label8.Text = "";
                timer1.Interval = 500;
                grid = new int[10, 18];
                prevPos.Clear();
                currentX = rand.Next(WIDTH - 4);
                currentY = 0;
                nextFigure = shapes[rand.Next(7)];
                paused = false;
                label5.Text = "0";
                label6.Text = "0";
                label7.Text = "0";
                timer1.Start();
                player.PlayLooping();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!paused)
            {
                if (!activeFigure)
                {
                    checkFilled();
                    //generate random fig and start from the top
                    figure = nextFigure;
                    nextFigure = shapes[rand.Next(7)];
                    currentX = rand.Next(WIDTH - 4);
                    currentY = 0;

                    for(int x = 0; x < 4; x++)
                    {
                        for(int y = 0; y < 4; y++)
                        {
                            if (figure[x, y] == 1 && grid[currentX + x, currentY + y] == 1)
                                gameEnded();
                        }
                    }

                    activeFigure = true;
                }
                removePrev();
                moveFigure(0, 1);
                updateGrid();
                saveToolStripMenuItem.Enabled = false;
                loadGameToolStripMenuItem.Enabled = false;
            }
            else
            {
                saveToolStripMenuItem.Enabled = true;
                loadGameToolStripMenuItem.Enabled = true;
            }

            drawGrid();
        }
        public void moveFigure(int dx, int dy)
        {
            bool valid = true;

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (figure[x, y] == 1)
                    {
                        //for right and left movement check if any block is outsied of borders or inside dropped figure
                        if (currentX + x + dx < 0 || currentX + x + dx > WIDTH)
                            valid = false;
                        else if ((x + dx < 0 || x+dx >= 4 || figure[x+dx,y] == 0) && grid[currentX +x + dx, currentY+y] == 1)
                        {
                             valid = false;
                        }
                           
                        //for vertical movement check if figure landed
                        if(currentY+y+dy > HEIGHT)
                        {
                            valid = false;
                            activeFigure = false;
                            prevPos.Clear();
                        }
                        else if((y+dy >3 || figure[x,y+dy] == 0) && grid[currentX+x+dx, currentY + y + dy] == 1)
                        {
                            valid = false;
                            activeFigure = false;
                            prevPos.Clear();
                        }
                    }

                }
            }
            if (valid)
            {
                currentX += dx;
                currentY += dy;
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (figure[x, y] == 1)
                        {
                            prevPos.Add(currentX + x);
                            prevPos.Add(currentY + y);
                        }
                    }
                }
            }
        }
        public void removePrev()
        {
            int nx, ny; //temp variables
            for (int i = 0; i < prevPos.Count; i += 2)
            {
                nx = prevPos[i];
                ny = prevPos[i + 1];
                grid[nx,ny] = 0;
            }
        }
        public void updateGrid()
        {
            for(int x = 0; x < 4; x++)
            {
                for(int y = 0; y < 4; y++)
                {
                    if(figure[x,y] == 1)
                    {
                        grid[currentX + x, currentY + y] = 1;
                    }
                }
            }
        }
        public bool isValid(int[,] fig)
        {
            bool valid = true;

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (fig[x, y] == 1)
                    {
                        //for right and left movement check if any block is outsied of borders or inside dropped figure
                        if (currentX + x  < 0 || currentX + x  > WIDTH)
                            valid = false;
                        else if (grid[currentX + x , y] == 1)
                        {
                            valid = false;
                            //for vertical movement check if figure landed
                            if (currentY + y > HEIGHT)
                            {
                                valid = false;
                            }
                            else if ((y > 3 || fig[x, y + 1] == 0) && grid[currentX + x, currentY + y + 1] == 1)
                            {
                                valid = false;
                            }
                        }
                    }

                }
            }
            return valid;
        }
        public void rotate()
        {
            //int temp;
            moveFigure(0, 0);
            removePrev();
            int[,] tempFigure = new int[4, 4];
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    tempFigure[3 - j, i] = figure[i,j];

                }
            }
            if(isValid(tempFigure))
                figure = tempFigure;
            moveFigure(0, 0);
            updateGrid();
        }
        public void checkFilled()
        {
            int count = 0; //number of rows filled
            int lowest = 0;
            int temp; //holds count of blocks in each row

            for(int y=HEIGHT;y>=0; y--)
            {
                temp = 0;
                for(int x=0;x<= WIDTH; x++)
                {
                    if (grid[x, y] == 1)
                        temp++;
                }
                if (temp == WIDTH+1)
                {
                    count++;
                    removeRow(y);
                    if (y > lowest)
                        lowest = y;
                }
            }
            for(int i = 0; i < count; i++)
            {
                for (int y = lowest; y >= 0; y--)
                {
                    for (int x = 0; x <= WIDTH; x++)
                    {
                        if (grid[x, y] == 1)
                        {
                            grid[x, y + 1] = grid[x, y];
                            grid[x, y] = 0;
                        }
                    }
                }
                lowest--;
            }
            if(count > 0)
                label5.Text = (int.Parse(label5.Text) + count * 100 + 50 * (count - 1)).ToString();
            rowsFinished += count;
            label6.Text = (int.Parse(label6.Text) + count).ToString();
            if(rowsFinished >= 10)
            {
                label7.Text = (int.Parse(label7.Text) + 1).ToString();
                rowsFinished -= 10;
                timer1.Interval -= timer1.Interval /4;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label8.Text = "";
            timer1.Interval = 500;
            grid = new int[10, 18];
            prevPos.Clear();
            currentX = rand.Next(WIDTH - 4);
            currentY = 0;
            nextFigure = shapes[rand.Next(7)];
            paused = false;
            label5.Text = "0";
            label6.Text = "0";
            label7.Text = "0";
            timer1.Start();
            player.PlayLooping();
        }

        public void removeRow(int row)
        {
            for(int x=0;x<= WIDTH; x++)
            {
                grid[x, row] = 0;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removePrev();
            saveFileDialog1.DefaultExt = "game";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog1.FileName;
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine($"{label5.Text} {label6.Text} {label7.Text}");
                    for (int y = 0; y <= HEIGHT; y++)
                    {
                        for(int x = 0; x <= WIDTH; x++)
                        {
                            writer.Write($"{grid[x, y]} ");
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                using (StreamReader reader = new StreamReader(path))
                {
                    string[] words = reader.ReadLine().Split(' ');
                    label5.Text = words[0];
                    label6.Text = words[1];
                    label7.Text = words[2];
                    for (int y = 0; y <= HEIGHT; y++)
                    {
                        words = reader.ReadLine().Split(' ');
                        for (int x = 0; x <= WIDTH; x++)
                        {
                            grid[x, y] = int.Parse(words[x]);
                        }
                    }
                }
            }
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (music)
            {
                player.Stop();
                music = false;
                muteToolStripMenuItem.Text = "Play";
            }
            else
            {
                music = true;
                player.PlayLooping();
                muteToolStripMenuItem.Text = "Mute";
            }
            

        }

        public void gameEnded()
        {
            player.Stop();
            if(int.Parse(toolStripMenuItem2.Text) < int.Parse(label5.Text))
            {
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(label5.Text);
                }
            }
            paused = true;
            timer1.Stop();
            label8.Text = "You Lost";
        }
    }
}
