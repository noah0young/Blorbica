using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dialogue
{
    private int index = 0;
    private List<Message> messages;
    protected Dialogue next; // if null, the dialogue ends

    public class Builder
    {
        public List<Message> messages;
        public string defaultNextDialogue;
        public Dictionary<string, string> nextDialoguePaths;

        public Builder() { }

        public virtual Dialogue Build(Dictionary<string, Builder> idToDialogue)
        {
            if (nextDialoguePaths.Count == 0)
            {
                Dialogue next = null;
                if (defaultNextDialogue != null)
                {
                    next = idToDialogue[defaultNextDialogue].Build(idToDialogue);
                }
                return new Dialogue(messages, next);
            }
            else
            {
                Dictionary<string, Dialogue> chosenToDialogue = new Dictionary<string, Dialogue>();
                foreach (string key in nextDialoguePaths.Keys)
                {
                    chosenToDialogue.Add(key, idToDialogue[nextDialoguePaths[key]].Build(idToDialogue));
                }
                return new OptionsDialogueBranch(messages, chosenToDialogue);
            }
        }
    }

    public Dialogue(List<Message> messages, Dialogue next)
    {
        this.messages = messages;
        this.next = next;
    }

    public NPCTextUI.TextUIState NextTextState()
    {
        if (index < messages.Count)
        {
            return MyTextState();
        }
        else if (HasAnotherDialogue())
        {
            return next.NextTextState();
        }
        else
        {
            Debug.Log("index = " + index + ", count = " + messages.Count);
            throw new System.Exception("Unknown state");
        }
    }

    protected virtual NPCTextUI.TextUIState MyTextState()
    {
        return NPCTextUI.TextUIState.TALKING;
    }

    public virtual bool HasNext()
    {
        if (InThisDialogue())
        {
            return true;
        }
        else if (HasAnotherDialogue())
        {
            return next.HasNext();
        }
        else
        {
            return false;
        }
    }

    public virtual Message Next()
    {
        if (InThisDialogue())
        {
            index += 1;
            return messages[index - 1];
        }
        else if (HasAnotherDialogue() && next.HasNext())
        {
            return next.Next();
        }
        else
        {
            throw new System.Exception("No more dialogue remains");
        }
    }

    public virtual List<string> GetPathNames()
    {
        if (InThisDialogue())
        {
            throw new System.Exception("No paths exist right now");
        }
        else if (HasAnotherDialogue())
        {
            return next.GetPathNames();
        }
        else
        {
            throw new System.Exception("No more dialogue remains");
        }
    }

    protected virtual bool HasAnotherDialogue()
    {
        return next != null;
    }

    protected bool InThisDialogue()
    {
        return index < messages.Count;
    }

    public virtual void Reset()
    {
        index = 0;
        if (next != null)
        {
            next.Reset();
        }
    }

    public virtual void ChoosePath(string pathName)
    {
        if (InThisDialogue())
        {
            Debug.Log("index = " + index + ", count = " + messages.Count);
            throw new System.Exception("No paths exist right now");
        }
        else if (HasAnotherDialogue())
        {
            next.ChoosePath(pathName);
        }
        else
        {
            throw new System.Exception("No more dialogue remains");
        }
    }

    protected bool IsLastLine()
    {
        return index == messages.Count - 1;
    }
}

public abstract class DialogueBranch : Dialogue
{
    // Which ever path you choose leads to a dialogue
    private Dictionary<string, Dialogue> chosenToDialogue;

    public DialogueBranch(List<Message> messages, Dialogue next, Dictionary<string, Dialogue> chosenToDialogue) : base(messages, next)
    {
        this.chosenToDialogue = chosenToDialogue;
    }

    public override List<string> GetPathNames()
    {
        if (next == null)
        {
            return new List<string>(chosenToDialogue.Keys);
        }
        return next.GetPathNames();
    }

    public override void ChoosePath(string pathName)
    {
        if (next == null)
        {
            if (chosenToDialogue.ContainsKey(pathName))
            {
                next = chosenToDialogue[pathName];
            }
            else
            {
                throw new NoDialoguePathExists("No path exists for the given pathName");
            }
        }
        else
        {
            next.ChoosePath(pathName);
        }
    }

    protected override bool HasAnotherDialogue()
    {
        return true;
    }

    public override void Reset()
    {
        base.Reset(); // Resets the previous chosen path
        next = null; // Unselects path
    }
}

