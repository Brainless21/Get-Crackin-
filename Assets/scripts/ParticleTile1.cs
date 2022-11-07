using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleTile1 : MapTile
{
    float interfaceStrengh = 0.7f;
   
    private void Awake()
    {
        Initialize(this);
        this.SetBaseToughness(2);
        this.AddBehavior(c.grenzflaeche, interfaceStrengh);
        this.typeKey = c.particleTile1;
        this.cost = FundsAccount.instance.GetPriceByType(typeKey);
        // this.SetBaseStressState(EventManager.instance.GetGlobalStress());
        //TileLedger.ledgerInstance.SetGlobalStress(this.cords, EventManager.instance.GetGlobalStress());
        //Debug.Log("bin eingeschrieben(awake)");
    }
    

    public float GetInterfaceStrengh()
    {
        return interfaceStrengh;
    }

    public override float GetToughness()
    {
        return baseToughness;
    }
   
}
