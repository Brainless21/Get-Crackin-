using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Tutorial : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private int order;
    [SerializeField] private c.tutorialEvents triggeringEvent;
    // private static Vector3 upward = new Vector3(1f,-2f,1f);
    //private Color fadeColor;
    private float fadeSpeed = 3;

    private float fadeDelay = 0.5f;

    private float stepsize = 2f;

    void Update()
    {
        
    }


}
