using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasicInventoryUI : InventoryUI
{
    [SerializeField] protected ThoughtImage[] thoughImages;
    ///[SerializeField] private ThoughtImage[] removeOneThoughtImage;
    [SerializeField] private GameObject uiObj;
    //[SerializeField] private GameObject removeThoughtUI;
    [SerializeField] private GameObject thoughtDescBoxObj;
    [SerializeField] private TMP_Text thoughtNameText;
    [SerializeField] private TMP_Text thoughtDescText;
    private Thought curShowingThought = null;

    public override void Init()
    {
        foreach (ThoughtImage t in thoughImages)
        {
            t.Set(this, null);
        }
    }

    public override void SetKnowledgeSlot(int index, Knowledge idea)
    {
        throw new System.NotImplementedException();
    }

    public override void SetMemorySlot(int index, Thought idea)
    {
        thoughImages[index].Set(this, idea);
    }

    public override void ShowHide(bool show)
    {
        uiObj.SetActive(show);
    }

    public override void ShowThought(Thought e)
    {
        curShowingThought = e;
        thoughtDescBoxObj.SetActive(true);
        thoughtNameText.text = e.GetName();
        thoughtDescText.text = e.GetDescription();
    }

    public override void HideThought(Thought e)
    {
        if (curShowingThought == e)
        {
            thoughtDescBoxObj.SetActive(false);
            curShowingThought = null;
        }
    }
}