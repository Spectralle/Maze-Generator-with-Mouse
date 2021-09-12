using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MazeGenerator
{
    public class Target : MonoBehaviour
    {
        private Camera _cam;
        private Maze _mazeObject;
        private Mouse _mouse;


        private void Awake()
        {
            TryGetComponent(out _cam);
            _mazeObject = FindObjectOfType<Maze>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && _cam)
            {
                Vector2 dest = _cam.ScreenToWorldPoint(Input.mousePosition);

                Maze.Data _data = _mazeObject.GetData();
                if (_data._mazeExists)
                {
                    if (dest.x >= -(_data._width / 2f) && dest.x < (_data._width / 2f) && dest.y >= -(_data._height / 2f) && dest.y < (_data._height / 2))
                    {
                        if (!_mouse)
                            _mouse = FindObjectOfType<Mouse>();
                        _mouse.StartPathfinding(dest);
                    }
                    else
                        Debug.Log("Invalid destination");
                }
                else
                    Debug.Log("Maze doesn't exist");
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
    }
}