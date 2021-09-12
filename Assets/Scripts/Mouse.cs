using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MazeGenerator
{
    public class Mouse : MonoBehaviour
    {
        public static MazeCell Destination { get; private set; }

        public Transform _pathNode;

        private MazeCell _currentCell;


        public void Initialize(MazeCell startPoint) => _currentCell = startPoint;

        public void StartPathfinding(Vector2 dest)
        {
            Destination = ConvertPositionToCell(dest);

            if (Destination.Position != _currentCell.Position)
                Pathfind();
            else
                Debug.LogWarning("Destination is same as current");
        }

        private void Pathfind()
        {
            Maze.Data _data = FindObjectOfType<Maze>().GetData();
            _currentCell = ConvertPositionToCell(transform.position);
            List<MazeCell> path = MazeGenerator.GetPath(Maze.MazeGrid, new Vector2Int(
                _data._width,
                _data._height),
                _currentCell,
                Destination);

            StartCoroutine(MoveMouse(path));
        }

        private List<MazeCell> GetAccessibleNeighbours(MazeCell currentCell)
        {
            List<MazeCell> accessibleNeighbours = new List<MazeCell>();
            foreach (MazeCell n in currentCell.Neighbours)
            {
                if (n.Position.x == currentCell.Position.x)
                {
                    if (n.Position.y > currentCell.Position.y)
                    {
                        // top neighbour
                        if (!currentCell.hasTopWall && !n.hasBottomWall)
                            accessibleNeighbours.Add(n);
                    }
                    else if (n.Position.y < currentCell.Position.y)
                    {
                        // bottom neighbour
                        if (!currentCell.hasBottomWall && !n.hasTopWall)
                            accessibleNeighbours.Add(n);
                    }
                }
                else
                {
                    if (n.Position.x > currentCell.Position.x)
                    {
                        // right neighbour
                        if (!currentCell.hasRightWall && !n.hasLeftWall)
                            accessibleNeighbours.Add(n);

                    }
                    else if (n.Position.x < currentCell.Position.x)
                    {
                        // left neighbour
                        if (!currentCell.hasLeftWall && !n.hasRightWall)
                            accessibleNeighbours.Add(n);
                    }
                }
            }

            return accessibleNeighbours;
        }

        private MazeCell ConvertPositionToCell(Vector2 pos)
        {
            Maze.Data _data = FindObjectOfType<Maze>().GetData();
            MazeCell closest = GetAccessibleNeighbours(Maze.MazeGrid[0,0])[0];

            foreach (MazeCell cell in Maze.MazeGrid)
            {
                Vector3 calcdPosition = new Vector3(
                    _data._cellSize * cell.Position.x + (_data._cellSize / 2f) - ((_data._width * _data._cellSize) / 2f),
                    _data._cellSize * cell.Position.y + (_data._cellSize / 2f) - ((_data._height * _data._cellSize) / 2f),
                    0
                );

                if (Vector2.Distance(calcdPosition, pos) < _data._cellSize / 2f)
                {
                    closest = cell;
                    break;
                }
            }
            return closest;
        }

        private IEnumerator MoveMouse(List<MazeCell> path)
        {
            while (path.Count > 0)
            {
                Maze.Data _data = FindObjectOfType<Maze>().GetData();
                Vector2 position = path[0].Position;
                path.RemoveAt(0);
                Vector3 calcdCellPosition = new Vector3(
                    _data._cellSize * position.x + (_data._cellSize / 2f) - ((_data._width * _data._cellSize) / 2f),
                    _data._cellSize * position.y + (_data._cellSize / 2f) - ((_data._height * _data._cellSize) / 2f),
                    0
                );

                transform.position = calcdCellPosition;

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}