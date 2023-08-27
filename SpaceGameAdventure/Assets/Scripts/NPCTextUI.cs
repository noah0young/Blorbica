using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TextTyper))]
public class NPCTextUI : MonoBehaviour
{
    public static NPCTextUI instance { get; private set; }

    [SerializeField] private GameObject preventSceneClicksCanvas;
    [Header("State")]
    [SerializeField] private GameObject talkingUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject presentingUI;
    public enum TextUIState
    {
        TALKING, OPTIONS, PRESENT_EVIDENCE, SCENE_TRANSITION
    }
    private TextUIState state;

    private Dialogue dialogue;

    private TextTyper typer;

    [Header("Talking")]
    [SerializeField] private TMP_Text talkingNameText;
    [SerializeField] private TMP_Text talkingText;
    [SerializeField] private GameObject talkingNameBox;

    [Header("Options")]
    [SerializeField] private TMP_Text optionsText;
    [SerializeField] private GameObject optionsHolder;
    [SerializeField] private GameObject optionPrefab;
    private bool optionPressed;
    private List<GameObject> curOptionObjects;

    [Header("Presenting")]
    [SerializeField] private TMP_Text presentingText;

    [Header("NPC Sprite")]
    [SerializeField] private GameObject npcSpriteHolder;
    [SerializeField] private GameObject npcSpritePreFab;
    private GameObject[] curNPCSprites;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Hide();
        typer = GetComponent<TextTyper>();
    }

    private void CreateSprites(Sprite[] sprites)
    {
        RemoveAllSprites();
        npcSpriteHolder.SetActive(true);
        curNPCSprites = new GameObject[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject npcSpriteObj = Instantiate(npcSpritePreFab, npcSpriteHolder.transform);
            npcSpritePreFab.GetComponent<Image>().sprite = sprites[i];
            curNPCSprites[i] = npcSpriteObj;
        }
    }

    private void RemoveAllSprites()
    {
        npcSpriteHolder.SetActive(false);
        if (curNPCSprites != null)
        {
            foreach (GameObject spriteObj in curNPCSprites)
            {
                Destroy(spriteObj);
            }
            curNPCSprites = null;
        }
    }

    public void StartDialogue(Dialogue d)
    {
        this.dialogue = d;
        preventSceneClicksCanvas.SetActive(true);
        StartCoroutine(RunDialogue());
    }

    private IEnumerator RunDialogue()
    {
        while (dialogue.HasNext())
        {
            SetState(dialogue.NextTextState());
            Message message = dialogue.Next();
            SetNPC(message.GetNpcID());
            SetNPCImages(message.GetNPCsImageID());
            switch (state)
            {
                case TextUIState.TALKING:
                    talkingText.text = "";
                    typer.TypeText(talkingText, message.GetText());
                    //talkingText.text = message.GetText(); // should become its own object for visualizing text
                    // So that text can be typed out a letter at a time
                    SetName(message.GetName());
                    break;
                case TextUIState.OPTIONS:
                    optionsText.text = message.GetText();
                    MakeOptions();
                    break;
                case TextUIState.PRESENT_EVIDENCE:
                    presentingText.text = message.GetText();
                    GameManager.SetPresentingUI();
                    break;
            }
            if (state == TextUIState.SCENE_TRANSITION)
            {
                break;
            }
            yield return new WaitUntil(() => optionPressed && typer.NotTyping());
            DestroyOptionObjects();
            optionPressed = false;
            if (!dialogue.HasNext())
            {
                dialogue = dialogue.GetNextDialogue();
            }
        }
        Hide();
        RemoveAllSprites();
    }

    private void SetNPC(string id)
    {
        NPCData npc = ScriptParser.GetNPCData(id);
        typer.SetSound(NPCAssetManager.instance.GetSound(npc.talkSoundID));
        typer.SetTextColor(NPCAssetManager.instance.GetColor(npc.textColorID));
        typer.SetFontSize(npc.defaultFontSize);
        typer.SetFont(NPCAssetManager.instance.GetFont(npc.fontID));
    }

    private void SetNPCImages(List<string> ids)
    {
        Sprite[] sprites = new Sprite[ids.Count];
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = NPCAssetManager.instance.GetSprite(ids[i]);
        }
        CreateSprites(sprites);
    }

    public void SetName(string name)
    {
        if (name != null)
        {
            talkingNameText.text = name;
            talkingNameBox.SetActive(true);
        }
        else
        {
            talkingNameBox.SetActive(false);
        }
    }

    public void ContinueDialogue()
    {
        if (typer.NotTyping())
        {
            optionPressed = true;
        }
        else
        {
            typer.ForceTypeAll();
        }
    }

    private void DestroyOptionObjects()
    {
        if (curOptionObjects != null)
        {
            for (int i = curOptionObjects.Count - 1; i >= 0; i--)
            {
                Destroy(curOptionObjects[i]);
            }
            curOptionObjects = null;
        }
    }

    private void MakeOptions()
    {
        List<string> options = dialogue.GetPathNames();
        DestroyOptionObjects();
        curOptionObjects = new List<GameObject>();
        foreach (string option in options)
        {
            GameObject objectInstance = Instantiate(optionPrefab, optionsHolder.transform);
            objectInstance.GetComponentInChildren<TMP_Text>().text = option;
            objectInstance.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                dialogue.ChoosePath(option);
                ContinueDialogue();
            });
            curOptionObjects.Add(objectInstance);
        }
    }

    public void ChooseEvidence(Thought idea)
    {
        if (idea != null)
        {
            dialogue.ChoosePath(idea.GetName());
        }
        else
        {
            dialogue.ChoosePath(null);
        }
        ContinueDialogue();
    }

    protected void SetState(TextUIState state)
    {
        this.state = state;
        talkingUI.SetActive(false);
        optionsUI.SetActive(false);
        presentingUI.SetActive(false);
        switch (state)
        {
            case TextUIState.TALKING:
                talkingUI.SetActive(true);
                break;
            case TextUIState.OPTIONS:
                optionsUI.SetActive(true);
                break;
            case TextUIState.PRESENT_EVIDENCE:
                presentingUI.SetActive(true);
                break;
        }
    }

    public void Hide()
    {
        talkingUI.SetActive(false);
        optionsUI.SetActive(false);
        presentingUI.SetActive(false);
        preventSceneClicksCanvas.SetActive(false);
    }
}