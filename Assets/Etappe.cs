using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Etappe : MonoBehaviour
{
    public List<Vector3Int> destinations;
    public c.richtungsvektoren globalStress;
    public Vector3Int start;
    [SerializeField] Vector3Int startTile;
    [SerializeField] Vector3Int endTile;
    [SerializeField] int index;
    [SerializeField] int score;
    void Awake()
    {
        FindDestinations();
    }

    public void SetScore(int newScore)
    {
        score = newScore;
    }

    public int GetScore() => score;

    void FindDestinations()
    {
    
        Vector3Int gerade = endTile - startTile;
        Vector3Int step = new Vector3Int();

        foreach(Vector3Int richtung in c.positionsArray)
        {
            if(Utilities.IsBasicallyEqual(Vector3.Angle(gerade,richtung),0))
            {
                step = richtung;
                Debug.Log(string.Format("passende richtung gefunden: {0}", richtung));
            } 
        }
        foreach(Vector3Int richtung in c.positionsArrayHalb)
        {
            if(Utilities.IsBasicallyEqual(Vector3.Angle(richtung,gerade),0))
            {
                step = richtung;
                Debug.Log(string.Format("passende richtung gefunden: {0}", richtung));
            } 
        }
        if(step==null) Debug.Log("das richtung finden hat nicht geklappt :(");

        int längeInSteps = Mathf.RoundToInt(gerade.magnitude/step.magnitude);
        for(int i=0; i<=längeInSteps; i++)
        {
            destinations.Add(startTile+i*step);
        }

        

    }

    
}
