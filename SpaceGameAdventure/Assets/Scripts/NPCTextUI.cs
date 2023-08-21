using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCTextUI : MonoBehaviour
{
    public static NPCTextUI instance { get; private set; }

    [Header("State")]
    [SerializeField] private GameObject talkingUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject presentingUI;
    public enum TextUIState
    {
        TALKING, OPTIONS, PRESENT_EVIDENCE
    }
    private TextUIState state;

    private Dialogue dialogue;

    [Header("Talking")]
    [SerializeField] private TMP_Text talkingText;

    [Header("Options")]
    [SerializeField] private TMP_Text optionsText;
    [SerializeField] private GameObject optionsHolder;
    [SerializeField] private GameObject optionPrefab;
    private bool optionPressed;
    private List<GameObject> curOptionObjects;

    [Header("Presenting")]
    [SerializeField] private TMP_Text presentingText;
    [SerializeField] private Button[] presentButtons;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Hide();
    }

    public void StartDialogue(Dialogue d)
    {
        this.dialogue = d;
        StartCoroutine(RunDialogue());
    }

    private IEnumerator RunDialogue()
    {
        while (dialogue.HasNext())
        {
            SetState(dialogue.NextTextState());
            switch(state)
            {
                case TextUIState.TALKING:
                    talkingText.text = dialogue.Next(); // should become its own object for visualizing text
                    // So that text can be typed out a letter at a time
                    break;
                case TextUIState.OPTIONS:
                    optionsText.text = dialogue.Next();
                    MakeOptions();
                    break;
                case TextUIState.PRESENT_EVIDENCE:
                    throw new System.Exception("not implemented yet");
                    break;
            }
            yield return new WaitUntil(() => optionPressed);
            DestroyOptionObjects();
            optionPressed = false;
        }
        Hide();
    }

    public void ContinueDialogue()
    {
        optionPressed = true;
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
        dialogue.ChoosePath(idea.GetName());
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
    }
}