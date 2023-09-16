using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseChangeTile : MapTile
{
    float interfaceStrengh = 0.7f;
    [SerializeField] bool isActive = false;
    [SerializeField] bool isCenter = false;
    [SerializeField] public bool isMaxPhase = false;
    [SerializeField] float stressFieldStrength=1;
    [SerializeField] bool star = true;
    public bool GetStar() => star;
    private Dictionary<Vector3Int, Vector3> stressFieldOutside;
    private Dictionary<Vector3Int, Vector3> stressFieldInside;
    public void SetStar(bool star) { this.star = star;}
    public void SetStressFieldStrength(float strength) {stressFieldStrength = strength;}
    public float GetStressFieldStrength() => stressFieldStrength;
    internal int minRange=2; // wird beim placement overridden durch blob size
    public int GetMinrange() => minRange;
    public void SetMinRange(int blobSize)
    {
        minRange = blobSize;
    }    

    public void MakeActive() 
    {
        isActive = true;
        stressFieldOutside = StressOutFriendsButOnlyMakeList(stressFieldStrength, star, minRange, 15);
        stressFieldInside = StressOutFriendsButOnlyMakeList(stressFieldStrength, !star, 1, 1);
    
        TileLedger.ledgerInstance.AddStressField(stressFieldOutside);
        TileLedger.ledgerInstance.AddStressField(stressFieldInside);
        if(!isMaxPhase) return; // wenn wir nicht bei crack erst aktiv werden, sind wir heir fertig
        // hier kommt der code für die farbänderung rein:
        List<MapTile> particle = TileLedger.ledgerInstance.GetTileByCords(associatedShape);
        foreach(MapTile tile in particle)
        {
            //tile.gameObject.GetComponent<MeshRenderer>().material.color *= 0.5f;
            // because I dont undersand my rendering function anymore and i dont want to, well just crack these mfs because that darkens them as well, and the crack shouldnt go thru them so no one would ever know
           // tile.Crack(); // that looks ugly tho
           tile.BrightenUp();
        }
        //delayedStress = StartCoroutine(WaitFrameAndStress());
    }

    public void MakeInactive()
    {
        if(!isActive) return;
        isActive = false;
        // Utilities.PrintDictionary(stressFieldInside);
        // Utilities.PrintDictionary(stressFieldOutside);

        TileLedger.ledgerInstance.RemoveStressField(stressFieldInside);
        TileLedger.ledgerInstance.RemoveStressField(stressFieldOutside);

        if(!isMaxPhase) return; // wenn wir nicht bei crack erst aktiv werden, sind wir heir fertig
        // hier kommt der code für die farbänderung rein:
        List<MapTile> particle = TileLedger.ledgerInstance.GetTileByCords(associatedShape);
        foreach(MapTile tile in particle)
        {
            //tile.gameObject.GetComponent<MeshRenderer>().material.color *= 0.5f;
            // because I dont undersand my rendering function anymore and i dont want to, well just crack these mfs because that darkens them as well, and the crack shouldnt go thru them so no one would ever know
           // tile.Crack(); // that looks ugly tho
           tile.BrightenDown();
        }
    }

    public override void Reset()
    {
        Debug.Log("base reset about to be called");
        base.Reset();
        if(!isMaxPhase) return;
        Debug.Log("makeInactive aout ot be called");
        MakeInactive();
    }
    private void Awake()
    {
        Initialize(this);
        this.SetBaseToughness(2);
        this.AddBehavior(c.grenzflaeche, interfaceStrengh);
        this.typeKey = c.particleTile1;
        //this.cost = FundsAccount.instance.GetPriceByType(typeKey);
        this.UpdateStressState();//war mal TileLedger.ledgerInstance.SetGlobalStress(this.cords,EventManager.instance.GetGlobalStress()); //war mal this.SetBaseStressState(EventManager.instance.GetGlobalStress());
        //Debug.Log("bin eingeschrieben(awake)");
        EventManager.instance.etappenEnde+=Reset;
    }

    // private void Start()
    // {
    // }

    private void OnDisable() 
    {
        if(isActive)
        {
        Debug.Log("imma remove some stress");
        //StressOutFriendsButOnlyMakeList(stressFieldStrength, star, minRange, 15); //removed die stresstates, die es vorher platziert hat. klappt nur wenn sich zwischendurch die werte für fieldsStrength, star und minRange nicht ändern!
        TileLedger.ledgerInstance.RemoveStressField(stressFieldOutside);
        TileLedger.ledgerInstance.RemoveStressField(stressFieldInside);
        }
        EventManager.instance.etappenEnde-=Reset;
    }

    public override void SetAssociatedShape(List<Vector3Int> shape)
    {
        base.SetAssociatedShape(shape);

        if(!(this.cords==this.associatedShape[0])) return; //wenn das tile nicht die mitte (erster eintrag in Shape Liste) ist, dann sind wir hier fertig
        
        isCenter = true; // falls doch, sind wird die mitte

        if(!isMaxPhase) MakeActive(); // wenn wir keine MaxPhase sind, können wir uns einschalten. ansonsten müssen wir auf anmeldung warten.
        
    }

    public override void Anmelden(int range)
    {
        if(range==4 && isActive == false && isCenter==true && isMaxPhase==true)
        {
        
            MakeActive();
            
        }
    }

    public float GetInterfaceStrengh()
    {
        return interfaceStrengh;
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
            List<Vector3Int> currentFriends = this.GetFriendsCordsByRange(i);
            // Debug.Log(i);
            // Utilities.PrintList(currentFriends);

            // give them all their stress by figuring out the vector between source and friend, and then if the stress should be parallel or perpendicular
            foreach(Vector3Int tile in currentFriends) 
            {
                Vector3 connection = this.cords - tile; // this has information about the direction but also already about the strength because the farther, the longer the vector
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
                    ausgabe.Add(tile,result);
                    continue;
                }

                if(!isStar)
                {
                    ausgabe.Add(tile,resultGedreht);
                    continue;
                }
                
            }

            if(i==maxRange&&star==isStar) Debug.Log("maxed out"); // this tells us that the automatic stop when the magnitude gets too low didnt work and we loopedto the max i
            
        }
    
        return ausgabe; // sollte idr nicht erreicht werden
    }
}
