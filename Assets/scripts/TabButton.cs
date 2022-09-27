using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image background;
    public Color backgroudTabIdle;
    public GameObject page;

    private void Start() 
    {
        background = GetComponent<Image>();

        backgroudTabIdle = background.color;

        tabGroup.Subscribe(this);
    }

    public void hidePage()
    {
        page.SetActive(false);
    }

    public void showPage()
    {
        page.SetActive(true);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }
}
