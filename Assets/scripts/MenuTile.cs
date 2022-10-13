using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MenuTile : Entity, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] bool isType;
    [SerializeField] c.types type; 

    [SerializeField] bool isSizeMod;
    [SerializeField] int sizeMod;
    [SerializeField] int size;
    
    [SerializeField] bool isShape; 
    [SerializeField] int cost;
    public int GetCost() 
    {
        if(cost!=0) return cost; 
        return FundsAccount.instance.GetPriceByType((int)type);
    }
    [SerializeField] private int baseToughness;
    public int GetBaseToughness() { return baseToughness; }
    [SerializeField] string menuTileName;
    public string GetName() { return menuTileName; }
    [SerializeField] Mesh mesh;
    [SerializeField] float interfaceStrength;
    public float GetInterfaceStrengh() {return interfaceStrength;}


    int typeKey;
    int shapeTypeKey;

    [SerializeField] bool isEraser;

    [SerializeField] c.Shapes shape;


    private void Awake() 
    {
        cords = Utilities.ConvertCordsToInt(transform.position);
        mesh = this.gameObject.GetComponentInChildren<MeshFilter>().mesh;
    }

    public int GetShapeTypeKey()
    {
        return (int)shape;
    }

    public int GetTypeKey()
    {
        return (int)type;
    }

    public override void MouseInteractionLeft(Vector3Int cords)
    {
        Debug.Log("wir sind nach event data umgezogen");
        // if(cords!=this.cords) return;
        // Debug.Log("OG click also registered");
        // int mouseMode = EventManager.instance.GetMouseMode();

        // // ändert den placetile Type, wenn das Menutile ein type hat
        // if(isType==true)
        // {
        //     int typeKey = GetTypeKey();
        //     //EventManager.instance.SetMouseMode(-1,typeKey,-1); // grade ist hier noch der verweis auf sowohl den MouseMode als auch Mapbuilder. Am edne soll alles über den Mapbuilder laufen
        //     MapBuilder.instance.SetTileType(typeKey);
        //     EventManager.instance.SetMouseMode(c.placeTile);
        // }

        // // ändert den shapeType, wenn das Menutile eine Shape hat
        // if(isShape==true&&mouseMode==c.placeTile)
        // {
        //     int shapeType = GetShapeTypeKey();
        //     //EventManager.instance.SetMouseMode(-1,-1,shapeType);
        //     MapBuilder.instance.SetShape(shapeType);
        //     if(shapeType==c.fiber) { MapBuilder.instance.adjustShapeRotation(1); } // klicken auf die fiber dreht die fiber
            
        // }
        // if(true) //I feel like later it will become apparent that I need some kind of chekc here
        // {
        //     MapBuilder.instance.SetCost(cost);
        //     MapBuilder.instance.SetBaseToughness(baseToughness);
        //     MapBuilder.instance.SetShapeSize(size);
        //     Debug.Log("done boss");
        // }
        // if(isSizeMod==true&&mouseMode==c.placeTile)
        // {
        //     MapBuilder.instance.AdjustShapeSize(sizeMod);
        // }

        // if(isEraser==true)
        // {
        //     EventManager.instance.SetMouseMode(c.clearTile);
        // }


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click registered");
        int mouseMode = EventManager.instance.GetMouseMode();

        // ändert den placetile Type, wenn das Menutile ein type hat
        if(isType==true)
        {
            int typeKey = GetTypeKey();
            //EventManager.instance.SetMouseMode(-1,typeKey,-1); // grade ist hier noch der verweis auf sowohl den MouseMode als auch Mapbuilder. Am edne soll alles über den Mapbuilder laufen
            MapBuilder.instance.SetTileType(typeKey);
            EventManager.instance.SetMouseMode(c.placeTile);
        }

        // ändert den shapeType, wenn das Menutile eine Shape hat
        if(isShape==true&&mouseMode==c.placeTile)
        {
            int shapeType = GetShapeTypeKey();
            //EventManager.instance.SetMouseMode(-1,-1,shapeType);
            MapBuilder.instance.SetShape(shapeType);
            if(shapeType==c.fiber) { MapBuilder.instance.adjustShapeRotation(1); } // klicken auf die fiber dreht die fiber
            
        }

        if(true) //I feel like later it will become apparent that I need some kind of chekc here
        {
            MapBuilder.instance.SetCost(cost);
            MapBuilder.instance.SetBaseToughness(baseToughness);
            MapBuilder.instance.SetShapeSize(size);
            Debug.Log("my size is");
            Debug.Log(size);
            MapBuilder.instance.SetMesh(mesh);
            MapBuilder.instance.SetTileName(menuTileName);
            MapBuilder.instance.SetInterfaceStrenth(interfaceStrength);
            
            
        }

        if(isSizeMod==true&&mouseMode==c.placeTile)
        {
            MapBuilder.instance.AdjustShapeSize(sizeMod);
        }

        if(isEraser==true)
        {
            EventManager.instance.SetMouseMode(c.clearTile);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InfoDisplay.instance.UpdateInfoDisplay(this);
    }
}
