using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static void PrintDictionary(Dictionary<Vector3Int, MapTile> dict)
    {
        string ausgabe = "";
        foreach (KeyValuePair<Vector3Int, MapTile> kvp in dict)
    	{
            ausgabe += string.Format("Key = {0}, ", kvp.Key);
        }
        Debug.Log(ausgabe);
    }

      public static void PrintDictionary(Dictionary<int, int> dict)
    {
        string ausgabe = "";
        foreach (KeyValuePair<int, int> kvp in dict)
    	{
            ausgabe += string.Format("Key = {0}, Value = {1} \n", kvp.Key, kvp.Value);
        }
        Debug.Log(ausgabe);
    }
   
   
}
