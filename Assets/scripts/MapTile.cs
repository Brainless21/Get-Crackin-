using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : Entity
{
   
    //public Vector3Int cords{get;set;}
   public static Vector3Int[] positionsArray = new [] 
    { 
        new Vector3Int(1,-1,0), new Vector3Int(1,0,-1), new Vector3Int(0,1,-1), //ein array mitden richtungsvektoren, durchgezaehlt von oben rechts, im uhrzeigersinn
        new Vector3Int(-1,1,0), new Vector3Int(-1,0,1), new Vector3Int(0,-1,1) 
    };
    internal List<Vector3Int> friendsCords = new List<Vector3Int>();
    public List<Vector3Int> GetFriendsCords() { return friendsCords; }
    public Vector3Int GetFriendsCords(int position) { return friendsCords[position]; }
    internal TileBehaviour behaviour = new TileBehaviour();
    public Color testColor;
    private Color baseColor;
    internal Color currentRestingColor;
    internal void ChangeRestingColor(Color newColor) {currentRestingColor=newColor;}
    internal void ChangeRestingColor(float increment) {currentRestingColor *=increment;}
    internal Color currentDisplayedColor;
    internal void ChangeDisplayedColor(Color newColor) {currentDisplayedColor=newColor;}
    public int typeKey { get;set; }
    public int cost { get; set; }
    [SerializeField] protected float baseToughness = 1;
    [SerializeField] public bool isMortal = true;
    bool isCracked = false;
    bool mouseOver = false;
    public bool isErasable = true;
    [SerializeField] internal List<Vector3Int> associatedShape = new List<Vector3Int>();
    int mouseOverValidity;
    string tileName = "Matrix";
    public void SetName(string newName) {this.tileName = newName;}
    public string GetTileName() { return this.tileName; }
    [SerializeField] bool isJiggly = false;
    Vector3 baseStressState; // that is given by the level, should be the same for all tiles. (unless I add some weird wavy stress field later who knows)
    [SerializeField] List<Vector3> stressStates = new List<Vector3>();
    [SerializeField] Vector3 currentStressVector =new Vector3(0f,0f,0f);
    internal GameObject stressStateIndicatorArrow;  
    GameObject arrowHandle;
    Coroutine Jiggle;
    [SerializeField] float interfaceStrenghBase;

    [SerializeField] int typeForInspection;

    bool isBrightenedUp = false;


    float[] propertyArray = new float[11]; //das sollte okay sein, weil bei der zuweisung über die placetileshape funktion das eh komplett neu zugewiesen wird

    public void SetPropertyArray(float[] array)
    {
        propertyArray = array;
    }

    public float[] GetPropertyArray() => propertyArray;


    //interface strength und alles andere der anderen unterklassen I guess. Ich hätte echt keine unterklassen gebraucht, huh...
   
    void Awake() //apparently never happens? weil die kinder drunter (matrixTile, ParticleTile usw.) schon eine Awake funktion defineirt haben probably
    {
        
    }
    
    // public void AddToStressStates(Vector3 newStressState) // das muss alles in das stressdict rein
    // {
    //     stressStates.Add(newStressState);
    //     UpdateStressState();
    // }

    // public bool RemoveFromStressStates(Vector3 outdatedStressState) // im Stressledger jetzt
    // {
    //     float delta;
    //     foreach(Vector3 stressState in stressStates)
    //     {
    //         delta = (stressState-outdatedStressState).magnitude;
    //         if(delta<0.001)
    //         {
    //             stressStates.Remove(stressState);
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    // public void SetBaseStressState (Vector3 baseStress) // ist jetzt zum tileledger umgezogen
    // {
    //     //Debug.Log(baseStress);
        
    //     Vector3 result = new Vector3();

    //     // if(EventManager.instance.GetCrackMode()==c.CrackMode.Direction)
    //     // {
    //     // result = baseStress; 
    //     // }

    //     // if(EventManager.instance.GetCrackMode()==c.CrackMode.Point)
    //     // {
    //     //     Debug.Log("what are you doing with stresses, youre in point mode my dude");
    //     // }

        
    //     result = baseStress;
    //     //Debug.Log(result);
    //     if(result.magnitude>0.0001) result /= result.magnitude; //skaliert länge auf 1
    //     AddToStressStates(result);
    //     //Debug.Log("I happened, follow the rabbit");
    // }

    public Vector3 GetStressState()
    {
        UpdateStressState();
        return currentStressVector; // "=> currentStressVector;" apparently means: {return currentStressVector;}
    } 

    
    public void UpdateStressState()
    {

        Vector3 globalDirection = EventManager.instance.GetGlobalStress();
       
        // nudel alle stressVectors zusammen in einen Current Vector, dafür wird er erst einmal resettet:
        currentStressVector =new Vector3(0f,0f,0f);

        // erstellt eine liste mit allen hier grade wirkenden vektoren. Das sind die im ledger plus der globalStress
        List<Vector3> stressStatesHere = new List<Vector3>(TileLedger.ledgerInstance.GetStressStatesAtPosition(this.cords));
        
        stressStatesHere.Add(globalDirection); //so its always there, but doesnt need to be in the stressdict for every single entry or changed for every entry when the globalStress changes.
        
        this.stressStates = new List<Vector3>(stressStatesHere); //für visualisierungs purposes
        
        foreach(Vector3 stressVector in stressStatesHere)
        {
            // if the angle between the vector and the base stress is greater than 90 degrees, flip the vector around, so all the vectors point in the same direction. this assures symetric stress field overlapping
            if(Vector3.Angle(globalDirection,stressVector)>90)
            {
                
                Vector3 flippedStressvector = stressVector*-1;
                currentStressVector += flippedStressvector;
            }
            else currentStressVector += stressVector;

            // addiert die vektoren aufeinender, entweder erst nachdem der vektor einmal geflippt wird (das ist der fall wenn die beiden nicht in die gleiche richtung zeigen) oder einfach so

        }

        // wenn der sress nicht angezeigt werden muss, und auch keiner da ist, sind wir hier fertig. Ansonsten wird der pfeil erstellt bzw. gedreht
        if(!EventManager.instance.GetStressVisibility&arrowHandle==null) return; // das kann ich auch noch machen wenn es wegen performance sein muss, grade komm ihc auch damit klar die pfeile im ersntfall einmal zu kreiren und dann direkt wieder zu löschen
        // wenn schon (von vorher) ein pfeil da ist aber die visibility jetzt aus ist, muss der pfeil ausßerdem gelöscht werden
        if(!EventManager.instance.GetStressVisibility&arrowHandle!=null)
         {
            Destroy(arrowHandle);
            arrowHandle = null;
            return;
        }
        
        if(arrowHandle==null)
        {
            stressStateIndicatorArrow = GameAssets.instance.stressArrow;
            arrowHandle = Instantiate(stressStateIndicatorArrow, transform.position, Quaternion.identity);
            arrowHandle.transform.parent = this.transform;
        }
        // wenn der stressvektor zu klein ist, wird kein pfeil mehr angezeigt und es wird auch nicht versucht, irgendwas zu drehen
        if(currentStressVector.magnitude<0.001|!EventManager.instance.GetStressVisibility)
        {
            Destroy(arrowHandle);
            arrowHandle = null;
            return;
        }

        float rotationAngle = Utilities.ConvertVectorToRotationAngle(currentStressVector); //aus dem vektor wird über skalarprodukt ein winkel gemacht, der wird in grad umgerechnet und damit dann der arrow gedreht.
        if(float.IsNaN(rotationAngle))
        {
            Debug.Log("ERRORO: es konnte kein winkel errechnet werden");
            return;
        }
        arrowHandle.transform.localRotation=Quaternion.Euler(0,Utilities.ConvertRadiansToDegree(rotationAngle),0);
        arrowHandle.transform.localScale = new Vector3(0.6f,1f,0.3f);
    }

    public virtual void SetAssociatedShape(List<Vector3Int> shape)
    {  
        associatedShape = shape;   
    }
    public void SetAssociatedShape(Vector3Int shape)
    {
        List<Vector3Int> singleShape = new List<Vector3Int>() {shape};
        associatedShape = singleShape;
    }

    // outputs a list of all the coordinates in the assiciated shape, {x1,y1,z1,x2,y2,z2,...}
    public int[] GetAssociatedShapeArray()
    {
        //int[] shapeArray = associatedShape.ToArray();
        List<int> shapeList = new List<int>();
        foreach(Vector3Int spot in associatedShape)
        {
            shapeList.Add(spot.x);
            shapeList.Add(spot.y);
            shapeList.Add(spot.z);
        }

        int[] output = shapeList.ToArray();
        return output;
    }

    protected Dictionary<int, float> myBehaviors = new Dictionary<int, float>(); //"what behaviors do I (possibly, if they care about that) exert on my fellow neighbor tiles?"

    internal Dictionary<int,Color> colorList = new Dictionary<int, Color>()
    {
        {0, Color.white},
        {1, Color.yellow},
        {2, Color.red},
        {3, Color.yellow}
    };
    // private void Start() // wenn das an ist, dann wird die entity start funktion nicht mehr ausgeführt und keine der tiles reagiert mehr au die maus
    // {
    //     EventManager.instance.stressSetupNeedsToHappen+=SetBaseStressState;
    //     Debug.Log("subscribed");
    // }
    private void Update() 
    {
        //if(GetComponent<MeshRenderer>().material.color!=currentDisplayedColor) GetComponent<MeshRenderer>().material.color = currentDisplayedColor; 
        //currentDisplayedColor = currentRestingColor;
        Render();
        if(typeForInspection!=typeKey) typeForInspection = typeKey;
        if(baseToughness<0.1) 
        {
            gameObject.GetComponent<MeshFilter>().mesh = GameAssets.instance.emptyMesh;
        }
       
    }

    protected void Initialize(MapTile myself)
    {
        cords = ConvertToHex(myself);
        WriteFriendsCords();
        if(!TileLedger.ledgerInstance.AddTileToLedger(myself)) // das tile schreibt sich in den ledger. falls das fehlschlägt, löscht es sich direkt wieder
        {
            Destroy(this.gameObject);
        }
        associatedShape.Add(cords);
        baseColor = GetComponent<MeshRenderer>().material.color;
        currentRestingColor = baseColor;
        currentDisplayedColor = baseColor;
        UpdateStressState();
        //Debug.Log("bin eingeschrieben(initialize)");
        //this.interfaceStrengh = myself.interfaceStrengh;
        this.AdjustPosition();

    }

    private void AdjustPosition()
    {
        transform.position+=MapBuilder.instance.GetTileAdjustment();
    }

    void Render()
    {
        int mouseMode = EventManager.instance.GetMouseMode();
        Color finalColor = new Color();

        if(mouseOver==true) // if youre being moused over
        {
            finalColor = colorList[mouseMode]; // get the color corresponding to the type

            if(mouseOverValidity!=c.valid) finalColor=Color.red; // if its not valid tho, make it red.

            GetComponent<MeshRenderer>().material.color = finalColor; // make tile that color
            return;
        }


        finalColor = currentRestingColor;

        if(isBrightenedUp==true)
        {
            finalColor *= 2;
            finalColor.r *= 0.9f;
            finalColor.a =1;
            finalColor = Color.yellow;
            finalColor.g = 0.65f;
        }

        if(isCracked==true) 
        {
            finalColor *=0.5f;
            finalColor.a =1;
        }

        GetComponent<MeshRenderer>().material.color = finalColor;
    }

    public void BrightenUp()
    {
        isBrightenedUp = true;
    }
    internal void BrightenDown()
    {
        isBrightenedUp = false;
    }



    private IEnumerator Stretch(Vector3 axis, Vector3 origin, float animationTime)
    {
        //if(isJiggly==false) yield break;
        yield return 0; // Warte auf den nächsten frame, damit alle gleichzeitig anfangen
        //  unsubscribe from the mouse events so you cant get clicked on
        EventManager.instance.MouseEnter-=MouseEnter;
        EventManager.instance.MouseExit-=MouseExit;
        EventManager.instance.MouseClickRight-=MouseInteractionRight;
        EventManager.instance.MouseClickLeft-=MouseInteractionLeft;
        

        Vector3 P = new Vector3(); // der Punkt auf der Axe, der dem Tile am nächsten liegt.
        Vector3 toAxis = new Vector3(); //der Vector vom Tile zu P
        int stepsPerSecond = 30;
        float stepSize = Mathf.PI/(stepsPerSecond*animationTime);
        float maxDistance = 0.25f;
        float stepTime = (animationTime*stepSize)/Mathf.PI;

        P=origin+axis*Vector3.Dot(this.cords-origin,axis); // https://math.stackexchange.com/questions/1905533/find-perpendicular-distance-from-point-to-line-in-3d
        toAxis = this.cords-P;
        Vector3 toAxisScaledToOne = toAxis/toAxis.magnitude;
        if(toAxis.magnitude==0) toAxisScaledToOne = c.originf;
        

        Vector3 originalPosition = this.transform.position;
        int i=0;

        for(float delta = 0; delta<Mathf.PI; delta+=stepSize)
        {
            Debug.Log(string.Format("original position:{0}\n next step: {1}",Utilities.GetPreciseVectorString(originalPosition),Utilities.GetPreciseVectorString((toAxis/toAxis.magnitude)*Mathf.Sqrt(toAxis.magnitude)*Mathf.Sin(delta)*maxDistance)));
            this.transform.position=originalPosition+(toAxisScaledToOne)*Mathf.Sqrt(toAxis.magnitude)*Mathf.Sin(delta)*maxDistance; // die verschiebung wird skaliert mit wie weit das tile von der achse weg ist, aber mitwurzel, sodass weit weg liegende nicht so sehr krass nach aussen yeeten
            yield return new WaitForSeconds(stepTime);
            i++;
        }

        this.transform.position = originalPosition;
        Debug.Log(i);
        Debug.Log(stepTime);

        // this.MouseExit(this.cords,new List<Vector3Int>{this.cords}); // thats just here so the mouseover color change doesnt stick, it shouldnt be relevant however because the stretch is usually not initiated by a mouse click on the map
        // re-subscribe to all the mouse events
        EventManager.instance.MouseExit+=MouseExit;
        EventManager.instance.MouseEnter+=MouseEnter;
        EventManager.instance.MouseClickRight+=MouseInteractionRight;
        EventManager.instance.MouseClickLeft+=MouseInteractionLeft;

        yield break;
    }

    public void StartStretching(Vector3 axis, Vector3 origin)
    {
       Jiggle = StartCoroutine(Stretch(axis,origin,0.25f));
    }
    public virtual float GetToughness() //wird glaub ich nie actually verwendet, alle tiles overriden das mit ihrem eigenen ding
    {
        float modifier = 1;
        List<int> appliedBehaviours = new List<int>();

        List<MapTile> friendsRange1 = this.GetFriendsByRange(1);

        foreach(MapTile inspectedTile in friendsRange1)
        {
            
            //now check for interface behavior (behaviour#1)
            if(inspectedTile.behaviour.HasBehavior(c.grenzflaeche) & !appliedBehaviours.Contains(c.grenzflaeche))
            {
                modifier*=inspectedTile.behaviour.GetBehaviourByType(c.grenzflaeche);
                appliedBehaviours.Add(c.grenzflaeche);
            }
        }

        currentRestingColor *= modifier;
        currentRestingColor.a = 1;
        UpdateDisplayColor();
        return baseToughness*modifier;
    }
    public void SetBaseToughness(float value){baseToughness = value;}



    private static Vector3Int ConvertToHex(MapTile tile)
    {
        return Vector3Int.FloorToInt(tile.transform.position);
    }

    private void WriteFriendsCords()
    {
        foreach (Vector3Int position in positionsArray)
        {
            friendsCords.Add(cords+position);
        }
    }

    public List<MapTile> GetFriendsByRange(int range)
    {
        List<MapTile> visitedFriends = new List<MapTile>();
        List<MapTile> targetFriends = new List<MapTile>();
        List<MapTile> targetFriendsBuffer = new List<MapTile>();
        
        targetFriends.Add(this);
        visitedFriends.Add(this);

        for(int i=1; i<=range; i++)
        {
            // jedes tile in targetFriends findet seine nachbarn und trägt die in targetfriendsBuffer ein
            foreach (MapTile tile in targetFriends) 
            {
                targetFriendsBuffer.AddRange(TileLedger.ledgerInstance.GetTileByCords(tile.GetFriendsCords()));
            }

            // targetFriends liste wird gewiped, damit am ende wirklich nur die Äußersten tiles Drinstehen
            targetFriends.Clear();

            // sofern der eintrag aus dem Buffer nicht schon in visitedfriends steht, oder schon in target friends eingetragen wurde, wird er in targetFriends Übertragen
            foreach(MapTile tile in targetFriendsBuffer)
            {
                if (!visitedFriends.Contains(tile)&&!targetFriends.Contains(tile)) { targetFriends.Add(tile); }
            }

            // dann werden alle TargetFriends als visited abgespeichert
            visitedFriends.AddRange(targetFriends);

           
        }

        
        return targetFriends;
    }

    public List<Vector3Int> GetFriendsCordsByRange(int range) //soll nur die koordinaten zurückgeben, egal ob da jemand sitzt oder nicht
    {
        List<Vector3Int> visitedFriends = new List<Vector3Int>();
        List<Vector3Int> targetFriends = new List<Vector3Int>();
        List<Vector3Int> targetFriendsBuffer = new List<Vector3Int>();
        
        targetFriends.Add(this.cords);
        visitedFriends.Add(this.cords);

        for(int i=1; i<=range; i++)
        {
            // jedes tile in targetFriends findet seine nachbarn und trägt die in targetfriendsBuffer ein
            foreach (Vector3Int tile in targetFriends) 
            {
                List<Vector3Int> friendsCords = new List<Vector3Int>();
                foreach(Vector3Int spot in positionsArray)
                {
                    friendsCords.Add(tile+spot);
                }
                targetFriendsBuffer.AddRange(friendsCords); // beautiful. To get the positions of surrounding tiles, we could add the positions vector to eachof them,but we can also find the tile at that position and then ask it what the positions of its freinds are.because it knows, by doing that one very simple calculation
                
            }

            // targetFriends liste wird gewiped, damit am ende wirklich nur die Äußersten tiles Drinstehen
            targetFriends.Clear();

            // sofern der eintrag aus dem Buffer nicht schon in visitedfriends steht, oder schon in target friends eingetragen wurde, wird er in targetFriends Übertragen
            foreach(Vector3Int tile in targetFriendsBuffer)
            {
                if (!visitedFriends.Contains(tile)&&!targetFriends.Contains(tile)) { targetFriends.Add(tile); }
            }

            // dann werden alle TargetFriends als visited abgespeichert
            visitedFriends.AddRange(targetFriends);

           
        }

        
        return targetFriends;
    }

   
    //public List<Vector3Int> GetFriendsCords(int range) {return friendsCordsListArray[range];}

    //der aufrufer sagt allen tiles in range callRange bescheid, dass er in range callRange von ihnen ist
    public void SayHelloToFriends(int callRange) 
    {
        // es werden für jede range bis einschl. call range alle tiles dort in eine liste gepackt und dann wird für alle tiles aus der liste die anmelden funktion aufgerufen
        for(int range = callRange; range>0 ; range--)
        {
            List<MapTile> calledTiles = new List<MapTile>();
            calledTiles.AddRange(GetFriendsByRange(range));
            foreach(MapTile tile in calledTiles) 
            { 
                tile.Anmelden(range); 
            }
        }
        
    }
   

   

    public bool HasCords(){if(cords==null){return false;}return true;}

    public override void MouseEnter(Vector3Int calledCords,List<Vector3Int> calledShape)
    {   
        int mouseMode = EventManager.instance.GetMouseMode();
        // wenn die eigenen koordinaten auf die shape des mouse cursors passen, matrix behavior
        if(calledShape.Contains(this.cords)&&EventManager.instance.IsMouseModeEqualTo(c.placeTile))
        {
            mouseOver = true; // mouseOver "vorschau"

            // schaut bei allen tiles der shape nach, ob eins davon nicht overridable ist. falls eins gefunden wird, wird sich das gemerkt.
            if(TileLedger.ledgerInstance.DoTheyAllExist(calledShape)==false)
            {
                this.mouseOverValidity = c.outOfBounds;
            } 

            if(TileLedger.ledgerInstance.CanTheyAllBeOverridden(calledShape)==false) 
            {
                //Debug.Log("mouseOverError: collsion with other particle");
                mouseOverValidity = c.noSpace;
            }

            if(TileLedger.ledgerInstance.AreNoneOfThemEdgy(calledShape) == false)
            {
                mouseOverValidity = c.edgy;
            }
            
 
        }

        // wenn die "mitte" des mousecursors auf ein member der eigenen shape zeigt, particle behaviour
        if(associatedShape.Contains(calledCords))
        {
            //Debug.Log(string.Format( "map tile mouse enter at {0}",this.cords));
            mouseOver = true;
            if(EventManager.instance.IsMouseModeEqualTo(c.inspect))
            {
                InfoDisplay.instance.UpdateInfoDisplay(this);
            }
        }

    }

    public override void MouseExit(Vector3Int calledCords, List<Vector3Int> shape)
    {
        
        if(shape.Contains(this.cords))
        {
            mouseOver = false;
            mouseOverValidity = 0;
        }

        //currentDisplayedColor = currentRestingColor; 
        if(associatedShape.Contains(calledCords))
        {
        mouseOver = false;
        //Debug.Log("mapTile mouseExit");
        }

    }
    public override void MouseInteractionRight(Vector3Int cords)
    {
        //SetBaseStressState(c.orf);
        //this.StartStretching(c.rf*(1/Mathf.Sqrt(2)) ,new Vector3(-2.5f,-2f,4.5f));
        if(cords!=this.cords) return;
        int mouseMode = EventManager.instance.GetMouseMode();
        if(mouseMode==c.inspect)
        {
            
            Debug.Log("inspect right click");
            foreach(Vector3Int position in associatedShape)
            {
                Debug.Log(position);
            }
    
        }
        if(mouseMode==c.placeTile) //mode:place type:leftclick
        {

            MapBuilder.instance.PlaceTileShape(cords);
            
        }

        
        
    }
    public override void MouseInteractionLeft(Vector3Int cords)
    {
        
        if(!associatedShape.Contains(cords)) return;
        int mouseMode = EventManager.instance.GetMouseMode();

        // plaziertung eines matrix tiles= radiergummi tool, löscht also das tile und die associated shape
        if(mouseMode==c.clearTile)
        {
            if(isErasable==false) return;
            MapBuilder.instance.SetTileType(c.matrixTile);
            MapBuilder.instance.PlaceTile(this.cords,false,false,true);
            
            Debug.Log("attempted to erase");
            return;
        }
        

        if(mouseMode==c.inspect)
        {
            Debug.Log(cords);
        }

        if(mouseMode==c.placeTile) //mode:place type:leftclick
        {

            //MapBuilder.instance.PlaceTile(cords,mouseMode[c.modeBranch],true);
            
            MapBuilder.instance.PlaceTileShape(cords);

        }
        
    }

    public Vector3Int SelfRemoveTile()
    {
        if(!isMortal) return cords;
        Vector3Int emptySpace = this.cords;
        TileLedger.ledgerInstance.RemoveTileFromLedger(this);
        //Debug.Log(emptySpace);

        // // replaced alle tils des shapes mit matrix tiles, damit die ganze shape gelöscht wird
        // List<Vector3Int> suicideSquad = associatedShape;
        // //Debug.Log(associatedShape);
        // foreach(Vector3Int spot in suicideSquad)
        // {
        // MapBuilder.instance.PlaceTile(spot,c.matrixTile,false,false,true);
        // //tile.SelfRemoveTile();
        // //Debug.Log(tile.cords);
        // }
       
        Destroy(this.gameObject);
        return emptySpace;
    }

    public virtual void Anmelden(int callRange)
    {
        // wird in child klasse verwendet (glaube ich)
    }

    public virtual void Reset()
    {
        currentRestingColor=baseColor;
        currentDisplayedColor=baseColor;
        isCracked = false;
    }
    public virtual void Crack()
    {
        //this.GetComponent<MeshRenderer>().material.color = Color.black;
        isCracked = true;
        Debug.Log("crack");
        // currentRestingColor *=0.5f;
        // currentRestingColor.a =1;
        // UpdateDisplayColor();
    }

    protected bool AddBehavior(int key, float strength)
    {
        if(myBehaviors.ContainsKey(key))
        {
            Debug.Log("tag existiert schon");
            return false;
        }

        myBehaviors.Add(key,strength);
        return true;
    }

    public bool ModifyBehavior(int key, float strenght)
    {
        if(!myBehaviors.ContainsKey(key)) 
        {
            Debug.Log("ein nicht vorhandenes behavior kann nicht modifiziert werden");
            return false;
        }

        myBehaviors.Remove(key);
        myBehaviors.Add(key, strenght);
        return true;

    }

    protected bool RemoveBehavior(int key)
    {
        if(!myBehaviors.ContainsKey(key))
        {
            Debug.Log("this behavior doesnt exist so it cant be removed. duh.");
            return false;
        }

        myBehaviors.Remove(key);
        return true;
    }
    
    internal bool HasBehavior(int behaviorTag)
    {
        if(myBehaviors.ContainsKey(behaviorTag)) return true;
        return false;
    }

    public bool IsThisMortal()
    {
        if (isMortal == false) { return false; }
        return true;
    }

    public bool CanThisBeOverridden(int whatTypeWantsToGoHere=1, bool isShape=false)
    {
        if(whatTypeWantsToGoHere==c.matrixTile) return true; // matrix tile darf überall hin
        if(this.typeKey!=c.matrixTile) return false; // niemand darf auf nicht-matrix tiles

        List<MapTile> friends = TileLedger.ledgerInstance.GetTileByCords(friendsCords); // wenn ein matrix tile ein nicht matrix tile als nachbarn hat, darf man da nicht drauf
        foreach(MapTile tile in friends)
        {
            if(tile.typeKey!=c.matrixTile) return false;
        }

        return true;
    }


   internal float GetBehaviourByTag(int behaviorTag)
   {
        if(!myBehaviors.ContainsKey(behaviorTag))
        {
            Debug.Log("Verhalten nicht definiert");
            return 0;
        }

        return myBehaviors[behaviorTag];
    }

   internal void UpdateDisplayColor()
    {
        currentDisplayedColor = currentRestingColor;
    }

    public float GetBaseToughness()
    {
        return baseToughness;
    }

    // public virtual float GetInterfaceStrengh()
    // {
    //     return interfaceStrenghBase;
    // }
    [SerializeField] bool Edge;
    public bool isEdge()
    {
        bool result = false;

        List<MapTile> friendsList = TileLedger.ledgerInstance.GetTileByCords(friendsCords);
        if(friendsList.Count<6) result = true;
        Edge = result;
        return result;
    }

    
}
