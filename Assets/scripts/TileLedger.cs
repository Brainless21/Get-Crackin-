using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLedger : MonoBehaviour
{
    public static TileLedger ledgerInstance;

    [SerializeField] private Dictionary<Vector3Int,MapTile> tilesDict = new Dictionary<Vector3Int, MapTile>();
    [SerializeField] private Dictionary<Vector3Int,List<Vector3>> stressDict = new Dictionary<Vector3Int, List<Vector3>>();

    public Dictionary<Vector3Int,MapTile> PassTilesDict()
    {
        return tilesDict;
    }

    void Awake() 
    {
        // MAKE ledgerINSTANCE
        if      (ledgerInstance == null) { ledgerInstance = this; }
        else if (ledgerInstance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        //DontDestroyOnLoad(this.gameObject);
    
    }

    private bool IsVectorAtPosition(Vector3Int position, Vector3 gesuchterStressVektor)
    {
        if(!stressDict.ContainsKey(position)) return false; // wenn an der stelle noch kein eintrag besteht, ist da auch kein vektor drin
        
        float delta;
        foreach(Vector3 stressAtPosition in stressDict[position]) // jeder an position anzutreffender vektor wird mit dem gesuchten abgeglichen, wenn die differenz klein genug ist, wird true zurückgegeben, wenn alle durch sind wird flase durchgegeben
        {
            delta = (stressAtPosition - gesuchterStressVektor).magnitude;
            if(delta<0.001) return true;
        }
        return false;
    }
    
    public void RemoveStressFromPosition(Vector3Int position, Vector3 obsoleteStressVector)
    {
        if(!stressDict.ContainsKey(position))
        {
            Debug.Log("hier wurde versucht ein stress zu removen, wo gar nichts ist. weird.");
            return;
        }

        float delta;
        foreach(Vector3 stressAtPosition in stressDict[position]) // jeder an position anzutreffender vektor wird mit dem gesuchten abgeglichen, wenn die differenz klein genug ist, wird true zurückgegeben, wenn alle durch sind wird flase durchgegeben
        {
            delta = (stressAtPosition - obsoleteStressVector).magnitude;
            if(delta<0.001)
            {
                stressDict[position].Remove(stressAtPosition); // wenn der grade überprüfte vektor der Liste auf den gesuchten passt, wird er aus der Liste entfernt
            }
        }

    }

    public void SetGlobalStress(Vector3Int position, Vector3 globalStressState)  // this function will only add the vector if its not at the position already
    {
       if(!IsVectorAtPosition(position, globalStressState))
       {
           AddToStressStates(position, globalStressState);
           return;
       }

       Debug.Log(string.Format("hier ist schon ein base stress eingetragen:{0}",position));
       GetTileByCords(position).UpdateStressState();
    }
    public void AddToStressStates(Vector3Int position, Vector3 stressState)
    {
        // wenn an der position noch nichts vermerkt ist, wird ein eintrag erstellt mit der position und einer leeren Liste
       if(!stressDict.ContainsKey(position))
       {
           stressDict.Add(position, new List<Vector3>());
       }
       
        stressDict[position].Add(stressState); // zu der Liste an stelle position wird der Stress state geadded
        GetTileByCords(position).UpdateStressState(); // dem tile an der stelle, wo sich der stress verändert hat, wird gesagt seinen state zu updaten
    }

    public List<Vector3> GetStressStatesAtPosition(Vector3Int position)
    {
        if(!stressDict.ContainsKey(position))
        {
            Debug.Log("hier sind keine StressStates eingetragen. du bekommst eine ganz frische, leere liste");
            return new List<Vector3>();
        }

        return stressDict[position];
    }
 
    public bool AddTileToLedger(MapTile tile)
    {
        if(!tile.HasCords()) 
        {
            Debug.Log("Tile hat noch keine koordinaten");
            return false;
        }

        
        if(tilesDict.ContainsKey(tile.cords))
        {
           // Debug.Log("Eintrag exisitiert bereits im dict");
            return false;
        }

        tilesDict.Add(tile.cords,tile);
        return true;
        
    } 

    public bool RemoveTileFromLedger(MapTile tile)
    {
        if(!tilesDict.ContainsKey(tile.cords))
        {
            Debug.Log("das zu entfernende tile steht nicht im ledger");
            return false;
        }

        tilesDict.Remove(tile.cords);
        return true;
    }

    public MapTile GetTileByCords(Vector3Int cords)
    {
        if(!tilesDict.ContainsKey(cords))
        {
        
           // Debug.Log("hier ist kein tile");
           // Debug.Log(cords);
            return null;
        }
        return tilesDict[cords];
    }

    public List<MapTile> GetTileByCords(List<Vector3Int> cordsList)
    {
        List<MapTile> tilesList = new List<MapTile>();

        foreach(Vector3Int cord in cordsList)
        {

            if(tilesDict.ContainsKey(cord))
            {
                tilesList.Add(tilesDict[cord]);
            }
        }

        return tilesList;
    }

    public bool CanTheyAllBeOverridden(List<Vector3Int> TileSpotList)
    {
        List<MapTile> TileList = GetTileByCords(TileSpotList);
        foreach(MapTile tile in TileList)
        {
            if(tile.CanThisBeOverridden()==false) return false;
        }

        return true;
    }

    public bool DoTheyAllExist(List<Vector3Int> TileSpotList)
    {
        List<MapTile> TileList = GetTileByCords(TileSpotList);
        if(TileList.Count!=TileSpotList.Count) return false;

        return true;
    }


    public int CountCost()
    {
        int cost = 0;

        foreach(KeyValuePair<Vector3Int, MapTile> tile in tilesDict)
        {
            if(tile.Value.isMortal) cost+=tile.Value.cost;
        }

        return cost;
    }

   //public void ChangeToughnessByCords(Vector3Int cords, int newToughness)
   //{
   //   tilesDict[]
   //}



   
// die nachbar suchfunktion vom crack muss damit umgehen können, an einem rand zu stehen und an manchen stellen kein tile zu sehen.
// dazu gibt die GetTileByCords funktion einen default Unpassable Tile zurück, wenn die cords, an denen geschaut wird, nicht im TileLedger stehen
// dieses tile steht selber im Ledger, zu finden unter den unpassable cords (-10-10-10), da hab ich nen MapTile Würfel hingepackt.
// die MakeUnpassable() funktion sucht sich diesen Würfel aus dem Ledger und setzt die toughness auf sehr viel, sodass dieser niemals als besten stop ausgewählt würde.
    public void PrintDict()
    {
        Utilities.PrintDictionary(tilesDict);
    }
  
}
