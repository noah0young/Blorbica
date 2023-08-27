using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresentEvidenceUI : BasicInventoryUI
{
    public void ResetPresentUI()
    {
        Button[] thoughtButtons = new Button[thoughImages.Length];
        for (int i = 0; i < thoughImages.Length; i++)
        {
            int memoryIndex = i;
            thoughtButtons[i] = thoughImages[i].GetComponent<Button>();
            thoughtButtons[i].onClick.RemoveAllListeners();
            thoughtButtons[i].onClick.AddListener(() => NPCTextUI.instance.ChooseEvidence(thoughImages[memoryIndex].GetEvidence()));
        }
    }
}
