using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ScriptParser
{
    public enum ParserLineType
    {
        COMMENT, OBJECT_START, DIALOGUE_PATH, OBJECT_END, SETTER, MESSAGE
    }

    private static ISet<string> objectIDsUsed = new HashSet<string>();
    private static Dictionary<string, NPCData.Builder> npcBuilders = new Dictionary<string, NPCData.Builder>();
    private static NPCData.Builder curNPCBuilder = null;
    private static Dictionary<string, Dialogue.Builder> dialogueBuilders = new Dictionary<string, Dialogue.Builder>();
    private static Dialogue.Builder curDialogueBuilder = null;
    private static List<Message> curMessageList;

    public static Dialogue GetDialogue(string id)
    {
        return dialogueBuilders[id].Build(dialogueBuilders);
    }

    public static NPCData GetNPCData(string id)
    {
        Debug.Log("npcdata id = " + id);
        return npcBuilders[id].Build();
    }

    public static void Parse(TextAsset[] files)//string[] filePaths)
    {
        foreach (TextAsset file in files)
        {
            ParseFile(file);
        }
    }

    private static void ParseFile(TextAsset file)
    {
        //FileStream file = File.Open(filePath, FileMode.Open);
        //StreamReader reader = new StreamReader(file);
        string fileText = file.text;
        int lineNum = 1;
        while (fileText.IndexOf("\n") != -1)//reader.Peek() != -1)
        {
            string line = fileText.Substring(0, fileText.IndexOf("\n"));//reader.ReadLine();
            fileText = fileText.Substring(fileText.IndexOf("\n") + 1);
            line = RemoveLeadingSpaces(line);
            ParserLineType lineType = GetLineType(line);
            switch (lineType)
            {
                case (ParserLineType.COMMENT):
                    break;
                case (ParserLineType.OBJECT_START):
                    StartObject(line, lineNum);
                    break;
                case (ParserLineType.DIALOGUE_PATH):
                    AddDialoguePath(line, lineNum);
                    break;
                case (ParserLineType.OBJECT_END):
                    EndObject(line, lineNum);
                    break;
                case (ParserLineType.SETTER):
                    Setter(line, lineNum);
                    break;
                case (ParserLineType.MESSAGE):
                    AddMessage(line, lineNum);
                    break;
            }
            lineNum++;
        }
    }

    public static string ReadBetween(string text, string start, string end)
    {
        if (!text.Contains(start))
        {
            throw new System.Exception("Start does not exist in ReadBetween");
        }
        else if (!text.Contains(end))
        {
            throw new System.Exception("End does not exist in ReadBetween");
        }
        int readStart = IndexOfOutOfQuote(text, start) + start.Length;//text.IndexOf(start) + start.Length;
        int readEnd = IndexOfOutsideOfQuote(text, end, readStart);//text.IndexOf(end, readStart);
        if (readEnd < readStart)
        {
            Debug.Log("end = " + readEnd);
            Debug.Log("start = " + readStart);
            throw new System.Exception("readEnd is less than readStart, text  = " + text + ", start = " + start + ", end = " + end);
        }
        return text.Substring(readStart, readEnd - readStart);
    }

    public static List<string> ReadListBetween(string text, string start, string end, string separator)
    {
        string subtext = ReadBetween(text, start, end);
        List<string> list = new List<string>();
        while (IndexOfOutOfQuote(subtext, separator) != -1)//subtext.Contains(separator))
        {
            int indexOfSeparator = IndexOfOutOfQuote(subtext, separator);//subtext.IndexOf(separator);
            list.Add(subtext.Substring(0, indexOfSeparator));
            subtext = subtext.Substring(indexOfSeparator + 1);
        }
        list.Add(subtext);
        return list;
    }

    public static int IndexOfOutOfQuote(string text, string s)
    {
        return IndexOfOutsideOfQuote(text, s, 0);
    }

    public static int IndexOfOutsideOfQuote(string text, string s, int startIndex)
    {
        bool inQuote = false;
        for (int i = 0; i < startIndex; i++)
        {
            if ((text.Substring(i, 1) == "\"") && (i == 0 || text.Substring(i - 1, 2) != "\\\""))
            {
                inQuote = !inQuote;
            }
        }
        for (int i = startIndex; i < text.Length - s.Length + 1; i++)
        {
            bool prevInQuote = inQuote;
            if ((text.Substring(i, 1) == "\"") && (i == 0 || text.Substring(i - 1, 2) != "\\\""))
            {
                inQuote = !inQuote;
            }

            if (!inQuote || !prevInQuote)
            {
                if (text.Substring(i, s.Length) == s)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public static string RemoveLeadingSpaces(string str)
    {
        int startIndex = 0;
        while (startIndex < str.Length && str[startIndex] == ' ')
        {
            startIndex++;
        }
        return str.Substring(startIndex);
    }

    public static string RemoveEndSemiColonIfThere(string str)
    {
        if (str == null || str == "")
        {
            return str;
        }
        int lastCharIndex = str.Length - 1;
        if (str[lastCharIndex] == ';')
        {
            return str.Substring(0, lastCharIndex);
        }
        return str;
    }

    public static string RemoveSurroundingSpaces(string str)
    {
        str = RemoveLeadingSpaces(str);
        int endIndex = str.Length;
        while (endIndex > 0 && str[endIndex - 1] == ' ')
        {
            endIndex--;
        }
        return str.Substring(0, endIndex);
    }

    private static ParserLineType GetLineType(string line)
    {
        if (line.StartsWith("//"))
        {
            return ParserLineType.COMMENT;
        }
        else if (line.StartsWith("{"))
        {
            return ParserLineType.OBJECT_START;
        }
        else if (line.Equals("</>"))
        {
            return ParserLineType.OBJECT_END;
        }
        else if (line.StartsWith("<"))
        {
            return ParserLineType.DIALOGUE_PATH;
        }
        else if (line.StartsWith("["))
        {
            return ParserLineType.MESSAGE;
        }
        else if (line.Contains("="))
        {
            return ParserLineType.SETTER;
        }
        else if (line == "")
        {
            // Blank lines are ignored
            return ParserLineType.COMMENT;
        }
        else
        {
            throw new System.Exception("No ParserLineType matches");
        }
    }

    private static void StartObject(string line, int lineNum)
    {
        // Parsing
        if (curNPCBuilder != null || curDialogueBuilder != null)
        {
            throw new System.Exception("Must end ScriptObject before creating a new one at line " + lineNum);
        }
        string objectType = ReadBetween(line, "{", ":");
        string objectIDUncropped = ReadBetween(line, ":", "}");
        string objectID = ReadBetween(objectIDUncropped, "\"", "\"");
        Debug.Log("objectID = " + objectID);
        UseObjectID(objectID, lineNum);
        // Object Building Start
        curMessageList = new List<Message>();
        if (objectType.Contains("NPC"))
        {
            curNPCBuilder = new NPCData.Builder();
            npcBuilders.Add(objectID, curNPCBuilder);
        }
        else if (objectType.Contains("PresentingDialogue"))
        {
            curDialogueBuilder = new PresentingDialogueBranch.Builder();
            curDialogueBuilder.nextDialoguePaths = new Dictionary<string, string>();
            dialogueBuilders.Add(objectID, curDialogueBuilder);
        }
        else if (objectType.Contains("Dialogue"))
        {
            curDialogueBuilder = new Dialogue.Builder();
            curDialogueBuilder.nextDialoguePaths = new Dictionary<string, string>();
            dialogueBuilders.Add(objectID, curDialogueBuilder);
        }
        else
        {
            throw new System.Exception("No Object Type Exists at " + lineNum);
        }
    }

    private static void UseObjectID(string objectID, int lineNum)
    {
        if (objectIDsUsed.Contains(objectID))
        {
            throw new System.Exception("All objectIDs must be unique at line " + lineNum);
        }
        else
        {
            objectIDsUsed.Add(objectID);
        }
    }

    private static void EndObject(string line, int lineNum)
    {
        if (curNPCBuilder == null && curDialogueBuilder == null)
        {
            throw new System.Exception("Must start ScriptObject before ending one at line " + lineNum);
        }
        if (curDialogueBuilder != null)
        {
            curDialogueBuilder.messages = curMessageList;
        }
        curNPCBuilder = null;
        curDialogueBuilder = null;
    }

    private static void AddDialoguePath(string line, int lineNum)
    {
        if (curDialogueBuilder == null)
        {
            throw new System.Exception("Must start DialogueObject before adding a DialoguePath at line " + lineNum);
        }

        line = line.Substring(1); // Removes the '<'
        line = RemoveLeadingSpaces(line);
        if (line.StartsWith("\""))
        {
            // This is a path option
            string option = ReadBetween(line, "\"", "\"");
            line = line.Substring(line.IndexOf(",", line.IndexOf("\"", 1)) + 1);
            // Index of first comma after the first end quote
            // Really the index of the comma between the option and the resulting path
            AddDialoguePathHelper(option, line, lineNum);
        }
        else
        {
            // This is the default path
            AddDialoguePathHelper(null, line, lineNum);
        }
    }

    private static void AddDialoguePathHelper(string option, string line, int lineNum)
    {
        string nextDialogueType = RemoveLeadingSpaces(ReadBetween(line, "", ":"));
        line = line.Substring(line.IndexOf(":"));
        if (nextDialogueType.StartsWith("Dialogue"))
        {
            string nextDialogueID = ReadBetween(line, "\"", "\"");
            if (option == null)
            {
                curDialogueBuilder.defaultNextDialogue = nextDialogueID;
            }
            else
            {
                curDialogueBuilder.nextDialoguePaths.Add(option, nextDialogueID);
            }
        }
        else if (nextDialogueType.StartsWith("Scene"))
        {
            string nextSceneID = ReadBetween(line, "\"", "\"");
            try
            {
                UseObjectID(nextSceneID, lineNum);
                SceneRedirectDialogue.Builder scenRedirectBuilder = new SceneRedirectDialogue.Builder();
                scenRedirectBuilder.nextScene = nextSceneID;
                scenRedirectBuilder.messages = new List<Message>();
                dialogueBuilders.Add(nextSceneID, scenRedirectBuilder);
            }
            catch (System.Exception e)
            {
                Debug.Log("Caught exception because dialogueBuilders[nextScene] = " + dialogueBuilders[nextSceneID]);
                // Scene already exists
            }
            if (option == null)
            {
                curDialogueBuilder.defaultNextDialogue = nextSceneID;
            }
            else
            {
                curDialogueBuilder.nextDialoguePaths.Add(option, nextSceneID);
            }
        }
        else
        {
            Debug.Log("next dialogue type is " + nextDialogueType);
            throw new System.Exception("Next Dialogue Type is unknown at line " + lineNum);
        }
    }

    private static void Setter(string line, int lineNum)
    {
        string varName = RemoveSurroundingSpaces(ReadBetween(line, "", "="));
        string val = RemoveSurroundingSpaces(ReadBetween(line, "=", ";"));
        if (val.StartsWith("null"))
        {
            val = null;
        }
        else if (val.Contains("\""))
        {
            val = ReadBetween(val, "\"", "\"");
        }
        if (curNPCBuilder != null)
        {
            switch (varName)
            {
                case "timeBetweenChars":
                    curNPCBuilder.timeBetweenChars = float.Parse(val);
                    break;
                case "defaultSprite":
                    curNPCBuilder.defaultSpriteID = val;
                    break;
                case "font":
                    curNPCBuilder.fontID = val;
                    break;
                case "textColor":
                    curNPCBuilder.textColorID = val;
                    break;
                case "backgroundColor":
                    curNPCBuilder.backgroundColorID = val;
                    break;
                case "textBoxImage":
                    curNPCBuilder.textBoxImageID = val;
                    break;
                case "talkSound":
                    curNPCBuilder.talkSoundID = val;
                    break;
                case "defaultFontSize":
                    curNPCBuilder.defaultSpriteID = val;
                    break;
                default:
                    throw new System.Exception("NPCDataBuilder does not contain \"" + varName + "\" as a variable at line " + lineNum);
            }
        }
        else if (curDialogueBuilder != null)
        {
            throw new System.Exception("DialogueBuilder has no variables to set at line " + lineNum);
        }
        else
        {
            throw new System.Exception("No ScriptObject is being built right now at line " + lineNum);
        }
    }

    private static void AddMessage(string line, int lineNum)
    {
        if (curDialogueBuilder == null)
        {
            throw new System.Exception("Must start a DialogueObject before adding a message at line " + lineNum);
        }
        List<string> paramList = ReadListBetween(line, "[", "]", ",");
        if (paramList.Count < 3)
        {
            throw new System.Exception("Message must contain an npcID, name, and text at line " + lineNum);
        }
        string npcID = ReadBetween(paramList[0], "\"", "\"");
        string name = paramList[1];
        if (name.Contains("null") && !name.Contains("\""))
        {
            name = null;
        }
        else
        {
            name = ReadBetween(paramList[1], "\"", "\"");
        }
        string text = ReadBetween(paramList[2], "\"", "\"");
        float fontSize = -1;
        List<string> spriteIDs = new List<string>();
        if (paramList.Count > 3)
        {
            fontSize = float.Parse(paramList[3]);
        }
        if (paramList.Count > 4)
        {
            for (int i = 4; i < paramList.Count; i++)
            {
                if (paramList[i].Contains("\""))
                {
                    spriteIDs.Add(ReadBetween(paramList[i], "\"", "\""));
                }
                else
                {
                    spriteIDs.Add(paramList[i]);
                }
            }
        }
        curMessageList.Add(new BasicMessage(npcID, name, text, fontSize, spriteIDs));
    }
}

public class NPCData
{
    public float timeBetweenChars { get; private set; }
    public string defaultSpriteID { get; private set; }
    public string fontID { get; private set; }
    public string textColorID { get; private set; }
    public string backgroundColorID { get; private set; }
    public string textBoxImageID { get; private set; }
    public string talkSoundID { get; private set; }
    public float defaultFontSize { get; private set; }

    public class Builder
    {
        public float timeBetweenChars = .1f;
        public string defaultSpriteID = null;
        public string fontID = null;
        public string textColorID = null;
        public string backgroundColorID = null;
        public string textBoxImageID = null;
        public string talkSoundID = null;
        public float defaultFontSize = 67.7f;

        public Builder() { }

        public NPCData Build()
        {
            return new NPCData(timeBetweenChars, defaultSpriteID, fontID, textColorID,
                backgroundColorID, textBoxImageID, talkSoundID, defaultFontSize);
        }
    }

    private NPCData(float timeBetweenChars, string defaultSpriteID, string fontID, string textColorID, string backgroundColorID,
        string textBoxImageID, string talkSoundID, float defaultFontSize)
    {
        this.timeBetweenChars = timeBetweenChars;
        this.defaultSpriteID = defaultSpriteID;
        this.fontID = fontID;
        this.textColorID = textColorID;
        this.backgroundColorID = backgroundColorID;
        this.textBoxImageID = textBoxImageID;
        this.talkSoundID = talkSoundID;
        this.defaultFontSize = defaultFontSize;
    }
}