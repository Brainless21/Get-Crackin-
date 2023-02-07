using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField] TextMeshPro infoDisplay;
    private void Awake() 
    {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }
    }
    public static InfoDisplay instance;
    public void UpdateInfoDisplay(MapTile mapTile)
    {   
       string displaytext="";
       //displaytext += "name: ";
       displaytext += mapTile.GetTileName();
       displaytext += "\ntoughness: ";
       displaytext += mapTile.GetBaseToughness().ToString();
       if(mapTile.typeKey==c.particleTile1) // i think ill just give mapTile a field for interface strength
       {
       ParticleTile1 handle = mapTile.gameObject.GetComponent<ParticleTile1>();
       displaytext += "\nInterface: ";
       displaytext += handle.GetInterfaceStrengh().ToString(); // sooo sometimes they dont have an interface strengh. like when they are a matrix tile. mostly then honestly.
       }
       if(mapTile.typeKey==c.PhaseChangeTile) // i think ill just give mapTile a field for interface strength
       {
       PhaseChangeTile handle = mapTile.gameObject.GetComponent<PhaseChangeTile>();
       displaytext += "\nInterface: ";
       displaytext += handle.GetInterfaceStrengh().ToString(); // sooo sometimes they dont have an interface strengh. like when they are a matrix tile. mostly then honestly.
       }
       displaytext += "\ncost: ";
       displaytext += FundsAccount.instance.GetPriceByType(mapTile.typeKey).ToString();
       infoDisplay.SetText(displaytext);

    }

    public void UpdateInfoDisplay(MenuTile menuTile)
    {   
       string displaytext="";
       //displaytext += "Name: ";
       displaytext += menuTile.GetName();
       displaytext += "\nToughness: ";
       displaytext += menuTile.GetBaseToughness().ToString();
       displaytext += "\nInterface: ";
       displaytext += menuTile.GetInterfaceStrengh().ToString();
       displaytext += "\nCost: ";
       displaytext += menuTile.GetCost().ToString();
       infoDisplay.SetText(displaytext);

    }

}
