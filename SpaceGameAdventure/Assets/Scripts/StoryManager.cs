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
    private static Dictionary<string, Dialogue> npcToDialogue;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        objectives = new Objective[objectiveNames.Length];
        for (int i = 0; i < objectiveNames.Length; i++)
        {
            objectives[i] = new Objective(objectiveNames[i], objectiveDescriptions[i]);
        }
        npcToDialogue = new Dictionary<string, Dialogue>();
        // Temp Dialogue
        List<string> tempLines = new List<string>();
        tempLines.Add("Hi there!");
        tempLines.Add("Welcome!");
        tempLines.Add("Wait, am I talking to you");
        Dialogue tempDialogue = new Dialogue(tempLines, null);
        npcToDialogue.Add("", tempDialogue);

        // a Dialogue
        tempLines = new List<string>();
        tempLines.Add("Are you sure about that. To me, you are");
        Dialogue tempDialogueNo = new Dialogue(tempLines, null);
        tempLines = new List<string>();
        tempLines.Add("You are!");
        Dialogue tempDialogueYes = new Dialogue(tempLines, null);
        Dictionary<string, Dialogue> tempOptions = new Dictionary<string, Dialogue>();
        tempOptions.Add("Yes", tempDialogueYes);
        tempOptions.Add("No", tempDialogueNo);
        tempLines = new List<string>();
        tempLines.Add("Are you an abo?");
        tempLines.Add("Like really truely an abo?");
        tempLines.Add("Are you?");
        OptionsDialogueBranch tempDialogue2 = new OptionsDialogueBranch(tempLines, tempOptions);
        tempLines = new List<string>();
        tempLines.Add("Hi there");
        tempLines.Add("First I will ask...");
        tempDialogue = new Dialogue(tempLines, tempDialogue2);
        npcToDialogue.Add("a", tempDialogue);
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
        return npcToDialogue[npcID];
    }
}
