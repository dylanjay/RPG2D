using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

// Place this file in any folder that is or is a descendant of a folder named "Editor"
namespace RPG2DAttributes
{
    [CanEditMultipleObjects] // Don't ruin everyone's day
    [CustomEditor(typeof(MonoBehaviour), true)] // Target all MonoBehaviours and descendants
    public class MonoBehaviourCustomEditor : Editor
    {
        void OnEnable()
        {
            Type type = target.GetType();
            foreach (PropertyInfo method in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                // make sure it is decorated by our custom attribute
                Attribute[] attributes = (Attribute[])method.GetCustomAttributes(typeof(ShowTogglePropertyAttribute), true);
                if (attributes.Length > 0)
                {
                    method.GetSetMethod().Invoke(target, new object[] { (bool)method.GetGetMethod().Invoke(target, null) });
                }

                attributes = (Attribute[])method.GetCustomAttributes(typeof(ShowNumberPropertyAttribute), true);
                if (attributes.Length > 0)
                {
                    method.GetSetMethod().Invoke(target, new object[] { (int)method.GetGetMethod().Invoke(target, null) });
                }
            }
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // Draw the normal inspector
            
            // Get the type descriptor for the MonoBehaviour we are drawing
            Type type = target.GetType();
                
            // Iterate over each private or public instance method (no static methods atm)
            foreach (PropertyInfo method in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                // make sure it is decorated by our custom attribute
                Attribute[] attributes = (Attribute[])method.GetCustomAttributes(typeof(ShowTogglePropertyAttribute), true);
                if (attributes.Length > 0)
                {
                    //ShowTogglePropertyAttribute attribute = (ShowTogglePropertyAttribute)attributes[0];
                    bool underlyingValue = (bool)method.GetGetMethod().Invoke(target, null);
                    if (EditorGUILayout.Toggle(FixName(method.Name), underlyingValue) != underlyingValue)
                    {
                        //Invokes the setter method
                        method.GetSetMethod().Invoke(target, new object[] { !underlyingValue } );
                    }
                }

                attributes = (Attribute[])method.GetCustomAttributes(typeof(ShowNumberPropertyAttribute), true);
                if (attributes.Length > 0)
                {
                    //ShowNumberPropertyAttribute attribute = (ShowNumberPropertyAttribute)attributes[0];
                    int underlyingValue = (int)method.GetGetMethod().Invoke(target, null);
                    int recievedValue = EditorGUILayout.DelayedIntField(FixName(method.Name), underlyingValue);
                    if (recievedValue != underlyingValue)
                    {
                        //Invokes the setter method
                        method.GetSetMethod().Invoke(target, new object[] { recievedValue });
                    }
                }

                attributes = (Attribute[])method.GetCustomAttributes(typeof(ShowStringPropertyAttribute), true);
                if (attributes.Length > 0)
                {
                    //ShowNumberPropertyAttribute attribute = (ShowNumberPropertyAttribute)attributes[0];
                    string underlyingValue = (string)method.GetGetMethod().Invoke(target, null);
                    string recievedValue = EditorGUILayout.TextField(FixName(method.Name), underlyingValue);
                    if (recievedValue != underlyingValue)
                    {
                        //Invokes the setter method
                        method.GetSetMethod().Invoke(target, new object[] { recievedValue });
                    }
                }
            }
        }

        private string FixName(string name)
        {
            return EditorUtilities.FixName(name);
        }
    }
}