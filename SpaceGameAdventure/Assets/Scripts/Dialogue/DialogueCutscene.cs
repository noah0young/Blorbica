using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueCutscene : MonoBehaviour
{
    [Serializable]
    public struct DialogueCutsceneMessage
    {
        [TextArea(5, 5)]
        public string text;
        public float timeShown;
        public float timeBeforeShow;
        public Sprite image;
    }
    [SerializeField] private List<DialogueCutsceneMessage> messages;
    [SerializeField] private TMP_Text cutsceneText;
    [SerializeField] private Image cutsceneImage;
    [SerializeField] private string nextScene;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(CutsceneStart());
    }

    private IEnumerator CutsceneStart()
    {
        int index = 0;
        while (messages != null && index < messages.Count)
        {
            Debug.Log("Message Update");
            ResetCutsceneScreen();
            yield return new WaitForSeconds(messages[index].timeBeforeShow);
            SetCutsceneScreen(messages[index].text, messages[index].image);
            yield return new WaitForSeconds(messages[index].timeShown);
            index += 1;
        }
        SceneManager.LoadScene(nextScene);
    }

    private void ResetCutsceneScreen()
    {
        cutsceneText.text = "";
        cutsceneImage.gameObject.SetActive(false);
        cutsceneImage.sprite = null;
    }

    private void SetCutsceneScreen(string text, Sprite image)
    {
        cutsceneText.text = text;
        if (image != null)
        {
            cutsceneImage.gameObject.SetActive(true);
            cutsceneImage.sprite = image;
        }
    }
}
