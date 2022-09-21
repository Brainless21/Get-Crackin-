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
    protected int baseToughness = 1;
    [SerializeField] public bool isMortal = true;
    bool isCracked = false;
    bool mouseOver = false;
    [SerializeField] private List<Vector3Int> associatedShape = new List<Vector3Int>();
    int mouseOverValidity;


    public void SetAssociatedShape(List<Vector3Int> shape)
    {  
        associatedShape = shape;   
    }
    public void SetAssociatedShape(Vector3Int shape)
    {
        List<Vector3Int> singleShape = new List<Vector3Int>() {shape};
        associatedShape = singleShape;
    }

    protected Dictionary<int, float> myBehaviors = new Dictionary<int, float>(); //"what behaviors do I (possibly, if they care about that) exert on my fellow neighbor tiles?"

    internal Dictionary<int,Color> colorList = new Dictionary<int, Color>()
    {
        {0, Color.white},
        {1, Color.yellow},
        {2, Color.red},
        {3, Color.yellow}
    };
    // private void Start() 
    // {
    //     EventManager.instance.MouseExit+=
    // }
    private void Update() 
    {
        //if(GetComponent<MeshRenderer>().material.color!=currentDisplayedColor) GetComponent<MeshRenderer>().material.color = currentDisplayedColor; 
        //currentDisplayedColor = currentRestingColor;
        Render();
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
        //Debug.Log("bin eingeschrieben(initialize)");

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


        if(isCracked==true) 
        {
            finalColor *=0.5f;
            finalColor.a =1;
        }

        GetComponent<MeshRenderer>().material.color = finalColor;
    }
    public virtual float GetToughness()
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
    public void SetBaseToughness(int value){baseToughness = value;}



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
                Debug.Log("mouseOverError: collsion with other particle");
                mouseOverValidity = c.noSpace;
            }
            
 
        }

        // wenn die "mitte" des mousecursors auf ein member der eigenen shape zeigt, particle behaviour
        if(associatedShape.Contains(calledCords))
        {
            mouseOver = true;
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
}
