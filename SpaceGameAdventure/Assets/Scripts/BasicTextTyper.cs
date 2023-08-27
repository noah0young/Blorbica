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
    private Color color;
    private float fontSize;
    private TMP_FontAsset font;
    private TMP_Text textBox;
    private char[] text;
    private int index = 0;
    private Dictionary<int, BasicTextTyperCommand> locToTextCommand
        = new Dictionary<int, BasicTextTyperCommand>();
    private Dictionary<string, BasicTextTyperCommand.Build> idToCommand;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InitPossibleCommands();
    }

    protected void InitPossibleCommands()
    {
        idToCommand = new Dictionary<string, BasicTextTyperCommand.Build>();
        idToCommand.Add("gain evidence", AddEvidenceCommand.GetBuild());
    }

    public override void TypeText(TMP_Text textBox, string text)
    {
        locToTextCommand = new Dictionary<int, BasicTextTyperCommand>();
        this.textBox = textBox;
        this.textBox.font = font;
        this.textBox.color = color;
        this.textBox.fontSize = fontSize;
        this.text = ParseText(text);
        typingRoutine = TypeTextRoutine();
        StartCoroutine(typingRoutine);
    }

    protected IEnumerator TypeTextRoutine()
    {
        for (int index = 0; index < text.Length; index++)
        {
            textBox.text += text[index];
            PlaySound(charSound, text, index);
            if (locToTextCommand.ContainsKey(index))
            {
                locToTextCommand[index].ChangeBasicTextTyper();
            }
            yield return new WaitForSeconds(timeBetweenChar);
        }
        typingRoutine = null;
        locToTextCommand = null;
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
            RunCommandsAfter();
            locToTextCommand = null;
        }
    }

    private void RunCommandsAfter()
    {
        for (;  index < text.Length; index++)
        {
            if (locToTextCommand.ContainsKey(index))
            {
                locToTextCommand[index].ChangeBasicTextTyper();
            }
        }
    }

    public override void SetSound(AudioClip sfx)
    {
        charSound = sfx;
    }

    public override void SetFont(TMP_FontAsset font)
    {
        this.font = font;
    }

    public override void SetFontSize(float size)
    {
        this.fontSize = size;
    }

    public override void SetTextColor(Color color)
    {
        this.color = color;
    }

    protected virtual char[] ParseText(string text)
    {
        // Parse string
        while (ContainsTag(text))
        {
            Debug.Log("Tag is in text");
            text = ParseTag(text);
        }
        return RemoveAllEscapedEscapeKeys(text.ToCharArray());
    }

    private string ParseTag(string text)
    {
        int tagLoc = IndexOfTag(text);
        string tag = GetTag(text);
        string commandID = ScriptParser.ReadBetween(tag, "", "=");
        commandID = ScriptParser.RemoveSurroundingSpaces(commandID);
        string args = tag.Substring(tag.IndexOf("=") + 1);
        args = ScriptParser.RemoveSurroundingSpaces(args);
        text = RemoveTag(text);
        BasicTextTyperCommand command = idToCommand[commandID]();
        command.SetArgs(args);
        Debug.Log("index of tag = " + tagLoc);
        if (!locToTextCommand.ContainsKey(tagLoc))
        {
            locToTextCommand.Add(tagLoc, command);
        }
        else
        {
            BasicTextTyperCommand doubleCommand
                = new TwoCommand(locToTextCommand[tagLoc], command);
            locToTextCommand[tagLoc] = doubleCommand;
        }
        return text;
    }

    protected bool ContainsTag(string text)
    {
        return IndexOfTag(text) != -1;
    }

    protected int IndexOfTag(string text)
    {
        return IndexOfTag(text, 0);
    }

    /// <summary>
    /// Returns true if the text contains | and it is not escaped.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected int IndexOfTag(string text, int startIndex)
    {
        // Assumes that if you place the start of a tag, there must be an end of the tag later in the text
        for (int i = startIndex; i < text.Length; i++)
        {
            if (text.Substring(i, 1) == "|" && (i  == 0 || text.Substring(i - 1, 2) != "\\|")) {
                return i;
            }
        }
        return -1;
    }

    protected char[] RemoveAllEscapedEscapeKeys(char[] text)
    {
        List<char> newChars = new List<char>();
        for (int i = 0; i < text.Length - 1; i++)
        {
            if (!(text[i] == '\\' && (text[i+1] == '\"' || text[i+1] == '|')))
            {
                newChars.Add(text[i]);
            }
        }
        newChars.Add(text[text.Length - 1]);
        return newChars.ToArray();
    }

    protected string RemoveTag(string text)
    {
        int tagStart = IndexOfTag(text);
        int tagEnd = IndexOfTag(text, tagStart + 1);
        return text.Substring(0, tagStart) + text.Substring(tagEnd + 1);
    }

    protected string GetTag(string text)
    {
        int tagStart = IndexOfTag(text);
        int tagEnd = IndexOfTag(text, tagStart + 1);
        Debug.Log("text = " + text + ", start = " + tagStart + ", end = " + tagEnd);
        return text.Substring(tagStart + 1, tagEnd - tagStart - 1);
    }
}

public abstract class BasicTextTyperCommand
{
    protected string args = null;

    public delegate BasicTextTyperCommand Build();

    public delegate void BasicTextTyperCommandMethod();

    public abstract void ChangeBasicTextTyper();

    public void SetArgs(string args)
    {
        this.args = args;
    }
}

public class TwoCommand : BasicTextTyperCommand
{
    private BasicTextTyperCommand one;
    private BasicTextTyperCommand two;

    public TwoCommand(BasicTextTyperCommand one, BasicTextTyperCommand two)
    {
        this.one = one;
        this.two = two;
    }

    public override void ChangeBasicTextTyper()
    {
        this.one.ChangeBasicTextTyper();
        this.two.ChangeBasicTextTyper();
    }

    public override string ToString()
    {
        return one + ", " + two;
    }
}

public class AddEvidenceCommand : BasicTextTyperCommand
{
    public static Build GetBuild()
    {
        return () => { return new AddEvidenceCommand(); };
    }

    public override void ChangeBasicTextTyper()
    {
        Thought evidence = NPCAssetManager.instance.GetEvidence(args);
        GameManager.AddEvidenceMenu(evidence);
    }

    public override string ToString()
    {
        return "AddEvidenceCommand " + this.args;
    }
}