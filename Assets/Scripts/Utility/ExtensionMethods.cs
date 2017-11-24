using System;
using System.Linq;
using MemberInfo = System.Reflection.MemberInfo;
using FieldInfo = System.Reflection.FieldInfo;
using BindingFlags = System.Reflection.BindingFlags;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Creates a copy of this array, with element appended to the end.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array being copied.</param>
        /// <param name="element">The element to append to the array.</param>
        /// <returns></returns>
        public static T[] CopyAdd<T>(this T[] array, T element)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = element;
            return newArray;
        }

        public static T[] CopyPushFront<T>(this T[] array, T element)
        {
            T[] newArray = new T[array.Length + 1];
            newArray[0] = element;
            Array.Copy(array, 0, newArray, 1, array.Length);
            return newArray;
        }

        /// <summary>
        /// Removes the element at <paramref name="index"/> from <paramref name="array"/>.
        /// </summary>
        /// <returns>A copy of the new array.</returns>
        /// <remarks>
        /// This function is assert guarded, but will do the following when those asserts are compiled out:
        /// If index is less than 0, it will remove the first element.
        /// If index is gt or eq to <paramref name="array"/>.Length, it will remove the last element.
        /// </remarks>
        public static T[] WithIndexRemoved<T>(this T[] array, int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < array.Length);

            T[] retArray = new T[array.Length - 1];
            int i;
            for (i = 0; i < index; i++)
            {
                retArray[i] = array[i];
            }
            for (i++; i < array.Length - 1; i++)
            {
                retArray[i] = array[i + 1];
            }
            return retArray;
        }

        public static int GetIndex<T>(this T[] array, T element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if(array[i].Equals(element)) { return i; }
            }
            return -1;
        }

        public static int IndexOf<T>(this T[] array, Predicate<T> match)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (match(array[i])) { return i; }
            }
            return -1;
        }

        public static T Find<T>(this T[] array, Predicate<T> match)
        {
            return Array.Find(array, match);
        }

        public static bool HasAttribute<T>(this MemberInfo fieldInfo, bool inherit = false) where T : Attribute
        {
            return ((T[])fieldInfo.GetCustomAttributes(typeof(T), inherit)).Length != 0;
        }

        public static T GetAttribute<T>(this MemberInfo fieldInfo, bool inherit = false) where T : Attribute
        {
            return ((T[])fieldInfo.GetCustomAttributes(typeof(T), inherit)).FirstOrDefault();
        }

        public static FieldInfo[] GetInstanceFields(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public delegate string PrintDelegate<T>(T t);

        public static void PrintAll<T>(this IEnumerable<T> list, PrintDelegate<T> lambda)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (T t in list)
            {
                stringBuilder.Append(lambda(t));
                stringBuilder.Append(", ");
            }
            Debug.Log(stringBuilder.ToString());
        }
    }
}
