using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGroup : MonoBehaviour
{
    [SerializeField] List<GameObject> disabledOnStart;
    [SerializeField] List<GameObject> enabledOnStart;

    public void Subscribe(GameObject thing)
    {
        if(disabledOnStart == null)
        {
            disabledOnStart = new List<GameObject>();
        }
        if(enabledOnStart == null)
        {
            enabledOnStart = new List<GameObject>();
        }

        if(thing.activeSelf) enabledOnStart.Add(thing);
        if(!thing.activeSelf) disabledOnStart.Add(thing);

    }
    public void Reset() {
        {
            foreach(GameObject virgin in disabledOnStart)
            {
                virgin.SetActive(false);
            }

            foreach(GameObject chad in enabledOnStart)
            {
                chad.SetActive(true);
            }

        }
    }
}
