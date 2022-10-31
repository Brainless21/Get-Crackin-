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
   
    public static void PrintList(List<MapTile> list)
    {
        string ausgabe = "";
        foreach(MapTile tile in list)
        {
            ausgabe += string.Format("{0} \n", tile.cords);
        }
        Debug.Log(ausgabe);
    }

    public static void PrintVector(Vector3 vector)
    {
        string ausgabe = "";
        ausgabe += string.Format("\n {0} \n {1} \n {2}",vector.x,vector.y,vector.z);
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
        if(vector==c.rf) return 0;
        if(vector==c.rf)
        {
            float test = Vector3.Dot(c.rf,vector);
            Debug.Log(test);
        }

        float result = Mathf.Acos((Vector3.Dot(c.rf,vector))/(c.rf.magnitude*vector.magnitude)); //mathemagie, phi = arccos(a*b/|a|*|b|)
        bool isPointingDown = false;
        
        //vektor wird überprüft ob er nach unten zeigt
        vector/=vector.magnitude; // skaliert auf länge 1
        
        vector*=Mathf.Sqrt(2); // skaliert auf länge wurzel(2) //und weil das nicht klappt versuchen wirs direkt nochmal
        if(vector.y<0) isPointingDown = true; // das überprüft ob der vektor nach unten zeigt. wenn die werte der einheitsvektoren ungefähr zu den angegebenen stimmen, ist es nach unten, dann wird der winkel andersrum gezählt

        if(isPointingDown==false)
        {
            result*=-1;
        }


        // if(result!=0)
        // {   
        //    if(vector==c.rf) Debug.Log(vector);
        //    if(vector==c.rf) Debug.Log(result);
        // }

        if(vector==c.rf)
        {
            if(result!=0)
            {
                Debug.Log(vector);
                Debug.Log(result);
            }
        }

        return result;
    }

    public static float ConvertRadiansToDegree(float radians)
    {   
        float result = (radians*360)/(2*Mathf.PI);
        return result;
    }
}
