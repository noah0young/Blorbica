using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Your inventory consists of ideas, an objective, and previous knowledge
/// </summary>
public abstract class Inventory : MonoBehaviour
{
    /// <summary>
    /// Adds a thought to your inventory.
    /// </summary>
    /// <param name="idea"></param>
    /// <throws>NoMoreThoughtSpaceException</throws>
    public abstract void AddThought(Thought idea);

    /// <summary>
    /// Removes a thought from your inventory
    /// </summary>
    /// <param name="idea"></param>
    /// <throws>NoThoughtExistsException</throws>
    public abstract void RemoveThought(Thought idea);

    public abstract void SetObjective(Objective o);

    public abstract void AddKnowledge(Knowledge idea);
}

public abstract class InventoryUI : MonoBehaviour
{
    public abstract void SetMemorySlot(int index, Thought idea);

    public abstract void SetKnowledgeSlot(int index, Knowledge idea);

    public abstract void ShowHide(bool show);
}

public class NoMoreThoughtSpaceException : System.Exception
{
    public NoMoreThoughtSpaceException(string message) : base(message) { }
}

public class NoThoughtExistsException : System.Exception
{
    public NoThoughtExistsException(string message) : base(message) { }
}

/// <summary>
/// Like evidence in Ace Attorney, these are the items you have that you can present.
/// </summary>
public interface Thought
{
    public string GetName();

    public string GetDescription();

    public Sprite GetImage();
}

public class BasicThought : Thought
{
    private string name;
    private string description;
    private Sprite image;

    public BasicThought(string name, string description, Sprite image)
    {
        this.name = name;
        this.description = description;
        this.image = image;
    }

    public string GetDescription()
    {
        return description;
    }

    public Sprite GetImage()
    {
        return image;
    }

    public string GetName()
    {
        return name;
    }
}

/// <summary>
/// This is what you get for answering a question correct at night,
/// this is used at the end of the game to save the planet.
/// This is a permenant thought, you always have this and can present it.
/// </summary>
public class Knowledge : BasicThought
{
    public Knowledge(Thought known) : base(known.GetName(), known.GetDescription(), known.GetImage()) { }
}


public class Objective
{
    private string objectiveName;
    private string objectiveDescription;

    public Objective(string objectiveName, string objectiveDescription)
    {
        this.objectiveName = objectiveName;
        this.objectiveDescription = objectiveDescription;
    }

    public string GetName()
    {
        return objectiveName;
    }

    public string GetDescription()
    {
        return objectiveDescription;
    }
}