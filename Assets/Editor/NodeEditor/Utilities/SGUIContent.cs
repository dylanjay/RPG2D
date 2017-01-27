using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SGUIContent : GUIContent
{
    public static readonly new SGUIContent none = new SGUIContent(GUIContent.none);

    public SGUIContent(string text) : base(text)
    {
    }

    public SGUIContent(string text, string tooltip) : base(text, tooltip)
    {
    }

    public SGUIContent(GUIContent guiContent) : base(guiContent)
    {
    }
}
