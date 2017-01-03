public class EditorUtilities
{
    public static string FixName(string name)
    {
        string fixedString = name;
        if (name.Contains("_EditorOnly_"))
        {
            fixedString = fixedString.Remove(name.IndexOf("_EditorOnly_"), "_EditorOnly_".Length);
        }
        fixedString = fixedString.Replace('_', ' ');
        for (int i = 1; i < fixedString.Length; i++)
        {
            if (char.IsUpper(fixedString[i]))
            {
                fixedString = fixedString.Insert(i, " ");
                i++;
            }
        }

        fixedString = char.ToUpper(fixedString[0]) + fixedString.Substring(1);

        return fixedString;
    }
}