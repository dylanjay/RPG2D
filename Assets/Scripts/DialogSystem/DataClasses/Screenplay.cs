using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

//This is a standin class until a I implement a proper class.
public class Dialogue { }

/// <summary>
/// This class is a work in progress.
/// 
/// Rules: 
/// Comments must have the first character as a # sign. There cannot be whitespace before it.
/// 
/// </summary>

public class Screenplay : MonoBehaviour {

    Dictionary<string, Dialogue> dialogues;

    public Screenplay(string filename)
    {
        string[] screenplayLines = File.ReadAllLines(filename);

        for (int i = 0; i < screenplayLines.Length; i++)
        {
            if (screenplayLines[i].Length > 0 && screenplayLines[i][0] == '#')
            {
                string curLine = screenplayLines[i].Trim();

                MatchCollection matchCollection = Regex.Matches(curLine, "{(.*?)}");

                for(int j = 0; j < matchCollection.Count; j++)
                {
                    string dialogVariable = matchCollection[i].Captures[1].Value;
                }
            }
        }
    }

    private enum TagType { None, Float, Int, String, Bool, PositionEnum }
    private enum TextTag { Bold, Italics, Speed, Rainbow, Shake, Underline, Glow }
    private enum PositionEnum { }

    private enum DialogueTag { Script, EndScript, Pause, Skippable, Position, Character, Option }

    private static class DialogueTags
    {
        private struct DialogueTagInfo
        {
            public readonly DialogueTag enumVal;
            public readonly string name;
            public readonly System.Type type;

            public DialogueTagInfo(DialogueTag enumVal, string name, System.Type type)
            {
                this.enumVal = enumVal;
                this.name = name.ToUpperInvariant();
                this.type = type;
            }
        }

        private static DialogueTagInfo[] dialogueTags = new DialogueTagInfo[]
        {
            new DialogueTagInfo(enumVal: DialogueTag.Script,    name: "SCRIPT",    type: typeof(string)),
            new DialogueTagInfo(enumVal: DialogueTag.EndScript, name: "ENDSCRIPT", type: typeof(void)),
            new DialogueTagInfo(enumVal: DialogueTag.Pause,     name: "PAUSE",     type: typeof(bool)),
            new DialogueTagInfo(enumVal: DialogueTag.Skippable, name: "SKIPPABLE", type: typeof(bool)),
            new DialogueTagInfo(enumVal: DialogueTag.Position,  name: "POSITION",  type: typeof(PositionEnum)),
            new DialogueTagInfo(enumVal: DialogueTag.Character, name: "CHARACTER", type: typeof(string)),
            new DialogueTagInfo(enumVal: DialogueTag.Option,    name: "OPTION",    type: typeof(string))
        };

        private static void ParseError(int lineNumber, string rawValue, string dialogueTagName, string typeName)
        {
            Debug.LogError(
                string.Format(
                    "Error on line {0}: Cannot parse \"{1}\" to {2}'s expected type \"{3}\".",
                    lineNumber,
                    rawValue,
                    dialogueTagName,
                    typeName
            ));
        }

        private static object MatchTag(string property, out DialogueTag dialogueTag, int lineNumber = -1)
        {
            int endTag = property.IndexOf('=');

            if (endTag < 0)
            {
                endTag = property.Length;
            }
            string tag = property.Substring(0,endTag);

            for (int i = 0; i < dialogueTags.Length; i++)
            {
                if (tag.Equals(dialogueTags[i].name, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    //alternatively: dialogueTag = (DialogueTag)i;
                    dialogueTag = dialogueTags[i].enumVal;

                    System.Type type = dialogueTags[i].type;
                    if(type.IsEnum)
                    {
                        return System.Enum.Parse(type, property);
                    }
                    else if(type == typeof(int))
                    {
                        int propertyValue;
                        string rawValue = property.Substring(endTag + 1);
                        if (!int.TryParse(rawValue, out propertyValue))
                        {
                            ParseError(lineNumber, rawValue, dialogueTags[i].name, type.Name);
                        }
                        return propertyValue;
                    }
                    else if (type == typeof(float))
                    {
                        float propertyValue;
                        string rawValue = property.Substring(endTag + 1);
                        if (!float.TryParse(rawValue, out propertyValue))
                        {
                            ParseError(lineNumber, rawValue, dialogueTags[i].name, type.Name);
                        }
                        return propertyValue;
                    }
                    else if(type == typeof(string))
                    {
                        return property.Substring(endTag + 1);
                    }
                    else if(type == typeof(bool))
                    {
                        bool propertyValue;
                        string rawValue = property.Substring(endTag + 1);
                        if (!bool.TryParse(rawValue, out propertyValue))
                        {
                            ParseError(lineNumber, rawValue, dialogueTags[i].name, type.Name);
                        }
                        return propertyValue;
                    }
                    else
                    {
                        Debug.LogError(
                            string.Format("Error (line {0}): Trying to parse uncaught type {1} for tag \"{2}\".", 
                                lineNumber, 
                                type.ToString(), 
                                dialogueTags[i].name
                        ));
                        return null;
                    }
                }
            }
            Debug.LogError(string.Format("Error on line {0}: Unrecognized DialogueTag \"{1}\".", lineNumber, tag));
            dialogueTag = (DialogueTag)(-1);
            return null;
        }
    }

    
}
