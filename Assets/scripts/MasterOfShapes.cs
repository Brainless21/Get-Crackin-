using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterOfShapes : MonoBehaviour
{
    public static MasterOfShapes instance;
    private void Awake() 
    {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

    }

    private List<List<Vector3Int>> shapeListList = new List<List<Vector3Int>>();
    private List<Vector3Int> spotsInShapelist;

    public List<Vector3Int> FindAssociatedShapes(Vector3Int origin)
    {
        List<Vector3Int> results = new List<Vector3Int>();

        foreach(List<Vector3Int>shape in shapeListList)
        {
            if(shape.Contains(origin)) return shape;
        }
        //Debug.Log("dieses tile hatte keine shape-freunde");
        return results;
    }

    public bool AddShapeToList(List<Vector3Int> shape)
    {
        foreach(Vector3Int spot in shape)
        {
            if(FindAssociatedShapes(spot)!=null)
            {
                //Debug.Log("ein spot der shape steht schon in einem anderen shape drin.");
            }
        }

        shapeListList.Add(shape);
        return true;
    }
}
