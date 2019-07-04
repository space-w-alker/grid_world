using System;
using System.Collections.Generic;
namespace GridWorld
{
    public static class ValueIteration
    {
        public static readonly float CHANGE_TRESHOLD = 0.0000001f;
        public static readonly float GAMMA = 1f; 

        public static Dictionary<string,float> EvaluateUniformRandomPolicy(Grid grid)
        {
            Dictionary<string, float> V = new Dictionary<string, float>();
            for (int i = 0; i < grid.Height; i++)
            {
                for (int j = 0; j < grid.Width; j++)
                {
                    var key = String.Format("{0}{1}",i,j);
                    V[key] = 0f;
                }
            }
            
            while (true)
            {
                float biggest_change = 0f;
                for (int i = 0; i < grid.Height; i++)
                {
                    for (int j = 0; j < grid.Width; j++)
                    {
                        grid.SetPosition(i, j);
                        if (grid.CurrentCell.CellType.Equals(Cell.Cell_Type.WALL)) continue;
                        if (grid.CurrentCell.IsTerminal)
                        {
                            V[String.Format("{0}{1}", i, j)] = 0;
                            continue;
                        }
                        float old_value = V[String.Format("{0}{1}", i, j)];
                        float new_value = 0;
                        float prob = 1.0f / grid.CurrentCell.AllActions.Count;
                        foreach (Grid.ACTION action in grid.CurrentCell.AllActions)
                        {
                            float r = grid.Move(action);
                            new_value += prob * (r + GAMMA * V[String.Format("{0}{1}", grid.Pos[0], grid.Pos[1])]);
                            grid.Move(action, Undo: true);
                        }
                        V[String.Format("{0}{1}", i, j)] = new_value;
                        var l = new List<float> {biggest_change, Math.Abs(new_value - old_value) };
                        l.Sort();
                        biggest_change = l[1];
                    }
                }

                if (biggest_change < CHANGE_TRESHOLD)
                {
                    break;
                }
            }
            return V;
        }
    }
}
