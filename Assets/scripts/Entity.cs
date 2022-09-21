using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
   public Vector3Int cords {get;set;}
    private void Start() 
    {
        // dont forget to like and subscribe to the MouseEvents!
        EventManager.instance.MouseExit+=MouseExit;
        EventManager.instance.MouseEnter+=MouseEnter;
        EventManager.instance.MouseClickRight+=MouseInteractionRight;
        EventManager.instance.MouseClickLeft+=MouseInteractionLeft;
    }
    private void OnDisable() 
    {
        // unsubscribe from the mouseEvents
        EventManager.instance.MouseEnter-=MouseEnter;
        EventManager.instance.MouseExit-=MouseExit;
        EventManager.instance.MouseClickRight-=MouseInteractionRight;
        EventManager.instance.MouseClickLeft-=MouseInteractionLeft;
        
    }
    public virtual void MouseEnter(Vector3Int cords, List<Vector3Int> shape)
    {
        if(this.cords!=cords) return;
        Debug.Log("base entity mouse enter");
    }
    
    public virtual void MouseExit(Vector3Int cords, List<Vector3Int> shape)
    {
        if(this.cords!=cords) return;
        Debug.Log("base entitiy mouse exit");
    }

    // public virtual void MouseInteraction(Vector2Int mouseMode, int interactionType)
    // {
    //     Debug.Log("base entity mouse interaction");
    // }
    public virtual void MouseInteractionRight(Vector3Int cords)
    {
        Debug.Log("base entity mouse interaction");
    }
      public virtual void MouseInteractionLeft(Vector3Int cords)
    {
        Debug.Log("base entity mouse interaction");
    }
}
