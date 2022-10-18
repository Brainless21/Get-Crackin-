using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    Vector3Int cords = new Vector3Int();
    MapTile occupiedTile;
    [SerializeField] Mesh lookOfDestiny;
    bool isActive = true;
    void Start()
    {
        cords = Utilities.ConvertCordsToInt(this.gameObject.transform.position);
        occupiedTile = TileLedger.ledgerInstance.GetTileByCords(cords);
        occupiedTile.gameObject.GetComponent<MeshFilter>().mesh = lookOfDestiny;
    }

    public void SetActive()
    {
    isActive = true;
    } 
    public void SetInactive()
    {
        isActive = false;
    }

    




    
}