public class NoDialoguePathExists : System.Exception
{
    public NoDialoguePathExists(string message) : base(message) { }
}

public class OptionsDialogueBranch : DialogueBranch
{
    public OptionsDialogueBranch(List<Message> messages, Dictionary<string, Dialogue> chosenToDialogue)
        : base(messages, null, chosenToDialogue) { }

    protected override NPCTextUI.TextUIState MyTextState()
    {
        if (IsLastLine())
        {
            return NPCTextUI.TextUIState.OPTIONS;
        }
        return NPCTextUI.TextUIState.TALKING;
    }
}

public class PresentingDialogueBranch : DialogueBranch
{
    private Dialogue incorrect;

    public PresentingDialogueBranch(List<Message> messages, Dictionary<string, Dialogue> chosenToDialogue, Dialogue incorrect)
        : base(messages, null, chosenToDialogue)
    {
        this.incorrect = incorrect;
    }

    public new class Builder : Dialogue.Builder
    {
        public Builder() { }

        public override Dialogue Build(Dictionary<string, Dialogue.Builder> idToDialogue)
        {
            Dictionary<string, Dialogue> chosenToDialogue = new Dictionary<string, Dialogue>();
            foreach (string key in nextDialoguePaths.Keys)
            {
                chosenToDialogue.Add(key, idToDialogue[nextDialoguePaths[key]].Build(idToDialogue));
            }
            Dialogue next = null;
            if (defaultNextDialogue != null) {
                next = idToDialogue[defaultNextDialogue].Build(idToDialogue);
            }
            return new PresentingDialogueBranch(messages, chosenToDialogue, next);
        }
    }

    protected override NPCTextUI.TextUIState MyTextState()
    {
        if (IsLastLine())
        {
            return NPCTextUI.TextUIState.PRESENT_EVIDENCE;
        }
        return NPCTextUI.TextUIState.TALKING;
    }

    public override void ChoosePath(string pathName)
    {
        try
        {
            base.ChoosePath(pathName);
        }
        catch (NoDialoguePathExists e)
        {
            next = incorrect;
        }
    }
}

public class SceneRedirectDialogue : Dialogue
{
    private string nextScene;

    public SceneRedirectDialogue(List<Message> messages, string nextScene) : base(messages, null)
    {
        this.nextScene = nextScene;
    }

    public new class Builder : Dialogue.Builder
    {
        public string nextScene = null;

        public Builder() { }

        public override Dialogue Build(Dictionary<string, Dialogue.Builder> idToDialogue)
        {
            if (nextDialoguePaths != null && nextDialoguePaths.Count != 0)
            {
                throw new Exception("A Scene Redirect cannot have branching dialogue");
            }
            else if (nextScene == null)
            {
                throw new Exception("Next scene cannot be null");
            }
            else
            {
                messages = new List<Message>();
                messages.Add(new BasicMessage(""));
                // This needs at least one message so the Next method will be called
                return new SceneRedirectDialogue(messages, nextScene);
            }
        }
    }

    public override bool HasNext()
    {
        return true;
    }

    public override Message Next()
    {
        if (IsLastLine())
        {
            SceneManager.LoadScene(nextScene);
        }
        return base.Next();
    }

    protected override NPCTextUI.TextUIState MyTextState()
    {
        return NPCTextUI.TextUIState.SCENE_TRANSITION;
    }
}

public interface Message
{
    public string GetText();

    public string GetName();

    public List<string> GetNPCsImageID();

    public string GetNpcID();

    public float GetFontSize();
}

public class BasicMessage : Message
{
    private string npcID;
    private string text;
    private string name;
    private float fontSize; // Uses default if this is negative
    private List<string> spriteIDs;

    public BasicMessage(string text)
    {
        this.npcID = "";
        this.text = text;
        this.name = "???";
        fontSize = -1;
        this.spriteIDs = new List<string>();
    }

    public BasicMessage(string npcID, string name, string text, float fontSize, List<string> spriteIDs)
    {
        this.npcID = npcID;
        this.text = text;
        this.name = name;
        this.fontSize = fontSize;
        this.spriteIDs = spriteIDs;
    }

    public string GetNpcID()
    {
        return npcID;
    }

    public float GetFontSize()
    {
        return fontSize;
    }

    public string GetName()
    {
        return name;
    }

    public List<string> GetNPCsImageID()
    {
        return spriteIDs;
    }

    public string GetText()
    {
        return text;
    }
}