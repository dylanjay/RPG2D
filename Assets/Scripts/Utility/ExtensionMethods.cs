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
    public delegate IComparable GetComparable<T>(T t);
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

        /// <summary>
        /// A helper function for <see cref="FindSortedPosition()"/>
        /// </summary>
        public static int _FindSortedPosition<T>(this IList<T> list, T element, Comparison<T> compare, int start, int end)
        {
            if (start >= end)
            {
                return start;
            }
            int currentPosition = (end + start) / 2;
            int comparison = compare(element, list[currentPosition]);
            if (comparison < 0)
            {
                return _FindSortedPosition(list, element, compare, start, currentPosition);
            }
            else if (comparison > 0)
            {
                return _FindSortedPosition(list, element, compare, currentPosition + 1, end);
            }
            else
            {
                // In the event of a tie, allow the existing element to maintain its place.
                return currentPosition + 1;
            }
        }

        /// <summary>
        /// Finds the position the element would have if it was added to the array.
        /// Note that his is different than FindSorted().
        /// 
        /// In the event of a tie, the element will be added after the element that already 
        /// existed within the array.
        /// </summary>
        /// <param name="comparison">The comparer the list has been sorted with.</param>
        public static int FindSortedPosition<T>(this IList<T> list, T element, Comparison<T> comparison)
        {
            return _FindSortedPosition(list, element, comparison, 0, list.Count);
        }

        /// <summary>
        /// Inserts an element into a copy of a sorted array, and returns the new array.
        /// 
        /// In the event of a tie, the element will be added after the element that already 
        /// existed within the array.
        /// </summary>
        public static T[] InsertSorted<T>(this T[] sortedArray, T element) where T : IComparable
        {
            Comparison<T> comparison = (x, y) => x.CompareTo(y);
            int position = sortedArray.FindSortedPosition(element, comparison);
            T[] newArray = new T[sortedArray.Length + 1];
            for (int i = 0; i < position; i++)
            {
                newArray[i] = sortedArray[i];
            }
            newArray[position] = element;
            for (int i = position + 1; i < newArray.Length; i++)
            {
                newArray[i] = sortedArray[i - 1];
            }
            return newArray;
        }

        /// <summary>
        /// Inserts an element into the list. Returns the list for fluent idiom.
        /// 
        /// In the event of a tie, the element will be added after the element that already 
        /// existed within the array.
        /// </summary>
        public static List<T> InsertSorted<T>(this List<T> sortedList, T element) where T : IComparable
        {
            Comparison<T> comparison = (x, y) => x.CompareTo(y);
            int position = sortedList.FindSortedPosition(element, comparison);
            sortedList.Insert(position, element);
            return sortedList;
        }

        public static List<T> InsertSorted<T>(this List<T> sortedList, T element, GetComparable<T> comp)
        {
            Comparison<T> comparison = (x, y) => comp(x).CompareTo(comp(y));
            int position = sortedList.FindSortedPosition(element, comparison);
            sortedList.Insert(position, element);
            return sortedList;
        }

        /// <summary>
        /// Inserts an element into the list. Returns the list for fluent idiom.
        /// 
        /// In the event of a tie, the element will be added after the element that already 
        /// existed within the array.
        /// </summary>
        public static List<T> InsertSorted<T>(this List<T> sortedList, T element, Comparison<T> comparison)
        {
            int position = sortedList.FindSortedPosition(element, comparison);
            sortedList.Insert(position, element);
            return sortedList;
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

        public static int FindIndex<T>(this IList<T> array, Predicate<T> match)
        {
            for (int i = 0; i < array.Count; i++)
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
