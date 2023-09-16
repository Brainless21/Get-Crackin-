using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] GameObject matrixTile;
    [SerializeField] GameObject particleTile1;
    [SerializeField] GameObject maxPhase;
    [SerializeField] GameObject phaseChangeTile;
    [SerializeField] GameObject testTile;
    [SerializeField] GameObject Crack;
    [SerializeField] int oben;
    [SerializeField] int obenLinks;
    [SerializeField] int obenRechts;
    [SerializeField] int unten;
    [SerializeField] int untenLinks;
    [SerializeField] int untenRechts;
    [SerializeField] int verticalRechts;
    [SerializeField] int verticalLinks;
    [SerializeField] int hexOr;
    [SerializeField] int hexOl;
    [SerializeField] int hexUr;
    [SerializeField] int hexUl;
    [SerializeField] c.mapSize mapSize = c.mapSize.small;
    [SerializeField] Camera levelCam;
    // public Button resetMapButton;
    // public Button restartMapButton;
    // public Button deleteMapButton;
    // public Button loadSavedMapButton;
    Coroutine coroutineBuild;    
    Dictionary<int,GameObject> tileTypeDict = new Dictionary<int,GameObject>(); // this would probably also better work as an enum but it works now so
    private Dictionary<int, int> verticalCutoffR = new Dictionary<int, int>();
    private Dictionary<int, int> verticalCutoffL = new Dictionary<int, int>();

    // here it saves the values used for the next tile it will place
    [SerializeField] private int type = c.particleTile1; // sollte eig nie 0 sein, weil wierdes behavior. ist jetzt hardgecodet
    [SerializeField] private int shape;
    public void SetShape(int shapeType) {shape=shapeType;}
    [SerializeField] private int cost;
    public void SetCost(int modifiedCost) {cost=modifiedCost;}
    [SerializeField] private float baseToughness;
    public void SetBaseToughness(float baseToughness) {this.baseToughness=baseToughness;}
    private int shapeRotation = 0;
    [SerializeField] private int shapeSize = 2;
    public void SetShapeSize(int size) { shapeSize = size;}
    [SerializeField] Mesh mesh;
    public void SetMesh(Mesh newMesh) { mesh = newMesh; }
    [SerializeField] string tileName;
    public void SetTileName(string newName) {tileName = newName;}
    [SerializeField] float interfaceStrength;
    public void SetInterfaceStrenth(float strength) { interfaceStrength = strength;}
    [SerializeField] float stressFieldStrength;
    public void SetStressFieldStrength(float strength) { stressFieldStrength = strength; }
    [SerializeField] bool star;
    public void SetStar(bool isStar) { star = isStar;}
    [SerializeField] c.richtungsvektoren adjustmentDirection;
    [SerializeField] float adjustmentValueX;
    [SerializeField] float adjustmentValueY;

    public Vector3 GetTileAdjustment()
    {
        Vector3 direction1 = Utilities.getDirectionVectorByEnum(adjustmentDirection); // standardmäßig vektor nach oben, kann aber auch geändert werden
        direction1 *= adjustmentValueY/10;
       
        Vector3 direction2 = c.r; //vektor nach rechts
        direction2 *= adjustmentValueX/10;
        return direction1+direction2;
    }

    

    

    private void BuildVerticalCutoff()
    {
        // es wird eine liste an valuePairs generiert, die später der Buildroutine sagt, ob das Tile außerhalb der rechten oder linken grenze ist. Dabei kommt es auf den X-wert an (horizontale achse, er muss innerhalb des oberen und unteren grenzwertes sein), aber wo genau die grenzwerte liegen, hängt ab vom y wert, also der höhe. Weil das ganze konstrukt halt so rautenmäßig aussieht
        int valueR = 0;
        int valueL = 0;
        for(int i = 0; i<21; i++)
        {
            verticalCutoffR[i] = valueR;
            verticalCutoffL[i] = valueL;
            if(i%2!=0) valueR--;
            if(i%2==0) valueL--;
        }

        valueR = 1;
        valueL = 0;
        for(int i = -1; i>-21; i--)
        {
            verticalCutoffR[i] = valueR;
            verticalCutoffL[i] = valueL;
            if(i%2==0) valueR ++;
            if(i%2!=0) valueL ++;
        }
    }

    public bool SetTileType(int typeKey)
    {
        if(typeKey<0|typeKey>4)
        {
            Debug.Log("tile type nicht definiert");
            return false;
        }
        type = typeKey;
        return true;
    }

    public void adjustShapeRotation(Vector3Int cords)
    {
        // this is for the visualisation of the tiles, so the yellow tile preview disappears from the old tiles 
        EventManager.instance.InvokeMouseExit(cords,DrawShape(cords));

        shapeRotation+=1;
        if(shapeRotation>5) { shapeRotation-=6; }
        if(shapeRotation<0) { shapeRotation+=6; }

        // this is so the yellow preview effect appears on the new tiles
        EventManager.instance.InvokeMouseEnter(cords, DrawShape(cords));
    }

    public void adjustShapeRotation(int increment)
    {
        shapeRotation+=increment;
        if(shapeRotation>5) { shapeRotation-=6; }
        if(shapeRotation<0) { shapeRotation+=6; } 
    }

    public bool AdjustShapeSize(int increment)
    {
        int testResult = shapeSize += increment;
        if(testResult<0|testResult>6)
        {
            Debug.Log("Size out of range");
            return false;
        }

        shapeSize = testResult;
        return true;

    }



    public static MapBuilder instance;

    void Awake() 
    {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        // DontDestroyOnLoad(this.gameObject);

        //resetMapButton.onClick.AddListener(ResetMap);
        //restartMapButton.onClick.AddListener(RestartMap);
        //deleteMapButton.onClick.AddListener(DeleteMap);
        //loadSavedMapButton.onClick.AddListener(BuildSavedMap);

        tileTypeDict.Add(c.matrixTile, matrixTile);
        tileTypeDict.Add(c.particleTile1, particleTile1);
        tileTypeDict.Add(c.maxPhase, phaseChangeTile); // das ist extra, MaxPhase sind jetzt einfach PhaseChangeTiles, die nen extre bool IsMaxPhase haben und sich damit dann leicht anders verhalten
        tileTypeDict.Add(c.PhaseChangeTile, phaseChangeTile);

        BuildVerticalCutoff();
    
    }

    private void Start() 
    {
        SetMapSize();
        BuildSavedMap();
        EventManager.instance.MouseClickRight += adjustShapeRotation;
        
    }
   
    private void OnDisable()
    {
        EventManager.instance.MouseClickRight -= adjustShapeRotation;   
    }

    void SetMapSize()
    {
        if(mapSize == c.mapSize.small)
        {
            oben           = 8;
            obenLinks      = 17;
            obenRechts     = 9;
            unten          = 4;
            untenLinks     = 14;
            untenRechts    = 6;
            verticalRechts = 4;
            verticalLinks  = 12;
            hexOr          = 30;
            hexOl          = 30;
            hexUr          = 30;
            hexUl          = 30;

            levelCam.orthographicSize = 10;

            adjustmentValueX = 10;
            adjustmentValueY = -2.5f;
            adjustmentDirection = c.richtungsvektoren.N;
        }

        // if(mapSize == c.mapSize.hexagonal)
        // {
        //     oben           = 20;
        //     obenLinks      = 20;
        //     unten          = 20;
        //     obenRechts     = 20;
        //     untenLinks     = 20;
        //     untenRechts    = 20;
        //     verticalRechts = 1;
        //     verticalLinks  = 13;
        //     hexOr          = 8;
        //     hexOl          = 16;
        //     hexUr          = 2;
        //     hexUl          = 10;

        //     levelCam.orthographicSize = 16;

        //     adjustmentValueX = 10;
        //     adjustmentValueY = -2.5f;
        //     adjustmentDirection = c.richtungsvektoren.N;
        // }

        if(mapSize == c.mapSize.hexagonal)
        {
            oben           = 20;
            obenLinks      = 20;
            unten          = 20;
            obenRechts     = 20;
            untenLinks     = 20;
            untenRechts    = 20;
            verticalRechts = 1;
            verticalLinks  = 13;
            hexOr          = 8;
            hexOl          = 16;
            hexUr          = 2;
            hexUl          = 10;

            levelCam.orthographicSize = 12.4f;

            adjustmentValueX = 20;
            adjustmentValueY = -4f;
            adjustmentDirection = c.richtungsvektoren.N;
        }

        if(mapSize == c.mapSize.hexagonalLarge)
        {
            oben           = 20;
            obenLinks      = 20;
            unten          = 20;
            obenRechts     = 20;
            untenLinks     = 20;
            untenRechts    = 20;
            verticalRechts = 3;
            verticalLinks  = 15;
            hexOr          = 10;
            hexOl          = 18;
            hexUr          = 4;
            hexUl          = 12;

            levelCam.orthographicSize = 15;

            adjustmentValueX = 7;
            adjustmentValueY = -3.5f;
            adjustmentDirection = c.richtungsvektoren.N;
        }

        

        if(mapSize == c.mapSize.large)
        {
            oben           = 11;
            obenLinks      = 25;
            unten          = 7;
            obenRechts     = 14;
            untenLinks     = 25;
            untenRechts    = 9;
            verticalRechts = 7;
            verticalLinks  = 17;
            hexOr          = 30;
            hexOl          = 30;
            hexUr          = 30;
            hexUl          = 30;

            levelCam.orthographicSize = 16;

            adjustmentValueX = 10;
            adjustmentValueY = -2.5f;
            adjustmentDirection = c.richtungsvektoren.N;
        }
        return;
    }

    GameObject GetTileByTag(int tag)
    {
        if(!tileTypeDict.ContainsKey(tag))
        {
            return null;
        }

        return tileTypeDict[tag];
    }


    public void RestartMap()
    {

        Dictionary<Vector3Int,MapTile> currentMap = TileLedger.ledgerInstance.PassTilesDict();

        foreach(KeyValuePair<Vector3Int,MapTile> tile in currentMap)
        {
            tile.Value.Reset();
        }
    }
    void ResetMap()
    {
        Dictionary<Vector3Int,MapTile> currentMap = TileLedger.ledgerInstance.PassTilesDict();
        Dictionary<Vector3Int,MapTile> currentMapCopy = new Dictionary<Vector3Int, MapTile>(currentMap);

        // jedes mortal tile das in der kopie ist, wird gelöscht
        foreach(KeyValuePair<Vector3Int,MapTile> entry in currentMapCopy)
        {
            entry.Value.SelfRemoveTile();  
        }

    
        coroutineBuild = StartCoroutine(Build());
        
    }

    void DeleteMap()
    {
        Dictionary<Vector3Int,MapTile> currentMap = TileLedger.ledgerInstance.PassTilesDict();
        Dictionary<Vector3Int,MapTile> currentMapCopy = new Dictionary<Vector3Int, MapTile>(currentMap);

        // jedes mortal tile das in der kopie ist, wird gelöscht
        foreach(KeyValuePair<Vector3Int,MapTile> entry in currentMapCopy)
        {
            entry.Value.SelfRemoveTile();  
        }
    }

    // funktion, die Tiles an stellen setzen kann, wo vorher noch keins war
    private MapTile CreateTile(Vector3Int cords, int type)
    {
        if(TileLedger.ledgerInstance.GetTileByCords(cords)!=null)
        {
           // Debug.Log("hier ist schon ein tile, nutze bitte die placeTile Funktion");
           return null;
        }

        GameObject tileType = GetTileByTag(type);

        MapTile handle = Instantiate(tileType, cords, Quaternion.Euler(55f, 45f, 0f)).GetComponent<MapTile>();
        //handle.SubscribeToStress(); for some reason passiert das 999+ mal?
        return handle;


    }
    
    public MapTile PlaceTile(Vector3Int cords, bool careAboutCost=false, bool canCreateNewSpace=false, bool canOverride=true)
    {
        
        MapTile currentOccupant = TileLedger.ledgerInstance.GetTileByCords(cords);
        
    	GameObject tileType = GetTileByTag(type); //muss gameObject sein, weil instantiate das so braucht

        if(currentOccupant==null&&canCreateNewSpace==false) 
        {
            Debug.Log("hier ist keine map area, und ich darf keine neue erschaffen");
            return null;
        }

        if(currentOccupant!=null&&canOverride==false)
        {
            // Debug.Log("hier ist schon besetzt und ich darf nicht überschreiben");
            return null;
        }

        if(tileType==null)
        {
            Debug.Log("der typ des zu plazierenden tiles ist nicht definiert");
            return null;
        }
       
        // überprüft, ob genug cash money für das platzierenverfügbar ist
        if(careAboutCost==true)
        {
            // die kosten des tiles werden vom gespeicherten übernommen, außer sie sind 0, dann wird der Fundsaccount nach den standard werten gefragt
            int currentCost;
            currentCost = cost;
            if(cost==0) currentCost = FundsAccount.instance.GetPriceByType(type);

            int currentBalance = FundsAccount.instance.GetBalance();
            int transactionBalance = currentOccupant.cost-currentCost;
            int futureBalance = currentBalance+transactionBalance;
            if(careAboutCost==true&futureBalance<0) 
            {
                //were never actually here you guys
                Debug.Log("nicht genug funds");
                PointPopup.Create(cords, transactionBalance, true);
                return null;
            }

            // spawn the cost popup
            PointPopup.Create(cords, transactionBalance);
        }
        

        

        // delete current tile, if present.
        if(currentOccupant!=null)
        {
            //Debug.Log("hier ist schon ein tile, es wird also vor platzierung removed")
            currentOccupant.SelfRemoveTile();
        }

        // place the tile and get the handle
        MapTile handle = Instantiate(tileType, cords, Quaternion.Euler(55f, 45f, 0f)).GetComponent<MapTile>();

        FundsAccount.instance.UpdateDisplay();
        // return the tile that was created so its accessible
        //handle.SubscribeToStress(); // steht auhc nochmal bei createTile(). Kann aber eigentlich nicht der way sein, das ding in das event zu bekommen, der müsste das selber machen können, aber awake kommt nicht und start nimmt das vom entity weg
        return handle;

    }

    public bool PlaceTileShape(Vector3Int origin)
    {
        List<Vector3Int> currentShape = new List<Vector3Int>();

        currentShape = DrawShape(origin);

        int currentBalance = FundsAccount.instance.GetBalance();
        int futureBalance = currentBalance;
        int transactionBalance = 0;

        // es wird überprüft, ob die shape out of bounds geht
        List<MapTile> targetTiles = TileLedger.ledgerInstance.GetTileByCords(currentShape); // liste aller tiles, auf denen platziert würde
        if(targetTiles.Count!=currentShape.Count) // wenn die liste der map tiles kleiner ist, als die shape liste, dann reicht die shape außerhalb der map
        {
            Debug.Log("shape is out of bounds");
            return false;
        }

        // es wird überprüft ob die shape am rand der map liegen würde
        foreach(MapTile tile in targetTiles)
        {
            if(tile.isEdge()==true)
            {
                Debug.Log("shape darf nicht den rand berühren");
                return false;
            }

        }

        // check the funds situation

        foreach(Vector3Int spot in currentShape)
        {
            // *sollte* bedeuten: currentcost ist Getpriceby(type) wen cost==0 und sonst = cost
            int currentCost = (cost==0)? FundsAccount.instance.GetPriceByType(type) : cost;
            transactionBalance += TileLedger.ledgerInstance.GetTileByCords(spot).cost-currentCost; // jedes zu platzierende tile wird durhgegangen und die balance errechnet
        }
        futureBalance+=transactionBalance;

        // wenn rauskommt, das am ende nicht genug geld wäre, wird nichts platziert
        if(futureBalance<0)
        {
            Debug.Log("nicht genug funds für die shape");
            PointPopup.CreateError(Utilities.ConvertIntToCords(origin));
            return false;
        }

       

        // es wird überprüft, ob auf der fläche des shapes auch nur tiles sitzen, die üebrschrieben weden dürfen (also nur matrix tiles, die nur matrix tiles als nachbarn haben)
        foreach(MapTile tile in targetTiles)
        {
            if(tile.CanThisBeOverridden(type,true)==false) return false;

            // if(tile.typeKey!=c.matrixTile)
            // {
            //     Debug.Log("hier ist kein platz für die shape, es würde etwas anderes überschnitten");
            //     return false;
            // }
        }

        // an jedem spot der shape wird ein tile platziert, diesem wird mitgegeben, zu welcher gruppe an tiles er gehört, shape wise.
        //bool firstIteration = true;
        foreach(Vector3Int spot in currentShape) // jedes tile bekommt ausserdem seine eigenschaften zugewiesen
        {
           MapTile handle = PlaceTile(spot,false);
           // nach dem platzieren werden die kosten und die toughness der Tiles gemäß der im builder gespeicherten custom values geändert, es sein denn die custom values sind 0 (standart fall, keine modifikation)
           // hier muss noch ein system hin, sodass tiles mit standartwerten diese auch bekommen, wenn sie nach tiles mit nicht standartwerten platziert werden. Grade wird ja der speicher mit den details zum tile nach dem platzieren nicht gewiped.


            handle.typeKey = type;

           if(cost!=0)
           {
               handle.cost=cost;
               // FundsAccount.instance.UpdateDisplay(); // is now in the acessor
               //Debug.Log(string.Format(string.Format("cost set to {0}",handle.cost)));
           }


           if(baseToughness!=0)
            {
               handle.SetBaseToughness(baseToughness);
            }
            
           handle.SetName(tileName);
           
           handle.gameObject.GetComponent<MeshFilter>().mesh = mesh;
           
           if(type==c.particleTile1)
           {
                handle.ModifyBehavior(c.grenzflaeche, interfaceStrength);
           }


           if(type==c.maxPhase)
           {
                PhaseChangeTile phaseChangeHandle = handle.gameObject.GetComponent<PhaseChangeTile>();
                phaseChangeHandle.SetStressFieldStrength(stressFieldStrength);
                phaseChangeHandle.SetStar(false);
                phaseChangeHandle.SetMinRange(shapeSize);
                phaseChangeHandle.isMaxPhase = true;  

           }

           if(type==c.PhaseChangeTile)
           {
                PhaseChangeTile phaseChangeHandle = handle.gameObject.GetComponent<PhaseChangeTile>();
                phaseChangeHandle.SetStressFieldStrength(stressFieldStrength);
                phaseChangeHandle.SetStar(star);
                phaseChangeHandle.SetMinRange(shapeSize);
                
           }
    
        
           
           handle.SetAssociatedShape(currentShape);
           MasterOfShapes.instance.AddShapeToList(currentShape);
           //firstIteration = false;
        }

        PointPopup.Create(origin,transactionBalance);
        return true;
    }

    // malt eine shape an einem punkt. gibt eine liste mit koordinaten aus, von allen tiles die diese shape bilden. die details dieser shape ist in Mapbuilder selbst gespeichert, und müssen also eventuell vor aufruf enttsprechend angepasst werden.
    public List<Vector3Int> DrawShape(Vector3Int origin)
    {
        
        List<Vector3Int> currentShape = new List<Vector3Int>();

        if(shape==c.empty) return currentShape;

        if(shape==c.single)
        {
            currentShape.Add(origin);
        }

        if(shape==c.blob)
        {
            List<Vector3Int> friendsPointers = c.positionsArray;
            List<Vector3Int> visitedFriends = new List<Vector3Int>();
            List<Vector3Int> targetFriends = new List<Vector3Int>();
            List<Vector3Int> targetFriendsBuffer = new List<Vector3Int>();
        
            targetFriends.Add(origin);
            visitedFriends.Add(origin);

            for(int i=1; i<=shapeSize-1; i++)
            {
                // jedes tile in targetFriends findet seine nachbarn und trägt die in targetfriendsBuffer ein
                foreach (Vector3Int tile in targetFriends) 
                {
                    List<Vector3Int> tileFriends = new List<Vector3Int>();
                    foreach(Vector3Int pointer in friendsPointers)
                    {
                        tileFriends.Add(tile+pointer);
                    }
                    targetFriendsBuffer.AddRange(tileFriends);
                }

                // sofern der eintrag aus dem Buffer nicht schon in visitedfriends steht, oder schon in target friends eingetragen wurde, wird er in targetFriends Übertragen
                foreach(Vector3Int tile in targetFriendsBuffer)
                {
                    if (!visitedFriends.Contains(tile)&&!targetFriends.Contains(tile)) { targetFriends.Add(tile); }
                }

                // dann werden alle TargetFriends als visited abgespeichert
                visitedFriends.AddRange(targetFriends);

           
        }

            currentShape = targetFriends;
        }

        if(shape==c.fiber)
        {
            //currentShape.Add(origin);
            for(float i=0;i<shapeSize;i++)
            {
                //int flippy = Convert.ToInt16(Math.Pow(Convert.ToDouble(-1),Convert.ToDouble(i*2))); // all this jazz for a simpe = -1^2i. smh.
                
                int flippy = (int)Mathf.Pow((float)-1,(float)i)*-1; // -1^i*-1
                int pointer = Mathf.CeilToInt(i/2);
                Vector3Int result = new Vector3Int();
                result = origin+pointer*flippy*c.positionsArray[shapeRotation];
                if(!currentShape.Contains(result)) currentShape.Add(result);
                
            }
        }

        if(shape==c.nugget)
        {
            currentShape.Add(origin);
            if(shapeRotation%2==0)
            {
                currentShape.Add(origin+c.or);
                currentShape.Add(origin+c.ol);
            }

            else
            {
                currentShape.Add(origin+c.ur);
                currentShape.Add(origin+c.ul);
            }

        }
       
        // foreach(Vector3Int spot in currentShape) 
        // {
        // Debug.Log("--------");
        // Debug.Log(spot);
        // }
        //Debug.Log(currentShape.Count);
        return currentShape;

    }

    private void BuildFromDictionary(string currentMapName, string[] currentTileNames, int[][] currentTileShapes, float[][] currentTileProperties)
    {
        int i = 0;
        foreach(string TileName in currentTileNames)
        {
            // Utilities.PrintArray(currentTileShapes[0]);
            // Utilities.PrintArray(currentTileProperties[0]); //should print out the properties of the fist tile in the map
            Vector3Int position = new Vector3Int(Mathf.RoundToInt(currentTileProperties[i][c.xCord]), Mathf.RoundToInt(currentTileProperties[i][c.yCord]), Mathf.RoundToInt(currentTileProperties[i][c.zCord]));
            int type = Mathf.RoundToInt(currentTileProperties[i][c.type]);
            MapTile handle = CreateTile(position, type); //Mathf.RoundToInt(currentTileProperties[i][c.type])
            handle.name = TileName;

            handle.cost = Mathf.RoundToInt(currentTileProperties[i][c.cost]);
            handle.SetBaseToughness(Mathf.RoundToInt(currentTileProperties[i][c.baseToughness]));

            handle.isErasable = false;

            if(type==c.particleTile1)
            {
                Debug.Log("hier ist ein grenzflächen behaviour zugeteilt worden");
                handle.ModifyBehavior(c.grenzflaeche, currentTileProperties[i][c.interfaceStrength]);
            }

            if(type==c.maxPhase)
           {
                PhaseChangeTile phaseChangeHandle = handle.gameObject.GetComponent<PhaseChangeTile>();
                phaseChangeHandle.SetStressFieldStrength(currentTileProperties[i][c.stressFieldStrength]);
                phaseChangeHandle.SetStar(false);
                phaseChangeHandle.SetMinRange(Mathf.RoundToInt(currentTileProperties[i][c.minRange]));
                phaseChangeHandle.isMaxPhase = true;
           }

             if(type==c.PhaseChangeTile)
           {
                PhaseChangeTile phaseChangeHandle = handle.gameObject.GetComponent<PhaseChangeTile>();
                phaseChangeHandle.SetStressFieldStrength(currentTileProperties[i][c.stressFieldStrength]);
                bool isStar = Mathf.RoundToInt(currentTileProperties[i][c.star]) == 1 ? true : false; // basically converting the float 1 or 0 into an int and then into a bool depending on if tis a 1 or a 0
                phaseChangeHandle.SetStar(isStar);
                phaseChangeHandle.SetMinRange(Mathf.RoundToInt(currentTileProperties[i][c.minRange]));

           }


            //now we collapse the array of the shapes into a neat list of Vector3Ints
            List<Vector3Int> associatedShapes = new List<Vector3Int>();
            Vector3Int intVector = new Vector3Int();
            int j = 0;

            foreach(int shapeCord in currentTileShapes[i])
            {
                if(j%3==0) intVector.x=shapeCord;     //bei 0,3,6, also den ersten einträgen
                if(j%3==1) intVector.y=shapeCord;
                if(j%3==2)
                {
                    intVector.z=shapeCord;
                    associatedShapes.Add(intVector);
                }
                j++;
            }
            // damit sollte die liste komplett sein, und kann nun dem tile zugewiesen werden. Danach wird sie gewiped. Wobei die wir ja eh in jedem durchgang neu erstellt
            handle.SetAssociatedShape(associatedShapes);
            
            
            i++;
        }
    }
    public void BuildSavedMap() //sollte jetzt nur die alte map löschen und die geladene bauen, wenn diese auch existiert, ansonsten tut sie nicht viel. Also sollte sie am anfang ausgeführt werden können, und dann das gespeicherte level bauen, falls vorhanden
    {
        String nameStr = EventManager.instance.GetLevelName();
        if(!save.LoadSavedMap(nameStr)) 
        {
            Debug.Log("es konnte keine map geladen werden, weil keine savedatei existiert");
            return;
        }
        DeleteMap();
        BuildFromDictionary(save.GetMapName(), save.GetTileNames(), save.GetTileShapes(), save.GetTileProperties());
        FundsAccount.instance.UpdateDisplay();
    }
    


    private IEnumerator Build()

    {
        
       //Debug.Log("coroutine started");
       int xStart = -untenLinks;
       int xEnd = obenRechts;
       int yStart = -oben;
       int yEnd = unten;
       int zStart = -untenRechts;
       int zEnd = obenLinks;

    

        for(int x=xStart ; x<xEnd ; x++)
        {
            for(int y=yStart ; y<yEnd ; y++)
            {
                for(int z=zStart; z<zEnd; z++)
                {
                    float xf = (float)x;
                    float yf = (float)y;
                    float zf = -xf-yf;
                    Vector3 cords = new Vector3(xf,yf,zf);
                    Vector3Int cordsInt = new Vector3Int(x,y,-x-y);

            

            
                    if(zf>=zStart&zf<zEnd) //schneidet die spitzen ecken der raute ab
                    {
                        if(x<verticalCutoffR[y]+verticalRechts && x>verticalCutoffL[y]-verticalLinks) // diese lovely abfrage schaut, ob die koordinaten innerhalb der rechten und linken grenzen liegen. dafür wird der y-Wert in die verticalcutoff Liste gegeben und raus kommt, welchen wert x maximal/minimal haben darf, unter berücksichtungung der im editor eingegebenen grenzen.
                        {
                            if((Utilities.CustomDotProduct(cords, hexOr, Utilities.getDirectionVectorByEnum(c.richtungsvektoren.NNE))>0)) continue;
                            if((Utilities.CustomDotProduct(cords, hexOl, Utilities.getDirectionVectorByEnum(c.richtungsvektoren.SSE))<0)) continue;
                            if((Utilities.CustomDotProduct(cords, -hexUl, Utilities.getDirectionVectorByEnum(c.richtungsvektoren.NNE))<0)) continue;
                            if((Utilities.CustomDotProduct(cords, -hexUr, Utilities.getDirectionVectorByEnum(c.richtungsvektoren.SSE))>0)) continue;
                            //Debug.Log("nope, rechter cutoff überschritten");
                            MapTile handle = CreateTile(cordsInt,c.matrixTile);
                            //handle.SetName("Matrix Tile");

                            
                        }

                    }

                }
                    //yield return new WaitForSeconds(zeit);//wartet zeit sekunden bevor er weitermacht
            }
        }

        //EventManager.instance.SetMouseMode(c.placeTile); // nach bauen der map wird tspe auf 1 gesetzt, weil matrix tile placen eigentlich jetzt von c.cleartile übernommen wird.
        FundsAccount.instance.UpdateDisplay();
        yield return new WaitForEndOfFrame();
        //EventManager.instance.InvokeStressSetup(c.orf); // das machen die ja schon alle selber on awake()
        yield break;
    }
}
