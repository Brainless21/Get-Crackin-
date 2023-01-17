using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Destination : MonoBehaviour
{
    public Vector3Int cords = new Vector3Int();
    MapTile occupiedTile;
    [SerializeField] Mesh lookOfDestiny;
    bool isActive = true;
    [SerializeField] Crack crack;
    void Start()
    {
        // cords = Utilities.ConvertCordsToInt(this.gameObject.transform.position);
        
        
    }

    public Destination(Vector3Int myCords)
    {
        cords = myCords;
    }

    public void FindAndMarkTile()
    {
        occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);
        occupiedTile.gameObject.GetComponent<MeshFilter>().mesh = lookOfDestiny;

    }

    public void MakeActive()
    {
        isActive = true;
    } 
    public void SetInactive()
    {
        isActive = false;
    }

    private void OnDisable() 
    {
        isActive = false;

    }




    
}
