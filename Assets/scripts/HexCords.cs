using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexCords : MonoBehaviour
{
    public float Q{get;set;}
    public float R{get;set;}
    public float S
    {
        set{S=value;}  
        get{return -Q-R;}
    }
    
    public HexCords (float q,float r)
    {
        
    }
    

    public double GetDistance(HexCords a, HexCords b)
    {
        double distance = Math.Sqrt((a.Q-b.Q)*(a.Q-b.Q)+(a.R-b.R)*(a.R-b.R)+(a.S-b.S)*(a.S-b.S));
        return distance;
    }

}
