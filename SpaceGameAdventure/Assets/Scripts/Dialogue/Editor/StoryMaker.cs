using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;

public class StoryMaker : EditorWindow
{
    private DialogueAssets storyData;
    private int sceneNum = 0;
    private int startingDialogueNum = 0;
    private string[] curStartingDialogues;
    private Dialogue.Builder curBuilder;

    [MenuItem("Visual Novel/StoryMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<StoryMaker>("Story Maker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Visual Novel Maker");
        storyData = (DialogueAssets)EditorGUILayout.ObjectField(storyData, typeof(DialogueAssets), false);
        if (storyData != null)
        {
            string[] allScenesInBuild = GetAllSceneNames();
            // Window Code
            if (allScenesInBuild != null && allScenesInBuild.Length > 0)
            {
                ChooseScene(allScenesInBuild);
                ChooseStartingDialogue();
                if (curStartingDialogues != null && curStartingDialogues.Length > 0)
                {
                    EditMessages(allScenesInBuild[sceneNum]);
                }
            }
        }
    }

    private void ChooseScene(string[] allScenesInBuild)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Select a scene:");
        sceneNum = EditorGUILayout.Popup(sceneNum, allScenesInBuild);
        GUILayout.EndHorizontal();
    }

    private void ChooseStartingDialogue()
    {
        SetStartingDialogues();
        if (curStartingDialogues != null && curStartingDialogues.Length > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select a starting dialogue:");
            startingDialogueNum = EditorGUILayout.Popup(startingDialogueNum, curStartingDialogues);
            GUILayout.EndHorizontal();
        }
    }

    private void EditMessages(string curScene)
    {
        string dialogueID = curStartingDialogues[startingDialogueNum];
        curBuilder = storyData.GetDialogueBuilder(dialogueID, curScene);
        ISet<Dialogue.Builder> seenBuilders = new HashSet<Dialogue.Builder>();
        while (curBuilder != null && !seenBuilders.Contains(curBuilder))
        {
            seenBuilders.Add(curBuilder);
            for (int i = 0; i < curBuilder.messages.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Message[" + i + "]");
                curBuilder.messages[i].SetName(EditorGUILayout.TextField(curBuilder.messages[i].GetName()));
                curBuilder.messages[i].SetText(EditorGUILayout.TextArea(curBuilder.messages[i].GetText()));
                //curBuilder.messages[i]
                GUILayout.EndHorizontal();
            }
            //TODO change curBuilder to the next in this tree path
        }
    }

    private string[] GetAllSceneNames()
    {
        string[] names = new string[SceneManager.sceneCount];
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            names[i] = SceneManager.GetSceneAt(i).name;
        }
        return names;
    }

    private string GetCurrentScene()
    {
        return SceneManager.GetSceneAt(sceneNum).name;
    }

    private void SetStartingDialogues()
    {
        curStartingDialogues = storyData.GetStartingDialogues(GetCurrentScene());
    }
}
