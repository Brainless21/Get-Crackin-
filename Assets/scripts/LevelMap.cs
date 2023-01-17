using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class LevelMap
{

    public string mapName;
    public int[][] tileShapes;
    public float[][] tileProperties; 
    public string[] tileNames;

    public LevelMap(string currentMapName, string[] currentTileNames, int[][] currentTileShapes, float[][] currentTileProperties)
    {
        
        int tilesInMap = currentTileNames.Length;


        tileNames = new string[tilesInMap];
        tileShapes = new int[tilesInMap][];
        tileProperties = new float [tilesInMap][];

        mapName = currentMapName;
        tileNames = currentTileNames;
        tileShapes = currentTileShapes;
        tileProperties = currentTileProperties;
    }
}
