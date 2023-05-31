using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileLedger : MonoBehaviour
{
    // public Button saveMapButton;
    public static TileLedger ledgerInstance;

    [SerializeField] private Dictionary<Vector3Int,MapTile> tilesDict = new Dictionary<Vector3Int, MapTile>();
    [SerializeField] private Dictionary<Vector3Int,List<Vector3>> stressDict = new Dictionary<Vector3Int, List<Vector3>>();
    //[SerializeField] List<Mesh> meshList = new List<Mesh>();
    private List<Vector3Int> positionsWithOutOfDateStress = new List<Vector3Int>();

    public Dictionary<Vector3Int,MapTile> PassTilesDict()
    {
        return tilesDict;
    }

    public void UpdateAllStresses()
    {
        // schreibt das gesamte stress dict in die zu updaten liste, damit die dann im nächsten frame geudated werden. Actually glaube ich aber dass die inert stresses da gar nicht dirn stehen...
        foreach (KeyValuePair<Vector3Int,MapTile> kvp in tilesDict)
        {
            positionsWithOutOfDateStress.Add(kvp.Key);
        }
        return;
    }
    void Awake() 
    {
        // MAKE ledgerINSTANCE
        if      (ledgerInstance == null) { ledgerInstance = this; }
        else if (ledgerInstance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        //DontDestroyOnLoad(this.gameObject);
        // saveMapButton.onClick.AddListener(SerializeMapAndSave);
    }

    private void Update() 
    {
        // wenn in der liste, in der die positionen stehen, die geänderte aber noch nicht geupdatete positionen haben, einträge drinne sind, werden die geupdated und die liste dann gecleart
        if(positionsWithOutOfDateStress.Count==0) return;
        int i = 0;
        foreach(Vector3Int position in positionsWithOutOfDateStress)
        {
            MapTile tile = GetTileByCords(position);
            if(tile==null) continue; // not sure if it will ever be relevant in use case, but if the map is deletd with the deleteMap button,this can cause a crash when a tile is tried tp be updated that has already been deleted, so this checks that
            tile.UpdateStressState();
            i++;
        }
        positionsWithOutOfDateStress.Clear();
        //Debug.Log(string.Format("es wurden {0} tiles geupdated",i)); //not exactly accurate anymore because sometimes the i++ will be skipped over but ultimately irrelevant
        
    }

    // speichert das aktuelle tilesdict als level dictionary unter dem nemen der scene ab.
    // void SaveMapAsDictionary()
    // {
    //     Dictionary<Vector3Int, GameObject> output = new Dictionary<Vector3Int, GameObject>();

    //     foreach(KeyValuePair<Vector3Int,MapTile> kvp in tilesDict)
    //     {
    //         GameObject currentTile = kvp.Value.gameObject;
    //         Vector3Int position = kvp.Key;

    //         output.Add(position,currentTile);
    //     }

    //     save.SaveCurrentMapUnder(output, EventManager.instance.GetLevelName());

    //     return;
    // }

    void SerializeMapAndSave()
    {
        int tilesInMap = tilesDict.Count;

        string mapName = "";
        string[] tileNames = new string[tilesInMap];
        int[][] tileShapes = new int[tilesInMap][];
        float[][] tileProperties = new float [tilesInMap][];

        int i = 0;
        foreach(KeyValuePair<Vector3Int, MapTile> kvp in tilesDict)
        {   
            mapName = EventManager.instance.GetLevelName();
            tileNames[i] = kvp.Value.GetTileName();
            tileShapes[i] = kvp.Value.GetAssociatedShapeArray();

            // hier wird jetzt der property array zusammengesammelt, aus den im tile stehenden values
            float[] propertyArray = new float[11];
            propertyArray[c.type]  = (float)kvp.Value.typeKey;
            propertyArray[c.xCord] = (float)kvp.Value.cords.x;
            propertyArray[c.yCord] = (float)kvp.Value.cords.y;
            propertyArray[c.zCord] = (float)kvp.Value.cords.z;
            propertyArray[c.cost]  = (float)kvp.Value.cost;
            propertyArray[c.baseToughness] = kvp.Value.GetBaseToughness();
            // now come the subtype specific values
            if(kvp.Value.typeKey == c.particleTile1)
            {
               propertyArray[c.interfaceStrength] = kvp.Value.gameObject.GetComponent<ParticleTile1>().GetInterfaceStrengh();
               Debug.Log(string.Format("saved interfaceStr: {0}",kvp.Value.gameObject.GetComponent<ParticleTile1>().GetInterfaceStrengh()));
            }

            if(kvp.Value.typeKey == c.maxPhase || kvp.Value.typeKey == c.PhaseChangeTile) // those are actually the same class, just one bool is different
            {
               PhaseChangeTile phaseChangeHandle = kvp.Value.gameObject.GetComponent<PhaseChangeTile>();
               propertyArray[c.stressFieldStrength] = phaseChangeHandle.GetStressFieldStrength();
               propertyArray[c.star]                = phaseChangeHandle.GetStar() ? 1 : 0;
               propertyArray[c.minRange]            = phaseChangeHandle.GetMinrange();
               propertyArray[c.isMaxPhase]          = kvp.Value.typeKey == c.maxPhase ? 1 : 0;
            }

            // am ende stehen in den nicht applicable feldern auch keine values,aber wenn das builden vom save richtig gebaut ist, werden die auch nie abgefragt. Ist auf jeden fall 100% clean code
            tileProperties[i] = propertyArray;
            i++;
        }

        save.SaveCurrentMapUnder(mapName, tileNames, tileShapes, tileProperties);

        //now store the arrays somewhere so they can be serialized and saved by the save class.
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
        // Debug.Log("stress remover called");
        //Debug.Log(string.Format("stress remover called to remove {0} from {1}",Utilities.GetPreciseVectorString(obsoleteStressVector),position));
        if(!stressDict.ContainsKey(position))
        {
            Debug.Log("hier wurde versucht ein stress zu removen, wo gar nichts ist. weird.");
            return;
        }

        List<Vector3> stressDictAtPositionCopy = new List<Vector3>(stressDict[position]);

        int i = 0;
        foreach(Vector3 stressAtPosition in stressDictAtPositionCopy) // jeder an position anzutreffender vektor wird mit dem gesuchten abgeglichen, wenn die differenz klein genug ist, wird true zurückgegeben, wenn alle durch sind wird flase durchgegeben
        {
            // float delta = (stressAtPosition - obsoleteStressVector).magnitude; // thats actually not good with all the symmetry going around... this only checks length
            // float angle = Vector3.Angle(stressAtPosition, obsoleteStressVector);
            Vector3 deltaVector = new Vector3(stressAtPosition.x-obsoleteStressVector.x, stressAtPosition.y-obsoleteStressVector.y, stressAtPosition.z-obsoleteStressVector.z);
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
                //Debug.Log("about to add the list back in");
                stressDict.Add(position,listCopy);
                //Debug.Log("list added ");
                // GetTileByCords(position).UpdateStressState(); wird replaced durch:
                positionsWithOutOfDateStress.Add(position);
                //Debug.Log("stress updated in list");
                //Debug.Log(string.Format("was steht an der stelle {0} jetzt im stressdict:{1}",position,Utilities.GetListString(listCopy)));
                
                //stressDictCopy[position].RemoveAt(i); //whyyy you no working. You got the stupid vector in there man.

                //stressDict[position].Remove(stressAtPosition); // wenn der grade überprüfte vektor der Liste auf den gesuchten passt, wird er aus der Liste entfernt
            }
            //if(deltaVector.magnitude>0.01) Debug.Log(string.Format("dieser Vektor wars nicht. An der stelle {0} wurden verglichen {1} \n und der vektor {2} wird gesucht", position, Utilities.GetPreciseVectorString(stressAtPosition), Utilities.GetPreciseVectorString(obsoleteStressVector)));

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
            positionsWithOutOfDateStress.Add(position);
            return;
       }


       //Debug.Log(string.Format("hier ist schon ein base stress eingetragen:{0}",position));
     
    }
    public void AddToStressStates(Vector3Int position, Vector3 stressState)
    {
        // wenn an der position noch nichts vermerkt ist, wird ein eintrag erstellt mit der position und einer leeren Liste
        if(position.x+position.y+position.z!=0)
        {
            Debug.Log("position not in correct plane");
            return;
        }
        
        if(!stressDict.ContainsKey(position))
        {
            stressDict.Add(position, new List<Vector3>());
           // Debug.Log("hier wurde eine neue position im stressdict erstellt");
        }
       
        stressDict[position].Add(stressState); // zu der Liste an stelle position wird der Stress state geadded
        positionsWithOutOfDateStress.Add(position); // die geänderten einträge werden in der Liste aufgenommen, um dann beim nächsten frame geupdated zu werden.
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
        // Debug.Log("removestressfield() got called");
        foreach(KeyValuePair<Vector3Int, Vector3> entry in fieldList)
        {
            // Debug.Log("start removing stress now");
            RemoveStressFromPosition(entry.Key, entry.Value);
        }
    }

    public List<Vector3> GetStressStatesAtPosition(Vector3Int position)
    {
        if(!stressDict.ContainsKey(position))
        {
            //Debug.Log("hier sind keine StressStates eingetragen. du bekommst eine ganz frische, leere liste");
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
            // bc we dont want to count the matrix tiles
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

    internal bool AreNoneOfThemEdgy(List<Vector3Int> calledShape)
    {
        List<MapTile> TileList = GetTileByCords(calledShape);

        foreach(MapTile tile in TileList)
        {
            if(tile.isEdge() == true) return false;
        }

        return true; // true gets returned only if none of the tiles have the edgy trait
    }
}
