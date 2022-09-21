using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FundsAccount : MonoBehaviour
{
    [SerializeField] TextMeshPro textMesh;

    //private TextMeshPro textMesh;
    [SerializeField] private int funds = 700;
    private int balance;
    public static FundsAccount instance;
    private Dictionary<int,int> priceList = new Dictionary<int, int>();
    public int GetBalance()
    {
        return balance;
    }

    public int GetPriceByType(int type)
    {
        if(!priceList.ContainsKey(type))
        {
            Debug.Log("item steht nicht in der Preisliste");
            return 0;
        }

        return priceList[type] ;
    }

    void Awake()
    {
        // MAKE ledgerINSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        //textMesh = GetComponent<TextMeshPro>();

        priceList.Add(c.matrixTile,0);
        priceList.Add(c.particleTile1,3);
        priceList.Add(c.maxPhase,5);
        balance = funds;
        
    }

    public void UpdateDisplay()
    {
        int currentExpenses = TileLedger.ledgerInstance.CountCost();
        balance = funds-currentExpenses;
        string displaytext = balance.ToString();
        displaytext += "$$";
        textMesh.SetText(displaytext);
        
    }
   
}
