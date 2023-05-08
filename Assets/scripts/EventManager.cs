using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    //public static event Action<int> exampleEvent; //event action: a variable that contains all functions that will be called when the event is triggered
    //with the int, we can define different types of evokes. When we call the function that invokes the event, we will need to pass that int with it.
    // that int will then be passed on to the function(s) that are called by the event being invoked.
    public event Action<Vector3Int,List<Vector3Int>> MouseExit;
    public event Action<Vector3Int,List<Vector3Int>> MouseEnter;
    public event Action<Vector3Int> MouseClickRight;
    public event Action<Vector3Int> MouseClickLeft;
    public event Action buttonPressQ;

    public event Action etappenEnde; // happens when the crack finishes an etappe

    [SerializeField] bool isStressVisible = false;
    public void SetStressVisibility(bool isVisible)
    {
        isStressVisible = isVisible;
    }
    public bool GetStressVisibility => isStressVisible;
    
    [SerializeField] string levelName;
    [SerializeField] c.CrackMode crackMode = c.CrackMode.Direction;
    public c.CrackMode GetCrackMode()
    {
        return crackMode;
    }

    int mouseMode = c.inspect;
    public int GetMouseMode() { return mouseMode; }
    
    public void SetMouseMode(int x)
    {
      mouseMode = x;
    }
    public bool IsMouseModeEqualTo(int mode)
    {
        if(mode!=mouseMode) return false;
        return true;
    }
    [SerializeField] private Vector3 globalStress;

    public void SetGlobalStress(Vector3Int newGlobalStress)
    {
        Debug.Log(string.Format("mein stress vector wurde zu {0} geändert",newGlobalStress));
        globalStress = newGlobalStress;
    }

    public void SetGlobalStress(c.richtungsvektoren newGlobalStress)
    {
        globalStress = Utilities.getDirectionVectorByEnum(newGlobalStress);
        TileLedger.ledgerInstance.UpdateAllStresses();
    }
    public Vector3 GetGlobalStress()
    {
        // geht sicher, dass der global stress vektor immer betraglich 1 ist
        Vector3 result = globalStress/globalStress.magnitude;
        if(globalStress.magnitude==0) return new Vector3 (0f,0f,0f); //falls der stress 0 ist wird auch nullvektor returned, das /0 macht sonst NaN bs
        //Debug.Log(string.Format("I was asked for the stress and this is what I said:{0}",result));
        return result;
    }
    // so basically instead of something happens-> that calls a function | we have the event manager as an intermediary.
    //Something happens-> that calls a function of EM-> EM function invokes that event-> that event calls the function |

    private void Awake() 
    {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        // DontDestroyOnLoad(this.gameObject);

        levelName = SceneManager.GetActiveScene().name;
    }
    public string GetLevelName()
    {
        return levelName;
    }
  
    
    public void InvokeMouseExit(Vector3Int cords, List<Vector3Int> shape)
    {
        if(MouseExit!=null) MouseExit(cords, shape);
       // MouseExit?.Invoke(cords, shape); <- thats the same as above
    }
    public void InvokeMouseEnter(Vector3Int cords, List<Vector3Int> shape)
    {
        MouseEnter?.Invoke(cords, shape);
    }
    public void InvokeRightMouseClick(Vector3Int cords)
    {
        MouseClickRight?.Invoke(cords);
    }
    public void InvokeLeftMouseClick(Vector3Int Cords)
    {
        MouseClickLeft?.Invoke(Cords);
    }

    public void InvokeButtonPressQ()
    {
        buttonPressQ?.Invoke();
    }

    public void InvokeEtappenEnde()
    {
        etappenEnde?.Invoke();
    }
   
}
