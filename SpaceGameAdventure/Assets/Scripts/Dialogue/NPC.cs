using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string id;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void OnClick()
    {
        Dialogue d = StoryManager.GetDialogueFor(id);
        d.Reset();
        NPCTextUI.instance.StartDialogue(d);
    }
}
