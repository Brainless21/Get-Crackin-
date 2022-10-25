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
   
   public static Vector3Int ConvertCordsToInt(Vector3 floatCords)
   {
       	Vector3Int cordsInt = new Vector3Int();
        cordsInt.x = Mathf.RoundToInt(floatCords.x);
        cordsInt.y = Mathf.RoundToInt(floatCords.y);
        cordsInt.z = Mathf.RoundToInt(floatCords.z);

        return cordsInt;
    }

    public static int CalculateCostOfShape(int shape, int size, int costPerTile)
    {
        int result = 0;

        if(shape==c.fiber) result = size*costPerTile;
        

        if(shape == c.nugget) result = 3*costPerTile;

        if(shape == c.single) result = costPerTile;

        if(shape == c.blob)
        {
            if(size <= 1) result = costPerTile;
            if(size == 2) result = 7*costPerTile;
            if(size == 3) result = 19*costPerTile;
            if(size>3) Debug.Log("jetzt aber mal kleine brötchen backen pls");
        }
        return result;
    }

    public static float ConvertVectorToRotationAngle(Vector3 vector)
    {
        float result = Mathf.Acos((Vector3.Dot(c.rf,vector))/(c.rf.magnitude*vector.magnitude)); //mathemagie, phi = arccos(a*b/|a|*|b|)
        return result;
    }

    public static float ConvertRadiansToDegree(float radians)
    {
        return (radians*360)/(2*Mathf.PI);
    }
}
