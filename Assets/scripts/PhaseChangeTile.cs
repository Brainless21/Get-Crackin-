using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseChangeTile : MapTile
{
    float interfaceStrengh = 0.7f;
     private void Awake()
    {
        Initialize(this);
        this.SetBaseToughness(2);
        this.AddBehavior(c.grenzflaeche, interfaceStrengh);
        this.typeKey = c.particleTile1;
        this.cost = FundsAccount.instance.GetPriceByType(typeKey);
        this.SetBaseStressState(c.orf);
        //Debug.Log("bin eingeschrieben(awake)");
    }
}
