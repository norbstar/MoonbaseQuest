using System;

using UnityEngine;

public enum BlockColors {blank, red, blue, green, yellow, cyan, white, purple};

[Serializable]
public class Level
{
    #if UNITY_EDITOR
    [HideInInspector] public bool showBoard;
    #endif

    public int rows = 8;
    public int columns = 8;
    public BlockColors [,] board;

    private Level()
    {
        board = new BlockColors[columns, rows];
    }
}