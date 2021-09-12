using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGenerator
{
    public class MazeCell
    {
        public Vector2Int Position { get; private set; }
        public void SetPosition(int x, int y) => Position = new Vector2Int(x, y);

        public bool Visited { get; private set; }
        public void Visit() => Visited = true;

        public List<MazeCell> Neighbours { get; private set; }
        public void SetNeighbours(List<MazeCell> neighbours) => Neighbours = neighbours;

        public bool hasTopWall;
        public bool hasBottomWall;
        public bool hasLeftWall;
        public bool hasRightWall;

        public GameObject TopWall;
        public GameObject BottomWall;
        public GameObject LeftWall;
        public GameObject RightWall;

        public int gcost;
        public int fcost;
        public int hcost;
        public MazeCell cameFrom;


        public MazeCell(int x, int y)
        {
            SetPosition(x, y);

            if (Neighbours == null)
                Neighbours = new List<MazeCell>();

            hasTopWall = true;
            hasBottomWall = true;
            hasRightWall = true;
            hasLeftWall = true;
        }

        public void CalculateFCost()
        {
            fcost = gcost + hcost;
        }
    }
}