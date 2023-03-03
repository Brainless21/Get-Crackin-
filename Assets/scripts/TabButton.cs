using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public ResetGroup resetGroup;
    public Image background;
    public Color backgroudTabIdle;
    public GameObject page;
    public bool sharesSpace;
    public bool isBackbutton;

    private void Start() 
    {
        background = GetComponent<Image>();

        backgroudTabIdle = background.color;

        tabGroup.Subscribe(this);

        // trägt sich (button) und die zugehörige page beim resetbutton ein, wenn er nicht selber der reset button ist
        if(!isBackbutton)
        {
            resetGroup.Subscribe(this.gameObject);
            resetGroup.Subscribe(page);
        }

    }

    public void HidePage()
    {
        page.SetActive(false);
    }

    public void HideSelf()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowPage()
    {
        page.SetActive(true);

    }

    public void ShowSelf()
    {
        this.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
        InfoDisplay.instance.UpdateInfoDisplay(Utilities.GetMenuTilesInAllChildren(page));
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
