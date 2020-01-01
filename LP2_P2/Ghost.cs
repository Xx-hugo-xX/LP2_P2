using System;
using System.Collections.Generic;
using System.Text;

namespace LP2_P2
{
    class Ghost : Object
    {
        private readonly List<Object> open = new List<Object>();
        private readonly List<Object> closed = new List<Object>();
        private readonly List<Object> neibors = new List<Object>();
        private readonly List<Object> allPieces = new List<Object>();

        public Ghost(int x, int y, List<Object> allMapPieces)
        {

            Pos = new Position(x, y);
            OldPos = new Position(x - 1, y);
            Visuals = 'U';
            BoxCollider = new int[4] { x, y, x + 1, y + 1 };
            allPieces = allMapPieces;
        }

        public List<Position> CalcuatePath(Object target)
        {
            open.Clear();
            closed.Clear();
            neibors.Clear();
            open.Add(this);

            while (open.Count > 0)
            {
                Object current = open[0];

                for (int i = 1; i < open.Count; i++)
                {
                    if (open[i].combinedCost < current.combinedCost
                        || open[i].combinedCost == current.combinedCost)
                    {
                        if (open[i].closenessCost < current.closenessCost)
                        {
                            current = open[i];
                        }
                    }
                }

                open.Remove(current);
                closed.Add(current);

                if (current.Pos == target.Pos)
                {
                    return TracePath(current);
                }

                for (int c = 0; c < allPieces.Count; c++)
                {
                    if (allPieces[c].Pos.X == current.Pos.X + 1
                        && allPieces[c].Pos.Y == current.Pos.Y ||
                        allPieces[c].Pos.X == current.Pos.X - 1
                        && allPieces[c].Pos.Y == current.Pos.Y ||
                        allPieces[c].Pos.Y == current.Pos.Y + 1
                        && allPieces[c].Pos.X == current.Pos.X ||
                        allPieces[c].Pos.Y == current.Pos.Y - 1
                        && allPieces[c].Pos.X == current.Pos.X)
                    {
                        if (allPieces[c].GetType() != typeof(Player) &&
                            allPieces[c].GetType() != typeof(MapPiece) &&
                            !(allPieces[c].Pos == OldPos))
                        {
                                neibors.Add(allPieces[c]);
                        }
                    }
                }

                for (int b = 0; b < neibors.Count; b++)
                {
                    if (closed.Contains(neibors[b]))
                    {
                        continue;
                    }

                    int newCostMov = current.distanceCost +
                        GetDistace(current.Pos, neibors[b].Pos);
                    if (newCostMov < neibors[b].distanceCost
                        || !open.Contains(neibors[b]))
                    {
                        neibors[b].distanceCost = newCostMov;
                        neibors[b].closenessCost =
                            GetDistace(neibors[b].Pos, target.Pos);
                        neibors[b].parent = current;

                        if (!open.Contains(neibors[b]))
                        {
                            open.Add(neibors[b]);
                        }
                    }
                }
                neibors.Clear();
            }
            return null;
        }
        private List<Position> TracePath(Object end)
        {
            List<Position> path = new List<Position>();
            Object currentPiece = end;

            while (currentPiece != this)
            {
                path.Add(currentPiece.Pos);
                currentPiece = currentPiece.parent;
            }
            path.Reverse();
            return path;
        }
        private int GetDistace(Position A, Position B)
        {
            int distanceX = Math.Abs(A.X - B.X);
            int distanceY = Math.Abs(A.Y - B.Y);

            if (distanceX > distanceY)
            {
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }

            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}
