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

    public static List<Vector3Int> positionsArrayHalb = new List<Vector3Int>()
    {
      new Vector3Int(1,-2,1),  new Vector3Int(2,-1,-1), new Vector3Int(1,1,-2),
      new Vector3Int(-1,2,-1), new Vector3Int(-2,1,1), new Vector3Int(-1,-1,2)
    };
    // richtungsvektoren
    public static Vector3Int or = new Vector3Int(1,-1,0);
    public static Vector3Int r = new Vector3Int(1,0,-1);
    public static Vector3Int ur = new Vector3Int(0,1,-1);
    public static Vector3Int ul = new Vector3Int(-1,1,0);
    public static Vector3Int l = new Vector3Int(-1,0,1);
    public static Vector3Int ol = new Vector3Int(0,-1,1);
    public static Vector3Int origin = new Vector3Int(0,0,0);

    // richtungsvektoren float
    public static Vector3 orf = new Vector3(1f,-1f,0f);
    public static Vector3 rf = new Vector3(1f,0f,-1f);
    public static Vector3 urf = new Vector3(0f,1f,-1f);
    public static Vector3 ulf = new Vector3(-1f,1f,0f);
    public static Vector3 lf = new Vector3(-1f,0f,1f);
    public static Vector3 olf = new Vector3(0f,-1f,1f);
    public static Vector3 originf = new Vector3(0f,0f,0f);

    // richtunsvektoren enum, alle 12
    public enum richtungsvektoren
    {
      N,
      NNE,
      ENE,
      E,
      ESE,
      SSE,
      S,
      SSW,
      WSW,
      W,
      WNW,
      NNW,
      none


    }

    // einträge für die drehmatrix 90°
    public static float a = 1/3 - 1/Mathf.Sqrt(3);
    public static float b = 1/3 + 1/Mathf.Sqrt(3);
    public static float t = 1/3;
    

    //tile arten
    public const int matrixTile = 0;
    public const int particleTile1 = 1;
    public const int maxPhase = 2;
    public const int PhaseChangeTile = 3;

    public enum types
    {
      MatrixTile,
      ParticleTile,
      MaxPhase,
      PhaseChangeTile

    }
  

    // crack modes
    public enum CrackMode
    {
        Point,
        Direction
    }

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
    public const int nugget = 2;
    public const int single =3;
    public const int empty = 4;

    public enum shapes
    {
      Blob,
      Fiber,
      Nugget,
      Single,
      Empty
    }

    public enum mapSize
    {
      tiny,
      small,
      medium,
      large,
      huge,
      hexagonal,
      hexagonalLarge,
      custom
    }

    // mouseOverValiditys arten
    public const int valid = 0;
    public const int noSpace = 1;
    public const int outOfBounds = 2;
    public const int edgy = 3;

    // behaviors
    public const int grenzflaeche = 1;
    public const int maxPhaseField = 2;

    //Siehe tiles

    // tag to int

    // static Dictionary<string,int> shapeTagDict = new Dictionary<string,int>()
    // {
    //   {"ShapeFiber",c.fiber},
    //   {"ShapeNugget",c.nugget},
    //   {"ShapeBlob",c.blob},
    //   {"ShapeSingle",c.single}
    // };
    
    // public static int GetShapeByString(string shape)
    // {
    //   if(shapeTagDict.ContainsKey(shape))
    //   {
    //     return shapeTagDict[shape];
    //   }
    //   Debug.Log("shape nicht definiert:c.tagDict");
    //   return 0;
    // }

    // public static int GetShapeByEnum(int shape) ??

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

// serialization

public const int xCord = 0;
public const int yCord = 1;
public const int zCord = 2;
public const int cost  = 3;
public const int baseToughness  = 4;
public const int interfaceStrength  = 5;
public const int stressFieldStrength  = 6;
public const int star  = 7;
public const int minRange  = 8;
public const int isMaxPhase  = 9;
public const int type  = 10;


  // misc constants
  public const float stressRangeToStrengthRatio = 2.5f;

}
