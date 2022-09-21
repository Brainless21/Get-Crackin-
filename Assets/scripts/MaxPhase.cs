using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxPhase : MapTile
{
    bool phaseChanged = false;
    float maxPhaseFieldStrengh = 2;
    private void Awake()
    {
        Initialize(this);
        this.typeKey = c.maxPhase;
        this.SetBaseToughness(5);
        this.cost = FundsAccount.instance.GetPriceByType(typeKey);
    }
    public override float GetToughness()
    {
        return base.GetToughness();
    }

    public override void Anmelden(int range)
    {
        if(range==1&&phaseChanged == false)
        {
        
            PhaseChange();
            
        }
    }
    void PhaseChange()
    {
        phaseChanged = true;
        this.ChangeRestingColor(0.4f);
        this.AddBehavior(c.maxPhaseField, maxPhaseFieldStrengh);
        // also ich glaub ja das sollte auch anders gehen aber ich machs jetzt ersmal so
        List<MapTile> currentFriends = GetFriendsByRange(1);
        foreach(MapTile tile in currentFriends)
        {
            tile.ChangeRestingColor(Color.gray);
            tile.ChangeRestingColor(0.4f);
        }
    }

    public override void Reset()
    {
        base.Reset();
        this.phaseChanged = false;
        this.RemoveBehavior(c.maxPhaseField);
    }
}
