using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryData", menuName = "Story/StoryData", order = 1)]
public class DialogueAssets : ScriptableObject
{
    private ISet<string> objectIDsUsed = new HashSet<string>();
    private Dictionary<string, NPCData.Builder> npcBuilders = new Dictionary<string, NPCData.Builder>();

    private Dictionary<string, Dictionary<string, Dialogue.Builder>> dialogueBuildersInScene
        = new Dictionary<string, Dictionary<string, Dialogue.Builder>>();
    private Dictionary<string, List<string>> sceneToAllStartingDialogues = new Dictionary<string, List<string>>();

    private NPCData.Builder curNPCBuilder = null;
    private Dialogue.Builder curDialogueBuilder = null;
    private List<Message> curMessageList;

    public Dialogue.Builder GetDialogueBuilder(string id, string sceneName)
    {
        if (!dialogueBuildersInScene.ContainsKey(sceneName))
        {
            dialogueBuildersInScene.Add(sceneName, new Dictionary<string, Dialogue.Builder>());
        }
        if (!dialogueBuildersInScene[sceneName].ContainsKey(id))
        {
            if (objectIDsUsed.Contains(id))
            {
                // ID is being used
                return null;
            }
            dialogueBuildersInScene[sceneName].Add(id, new Dialogue.Builder());
        }
        return dialogueBuildersInScene[sceneName][id];
    }

    public NPCData.Builder GetNPCData(string id)
    {
        if (!npcBuilders.ContainsKey(id))
        {
            if (objectIDsUsed.Contains(id))
            {
                // ID is being used
                return null;
            }
            npcBuilders.Add(id, new NPCData.Builder());
        }
        return npcBuilders[id];
    }

    public string[] GetStartingDialogues(string sceneName)
    {
        if (!sceneToAllStartingDialogues.ContainsKey(sceneName))
        {
            return new string[0];
        }
        return sceneToAllStartingDialogues[sceneName].ToArray();
    }
}
