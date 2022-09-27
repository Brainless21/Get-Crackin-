﻿using System.Collections;
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
        if(button!=selectedTab) button.background.color = tabHovered;
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
    }

    void ResetTabs()
    {
        foreach(TabButton tab in tabButtons)
        {
            if(tab!=selectedTab) tab.background.color = tab.backgroudTabIdle;
        }
    }

    void SwitchPageToSelected()
    {
        foreach(TabButton tabButton in tabButtons)
        {
            tabButton.hidePage();
            if(tabButton==selectedTab) tabButton.showPage();
        }

    }


}
