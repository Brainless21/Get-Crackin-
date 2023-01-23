using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static void PrintArray(int[] intArray)
    {
        string ausgabe = "";
        ausgabe = intArray.ToString();
        Debug.Log(ausgabe);
    }

    public static void PrintArray(float[] floatArray)
    {
        if(floatArray==null)
        {
            Debug.Log("Array ist NULL");
            return;
        }
        string ausgabe = "";
        ausgabe = floatArray.ToString();
        Debug.Log(ausgabe);
        Debug.Log("printing completed");
    }
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
    public static void PrintDictionary(Dictionary <Vector3Int,Vector3> dict)
    {
        string ausgabe = "";
        foreach (KeyValuePair<Vector3Int, Vector3> kvp in dict)
    	{
            ausgabe += string.Format("Key = {0}, Value = {1} \n", kvp.Key, Utilities.GetPreciseVectorString(kvp.Value));
        }
        Debug.Log(ausgabe);
    }

    public static Vector3 ScaleLenghtToOne(Vector3 vector)
    {
        if(vector.magnitude==0) return c.origin;
        return vector/vector.magnitude;
    }
    public static string GetListString(List<Vector3> List)
    {
        string ausgabe = "";
        foreach(Vector3 vector in List)
        {
            ausgabe+=string.Format("{0} \n",GetPreciseVectorString(vector));
        }
        return ausgabe;
    }

    public static string GetPreciseVectorString(Vector3 vector)
    {
        string ausgabe = "";
        ausgabe += string.Format("\nx: {0}\ny: {1}\nz: {2}",vector.x,vector.y,vector.z);
        return ausgabe;
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

    public static Vector3 ConvertIntToCords(Vector3Int intCords)
    {
        Vector3 floatCords = new Vector3((float)intCords.x,(float)intCords.y,(float)intCords.z);
        return floatCords;
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
        // Vector3 delta = vector-c.rf;
        // if(delta.magnitude<0.01) return 0; //didnt work :(
      
        float boundsCheck = (Vector3.Dot(c.rf,vector))/(c.rf.magnitude*vector.magnitude);
        if(boundsCheck>1) // da arccos nur bis 1 definiert ist, und manchmal floatbedingt eine 1.000001 rauskommt, wenn eigentlich 1 rauskommen müsste, wird das hier einmal abgefangen
        {
            float delta = boundsCheck-1;
            if(delta>0.01) Debug.Log(string.Format("die abweichung war mit {0} größer als erwartet",delta));
            boundsCheck =1;
        }

        if(boundsCheck<-1) // da arccos nur bis 1 definiert ist, und manchmal floatbedingt eine 1.000001 rauskommt, wenn eigentlich 1 rauskommen müsste, wird das hier einmal abgefangen
        {
            float delta = boundsCheck+1;
            if(delta>0.01) Debug.Log(string.Format("die abweichung war mit {0} größer als erwartet",delta));
            boundsCheck =-1;
        }


        float result = Mathf.Acos(boundsCheck); //mathemagie, phi = arccos(a*b/|a|*|b|)
        bool isPointingDown = false;
        
        //vektor wird überprüft ob er nach unten zeigt
        vector/=vector.magnitude; // skaliert auf länge 1       // DAS GLEICH WIEDER EINKOMMENTTIEREN!
        
        vector*=Mathf.Sqrt(2); // skaliert auf länge wurzel(2) //und weil das nicht klappt versuchen wirs direkt nochmal //DAS AUCH!!
        if(vector.y>0) isPointingDown = true; // das überprüft ob der vektor nach unten zeigt. wenn die werte der einheitsvektoren ungefähr zu den angegebenen stimmen, ist es nach unten, dann wird der winkel andersrum gezählt

        if(isPointingDown==false)
        {
            result*=-1;
        }

        if(float.IsNaN(result))
        {
            Debug.Log(string.Format("eingehender Vektor; {0}\n boundsCheck: {1}",vector,(Vector3.Dot(c.rf,vector))/(c.rf.magnitude*vector.magnitude)));
        }
       
        return result;
    }

    public bool isValidPosition(Vector3Int position)
    {
        if(position.x+position.y+position.z==0) return true;
        return false;
    }

    public static float ConvertRadiansToDegree(float radians)
    {   
        float result = (radians*360)/(2*Mathf.PI);
        return result;
    }

    public static bool IsBasicallyEqual(float number1, float number2)
    {
        if((number1 - number2)<0.001 ) return true;
        return false;
    }

   public static float FindSmallestDistance(Vector3Int pos1, List<Vector3Int> destinationsList) // returns the magnitude of the smallest possible distance between pos1 and all the spots in the list
   {
       if(destinationsList == null|destinationsList.Count==0)
       {
           Debug.Log("keine validen Destinations mitgegeben");
           return 0;
       }
       float smallestDistance = Vector3Int.Distance(pos1, destinationsList[0]);
       foreach(Vector3Int spot in destinationsList)
       {
           float distance = Vector3Int.Distance(spot, pos1);
           if(distance<smallestDistance) smallestDistance = distance;
       }

       return smallestDistance;
   }

   public static Vector3Int getDirectionVectorByEnum(c.richtungsvektoren direction)
   {
    bool wholeDirection = true; // wird danach eh geändert aber der compiler meckert wenn ich das hier nicht zuweise weil ja die direction auch null sien könnte I guess?
    if((int)direction%2!=0) wholeDirection = true;
    if((int)direction%2==0) wholeDirection = false;
    int index = Mathf.RoundToInt((int)direction / 2);

    if(wholeDirection == true) return c.positionsArray[index];
    else return c.positionsArrayHalb[index];



       
   }

}
