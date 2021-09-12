using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGenerator
{
    public static class MazeGenerator
    {
        public static MazeCell[,] GenerateMaze(int width, int height)
        {
            MazeCell[,] maze = new MazeCell[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    maze[x, y] = new MazeCell(x, y);

            return RecursiveBacktrack(maze, new Vector2Int(width, height));
        }

        private static MazeCell[,] RecursiveBacktrack(MazeCell[,] maze, Vector2Int dimensions)
        {
            Vector2Int startPosition = Vector2Int.zero;
            maze[startPosition.x, startPosition.y].Visit();

            Stack<Vector2Int> path = new Stack<Vector2Int>();
            path.Push(startPosition);

            while (path.Count > 0)
            {
                Vector2Int currentPosition = path.Pop();
                List<MazeCell> neighbours = GetUnvisitedNeighbours(maze, dimensions, maze[currentPosition.x, currentPosition.y]);

                if (neighbours.Count > 0)
                {
                    path.Push(currentPosition);

                    MazeCell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                    if (randomNeighbour.Position.x == currentPosition.x)
                    {
                        if (randomNeighbour.Position.y > currentPosition.y)
                        {
                            // top neighbour
                            maze[currentPosition.x, currentPosition.y].hasTopWall = false;
                            randomNeighbour.hasBottomWall = false;
                        }
                        else if (randomNeighbour.Position.y < currentPosition.y)
                        {
                            // bottom neighbour
                            maze[currentPosition.x, currentPosition.y].hasBottomWall = false;
                            randomNeighbour.hasTopWall = false;
                        }
                    }
                    else
                    {
                        if (randomNeighbour.Position.x > currentPosition.x)
                        {
                            // right neighbour
                            maze[currentPosition.x, currentPosition.y].hasRightWall = false;
                            randomNeighbour.hasLeftWall = false;
                        }
                        else if (randomNeighbour.Position.x < currentPosition.x)
                        {
                            // left neighbour
                            maze[currentPosition.x, currentPosition.y].hasLeftWall = false;
                            randomNeighbour.hasRightWall = false;
                        }
                    }

                    randomNeighbour.Visit();
                    path.Push(randomNeighbour.Position);
                }
            }

            // Entrance and exit
            maze[0, 0].hasLeftWall = false;
            maze[dimensions.x - 1, dimensions.y - 1].hasRightWall = false;

            return maze;
        }

        private static List<MazeCell> GetUnvisitedNeighbours(MazeCell[,] maze, Vector2Int dimensions, MazeCell cell)
        {
            List<MazeCell> neighbours = new List<MazeCell>();
            List<MazeCell> unvisitedNeighbours = new List<MazeCell>();

            if (cell.Position.x >= 0 && cell.Position.x < dimensions.x && cell.Position.y >= 0 && cell.Position.y < dimensions.y)
            {
                // Top neighbour
                if (cell.Position.y + 1 < dimensions.y)
                {
                    MazeCell top = maze[cell.Position.x, cell.Position.y + 1];
                    neighbours.Add(top);
                    if (!top.Visited)
                        unvisitedNeighbours.Add(top);
                }
                // Bottom neighbour
                if (cell.Position.y - 1 >= 0)
                {
                    MazeCell bottom = maze[cell.Position.x, cell.Position.y - 1];
                    neighbours.Add(bottom);
                    if (!bottom.Visited)
                        unvisitedNeighbours.Add(bottom);
                }
                // Right neighbour
                if (cell.Position.x + 1 < dimensions.x)
                {
                    MazeCell right = maze[cell.Position.x + 1, cell.Position.y];
                    neighbours.Add(right);
                    if (!right.Visited)
                        unvisitedNeighbours.Add(right);
                }
                // Left neighbour
                if (cell.Position.x - 1 >= 0)
                {
                    MazeCell left = maze[cell.Position.x - 1, cell.Position.y];
                    neighbours.Add(left);
                    if (!left.Visited)
                        unvisitedNeighbours.Add(left);
                }
            }

            cell.SetNeighbours(neighbours);
            return unvisitedNeighbours;
        }

        private static List<MazeCell> GetAccessibleNeighbours(MazeCell[,] maze, Vector2Int dimensions, List<MazeCell> neighbours, MazeCell current)
        {
            List<MazeCell> accessibleNeighbours = new List<MazeCell>();

            // Top neighbour
            if (current.Position.y + 1 < dimensions.y)
            {
                MazeCell top = maze[current.Position.x, current.Position.y + 1];
                if (!top.hasBottomWall)
                    accessibleNeighbours.Add(top);
            }
            // Bottom neighbour
            if (current.Position.y - 1 >= 0)
            {
                MazeCell bottom = maze[current.Position.x, current.Position.y - 1];
                if (!bottom.hasTopWall)
                    accessibleNeighbours.Add(bottom);
            }
            // Right neighbour
            if (current.Position.x + 1 < dimensions.x)
            {
                MazeCell right = maze[current.Position.x + 1, current.Position.y];
                if (!right.hasLeftWall)
                    accessibleNeighbours.Add(right);
            }
            // Left neighbour
            if (current.Position.x - 1 >= 0)
            {
                MazeCell left = maze[current.Position.x - 1, current.Position.y];
                if (!left.hasRightWall)
                    accessibleNeighbours.Add(left);
            }

            return accessibleNeighbours;
        }
    
        public static List<MazeCell> GetPath(MazeCell[,] maze, Vector2Int dimensions, MazeCell startCell, MazeCell destCell)
        {
            List<MazeCell> open = new List<MazeCell>();
            List<MazeCell> closed = new List<MazeCell>();

            open.Add(startCell);

            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    MazeCell current = maze[x, y];
                    current.gcost = int.MaxValue;
                    current.CalculateFCost();
                    current.cameFrom = null;
                }
            }

            startCell.gcost = 0;
            startCell.hcost = CalculateDistanceCost(startCell, destCell);
            startCell.CalculateFCost();

            while (open.Count > 0)
            {
                MazeCell currentCell = GetLowestFCostNode(open);
                if (currentCell == destCell)
                    return CalculateFinalPath(destCell);

                open.Remove(currentCell);
                closed.Add(currentCell);

                foreach (MazeCell neighbour in GetAccessibleNeighbours(maze, dimensions, currentCell.Neighbours, currentCell))
                {
                    if (closed.Contains(neighbour))
                        continue;

                    int tentGCost = currentCell.gcost + CalculateDistanceCost(currentCell, neighbour);
                    if (tentGCost < neighbour.gcost)
                    {
                        neighbour.cameFrom = currentCell;
                        neighbour.gcost = tentGCost;
                        neighbour.hcost = CalculateDistanceCost(neighbour, destCell);
                        neighbour.CalculateFCost();

                        if (!open.Contains(neighbour))
                            open.Add(neighbour);
                    }
                }
            }

            return null;
        }

        private static int CalculateDistanceCost(MazeCell a, MazeCell b)
        {
            int xDistance = Mathf.Abs(a.Position.x - b.Position.x);
            int yDistance = Mathf.Abs(a.Position.y - b.Position.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return Mathf.Min(xDistance, yDistance) + 1 * remaining;
        }

        private static MazeCell GetLowestFCostNode(List<MazeCell> pathNodes)
        {
            MazeCell lowestCost = pathNodes[0];
            for (int i = 0; i < pathNodes.Count; i++)
            {
                if (pathNodes[i].fcost < lowestCost.fcost)
                    lowestCost = pathNodes[i];
            }

            return lowestCost;
        }

        private static List<MazeCell> CalculateFinalPath(MazeCell dest)
        {
            List<MazeCell> path = new List<MazeCell>();
            path.Add(dest);
            MazeCell current = dest;
            while (current.cameFrom != null)
            {
                path.Add(current.cameFrom);
                current = current.cameFrom;
            }
            path.Reverse();
            return path;
        }
    }
}