using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveOneInventoryUI : BasicInventoryUI
{
    protected Button[] thoughtButtons;
    [SerializeField] private Button extraThoughtButton;
    public delegate void ReplaceEvidenceMethod(int index, Thought newThought);
    private ReplaceEvidenceMethod replaceMethod;
    private Thought extraThought;

    private void Start()
    {
        thoughtButtons = new Button[thoughImages.Length];
        for (int i = 0; i < thoughImages.Length; i++)
        {
            int memoryIndex = i;
            thoughtButtons[i] = thoughImages[i].GetComponent<Button>();
            thoughtButtons[i].onClick.AddListener(() => RemoveEvidence(memoryIndex));
        }
        extraThoughtButton.onClick.AddListener(() => RemoveEvidence(-1));
    }

    protected void RemoveEvidence(int index)
    {
        replaceMethod(index, extraThought);
    }

    public void SetReplaceMethod(ReplaceEvidenceMethod method)
    {
        replaceMethod = method;
    }

    public void SetExtraThought(Thought extraThought)
    {
        this.extraThought = extraThought;
        extraThoughtButton.GetComponent<ThoughtImage>().Set(this, extraThought);
    }
}
