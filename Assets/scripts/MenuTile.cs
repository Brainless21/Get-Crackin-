using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MenuTile : Entity, IPointerClickHandler, IPointerEnterHandler
{   
    [SerializeField] bool isTileSelect=true;
    [SerializeField] bool isType;
    [SerializeField] c.types type; 

    [SerializeField] bool isSizeMod;
    [SerializeField] int sizeMod;
    [SerializeField] int size;
    
    [SerializeField] bool isShape; 
    [SerializeField] int costOfShape;
    [SerializeField] int costOfTile; // wenn die einzelnen Tiles abweichende kosten haben sollen, man aber die neuen kosten nicht selber ausrechnen will weil faul
    public int GetCost() 
    {
        // wenn im menu tile selber was eingetragen wurde, sind das die gesamt Kosten
        if(costOfShape!=0) return costOfShape; 
        
        // wenn nichts drinsteht, werden die kosten nach tileanzahl und Cost of one Tile berechnet. Steht bei den Cost of Tile nichts drin, wird der standard wert nach type zurückgegeben
        Debug.Log(Utilities.CalculateCostOfShape((int)shape, size, GetCostOfTile()));
        return Utilities.CalculateCostOfShape((int)shape, size, GetCostOfTile());
    }

    int GetCostOfTile()
    {
        if(costOfTile==0) return FundsAccount.instance.GetPriceByType((int)type);
        return costOfTile;
    }

    [SerializeField] private int baseToughness;
    public int GetBaseToughness() { return baseToughness; }
    [SerializeField] string menuTileName;
    public string GetName() { return menuTileName; }
    [SerializeField] Mesh mesh;
    [SerializeField] float interfaceStrength =1;
    public float GetInterfaceStrengh() {return interfaceStrength;}
    [SerializeField] float stressFieldStrength;
    public float GetStressFieldStrength() {return stressFieldStrength;}
    [SerializeField] bool star;
    public bool IsStar() {return star;}


    int typeKey;
    int shapeTypeKey;

    [SerializeField] bool isEraser;
    [SerializeField] bool isInspector;

    [SerializeField] c.Shapes shape;

    //[SerializeField] MapTile exampleTile; // die idee alle relevanten infos in einem maptile zusammenzufassen und nurdas zuübergeben wird erstmal auf ice gelegt, weil mir keine schlaue lösung einfällt, wie man die verschiedenen datentypen mit particle oder maxphase tile handlen würde ohne das es irgendwie whack werden würd

    private void Awake() 
    {
        cords = Utilities.ConvertCordsToInt(transform.position);

        if(isEraser==false&&isInspector==false) mesh = this.gameObject.GetComponentInChildren<MeshFilter>().mesh;

       
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
        //Debug.Log("wir sind nach event data umgezogen");
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

        if(isTileSelect==true) //I feel like later it will become apparent that I need some kind of chekc here
        {
            MapBuilder.instance.SetCost(GetCostOfTile());
            MapBuilder.instance.SetBaseToughness(baseToughness);
            MapBuilder.instance.SetShapeSize(size);
            MapBuilder.instance.SetMesh(mesh);
            MapBuilder.instance.SetTileName(menuTileName);
            MapBuilder.instance.SetInterfaceStrenth(interfaceStrength);
            MapBuilder.instance.SetStar(star);
            MapBuilder.instance.SetStressFieldStrength(stressFieldStrength);
            
            
        }

        if(isSizeMod==true&&mouseMode==c.placeTile)
        {
            MapBuilder.instance.AdjustShapeSize(sizeMod);
        }

        if(isEraser==true)
        {
            EventManager.instance.SetMouseMode(c.clearTile);
        }

        if(isInspector==true)
        {
            EventManager.instance.SetMouseMode(c.inspect);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InfoDisplay.instance.UpdateInfoDisplay(this);
    }
}
