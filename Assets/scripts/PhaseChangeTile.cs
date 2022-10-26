using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseChangeTile : MapTile
{
    float interfaceStrengh = 0.7f;
    bool isActive = false;
    int maxrange=2;
    float stressFieldStrength=1;
    int minrange = 1;
    bool star = true;
    public void MakeActive() { isActive = true;}
     private void Awake()
    {
        Initialize(this);
        this.SetBaseToughness(2);
        this.AddBehavior(c.grenzflaeche, interfaceStrengh);
        this.typeKey = c.particleTile1;
        this.cost = FundsAccount.instance.GetPriceByType(typeKey);
        this.SetBaseStressState(c.orf);
        if(isActive) StressOutFriends(maxrange,stressFieldStrength,minrange,star); // wenn das vom timing nicht passt, muss wohl der mapbuilder die stressout funktion aufrufen, sobald er alle sachen eingestellt hat
        //Debug.Log("bin eingeschrieben(awake)");
    }

    public void SetStar(bool star) { this.star = star;}
    public void SetStressFieldStrength(float strength) {stressFieldStrength = strength;}

    void StressOutFriends(int maxRange, float strength, int minRange, bool star)
    {
        for(int i = minRange; i<=maxRange; i++)
        {
            //wenn das tile nicht im aktiv gemacht wurde nach dem plazieren (weil es im ursprung lag) dann passiert nichts.
            if(!isActive) return;
            // find the friends in range
            List<MapTile> currentFriends = GetFriendsByRange(i);
            // give them all their stress by figuring out the vector between source and friend, and then if the stress should be parallel or perpendicular
            foreach(MapTile tile in currentFriends)
            {
                Vector3 connection = tile.cords - this.cords; // this has information about the direction but also already about the strength because the farther, the longer the vector
                float rangeMod = 1/(connection.magnitude*connection.magnitude); // entspricht hoffentlich dem 1/x^2 verlauf vom spannungsabfall?
                Vector3 result = rangeMod*connection; // der resultierende vektor muss eventuell noch um 90° gefreht werden, falls das particle zu klein ist, also radiale druckspannung erzeugt

                if(star)
                {
                    tile.AddToStressStates(result);
                    return;
                }
                // wär wahrschienlich übersichtlicher gewesen, zu schauen wie man hier matrixmultiplikation macht und dann einfach die drehmatrix zu definieren. stattdessen steht hier schon ausgerechnet das ergebnis des vektors drin, die as und bs sind in c zu finden (lol) und sind 1/3 +- 1/sqrt3
                Vector3 resultGedreht = new Vector3(result.x*c.t+result.y*c.a+result.z*c.b,result.x*c.b+result.y*c.t+result.z*c.a,result.x*c.a+result.y*c.b+result.z*c.t); 
                
                float differenz = resultGedreht.magnitude - result.magnitude;
                Debug.Log(differenz);

                tile.AddToStressStates(resultGedreht);
            }
            
        }
    }
}
