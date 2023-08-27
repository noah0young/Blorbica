using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public string playerName { get; private set; }
    [SerializeField] private Inventory inventory;
    [SerializeField] private NPCTextUI talkingUI;

    // Start is called before the first frame update
    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Switches the current objective with the next one.
    /// </summary>
    public static void NextObjective()
    {
        instance.inventory.SetObjective(StoryManager.GetNextObjective());
    }

    public static void AddEvidenceMenu(Thought evidence)
    {
        instance.inventory.AddThought(evidence);
    }

    public static void SetPresentingUI()
    {
        instance.inventory.SetPresentingUI();
    }
}
