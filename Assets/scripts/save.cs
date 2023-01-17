using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class save : MonoBehaviour
{

    public static Dictionary<Vector3Int,GameObject> mapToBeSaved = new Dictionary<Vector3Int, GameObject>();

    public static string mapName;
    public static string[] tileNames;
    public static int[][] tileShapes;
    public static float[][] tileProperties;    

    public static string GetMapName() => mapName;
    public static string[] GetTileNames() => tileNames;
    public static int[][] GetTileShapes() => tileShapes;
    public static float[][] GetTileProperties() => tileProperties; 

    


    public static void SaveCurrentMapUnder(string currentMapName, string[] currentTileNames, int[][] currentTileShapes, float[][] currentTileProperties)
    {
        int tilesInMap = currentTileNames.Length;


        tileNames = new string[tilesInMap];
        tileShapes = new int[tilesInMap][];
        tileProperties = new float [tilesInMap][];

        mapName = currentMapName;
        tileNames = currentTileNames;
        tileShapes = currentTileShapes;
        tileProperties = currentTileProperties;
        
        SaveFile();
    }

    public static bool LoadSavedMap(string nameStr)
    {
        mapName = nameStr;
        return(LoadFile());
    }
    // void Start()
    // {
    //     SaveFile();
    //     LoadFile();
    // }

    public static void SaveFile()
    {
        string endung = "/";
        endung += mapName;
        endung += ".dat";

        string destination = Application.persistentDataPath + endung;
        FileStream file;

        if(File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        LevelMap map = new LevelMap(mapName, tileNames, tileShapes, tileProperties);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, map);
        file.Close();
        Debug.Log(destination);
    }

    static bool LoadFile()
    {
        string endung = "/";
        endung += mapName;
        endung += ".dat";
        
        string destination = Application.persistentDataPath + endung;
        FileStream file;

        if(File.Exists(destination)) file = File.OpenRead(destination);
        else 
        {
            Debug.Log("file not found");
            return false;
        }

        BinaryFormatter bf = new BinaryFormatter();
        LevelMap map = (LevelMap) bf.Deserialize(file);
        file.Close();

        tileNames=map.tileNames;
        tileShapes = map.tileShapes;
        tileProperties = map.tileProperties;
        return true;
    }
    
}
