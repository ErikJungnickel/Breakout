using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Author: Erik Jungnickel - http://backyard-dev.de
/// Data class that represents a single level (the arrangement of the blocks)
/// </summary>
[System.Serializable]
public class Level
{
    public string name;
    public int width;
    public int height;
    public int levelIndex;

    public int[] levelStructure;

    public Level(int width, int height)
    {
        this.width = width;

        this.height = height;

        levelStructure = new int[width * height];
    }

    public void FillRandom()
    {
        for (int i = 0; i < width * height; i++)
        {
            levelStructure[i] = Random.Range(0, 4); //set random hp
        }
    }
}
