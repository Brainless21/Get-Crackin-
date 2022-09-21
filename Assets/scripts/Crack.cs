using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crack : MonoBehaviour
{
    public Button playButton;
    public Vector3Int cords{get;set;}
    private float finalScore =0;
    private MapTile occupiedTile;
    //private Vector3Int destinationsCords = new Vector3Int(11,-2,-9);
    private List<Vector3Int> destinationsCords = new List<Vector3Int>
        {
            new Vector3Int(8,-2,-6),
            // new Vector3Int(-9,4,5),
            // new Vector3Int(11,-8,-3),
            // new Vector3Int(-9,-2,11) 
        };
    

    private int etappe = 0;
    float bestResult = 0; 
    float distanceCurrent=1;
    Coroutine propagate;
    void Awake()
    {
        cords = Vector3Int.FloorToInt(this.transform.position);

        UpdateDistance();

        //propagate = StartCoroutine(CrackPropagation());

        playButton.onClick.AddListener(StartCrackPropagation);
    }

    private void Update()
    {
        this.transform.position = cords;
    }

    void StartCrackPropagation()
    {
        propagate = StartCoroutine(CrackPropagation());
    }

    void UpdateDistance()
    {
        if(destinationsCords[etappe]!=null)
        {
            distanceCurrent = Vector3Int.Distance(destinationsCords[etappe], cords);
        }
    }


    private MapTile FindBestFriend(Vector3Int currentCords, Vector3Int destination)
    {
        if(occupiedTile==null)
        {
            Debug.Log("ich stehe nicht auf einem tile");
            return null;
        } 

        // gets all friends in range 1 and range 2, each in a seperate list
        List<MapTile> currentFriendsRange1 = occupiedTile.GetFriendsByRange(1);
        List<MapTile> currentFriendsRange2 = occupiedTile.GetFriendsByRange(2);
       
        // declares mapTile and cord variable for the end result
        MapTile bestFriend = null;
        Vector3Int bestFriendCords;
        bestResult = 0;

        foreach(MapTile inspectedTile in currentFriendsRange1)
        {
            
            if(inspectedTile==null) continue; //skips to next i if the inspected tile is null
            float distanceNew = Vector3Int.Distance(destination, inspectedTile.cords);
            float progress = distanceCurrent-distanceNew;   //berechnet die distanz vom betrachteten tile zum ziel, dann wie viel weiter man dem ziel kommt wenn man auf das tile geht
            if(progress<0)
            {
                //Debug.Log("dieser schritt führt uns nicht näher zum ziel");
                continue; //bricht ab, wenn das tile nicht näher zum ziel führt
            }

            // berechnet wie gut das tile ist, basierend darauf wie viel näher es dem ziel kommt, und wie schwierig es dem crack ist, dort hinzugehen
            float awesomeness = progress/inspectedTile.GetToughness();

            // wenn das tile besser ist, als das beste bis jetzt, wird es als neues bestes tile abgespeichert, zusammen mit seinen cords und dem score
            if(awesomeness>bestResult)
            {
               bestResult = awesomeness;
               bestFriend = inspectedTile;
               bestFriendCords = inspectedTile.cords; 
            } 

        }

        // der ganze spaß wird ctr v wiederholt, nur diesmal mit den friends in range 2. Deren score bekommt nen faktor von 1/4, um den wurzel-verlauf der spannung zu simulieren
        foreach(MapTile inspectedTile in currentFriendsRange2)
        {
            
            if(inspectedTile==null) continue; //skips to next i if the inspected tile is null
            float distanceNew = Vector3Int.Distance(destination, inspectedTile.cords);
            float progress = distanceCurrent-distanceNew;   //berechnet die distanz vom betrachteten tile zum ziel, dann wie viel weiter man dem ziel kommt wenn man auf das tile geht
            if(progress<0)
            {
                //Debug.Log("dieser schritt führt uns nicht näher zum ziel");
                continue; //bricht ab, wenn das tile nicht näher zum ziel führt
            }

            // berechnet wie gut das tile ist, basierend darauf wie viel näher es dem ziel kommt, und wie schwierig es dem crack ist, dort hinzugehen
            float awesomeness = 0.1f*progress/inspectedTile.GetToughness();

            // wenn das tile besser ist, als das beste bis jetzt, wird es als neues bestes tile abgespeichert, zusammen mit seinen cords und dem score
            if(awesomeness>bestResult)
            {
               bestResult = awesomeness;
               bestFriend = inspectedTile;
               bestFriendCords = inspectedTile.cords; 
            } 

        }

        if(bestFriend==null) Debug.Log("die tile auswahl war leider shit");
        return bestFriend;
    }

    private bool Propagate()
    {

        if(cords==destinationsCords[etappe])
        {   
            etappe ++; // steht hier, weil bei 0 angefangen wird zu zählen, aber count mind. 1 ausgibt (außer destinationsCords ist leer)
            
            if (etappe==destinationsCords.Count)
            {
            Debug.Log("bin angekommen");
            return false;
            }

            UpdateDistance();
        }

        if(occupiedTile!=null) occupiedTile.SayHelloToFriends(1); //der crack sagt allen tiles in range i bescheid, dass er in range i von ihnen ist

        if(FindBestFriend(cords, destinationsCords[etappe])==null)
        {
            Debug.Log("Kein passender nächster schritt gefunden");
            return false;        
        }

        {
            cords = FindBestFriend(cords, destinationsCords[etappe]).cords;
            occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);
            UpdateDistance();
            occupiedTile.Crack();

            float stepScore = 1/bestResult;
            PointPopup.Create(cords,stepScore);
            Debug.Log (stepScore);
            finalScore += stepScore;
            return true;
        }
    }

    private IEnumerator CrackPropagation()
    {
        float zeit = 0.7f;
        //yield return new WaitForSeconds(5f);
        occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);

        while(Propagate())
        {
            yield return new WaitForSeconds(zeit);
            float minimumZeitIntervall = 0.25f;
            if(zeit>minimumZeitIntervall) zeit /= 1.4f;
        }
        Debug.Log(finalScore);
        PointPopup.Create(new Vector3Int(-1,-2,3),finalScore,5,4);
        finalScore = 0;

        cords = new Vector3Int (-9,-2,11);
        etappe=0;
        UpdateDistance();

        yield break;
    }

    
}
