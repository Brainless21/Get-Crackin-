using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    public event Action<Vector3> stressSetupNeedsToHappen;
    
    [SerializeField] c.CrackMode crackMode = c.CrackMode.Direction;
    public c.CrackMode GetCrackMode()
    {
        return crackMode;
    }

    [SerializeField] int mouseMode = c.placeTile;
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
    public Vector3 globalStress = new Vector3(2f,3f,-5f);
    // so basically instead of something happens-> that calls a function | we have the event manager as an intermediary.
    //Something happens-> that calls a function of EM-> EM function invokes that event-> that event calls the function |

    private void Awake() 
    {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        // DontDestroyOnLoad(this.gameObject);
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

    public void InvokeStressSetup(Vector3 stress)
    {
        //Debug.Log(stressSetupNeedsToHappen.ToString());
        stressSetupNeedsToHappen?.Invoke(stress);
        Debug.Log("stressSetup invoked");
    }
   
}
