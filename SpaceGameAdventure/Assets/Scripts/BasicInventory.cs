using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInventory : Inventory
{
    [SerializeField] private InventoryUI ui;

    [Header("Thoughts")]
    [SerializeField] private int maxNumOfThoughts = 5;
    private Thought[] memory;
    private List<Knowledge> myKnowledge;

    [Header("Objective Details")]
    private Objective objective;

    private void Awake()
    {
        memory = new Thought[maxNumOfThoughts];
    }

    public override void AddKnowledge(Knowledge idea)
    {
        myKnowledge.Add(idea);
    }

    public override void AddThought(Thought idea)
    {
        for (int i = 0; i < memory.Length; i++)
        {
            if (memory[i] == null)
            {
                memory[i] = idea;
                return;
            }
        }
        throw new NoMoreThoughtSpaceException("Player memory for thoughts is full");
    }

    public override void RemoveThought(Thought idea)
    {
        for (int i = 0; i < memory.Length; i++)
        {
            if (memory[i] == idea)
            {
                memory[i] = null;
                return;
            }
        }
        throw new NoThoughtExistsException("Thought was not found");
    }

    public override void SetObjective(Objective o)
    {
        this.objective = o;
    }
}
