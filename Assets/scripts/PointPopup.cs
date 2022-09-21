using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointPopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private static Vector3 upward = new Vector3(1f,-2f,1f);
    private Color fadeColor;
    private float fadeSpeed = 3;

    private float fadeDelay = 0.5f;

    private float stepsize = 2f;

    private void Update() 
    {
        transform.position+=upward*stepsize*Time.deltaTime;
        stepsize*=0.5f;

        fadeDelay-=Time.deltaTime;
        if(fadeDelay<0)
        {
            fadeColor.a -= Time.deltaTime*fadeSpeed;
            if(fadeColor.a<0) Destroy(this.gameObject);
            textMesh.color = fadeColor;
        }
    }

    public static PointPopup Create(Vector3 tilePosition, float points, float scale=1, float duration=1)
    {
        // offsets cords so its not inside the tile
        Vector3 offset = new Vector3(1f,1f,1f);
        Vector3 position = tilePosition+offset;

        // points processing
        float cleanedPoints=Mathf.Round(points*10);
    	
        // instantiate Text, get a handle on the script 
        GameObject pointPopupObject = Instantiate(GameAssets.instance.pointPopup,position,Quaternion.Euler(45,-135,180));
        PointPopup pointInstance = pointPopupObject.GetComponent<PointPopup>();

        // set the Value that should be displayed
        pointInstance.SetupPoints(cleanedPoints);

        // scale the text
        pointInstance.transform.localScale *=scale;

        // set the duration before fading, in multiples of 0.5 seconds
        pointInstance.fadeDelay *= duration;
        return pointInstance;
    }

    public static PointPopup Create(Vector3 tilePosition, int price)
    {
        // offsets cords so its not inside the tile
        Vector3 offset = new Vector3(1f,1f,1f);
        Vector3 position = tilePosition+offset;

         // instantiate Text, get a handle on the script 
        GameObject pointPopupObject = Instantiate(GameAssets.instance.pointPopup,position,Quaternion.Euler(45,-135,180));
        PointPopup pointInstance = pointPopupObject.GetComponent<PointPopup>();

         // set the Value that should be displayed
        pointInstance.SetupPrice(price);
        return pointInstance;

        
    }

    private void Awake() 
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        fadeColor = textMesh.color;
    }
   public void SetupPoints(float points)
   {
       textMesh.SetText(points.ToString());
   }

   public void SetupPrice(int balance)
   {
       string message = balance.ToString();
       message+="$$";
       textMesh.SetText(message);
       if(balance<0) textMesh.color = Color.red;
       if(balance>=0) textMesh.color = Color.green;
   }

}
