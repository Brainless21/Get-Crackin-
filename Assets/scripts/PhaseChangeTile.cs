using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseChangeTile : MapTile
{
    float interfaceStrengh = 0.7f;
    [SerializeField] bool isActive = false;
    [SerializeField] float stressFieldStrength=1;
    [SerializeField] bool star = true;
    public void SetStar(bool star) { this.star = star;}
    public void SetStressFieldStrength(float strength) {stressFieldStrength = strength;}
    internal int minRange=2; // wird beim placement overridden durch blob size
    public void SetMinRange(int blobSize)
    {
        minRange = blobSize;
    }
    private Coroutine delayedStress;
    private IEnumerator WaitFrameAndStress()
    {
        // waits for one frame before calling the stressout function, so the new tiles are already placed when it happens. 
        yield return 0;
        StressOutFriends(stressFieldStrength, !star,1,1);
        yield break;
    }
    

    public void MakeActive() 
    {
        isActive = true;
        StressOutFriends(stressFieldStrength, star, minRange,15);
        delayedStress = StartCoroutine(WaitFrameAndStress());
    }
     private void Awake()
    {
        Initialize(this);
        this.SetBaseToughness(2);
        this.AddBehavior(c.grenzflaeche, interfaceStrengh);
        this.typeKey = c.particleTile1;
        this.cost = FundsAccount.instance.GetPriceByType(typeKey);
        //TileLedger.ledgerInstance.SetGlobalStress(this.cords,EventManager.instance.GetGlobalStress()); //war mal this.SetBaseStressState(EventManager.instance.GetGlobalStress());
        //Debug.Log("bin eingeschrieben(awake)");
    }

    private void OnDisable() 
    {
        if(isActive)
        {
        Debug.Log("imma remove some stress");
        StressOutFriends(stressFieldStrength, star, minRange, 15, true); //removed die stresstates, die es vorher platziert hat. klappt nur wenn sich zwischendurch die werte für fieldsStrength, star und minRange nicht ändern!
        }
    }

   

    // public void SetStar(bool star) { this.star = star;} // steht jetzt in mapTile
    // public void SetStressFieldStrength(float strength) {stressFieldStrength = strength;}

    void StressOutFriends( float strength, bool isStar, int minRange, int maxRange, bool remove=false)
    {
    
        
        //Vector3 result = new Vector3 (5,5,5); // wird hier schon deklariert, damit es im forloop als exit condition benutzt werden kann
        bool doNextStep = true;

        // weggabelung zwischen intern oder extern:
        
        
        for(int i = minRange; i<=maxRange; i++)
        {
            //Debug.Log(i);
            //wenn das tile nicht im aktiv gemacht wurde nach dem plazieren (weil es im ursprung lag) dann passiert nichts. redundant, weil die funktion nur aufgerufen wird wenn das tile aktiv geschaltet wird
            if(!isActive) return;
            if(doNextStep==false) return; // wenn das resultat der stressvektoren zu klein wird, setzt das den bool auf false, und es wird nichtmehr weitergemacht, selbst wenn die maxrange noch nicht erreicht ist
            // find the friends in range
            List<MapTile> currentFriends = this.GetFriendsByRange(i);
            // Debug.Log(i);
            // Utilities.PrintList(currentFriends);

            // give them all their stress by figuring out the vector between source and friend, and then if the stress should be parallel or perpendicular
            foreach(MapTile tile in currentFriends) 
            {
                //Debug.Log("one iteration in the current range group");
                Vector3 connection = this.cords - tile.cords; // this has information about the direction but also already about the strength because the farther, the longer the vector
                float rangeMod = 1;
                if(star==isStar) // dies überprüft, ob der aufruf dem PartikelÄußeren zugehört. Weil dann wurde die funktion als StressoutFriends(,x star,y,z) aufgerufen. Ist das nicht der Fall, wurde (x,!star,y,z) aufgerufen
                {
                    rangeMod = 1/((connection.magnitude-c.olf.magnitude)*(connection.magnitude-c.olf.magnitude)); // entspricht hoffentlich dem 1/x^2 verlauf vom spannungsabfall? der olf dadrin ist der offset, damit erst am interface angefangen wird.
                }
                
                Vector3 result = rangeMod*connection*stressFieldStrength; // der resultierende vektor muss eventuell noch um 90° gefreht werden, falls das particle zu klein ist, also radiale druckspannung erzeugt
                Vector3 resultGedreht = new Vector3(result.x*c.t+result.y*c.a+result.z*c.b,result.x*c.b+result.y*c.t+result.z*c.a,result.x*c.a+result.y*c.b+result.z*c.t); 
                // wär wahrschienlich übersichtlicher gewesen, zu schauen wie man hier matrixmultiplikation macht und dann einfach die drehmatrix zu definieren. stattdessen steht hier schon ausgerechnet das ergebnis des vektors drin, die as und bs sind in c zu finden (lol) und sind 1/3 +- 1/sqrt3
                
                if(result.magnitude<0.5) doNextStep = false;
                
                if(isStar&&!remove)
                { 
                    TileLedger.ledgerInstance.AddToStressStates(tile.cords, result); // tile.AddToStressStates(result);
                    continue;
                }

                if(isStar&&remove) 
                {
                    TileLedger.ledgerInstance.RemoveStressFromPosition(tile.cords, result);  // Debug.Log(tile.RemoveFromStressStates(result));
                    continue;
                }

                if(!isStar&&!remove)
                {
                    TileLedger.ledgerInstance.AddToStressStates(tile.cords,resultGedreht); // tile.AddToStressStates(resultGedreht);
                    continue;
                }

                if(!isStar&&remove)
                {
                    TileLedger.ledgerInstance.RemoveStressFromPosition(tile.cords,resultGedreht);  // Debug.Log(tile.RemoveFromStressStates(resultGedreht));
                    continue;
                }
                
            }

            if(i==maxRange&&star==isStar) Debug.Log("maxed out"); // this tells us that the automatic stop when the magnitude gets too low didnt work and we loopedto the max i
            
        }
    }

    Dictionary<Vector3Int,Vector3> StressOutFriendsButOnlyMakeList( float strength, bool isStar, int minRange, int maxRange)
    {
        Dictionary<Vector3Int,Vector3> ausgabe = new Dictionary<Vector3Int, Vector3>();
        
        //Vector3 result = new Vector3 (5,5,5); // wird hier schon deklariert, damit es im forloop als exit condition benutzt werden kann
        bool doNextStep = true;
        
        
        for(int i = minRange; i<=maxRange; i++)
        {
            //wenn das tile nicht im aktiv gemacht wurde nach dem plazieren (weil es im ursprung lag) dann passiert nichts. redundant, weil die funktion nur aufgerufen wird wenn das tile aktiv geschaltet wird
            // if(!isActive) return;
            if(doNextStep==false) return ausgabe; // wenn das resultat der stressvektoren zu klein wird, setzt das den bool auf false, und es wird nichtmehr weitergemacht, selbst wenn die maxrange noch nicht erreicht ist
            // find the friends in range
            List<MapTile> currentFriends = this.GetFriendsByRange(i);
            // Debug.Log(i);
            // Utilities.PrintList(currentFriends);

            // give them all their stress by figuring out the vector between source and friend, and then if the stress should be parallel or perpendicular
            foreach(MapTile tile in currentFriends) 
            {
                Debug.Log("one iteration in the current range group");
                Vector3 connection = this.cords - tile.cords; // this has information about the direction but also already about the strength because the farther, the longer the vector
                float rangeMod = 1;
                if(star==isStar) // dies überprüft, ob der aufruf dem PartikelÄußeren zugehört. Weil dann wurde die funktion als StressoutFriends(,x star,y,z) aufgerufen. Ist das nicht der Fall, wurde (x,!star,y,z) aufgerufen
                {
                    rangeMod = 1/((connection.magnitude-c.olf.magnitude)*(connection.magnitude-c.olf.magnitude)); // entspricht hoffentlich dem 1/x^2 verlauf vom spannungsabfall? der olf dadrin ist der offset, damit erst am interface angefangen wird.
                }
                
                Vector3 result = rangeMod*connection*stressFieldStrength; // der resultierende vektor muss eventuell noch um 90° gefreht werden, falls das particle zu klein ist, also radiale druckspannung erzeugt
                Vector3 resultGedreht = new Vector3(result.x*c.t+result.y*c.a+result.z*c.b,result.x*c.b+result.y*c.t+result.z*c.a,result.x*c.a+result.y*c.b+result.z*c.t); 
                // wär wahrschienlich übersichtlicher gewesen, zu schauen wie man hier matrixmultiplikation macht und dann einfach die drehmatrix zu definieren. stattdessen steht hier schon ausgerechnet das ergebnis des vektors drin, die as und bs sind in c zu finden (lol) und sind 1/3 +- 1/sqrt3
                
                if(result.magnitude<0.5) doNextStep = false;
                
                if(isStar)
                { 
                    ausgabe.Add(tile.cords,result);
                    continue;
                }

                if(!isStar)
                {
                    ausgabe.Add(tile.cords,resultGedreht);
                    continue;
                }
                
            }

            if(i==maxRange&&star==isStar) Debug.Log("maxed out"); // this tells us that the automatic stop when the magnitude gets too low didnt work and we loopedto the max i
            
        }
    
        return ausgabe; // sollte idr nicht erreicht werden
    }
}
