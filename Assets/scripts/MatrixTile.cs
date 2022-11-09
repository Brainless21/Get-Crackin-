using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixTile : MapTile
{
   
    private void Awake()
    {
        Initialize(this);
        this.typeKey = c.matrixTile;
        this.SetBaseToughness(1);
        this.cost = FundsAccount.instance.GetPriceByType(typeKey);
        
        TileLedger.ledgerInstance.SetGlobalStress(this.cords,EventManager.instance.GetGlobalStress()); // war früher this.SetBaseStressState(EventManager.instance.GetGlobalStress()); bevor die stresses zum ledger umgezogen sind
       
    }
  

   

    public override float GetToughness()
    {
        
        float modifier = 1;
        //float addition = 0;
        List<int> appliedBehaviours = new List<int>();
        List<MapTile> friendsRange1 = this.GetFriendsByRange(1);

        foreach(MapTile inspectedTile in friendsRange1)
        {
            if(inspectedTile!=null)
            {
                //now check for interface behavior (behaviour#1)
                if(inspectedTile.HasBehavior(c.grenzflaeche) & !appliedBehaviours.Contains(c.grenzflaeche))
                {
                    modifier*=inspectedTile.GetBehaviourByTag(c.grenzflaeche);
                    appliedBehaviours.Add(c.grenzflaeche);
                }

                // now check for MaxPhase Behavior
                if(inspectedTile.HasBehavior(c.maxPhaseField) & !appliedBehaviours.Contains(c.maxPhaseField))
                {
                    modifier*=inspectedTile.GetBehaviourByTag(c.maxPhaseField);
                    appliedBehaviours.Add(c.maxPhaseField);
                    
                }
            }
        }
        return baseToughness*modifier;
    }
}
