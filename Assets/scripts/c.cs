using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class c
{
    // positionsArray
    public static List<Vector3Int> positionsArray = new List<Vector3Int>()
      { 
        new Vector3Int(1,-1,0), new Vector3Int(1,0,-1), new Vector3Int(0,1,-1), //ein array mitden richtungsvektoren, durchgezaehlt von oben rechts, im uhrzeigersinn
        new Vector3Int(-1,1,0), new Vector3Int(-1,0,1), new Vector3Int(0,-1,1) 
    };
    // richtungsvektoren
    public static Vector3Int or = new Vector3Int(1,-1,0);
    public static Vector3Int r = new Vector3Int(1,0,-1);
    public static Vector3Int ur = new Vector3Int(0,1,-1);
    public static Vector3Int ul = new Vector3Int(-1,1,0);
    public static Vector3Int l = new Vector3Int(-1,0,1);
    public static Vector3Int ol = new Vector3Int(0,-1,1);
    

    //tile arten
    public const int matrixTile = 0;
    public const int particleTile1 = 1;
    public const int maxPhase = 2;

    //maus buttons
    public const int leftClick = 0;
    public const int rightClick = 1;
    public const int middleMouse = 2;

    //maus modes
    public const int modeType = 0;
    //types:
    public const int inspect = 0;
    public const int placeTile = 1;
    public const int clearTile = 2;
    public const int dragDrop = 3;
    public const int rotate = 4;
    //branches:
    // .x: types: placeTile, InspectTile etc.
    // .y: tile Type: matrix, particle, maxphase, etc. currently just defined for PlaceTile
    // .z: tile Shape: blob, nugget, fiber etc. currently just defined for placetile
    public const int modeBranch = 1;
    public const int tileTypeBranch = 1;
    public const int tileShapeBranch = 2;

    // shapes
    public const int blob = 0;
    public const int fiber = 1;
    public const int single =3;
    public const int nugget = 2;
    public const int empty = 4;

    // mouseOverValiditys arten
    public const int valid = 0;
    public const int noSpace = 1;
    public const int outOfBounds = 2;

    // behaviors
    public const int grenzflaeche = 1;
    public const int maxPhaseField = 2;

    //Siehe tiles

    // tag to int

    static Dictionary<string,int> shapeTagDict = new Dictionary<string,int>()
    {
      {"ShapeFiber",c.fiber},
      {"ShapeNugget",c.nugget},
      {"ShapeBlob",c.blob},
      {"ShapeSingle",c.single}
    };
    
    public static int GetShapeByString(string shape)
    {
      if(shapeTagDict.ContainsKey(shape))
      {
        return shapeTagDict[shape];
      }
      Debug.Log("shape nicht definiert:c.tagDict");
      return 0;
    }

    static Dictionary<string,int> typeTagDict = new Dictionary<string, int>()
    {
      {"MatrixTile",c.matrixTile},
      {"ParticleTile1",c.particleTile1},
      {"MaxPhase",c.maxPhase}
    };

    public static int getTypeByString(string type)
    {
      if(typeTagDict.ContainsKey(type))
      {
        return typeTagDict[type];
      }
      Debug.Log("tileType nicht definiert; c.typetagdict");
      return 0;
    }

}
