using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuTile : Entity
{
    [SerializeField] bool isType;
    [SerializeField] string type; 

    [SerializeField] bool isSize;
    [SerializeField] int sizeMod;
    
    [SerializeField] bool isShape;
    [SerializeField] string shapeType; 

    [SerializeField] bool isEraser;

    int typeKey;
    int shapeTypeKey;

    private void Awake() 
    {
        cords = Utilities.ConvertCordsToInt(transform.position);
    }

    public int GetShapeTypeKey()
    {
        shapeTypeKey = c.GetShapeByString(shapeType);
        return shapeTypeKey;
    }

    public int GetTypeKey()
    {
        typeKey = c.getTypeByString(type);
        return typeKey;
    }

    public override void MouseInteractionLeft(Vector3Int cords)
    {
        if(cords!=this.cords) return;
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
            MapBuilder.instance.shape = shapeType;
            if(shapeType==c.fiber) { MapBuilder.instance.adjustShapeRotation(1); } // klicken auf die fiber dreht die fiber
            
        }

        if(isSize==true&&mouseMode==c.placeTile)
        {
            MapBuilder.instance.AdjustShapeSize(sizeMod);
        }

        if(isEraser==true)
        {
            EventManager.instance.SetMouseMode(c.clearTile);
        }


    }

}
