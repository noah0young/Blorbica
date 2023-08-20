using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicInventoryUI : InventoryUI
{
    [SerializeField] private Image[] normalThoughtImage;
    [SerializeField] private Image[] removeOneThoughtImage;
    [SerializeField] private GameObject normalUI;
    [SerializeField] private GameObject removeThoughtUI;

    public override void SetKnowledgeSlot(int index, Knowledge idea)
    {
        throw new System.NotImplementedException();
    }

    public override void SetMemorySlot(int index, Thought idea)
    {
        throw new System.NotImplementedException();
    }

    public override void ShowHide(bool show)
    {
        normalUI.SetActive(show);
    }
}
