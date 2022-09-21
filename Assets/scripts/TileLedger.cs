using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLedger : MonoBehaviour
{
    public static TileLedger ledgerInstance;

    [SerializeField] private Dictionary<Vector3Int,MapTile> tilesDict = new Dictionary<Vector3Int, MapTile>();

    public Dictionary<Vector3Int,MapTile> PassTilesDict()
    {
        return tilesDict;
    }

    Vector3Int unpassableCords = new Vector3Int(-10,-10,-10);
    [SerializeField] GameObject InvisibleTile;


    void Awake() 
    {
        // MAKE ledgerINSTANCE
        if      (ledgerInstance == null) { ledgerInstance = this; }
        else if (ledgerInstance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        //DontDestroyOnLoad(this.gameObject);
    
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
