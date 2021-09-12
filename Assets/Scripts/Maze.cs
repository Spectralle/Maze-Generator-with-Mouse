using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MazeGenerator
{
    public class Maze : MonoBehaviour
    {
        [SerializeField, Range(2, 100)] private int _width = 10;
        [SerializeField, Range(2, 100)] private int _height = 10;
        [SerializeField, Range(0.1f, 2f)] private float _cellSize = 1f; 

        public struct Data
        {
            public bool _mazeExists;
            public int _width;
            public int _height;
            public float _cellSize;
        }
        public Data GetData()
        {
            Data data;
            data._mazeExists = MazeGrid != null && MazeGrid.Length == _width * _height;
            data._width = _width;
            data._height = _height;
            data._cellSize = _cellSize;
            return data;
        }

        [SerializeField] private Transform _wall;
        [SerializeField] private Transform _floor;

        [SerializeField] private Transform _mouse;

        public static MazeCell[,] MazeGrid { get; private set; }
        private Camera _cam;


        private void Awake() => _cam = Camera.main;

        [Button]
        public void Generate()
        {
            MazeGrid = MazeGenerator.GenerateMaze(_width, _height);
            DrawMaze(MazeGrid);
            _cam.orthographicSize = Mathf.Max(_width, _height) / 2f;
        }

        private void DrawMaze(MazeCell[,] maze)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            Transform floor = Instantiate(_floor, Vector3.zero, Quaternion.identity, transform).transform;
            floor.localScale = new Vector3(_cellSize * _width, _cellSize * _height, floor.localScale.z);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    MazeCell cell = maze[x, y];

                    Vector3 calcdCellPosition = new Vector3(
                        _cellSize * cell.Position.x + (_cellSize / 2f) - ((_width * _cellSize) / 2f),
                        _cellSize * cell.Position.y + (_cellSize / 2f) - ((_height * _cellSize) / 2f),
                        0
                    );

                    if (cell.hasTopWall)
                        cell.TopWall = CreateWall(calcdCellPosition + new Vector3(0, _cellSize / 2f, 0), true);
                    if (cell.hasRightWall)
                        cell.RightWall = CreateWall(calcdCellPosition + new Vector3(_cellSize / 2f, 0, 0), false);

                    if (x == 0 && cell.hasLeftWall)
                        cell.LeftWall = CreateWall(calcdCellPosition - new Vector3(_cellSize / 2f, 0, 0), false);
                    if (y == 0 && cell.hasBottomWall)
                        cell.BottomWall = CreateWall(calcdCellPosition - new Vector3(0, _cellSize / 2f, 0), true);
                }
            }

            CreateMouse();
        }

        private GameObject CreateWall(Vector3 pos, bool isHorizontal)
        {
            Transform wall = Instantiate(_wall, pos, Quaternion.identity, transform);

            if (isHorizontal)
                wall.localScale = new Vector3(_cellSize, wall.localScale.y, wall.localScale.z);
            else
                wall.localScale = new Vector3(wall.localScale.x, _cellSize, wall.localScale.z);

            return wall.gameObject;
        }

        private void CreateMouse()
        {
            Vector3 zeroPosition = new Vector3((_cellSize / 2) - ((_width * _cellSize) / 2), (_cellSize / 2) - ((_height * _cellSize) / 2), 0);
            Transform mouse = Instantiate(_mouse, zeroPosition, Quaternion.identity, transform).transform;
            mouse.localScale = new Vector3(_cellSize * 0.65f, _cellSize * 0.65f, mouse.localScale.z);
            mouse.GetComponent<Mouse>().Initialize(MazeGrid[0, 0]);
        }
    }
}