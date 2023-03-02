using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHovered;
    public Color tabSelected;
    public TabButton selectedTab;
    public string description;
    // [SerializeField] TMPro infodisplay;
    //[SerializeField] bool sharesSpace;

    // private void Start()
    // {
    //     tabIdle = GetComponent<Image>().color;
    // }
    public void Subscribe(TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        // if(button!=selectedTab) 
        button.background.color = tabHovered;

    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        button.background.color = tabSelected;
        selectedTab = button;
        ResetTabs();
        SwitchPageToSelected();

        if(button.isBackbutton) button.resetGroup.Reset();
    }

    void ResetTabs()
    {
        foreach(TabButton tab in tabButtons)
        {
            // if(tab!=selectedTab) 
            tab.background.color = tab.backgroudTabIdle;
        }
    }

    // public void HideAllPagesFromButtonsInThisGroup()
    // {
    //     foreach(TabButton tabButton in tabButtons)
    //     {
    //         tabButton.HidePage();
    //     }
    // }

    void SwitchPageToSelected()
    {
        foreach(TabButton tabButton in tabButtons)
        {
            // // this is so when there is a hierarchy of tabGroups with buttons inside the page of a button in a tab group, the "children" also get turned off, when the page gets disabled
            // TabGroup lowerLevel = tabButton.gameObject.GetComponent<TabGroup>();
            // if(lowerLevel!=null) lowerLevel.HideAllPagesFromButtonsInThisGroup();

            tabButton.HidePage(); // this just hides the pages of all buttons in the group
            if(tabButton.sharesSpace==true) tabButton.HideSelf();
            if(tabButton==selectedTab) tabButton.ShowPage();
        }

    }
    // private void OnEnable() 
    // {
    //     //when the scrip gets turned on, it checks if the gameObject is also turned on, and then shows all tabbuttons in there
    //     if(gameObject.activeSelf)
    //     {
    //         foreach(TabButton tabButton in tabButtons)
    //         {
    //             if(tabButton.sharesSpace==true) tabButton.ShowSelf();
    //         }
    //     }
    // }


}
