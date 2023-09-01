using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Just holds story details to be loaded by GameManager.
/// </summary>
public class StoryManager : MonoBehaviour
{
    public static StoryManager instance { get; private set; }
    private static int nextObjective = 0;
    [SerializeField] private string[] objectiveNames;
    [SerializeField] private string[] objectiveDescriptions;
    private static Objective[] objectives;
    [SerializeField] private TextAsset[] storyFiles;
    //private static Dictionary<string, Dialogue> npcToDialogue;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
    }

    private void Init()
    {
        objectives = new Objective[objectiveNames.Length];
        for (int i = 0; i < objectiveNames.Length; i++)
        {
            objectives[i] = new Objective(objectiveNames[i], objectiveDescriptions[i]);
        }
        Debug.Log("Parse Story");
        ScriptParser.Parse(storyFiles);
        /*npcToDialogue = new Dictionary<string, Dialogue>();
        // Temp Dialogue
        List<Message> tempLines = new List<Message>();
        tempLines.Add(new BasicMessage("Hi there!"));
        tempLines.Add(new BasicMessage("Welcome!"));
        tempLines.Add(new BasicMessage("Wait, am I talking to you"));
        Dialogue tempDialogue = new Dialogue(tempLines, null);
        npcToDialogue.Add("", tempDialogue);

        // a Dialogue
        tempLines = new List<Message>();
        tempLines.Add(new BasicMessage("Are you sure about that. To me, you are"));
        Dialogue tempDialogueNo = new Dialogue(tempLines, null);
        tempLines = new List<Message>();
        tempLines.Add(new BasicMessage("You are!"));
        Dialogue tempDialogueYes = new Dialogue(tempLines, null);
        Dictionary<string, Dialogue> tempOptions = new Dictionary<string, Dialogue>();
        tempOptions.Add("Yes", tempDialogueYes);
        tempOptions.Add("No", tempDialogueNo);
        tempLines = new List<Message>();
        tempLines.Add(new BasicMessage("Are you an abo?"));
        tempLines.Add(new BasicMessage("Like really truely an abo?"));
        tempLines.Add(new BasicMessage("Are you?"));
        OptionsDialogueBranch tempDialogue2 = new OptionsDialogueBranch(tempLines, tempOptions);
        tempLines = new List<Message>();
        tempLines.Add(new BasicMessage("Hi there"));
        tempLines.Add(new BasicMessage("First I will ask..."));
        tempDialogue = new Dialogue(tempLines, tempDialogue2);
        npcToDialogue.Add("a", tempDialogue);*/
    }

    public static Objective GetNextObjective()
    {
        if (nextObjective >= objectives.Length)
        {
            throw new System.Exception("No Objectives Remain");
        }
        Objective next = objectives[nextObjective];
        nextObjective += 1;
        return next;
    }

    public static Dialogue GetDialogueFor(string npcID)
    {
        return ScriptParser.GetDialogue(npcID);//npcToDialogue[npcID];
    }
}
