using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


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
       // displaytext += "Map: ";
       displaytext += mapTile.GetTileName();
       float toughness = mapTile.GetBaseToughness();
       if(toughness!=0)
       {
       displaytext += "\nToughness: ";
       displaytext += toughness.ToString();
       }
       if(mapTile.typeKey==c.particleTile1) // i think ill just give mapTile a field for interface strength
       {
       ParticleTile1 handle = mapTile.gameObject.GetComponent<ParticleTile1>();
       displaytext += "\nInterface: ";
       float interfacePercent = handle.GetInterfaceStrengh()*100;
       displaytext += interfacePercent.ToString(); // sooo sometimes they dont have an interface strengh. like when they are a matrix tile. mostly then honestly.
       displaytext += "%";
       }
       if(mapTile.typeKey==c.PhaseChangeTile) // i think ill just give mapTile a field for interface strength
       {
       PhaseChangeTile handle = mapTile.gameObject.GetComponent<PhaseChangeTile>();
       displaytext += "\nInterface: ";
       float interfacePercent = handle.GetInterfaceStrengh()*100;
       displaytext += interfacePercent.ToString(); // sooo sometimes they dont have an interface strengh. like when they are a matrix tile. mostly then honestly.
       displaytext += "%";
       }
       displaytext += "\ncost: ";
       displaytext += FundsAccount.instance.GetPriceByType(mapTile.typeKey).ToString();
       infoDisplay.SetText(displaytext);

    }

    public void UpdateInfoDisplay(MenuTile menuTile)
    {   
       string displaytext="";
       // displaytext += "menu: ";
       displaytext += menuTile.GetName();
       displaytext += "\nToughness: ";
       displaytext += menuTile.GetBaseToughness().ToString();
       displaytext += "\nInterface: ";
       float interfacePercent = menuTile.GetInterfaceStrengh() * 100;
       displaytext += interfacePercent.ToString();
       displaytext += "%";
       displaytext += "\nCost: ";
       displaytext += menuTile.GetCost().ToString();
       infoDisplay.SetText(displaytext);

    }

    public void UpdateInfoDisplay(List<MenuTile> list)
    {
        if(list.Contains(null)) return; // I think that didnt work? and instead the function providing the list just never gives out lists with null entries
        if(list.Count==0) return;
        if(list.Count==1) // do the same thing as the singleton menu tile version but without displaying the cost pls.
        {
            MenuTile menuTile = list[0];
            string displaytext1="";
            // displaytext += "menu: ";
            displaytext1 += menuTile.GetName();
            displaytext1 += "\nToughness: ";
            displaytext1 += menuTile.GetBaseToughness().ToString();
            displaytext1 += "\nInterface: ";
            float interfacePercent = menuTile.GetInterfaceStrengh() * 100;
            displaytext1 += interfacePercent.ToString();
            displaytext1 += "%%";
            infoDisplay.SetText(displaytext1);
        }

        List<float> differentInterFaces = new List<float>();
        List<float> diffentToughnesses = new List<float>();
        List<string> differentNames = new List<string>();

        foreach(MenuTile menuTile in list)
        {
            differentInterFaces.Add(menuTile.GetInterfaceStrengh());
            diffentToughnesses.Add(menuTile.GetBaseToughness());
            differentNames.Add(menuTile.GetName());
        }

        string displaytext="";
        // displaytext += "menu: ";
        displaytext += differentNames[0];
        displaytext += "\nToughness: ";
        displaytext += GenerateValueString(diffentToughnesses);
        displaytext += "\nInterface: ";
        displaytext += GenerateValueStringPercent(differentInterFaces);
        infoDisplay.SetText(displaytext);

        

    }

    string GenerateValueString(List<float> list)
    {
        string output= "";
        if(list.ToArray().Min()==list.ToArray().Max())
        {
            output += list.ToArray().Max().ToString();
            return output;
        }

        output += list.ToArray().Min().ToString();
        output += " - ";
        output += list.ToArray().Max().ToString();

        return output;



    }

    string GenerateValueStringPercent(List<float> list)
    {
        string output = "";
        List<float> listPercent = new List<float>();
        foreach(float entry in list)
        {
            listPercent.Add(entry * 100);
        }
        if(list.ToArray().Min()==list.ToArray().Max())
        {
            output += listPercent.ToArray().Max().ToString();
            output += "%";
            return output;
        }

        output += listPercent.ToArray().Min().ToString();
        output += " - ";
        output += listPercent.ToArray().Max().ToString();
        output += "%";

        return output;


    }

}
