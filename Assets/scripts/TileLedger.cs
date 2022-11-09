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
        
        Vector3 delta;
        foreach(Vector3 stressAtPosition in stressDict[position]) // jeder an position anzutreffender vektor wird mit dem gesuchten abgeglichen, wenn die differenz klein genug ist, wird true zurückgegeben, wenn alle durch sind wird flase durchgegeben
        {
            delta = new Vector3(stressAtPosition.x-gesuchterStressVektor.x,stressAtPosition.y-gesuchterStressVektor.y,stressAtPosition.z-gesuchterStressVektor.z);
            if(delta.magnitude<0.001) return true;
        }
        return false;
    }
    
    public void RemoveStressFromPosition(Vector3Int position, Vector3 obsoleteStressVector)
    {
        Debug.Log(" I got called");
        if(!stressDict.ContainsKey(position))
        {
            Debug.Log("hier wurde versucht ein stress zu removen, wo gar nichts ist. weird.");
            return;
        }

        Dictionary<Vector3Int,List<Vector3>> stressDictCopy = new Dictionary<Vector3Int, List<Vector3>>(stressDict);

        int i = 0;
        foreach(Vector3 stressAtPosition in stressDictCopy[position]) // jeder an position anzutreffender vektor wird mit dem gesuchten abgeglichen, wenn die differenz klein genug ist, wird true zurückgegeben, wenn alle durch sind wird flase durchgegeben
        {
            // float delta = (stressAtPosition - obsoleteStressVector).magnitude; // thats actually not good with all the symmetry going around... this only checks length
            // float angle = Vector3.Angle(stressAtPosition, obsoleteStressVector);
            Vector3 deltaVector = new Vector3(stressAtPosition.x-obsoleteStressVector.x,stressAtPosition.y-obsoleteStressVector.y,stressAtPosition.z-obsoleteStressVector.z);
            if(deltaVector.magnitude<=0.01)
            {
                //Debug.Log("imma try to remove this stress that I found");
                // if(!removeSpecificStressEntry(position, stressAtPosition))
                // {
                //     Debug.Log(string.Format( "I tried to remove {0} from {1} but something didnt work",obsoleteStressVector,position));
                // }
                //Debug.Log(string.Format("stress vector to be removed:{0} \n liste aus der removed werden soll: {1}",Utilities.GetPreciseVectorString(stressAtPosition), Utilities.GetListString(stressDict[position])));
                //Debug.Log(string.Format("\ni ist grade bei: {0}\n",i));
                List<Vector3> listCopy = new List<Vector3>(stressDict[position]);
                listCopy.RemoveAt(i);
                //Debug.Log(string.Format("korrigierte Liste: {0}",Utilities.GetListString(listCopy)));
                stressDict.Remove(position); // thtas gonna fuck me again isnt it?
                stressDict.Add(position,listCopy);
                GetTileByCords(position).UpdateStressState();
                //Debug.Log(string.Format("was steht an der stelle {0} jetzt im stressdict:{1}",position,Utilities.GetListString(listCopy)));
                
                //stressDictCopy[position].RemoveAt(i); //whyyy you no working. You got the stupid vector in there man.

                //stressDict[position].Remove(stressAtPosition); // wenn der grade überprüfte vektor der Liste auf den gesuchten passt, wird er aus der Liste entfernt
            }
            if(!(deltaVector.magnitude<=0.01)) Debug.Log(string.Format("vektor nicht gefunden. das steht an der stelle {0} jetzt im stressdict:{1}\n und der vektor {2} wird gesucht", position, Utilities.GetListString(stressDict[position]),Utilities.GetPreciseVectorString(obsoleteStressVector)));

            i++;
        }

    }

    private bool removeSpecificStressEntry(Vector3Int position, Vector3 stress)
    {
        if(!stressDict.ContainsKey(position))
        {
            Debug.Log(string.Format("an der position {0} gibt es keine liste, da ist wohl was schiefgelaufen",position));
            return false;
        }

        if(!stressDict[position].Contains(stress))
        {
            Debug.Log(string.Format("der gesuchte vektor {0} ist hier nicht aufzufinden",stress));
            return false;
        }

    	Debug.Log("all tests passed, ready for extraction");
        stressDict[position].Remove(stress);
        return true;
    }

    public void SetGlobalStress(Vector3Int position, Vector3 globalStressState)  // this function will only add the vector if its not at the position already
    {
       if(!IsVectorAtPosition(position, globalStressState))
       {
           AddToStressStates(position, globalStressState);
           return;
       }

       //Debug.Log(string.Format("hier ist schon ein base stress eingetragen:{0}",position));
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

    public void AddStressField(Dictionary<Vector3Int,Vector3> fieldList)
    {
       // Dictionary<Vector3Int,List<Vector3>> stressDictCopy = new Dictionary<Vector3Int, List<Vector3>>(stressDict);

        foreach (KeyValuePair<Vector3Int,Vector3> entry in fieldList)
        {
            AddToStressStates(entry.Key, entry.Value);
        }
    }

    public void RemoveStressField(Dictionary<Vector3Int,Vector3> fieldList)
    {
        foreach(KeyValuePair<Vector3Int, Vector3> entry in fieldList)
        RemoveStressFromPosition(entry.Key, entry.Value);
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
