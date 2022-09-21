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
        //Debug.Log("bin eingeschrieben(start)");
    }

    public override float GetToughness()
    {
        return baseToughness;
    }
   
}
