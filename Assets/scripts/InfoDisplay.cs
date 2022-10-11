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
       displaytext += "name: ";
       displaytext += mapTile.tileName;
       displaytext += "\n toughness: ";
       displaytext += mapTile.GetBaseToughness().ToString();
       displaytext += "\n cost: ";
       displaytext += FundsAccount.instance.GetPriceByType(mapTile.typeKey).ToString();
       infoDisplay.SetText(displaytext);

    }

    public void UpdateInfoDisplay(MenuTile menuTile)
    {   
       string displaytext="";
       displaytext += "name: ";
       displaytext += menuTile.GetName();
       displaytext += "\n toughness: ";
       displaytext += menuTile.GetBaseToughness().ToString();
       displaytext += "\n cost: ";
       displaytext += menuTile.GetCost().ToString();
       infoDisplay.SetText(displaytext);

    }

}
