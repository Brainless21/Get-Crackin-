using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour
{
    private Dictionary<int, float> behaviourTags = new Dictionary<int, float>();

    public TileBehaviour(int key = 0, float str=0)
    {
        AddBehavior(key, str);
    }

    public bool HasBehavior(int key)
    {
        if(behaviourTags.ContainsKey(key)) return true;
        return false;

    }

    public float GetBehaviourByType(int key)
    {
        if(!behaviourTags.ContainsKey(key))
        {
            Debug.Log("Verhalten nicht definiert");
            return 0;
        }

        return behaviourTags[key];
    }

    public bool AddBehavior(int key, float strength)
    {
        if(behaviourTags.ContainsKey(key))
        {
            Debug.Log("tag existiert schon");
            return false;
        }

        behaviourTags.Add(key,strength);
        return true;
    }
}
