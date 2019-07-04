using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GridWorld
{
    public class Grid
    {
        private int width;
        private int height;
        public Cell[,] grid;
        private int[] currentPosition;
        private Cell currentCell;

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public Cell CurrentCell { get { return currentCell; } }
        public int[] Pos { get { return currentPosition; } }

        public enum ACTION { UP,DOWN,RIGHT,LEFT}


        public Grid()
        {
        }

        public Grid(string fileName)
        {
            using(FileStream stream = File.OpenRead(fileName))
            {
                StreamReader reader = new StreamReader(stream);
                string topLine = reader.ReadLine();
                width = int.Parse(topLine[1].ToString());
                height = int.Parse(topLine[0].ToString());
                grid = new Cell[height, width];
                currentPosition = new int[2];
                currentPosition[0] = int.Parse(topLine[2].ToString());
                currentPosition[1] = int.Parse(topLine[3].ToString());
                
                for(int i =0; i < height; i++)
                {
                    string line = reader.ReadLine();

                    for(int j=0; j < width; j++)
                    {
                        grid[i, j] = new Cell(line[j]);
                    }
                }
                GenerateActions();
            }
        }

        public float Move(ACTION action, bool Undo = false)
        {
            int mul = 1;
            if (Undo)
            {
                mul = -1;
            }
            switch (action)
            {
                case ACTION.DOWN:
                    currentPosition[0] = currentPosition[0] + 1 * mul;
                    break;
                case ACTION.UP:
                    currentPosition[0] = currentPosition[0] - 1 * mul;
                    break;
                case ACTION.LEFT:
                    currentPosition[1] = currentPosition[1] -1 * mul;
                    break;
                case ACTION.RIGHT:
                    currentPosition[1] = currentPosition[1] + 1 * mul;
                    break;
            }
            currentCell = grid[currentPosition[0], currentPosition[1]];
            return currentCell.Reward;
        }

        public void SetPosition (int i, int j)
        {
            currentPosition[0] = i;
            currentPosition[1] = j;
            currentCell = grid[i, j];
        }

        public void GenerateActions()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    for (int act = 0; act < 4; act++)
                    {
                        SetPosition(i, j);
                        if (currentCell.CellType.Equals(Cell.Cell_Type.WALL))
                        {
                            break;
                        }
                        ACTION action = (ACTION)act;
                        try
                        {
                            Move(action);
                            if (currentCell.CellType.Equals(Cell.Cell_Type.WALL))
                            {
                                continue;
                            }
                            Move(action, Undo: true);
                            currentCell.AllActions.Add(action);
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < height; i++)
            {
                builder.AppendLine("_________________________");
                builder.Append("|");
                for (int j = 0; j < width; j++)
                {
                    builder.Append(String.Format("{0,-5}|", grid[i, j].CellType));
                }
                builder.Append("\n");
            }
            builder.AppendLine("_________________________");
            return builder.ToString();
        }

        public string PrintValueFunction(Dictionary<string,float> V)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < height; i++)
            {
                builder.AppendLine("_________________________");
                builder.Append("|");
                for (int j = 0; j < width; j++)
                {
                    builder.Append(String.Format("{0}|", V[String.Format("{0}{1}", i, j)]));
                }
                builder.Append("\n");
            }
            builder.AppendLine("_________________________");
            return builder.ToString();
        }
    }

    public class Cell{
        public enum Cell_Type
        {
            SPACE,
            WALL,
            GOAL,
            FAIL
        }
        public Cell_Type CellType { get; set; }
        public float Reward { get; set; }
        public bool IsTerminal { get; set; }
        public List<Grid.ACTION> AllActions;

        public Cell(char cellType,int Goal=1, int Fail= -1, float Cost = -0f )
        {
            AllActions = new List<Grid.ACTION>();

            switch (cellType)
            {
                case 'S':
                    CellType = Cell_Type.SPACE;
                    Reward = Cost;
                    break;
                case 'W':
                    CellType = Cell_Type.WALL;
                    break;
                case 'G':
                    CellType = Cell_Type.GOAL;
                    IsTerminal = true;
                    Reward = Goal;
                    break;
                case 'F':
                    CellType = Cell_Type.FAIL;
                    IsTerminal = true;
                    Reward = Fail;
                    break;
            }
        }
    }
}
