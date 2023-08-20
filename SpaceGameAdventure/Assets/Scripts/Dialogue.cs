using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Dialogue
{
    private int index = 0;
    private List<string> text;
    protected Dialogue next; // if null, the dialogue ends

    public Dialogue(List<string> text, Dialogue next)
    {
        this.text = text;
        this.next = next;
    }

    public NPCTextUI.TextUIState NextTextState()
    {
        if (index < text.Count)
        {
            return MyTextState();
        }
        else if (HasAnotherDialogue())
        {
            return next.NextTextState();
        }
        else
        {
            Debug.Log("index = " + index + ", count = " + text.Count);
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

    public virtual string Next()
    {
        if (InThisDialogue())
        {
            index += 1;
            return text[index - 1];
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
        return index < text.Count;
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
            Debug.Log("index = " + index + ", count = " + text.Count);
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
        return index == text.Count - 1;
    }
}

public abstract class DialogueBranch : Dialogue
{
    // Which ever path you choose leads to a dialogue
    private Dictionary<string, Dialogue> chosenToDialogue;

    public DialogueBranch(List<string> text, Dialogue next, Dictionary<string, Dialogue> chosenToDialogue) : base(text, next)
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
    public OptionsDialogueBranch(List<string> text, Dictionary<string, Dialogue> chosenToDialogue)
        : base(text, null, chosenToDialogue) { }

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

    public PresentingDialogueBranch(List<string> text, Dictionary<string, Dialogue> chosenToDialogue, Dialogue incorrect)
        : base(text, null, chosenToDialogue)
    {
        this.incorrect = incorrect;
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