using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    Vector3 mousePosition = new Vector3();
    private Vector2Int mouseMode = new Vector2Int (1,1);
    private Entity pointedEntity = null;
    public static MouseCursor instance;

     void Awake() 
    {
        // MAKE cursorINSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        DontDestroyOnLoad(this.gameObject);
    
    }
    void Update()
    {
        mousePosition = Input.mousePosition;
        this.transform.position = mousePosition;

        Point(); //zeigt mit der maus in die gegend und schaut, worauf grade gezeigt wird, speichert die information in pointedEntity ab.

        if(pointedEntity!=null)
        {
            if(Input.GetMouseButtonDown(c.leftClick)) { EventManager.instance.InvokeLeftMouseClick(pointedEntity.cords); }
            if(Input.GetMouseButtonDown(c.rightClick)) { EventManager.instance.InvokeRightMouseClick(pointedEntity.cords);}
        }

        if(mouseMode[c.modeType]==c.placeTile) //das soll eigentlich woanders passieren, ist nur übergangslösung
        {
            if(Input.GetKeyDown(KeyCode.Q)) 
            {
                EventManager.instance.SetMouseMode(c.inspect); 
                Debug.Log("now in inspect mode");
                return;
            }
            if(Input.GetKeyDown(KeyCode.W)) { mouseMode[c.modeBranch]=c.matrixTile; }
            if(Input.GetKeyDown(KeyCode.E)) { mouseMode[c.modeBranch]=c.particleTile1; }
        }

        if(mouseMode[c.modeType]==c.inspect)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                mouseMode[c.modeType]=c.placeTile;
                Debug.Log("now in placeTile Mode");
            }
        }
    }



    private Entity Point()
    {
        Ray mouseRay = mainCamera.ScreenPointToRay(mousePosition);      //aim the ray

        if (!Physics.Raycast(mouseRay, out RaycastHit raycastHit))         //shoot the ray. if it misses, pointedentity is null, and the current entities mouseexit function is called if it had been pointed at something before
        {
            if(pointedEntity!=null){ EventManager.instance.InvokeMouseExit(pointedEntity.cords, MapBuilder.instance.DrawShape(pointedEntity.cords)); } 
            pointedEntity = null;
            return pointedEntity;
        }

        if(pointedEntity!=null)
        {
            if(pointedEntity.gameObject==raycastHit.collider.gameObject)//if the ray hits, and the target is the same as before, nothing needs to happen
            {
                return pointedEntity;
            }
        }

        if(pointedEntity!=null){EventManager.instance.InvokeMouseExit(pointedEntity.cords, MapBuilder.instance.DrawShape(pointedEntity.cords));} //if its not hte same target as before, the previous one got exited, so the exit func is called

        MatrixTile matrixTile = raycastHit.collider.gameObject.GetComponent<MatrixTile>();
        if(matrixTile!=null)
        {
            pointedEntity = matrixTile;
            EventManager.instance.InvokeMouseEnter(pointedEntity.cords, MapBuilder.instance.DrawShape(pointedEntity.cords)); //es wird einmal die location der pointed entity weitergegeben, und die grade angesagte shape.
            return pointedEntity;
        }

    	ParticleTile1 particleTile1 = raycastHit.collider.gameObject.GetComponent<ParticleTile1>();
        if(particleTile1!=null)
        {
            pointedEntity = particleTile1;
            EventManager.instance.InvokeMouseEnter(pointedEntity.cords, MapBuilder.instance.DrawShape(pointedEntity.cords));
            return pointedEntity;
        }

        MaxPhase maxPhase = raycastHit.collider.gameObject.GetComponent<MaxPhase>();
        if(maxPhase!=null)
        {
            pointedEntity = maxPhase;
            EventManager.instance.InvokeMouseEnter(pointedEntity.cords, MapBuilder.instance.DrawShape(pointedEntity.cords));
            return pointedEntity;
        }

        MenuTile menuTile = raycastHit.collider.gameObject.GetComponent<MenuTile>();
        if(menuTile!=null)
        {
            pointedEntity = menuTile;
            EventManager.instance.InvokeMouseEnter(pointedEntity.cords, MapBuilder.instance.DrawShape(pointedEntity.cords));
            return pointedEntity;
        }

        TabButton tabButton = raycastHit.collider.gameObject.GetComponent<TabButton>();
        if(tabButton!=null)
        {
            pointedEntity = tabButton;
            EventManager.instance.InvokeMouseEnter(pointedEntity.cords, MapBuilder.instance.DrawShape(pointedEntity.cords));
            return pointedEntity;
        }

        pointedEntity = null;
        return pointedEntity;

    }

    public void ChangeMouseModeBranch(int newBranch)
    {
        mouseMode[c.modeBranch] = newBranch;
    }

    public void ChangeMouseModeType(int newType)
    {
        mouseMode[c.modeType] = newType;
    }


}
