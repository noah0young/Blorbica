using UnityEngine;

[CreateAssetMenu(fileName = "New Thought", menuName = "Thought/BasicThought", order = 1)]
public class BasicThought : ScriptableObject, Thought
{
    [SerializeField] private string thoughtName;
    [SerializeField] private string description;
    [SerializeField] private Sprite image;

    /*public BasicThought(string name, string description, Sprite image)
    {
        this.thoughtName = name;
        this.description = description;
        this.image = image;
    }*/

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
        return thoughtName;
    }
}