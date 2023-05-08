﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crack : MonoBehaviour
{
    public Button playButton;
    // public Button resetButton;
    public Vector3Int cords{get;set;}
    private float finalScore =0;
    private MapTile occupiedTile;
    [SerializeField] List<Etappe> etappen;
    [SerializeField] Mesh lookOfDestiny;
    [SerializeField] TextMeshPro HighscoreDisplay;
    int highscore;

    Etappe currentEtappe;
    [SerializeField] private int etappenCounter = 0;
    float bestResult = 0; 
    float distanceCurrent=1;
    // einheitsvektor in richtung crack Propagation
    Vector3 stressDirection = new Vector3(Mathf.Sqrt(2),0f,-Mathf.Sqrt(2));
    Vector3Int startPoint = new Vector3Int();
    [SerializeField] List<Vector3Int> activeDestinations = new List<Vector3Int>();
    Coroutine propagate;
    float zeit;
    bool isCrackCoroutineRunning = false;
    // public enum CrackMode
    // {
    //     Point,
    //     Direction
    // }
    c.CrackMode crackMode;
    // public CrackMode GetCrackMode()
    // {
    //     return crackMode;
    // }


    void Awake()
    {
        cords = Vector3Int.FloorToInt(this.transform.position);
        startPoint = cords;


        //propagate = StartCoroutine(CrackPropagation());

        // resetButton.onClick.AddListener(ResetCrack);
        playButton.onClick.AddListener(StartCrackPropagation); // the orderis very important bc they are both triggered by the same play button, so we need to reset before starting
    }

    private void Start() 
    {
        // currentEtappe = etappen[etappenCounter]; //lädt die aktuelle etappe (normalerweise die erste, counter=0) und setzt start und ziele fest
        // activeDestinations.AddRange(currentEtappe.destinations);
        // EventManager.instance.SetGlobalStress(currentEtappe.globalStress);
        // cords = currentEtappe.start;
        FindEtappen();
        LoadEtappeByIndex(etappenCounter);
    }

    private void Update()
    {
        this.transform.position = cords+MapBuilder.instance.GetTileAdjustment();
    }

    void FindEtappen()
    {
        int etappenCount = this.gameObject.transform.childCount;
        for(int i =0; i<etappenCount; i++)
        {
            etappen.Add(this.gameObject.transform.GetChild(i).GetComponent<Etappe>());
        }
    }

    void StartCrackPropagation()
    {
        
        if(isCrackCoroutineRunning) return;
        if(MouseCursor.instance.isEnabled==false) return;
        ResetCrack();
        UpdateDistance();
        crackMode = EventManager.instance.GetCrackMode();
        foreach(Vector3Int destination in activeDestinations)
        {
            // TileLedger.ledgerInstance.GetTileByCords(destination).gameObject.GetComponent<MeshFilter>().mesh = lookOfDestiny;
        }
        occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);
        occupiedTile.Crack();
        propagate = StartCoroutine(CrackPropagation());
        
    }

    void UpdateDistance()
    {
        if(activeDestinations==null) return;
        
        
        distanceCurrent = Utilities.FindSmallestDistance(cords, activeDestinations);
        
    }

    // float CalculateProgress(CrackMode mode)
    // {
    //     float progress;
    //     if(mode == CrackMode.Point)
    //     {
    //         distanceNew = Vector3Int.Distance(destination, inspectedTile.cords);
    //         progress = distanceCurrent
    //     }
    // }

    void ResetCrack()
    {
        if(isCrackCoroutineRunning) return;
        MapBuilder.instance.RestartMap();
        Debug.Log("crack resetted");
        LoadEtappeByIndex(0);
        tiebreaker = 1;
    }

    void LoadEtappeByIndex(int index)
    {
        etappenCounter = index;
        currentEtappe=etappen[index];
        cords = currentEtappe.start;
        occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);
        occupiedTile.Crack();
        activeDestinations.Clear();
        activeDestinations.AddRange(currentEtappe.destinations);
        EventManager.instance.SetGlobalStress(currentEtappe.globalStress);
    }

    int tiebreaker = 1;
    private MapTile FindBestFriend(Vector3Int currentCords, List<Vector3Int> destinations)
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
        int i = 0;
        foreach(MapTile inspectedTile in currentFriendsRange1)
        {
            i++;
            if(inspectedTile==null) continue; //skips to next i if the inspected tile is null
            Vector3Int step = new Vector3Int();
            // der vektor, der den aktuell considered schritt wiedergibt, also als richtung
            step = inspectedTile.cords-cords;
            float distanceNew = Utilities.FindSmallestDistance(inspectedTile.cords,destinations);
            float progress=0;

            // im Point Modus wird als Progress angesehen wie weit mann dem ziel näher gekommen ist
            if(crackMode==c.CrackMode.Point)
            {
                progress = distanceCurrent-distanceNew;  
            }

            // im Direction Mode wird als Progress angesehen, wie weit man gelaufen ist (step) aber projeziert auf die präferierte richtung
            if(crackMode==c.CrackMode.Direction)
            {
                // hier kann man einiges auf 1 skalieren. Die Beiden Vektoren vor dem zusammenwursten, den resultierenden vektor nach dem zusammenwursten. Jo das wars eig auch schon.
                // ich probier als erstes mal, die vorm zusammenwursten nicht zu skalieren, den resultierenden vor dem skalarprodukt aber schon. Die hoffnung ist, dass ich dadurch das abstoßende verhalten an der interfac der phase Change TIles behalte, aber nicht mehr das weirde hin her bei den abstoßenden 
                Vector3 stressDirection = 0.5f*inspectedTile.GetStressState()+0.5f*occupiedTile.GetStressState();
                // progress = Vector3.Dot(Utilities.ScaleLenghtToOne(stressDirection),step); //diese variante gibt punkte nur nach richtung des stressVectors, nicht nach länge
                // progress = Vector3.Dot(stressDirection,step); // diese variante berückscihtigt auch wie lang der stress vektor ist, also bei mehr stress in eine richtung gibts insgesamt weniger punkte für den schritt
                progress = Vector3.Dot(Utilities.LimitVectorLength(stressDirection),step); // diese variante ist eine mischung aus den zweien davor. Sie berücksichtigt die länge des stressVectors, aber der effekt wird nie größer als 1.5
            }
            //berechnet die distanz vom betrachteten tile zum ziel, dann wie viel weiter man dem ziel kommt wenn man auf das tile geht
            if(progress<0)
            {
                //Debug.Log("dieser schritt führt uns nicht näher zum ziel");
                //continue; //bricht ab, wenn das tile nicht näher zum ziel führt. I think not quite tho, bc we have the code underneath executed 6 times everytime
            }

            // berechnet wie gut das tile ist, basierend darauf wie viel näher es dem ziel kommt, und wie schwierig es dem crack ist, dort hinzugehen
            float awesomeness = progress/inspectedTile.GetToughness();
            // wenn das tile besser ist, als das beste bis jetzt, wird es als neues bestes tile abgespeichert, zusammen mit seinen cords und dem score
            //Debug.Log(string.Format("current awesomeness :{0}\ncurrent bestResult: {1}\n current tiebreaker; {2}\n current tile:{3}\n currentFriendlist has: {4}",awesomeness,bestResult,tiebreaker,i,currentFriendsRange1.Count));
            if(awesomeness>bestResult) // to give your tile the status of best friend, you either need to have a better score OR it needs to be very close AND we havent made an exeption bc of closeness last time
            {
        
               bestResult = awesomeness;
               bestFriend = inspectedTile;
               bestFriendCords = inspectedTile.cords;
               // Debug.Log(string.Format("non-tiebreaker case eingetragen ({0})",bestResult));
               
            } 

            else if(awesomeness==bestResult)
            {
                if(tiebreaker%4==0)
                {
                    bestResult = awesomeness;
                    bestFriend = inspectedTile;
                    bestFriendCords = inspectedTile.cords;
                    // Debug.Log(string.Format("tiebreaker case activated ({0})",bestResult)); 

                }
                Debug.Log(string.Format("we are in the tiebreaker bracket, tiebreaker is: {0}",tiebreaker));
                tiebreaker++;

            }
            
        }
        //Debug.Log("done mit einmal friends durchgehen");

        // der ganze spaß wird ctr v wiederholt, nur diesmal mit den friends in range 2. Deren score bekommt nen faktor von 0.1, um den wurzel-verlauf der spannung zu simulieren
        // foreach(MapTile inspectedTile in currentFriendsRange2)
        // {
            
        //     if(inspectedTile==null) continue; //skips to next i if the inspected tile is null
        //     Vector3Int step = new Vector3Int();
        //     // der vektor, der den aktuell considered schritt wiedergibt, also als richtung
        //     step = inspectedTile.cords-cords;
        //     float distanceNew = Utilities.FindSmallestDistance(inspectedTile.cords, destinations);
        //     float progress = 0;

        //     if(crackMode==c.CrackMode.Point) progress = distanceCurrent-distanceNew;   //berechnet die distanz vom betrachteten tile zum ziel, dann wie viel weiter man dem ziel kommt wenn man auf das tile geht
        //     if(crackMode==c.CrackMode.Direction) progress = Vector3.Dot(stressDirection, step);
            
        //     if(progress<0)
        //     {
        //         //Debug.Log("dieser schritt führt uns nicht näher zum ziel");
        //         continue; //bricht ab, wenn das tile nicht näher zum ziel führt
        //     }

        //     // berechnet wie gut das tile ist, basierend darauf wie viel näher es dem ziel kommt, und wie schwierig es dem crack ist, dort hinzugehen
        //     float awesomeness = 0.0001f*progress/inspectedTile.GetToughness();

        //     // wenn das tile besser ist, als das beste bis jetzt, wird es als neues bestes tile abgespeichert, zusammen mit seinen cords und dem score
        //     if(awesomeness>bestResult)
        //     {
        //        bestResult = awesomeness;
        //        bestFriend = inspectedTile;
        //        bestFriendCords = inspectedTile.cords; 
        //     } 

        // }

        if(bestFriend==null) Debug.Log("die tile auswahl war leider shit");
        zeit = 0.2f*1/(1+Mathf.Exp(-bestResult));
        return bestFriend;
    }
    
    bool InitiateNextStage()
    {
        // if(currentEtappe==null) currentEtappe = etappen[0]; I feel like not putting this in the beginning makes it more intuitive, and we can start by upping the etappe by one and then laoding all the new stuff in
        Debug.Log("next stage initiated at least attemptetively");
        currentEtappe.SetScore(Mathf.RoundToInt(finalScore*10));

        if(currentEtappe==etappen[etappen.Count-1]) //wenn man schon in der letzten etappe ist, wenn das aufgerufen wird, wird false ausgegeben und das heißt der crack ist fertig
        {
            Debug.Log("letzte wtappe erreicht, well done. Final score:");
            //SpawnEndCard();
            // berechnet den score insgesamt über alle etappen und setzt den highscore wenn er größer ist als der bestehende 
            int gesamtScore = currentEtappe.GetScore();
            Debug.Log(string.Format("gesamtscore ursprünglich gesetzt als: {0}",gesamtScore));

            foreach(Etappe etappe in etappen)
            {
            if(gesamtScore>etappe.GetScore()) gesamtScore = etappe.GetScore(); // jeder score wird durchgegangen, und der kleinste davon bleibt als gesamtScore stehen
            }

            Debug.Log(string.Format("gesamtscore am ende ausgewertet als: {0}",gesamtScore));

            if(gesamtScore>=highscore)
            {
                HighscoreDisplay.text = Mathf.Round(gesamtScore).ToString();
                highscore = gesamtScore;
            }
            LevelProgression.instance.Activate(gesamtScore);


            // so this whole score jiggle comes out as 0 actually, which kinda makes no sense because at least the last etappe has a score in the end so the result should be that but its not 
            return false;
        }

        EventManager.instance.InvokeEtappenEnde();
        etappenCounter ++; // zählt eins hoch
        finalScore = 0;
        // currentEtappe = etappen[etappenCounter]; // läd die werte der neuen etappe in den speicher
        
        // cords = currentEtappe.start; // versetzt den riss in die neue position
        // // aktualisiert die liste wo alle aktiven ziele drinstehen
        // activeDestinations.Clear();
        // activeDestinations.AddRange(currentEtappe.destinations);
        // EventManager.instance.SetGlobalStress(currentEtappe.globalStress);

        LoadEtappeByIndex(etappenCounter);
        return true;  
        
    }

    private bool Propagate()
    {

        bool amIOnADestination = false;
        foreach(Vector3Int destination in activeDestinations)
        {
            if(cords==destination)
            {
                amIOnADestination = true;
                break;
            }
        }

        if(amIOnADestination == true) 
        {   
            // etappe ++; // steht hier, weil bei 0 angefangen wird zu zählen, aber count mind. 1 ausgibt (außer activeDestinations ist leer)
            
            // if (etappe==activeDestinations.Count) // das stimmt jetzt so auch nicht mehr, es sollte eine extra variable geben die sagt wie vilee stationen es denn gibt
            // {
            // Debug.Log("bin angekommen");
            // return false;
            // }

            // overhaul für das obenstehende
            // currentEtappe.SetScore(Mathf.RoundToInt(finalScore*10)); // speichert den aktuellen score als etappenscore ab
            // finalScore = 0;
            UpdateDistance();
            // PointPopup.Create(this.cords,finalScore,5,4);
            return InitiateNextStage();

            
        }

        if(occupiedTile!=null) 
        {
        occupiedTile.SayHelloToFriends(4); //der crack sagt allen tiles in range i bescheid, dass er in range i von ihnen ist
        }

        if(FindBestFriend(cords, activeDestinations)==null)
        {
            Debug.Log("Kein passender nächster schritt gefunden");
            return false;        
        }

        {
            cords = FindBestFriend(cords, activeDestinations).cords;
            occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);
            occupiedTile.Crack();
            UpdateDistance();

            float stepScore = 1/bestResult;
            PointPopup.Create(cords,stepScore);
            //Debug.Log (stepScore);
            finalScore += stepScore;
            return true;
        }
    }

    private IEnumerator CrackPropagation()
    {
        isCrackCoroutineRunning = true;
        //float zeit = 0.7f;
        //yield return new WaitForSeconds(5f);
        occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);

        while(Propagate()) // the crack will look for a better tile and move there until there are not better spots found or a destination is reached
        {
            yield return new WaitForSeconds(zeit);
            // float minimumZeitIntervall = 0.25f;
            // if(zeit>minimumZeitIntervall) zeit /= 1.4f;
        }

        Debug.Log(finalScore);
        if(Mathf.Round(finalScore*10)>highscore) highscore = Mathf.RoundToInt(finalScore*10);
        // HighscoreDisplay.text = Mathf.Round(highscore).ToString();
        // PointPopup.Create(new Vector3Int(-1,-2,3),finalScore,5,4);

        finalScore = 0;

        //cords = startPoint;
        //etappenCounter=0;
        UpdateDistance();

        isCrackCoroutineRunning = false;
        yield break;
    }

    
}
