using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
public static GameAssets instance;
   private void Awake() 
   {
        // MAKE Mapbuilder an INSTANCE
        if      (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        // DONT DESTROY ON SCENE CHANGE
        DontDestroyOnLoad(this.gameObject);
   }

   public GameObject pointPopup;
   public GameObject MatrixTile;
}
