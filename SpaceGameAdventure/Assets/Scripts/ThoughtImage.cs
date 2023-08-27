using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThoughtImage : Image, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryUI ui;
    private Thought evidence;

    public void Set(InventoryUI ui, Thought evidence)
    {
        this.ui = ui;
        this.evidence = evidence;
        if (evidence != null)
        {
            this.sprite = evidence.GetImage();
        }
        else
        {
            this.sprite = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (evidence != null)
        {
            ui.ShowThought(evidence);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (evidence != null)
        {
            ui.HideThought(evidence);
        }
    }

    public Thought GetEvidence()
    {
        return evidence;
    }
}
