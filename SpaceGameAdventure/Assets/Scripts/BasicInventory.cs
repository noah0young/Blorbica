using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BasicInventory : Inventory
{
    public static Inventory instance { get; private set; }
    [SerializeField] private InventoryUI ui;

    [Header("Thoughts")]
    private static int maxNumOfThoughts = 5;
    private static Thought[] memory;
    private static List<Knowledge> myKnowledge;
    private static System.Collections.Generic.ISet<Thought> heldThoughts = new HashSet<Thought>();

    [Header("Objective Details")]
    private static Objective objective;

    [Header("Remove One UI")]
    [SerializeField] private RemoveOneInventoryUI removeOneUI;
    [SerializeField] private PresentEvidenceUI presentingUI;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            memory = new Thought[maxNumOfThoughts];
            ui.Init();
            removeOneUI.Init();
            removeOneUI.SetReplaceMethod(ReplaceThought);
            Refresh();
        }
    }

    private void Refresh()
    {
        for (int i = 0; i < memory.Length; i++)
        {
            if (memory[i] != null)
            {
                ui.SetMemorySlot(i, memory[i]);
                removeOneUI.SetMemorySlot(i, memory[i]);
                presentingUI.SetMemorySlot(i, memory[i]);
            }
        }
    }

    public override void AddKnowledge(Knowledge idea)
    {
        myKnowledge.Add(idea);
    }

    public override void AddThought(Thought idea)
    {
        if (heldThoughts.Contains(idea))
        {
            return;
        }
        for (int i = 0; i < memory.Length; i++)
        {
            if (memory[i] == null)
            {
                Debug.Log("memory at i = " + i + " is null");
                memory[i] = idea;
                heldThoughts.Add(idea);
                ui.SetMemorySlot(i, memory[i]);
                removeOneUI.SetMemorySlot(i, memory[i]);
                presentingUI.SetMemorySlot(i, memory[i]);
                return;
            }
        }
        MakeRemoveOneUI(idea);
        //throw new NoMoreThoughtSpaceException("Player memory for thoughts is full");
    }

    public override void RemoveThought(Thought idea)
    {
        for (int i = 0; i < memory.Length; i++)
        {
            if (memory[i] == idea)
            {
                memory[i] = null;
                heldThoughts.Remove(idea);
                ui.SetMemorySlot(i, null);
                removeOneUI.SetMemorySlot(i, null);
                presentingUI.SetMemorySlot(i, null);
                return;
            }
        }
        throw new NoThoughtExistsException("Thought was not found");
    }

    private void ReplaceThought(int index, Thought newThought)
    {
        if (index != -1)
        {
            if (index >= memory.Length || index < -1)
            {
                throw new NoThoughtExistsException("Out of bounds");
            }
            else if (memory[index] == null)
            {
                throw new NoThoughtExistsException("Thought was not found");
            }
            memory[index] = null;
            heldThoughts.Remove(memory[index]);
            heldThoughts.Add(newThought);
            ui.SetMemorySlot(index, newThought);
            removeOneUI.SetMemorySlot(index, newThought);
            presentingUI.SetMemorySlot(index, newThought);
        }
        removeOneUI.ShowHide(false);
    }

    public override void SetObjective(Objective o)
    {
        objective = o;
    }

    public override void MakeRemoveOneUI(Thought newEvidence)
    {
        removeOneUI.ShowHide(true);
        removeOneUI.SetExtraThought(newEvidence);
    }

    public override void SetPresentingUI()
    {
        presentingUI.ResetPresentUI();
    }
}
