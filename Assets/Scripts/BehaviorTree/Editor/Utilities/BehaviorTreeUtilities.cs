using System;
using System.Linq;
using UnityEngine;
using Benco.Utilities;

namespace Benco.BehaviorTree
{
    public class BehaviorTreeUtilities : MonoBehaviour
    {
        private static Type[] _validTypes;
        private static Type[] _sharedVariableDerivedTypes;
        private static GUIContent[] _validTypeOptions;

        public static void InitializeValidTypes()
        {
            if (_validTypeOptions == null)
            {
                _validTypes =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsSubclassOf(typeof(SharedVariable)) && type.BaseType.IsGenericType
                     let attributes = type.GetAttributes<TypeNameOverrideAttribute>(false)
                     let name = attributes.Length > 0 ? attributes[0].newDisplayName : type.BaseType.GetGenericArguments()[0].Name
                     orderby name
                     select type.BaseType.GetGenericArguments()[0]).ToArray();

                _validTypeOptions =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsSubclassOf(typeof(SharedVariable)) && type.BaseType.IsGenericType
                     let attributes = type.GetAttributes<TypeNameOverrideAttribute>(false)
                     let name = attributes.Length > 0 ? attributes[0].newDisplayName : type.BaseType.GetGenericArguments()[0].Name
                     orderby name
                     select new GUIContent(name)).ToArray();

                _sharedVariableDerivedTypes =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsSubclassOf(typeof(SharedVariable)) && type.BaseType.IsGenericType
                     let attributes = type.GetAttributes<TypeNameOverrideAttribute>(false)
                     let name = attributes.Length > 0 ? attributes[0].newDisplayName : type.Name
                     orderby name
                     select type).ToArray();
            }
        }

        public static GUIContent[] validTypeOptions
        {
            get
            {
                if (_validTypeOptions == null)
                {
                    InitializeValidTypes();
                }
                return _validTypeOptions;
            }
        }

        public static Type[] validTypes
        {
            get
            {
                if (_validTypes == null)
                {
                    InitializeValidTypes();
                }
                return _validTypes;
            }
        }

        public static Type[] sharedVariableDerivedTypes
        {
            get
            {
                if (_sharedVariableDerivedTypes == null)
                {
                    InitializeValidTypes();
                }
                return _sharedVariableDerivedTypes;
            }
        }
    }
}