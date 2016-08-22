using UnityEngine;
using System.Collections;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string SubstringFromChars(this string str, char firstChar, char lastChar, bool includeChars = false)
        {
            int firstIndex = str.IndexOf(firstChar);
            int lastIndex = str.IndexOf(lastChar);
            if (includeChars)
            {
                return str.Substring(firstIndex, lastIndex - firstIndex);
            }
            else
            {
                return str.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
            }

        }

        public static string SubstringFromCharsSqueeze(this string str, char firstChar, char lastChar, bool includeChars = false)
        {
            int firstIndex = str.LastIndexOf(firstChar);
            int lastIndex = str.IndexOf(lastChar);
            if (includeChars)
            {
                return str.Substring(firstIndex, lastIndex - firstIndex);
            }
            else
            {
                return str.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
            }

        }
    }

}
