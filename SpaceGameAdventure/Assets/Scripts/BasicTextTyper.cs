using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BasicTextTyper : TextTyper
{
    [SerializeField] private float timeBetweenChar = .1f;
    [SerializeField] private AudioClip charSound;
    protected AudioSource audioSource;
    private IEnumerator typingRoutine;
    private TMP_Text textBox;
    private char[] text;
    //private Dictionary<int, BasicTextTyperCommand.BasicTextTyperCommandMethod> 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void TypeText(TMP_Text textBox, string text)
    {
        this.textBox = textBox;
        this.text = ParseText(text);
        typingRoutine = TypeTextRoutine();
        StartCoroutine(typingRoutine);
    }

    protected IEnumerator TypeTextRoutine()
    {
        for (int i = 0; i < text.Length; i++)
        {
            textBox.text += text[i];
            PlaySound(charSound, text, i);
            yield return new WaitForSeconds(timeBetweenChar);
        }
        typingRoutine = null;
    }

    protected virtual void PlaySound(AudioClip charSound, char[] text, int index)
    {
        if (text[index] != ' ')
        {
            audioSource.clip = charSound;
            audioSource.Play();
        }
    }

    public override bool NotTyping()
    {
        return typingRoutine == null;
    }

    public override void ForceTypeAll()
    {
        if (!NotTyping())
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
            this.textBox.text = new string(this.text);
        }
    }

    protected virtual char[] ParseText(string text)
    {
        List<char> chars = new List<char>();
        // Parse string
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] != '<' && text[i + 1] != '<')
            {
                while (text[i] != '>' && text[i + 1] != '>')
                {
                    i++;
                } // Removes Tags in the text
            }
            else
            {
                chars.Add(text[i]);
            }
        }
        // Copy to Array
        char[] res = new char[chars.Count];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = chars[i];
        }
        return res;
    }
}

public interface BasicTextTyperCommand
{
    public delegate void BasicTextTyperCommandMethod();

    public void ChangeBasicTextTyper();

    public void UndoChange();
}