using System;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]
public class GameController : MonoBehaviour
{
    private MazeConstructor constructor;

    [SerializeField] private int rows;
    [SerializeField] private int cols;

    private void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
    }

    private void Start()
    {
        constructor.GenerateNewMaze(rows, cols);
    }
}
