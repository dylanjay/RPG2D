using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Benco.Graph;

using Type = System.Type;
using Attribute = System.Attribute;
using Object = UnityEngine.Object;
using UnityEditor;

namespace Benco.BehaviorTree
{
    [System.Serializable]
    public class SharedVariableCollection : ISerializationCallbackReceiver
    {
        private static Type[] sharedVariableDerivedTypes =
                    (from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
                     from type in assembly.GetTypes()
                     where type.IsSubclassOf(typeof(SharedVariable)) && type.BaseType.IsGenericType
                     let attributes = (TypeNameOverrideAttribute[])
                                      type.GetCustomAttributes(typeof(TypeNameOverrideAttribute), false)
                     let name = attributes.Length > 0 ? attributes[0].newDisplayName : type.Name
                     orderby name
                     select type).ToArray();

        [System.Serializable]
        public struct NodeFieldPair
        {
            [SerializeField]
            public BehaviorNodeBase node;

            [SerializeField]
            public string fieldName;

            public NodeFieldPair(BehaviorNodeBase node, string fieldName)
            {
                this.node = node;
                this.fieldName = fieldName;
            }
        }

        [SerializeField]
        protected NodeBehaviorTree nodeGraph;

        [System.Serializable]
        protected class SGUIContentList : List<SGUIContent> { }

        //This pattern is the only way to properly serialize these variables, since Unity/C#
        //cannot serialize Generic objects by default.
        [System.Serializable]
        protected class DropdownDictionary : SerializableDictionary<string, SGUIContentList> { }

        /// <summary>
        /// All possible dropdowns. The key is the full name type of the object, the value is the list of GUIContent 
        /// options (SharedVariables).
        /// </summary>
        [SerializeField]
        protected DropdownDictionary dropdowns = new DropdownDictionary();

        [System.Serializable]
        protected class NodeFieldPairList : List<NodeFieldPair> { };

        [System.Serializable]
        protected class ReferenceDictionary : SerializableDictionary<SharedVariable, NodeFieldPairList> { }
        
        /// <summary>
        /// A dictionary of all references found within the tree. The key is the SharedVariable reference,
        /// The value is a hashset of Node/FieldName pairs that correspond to that variable.
        /// </summary>
        [SerializeField]
        protected ReferenceDictionary references = new ReferenceDictionary();

        [System.Serializable]
        protected class ValuesDictionary : SerializableDictionary<string, SharedVariable> { }
        
        /// <summary>
        /// A dictinary that holds all SharedVariable values. The key is the name, the value is the object
        /// stored for that SharedVariable name.
        /// </summary>
        [SerializeField]
        protected ValuesDictionary values = new ValuesDictionary();

        public NullSharedVariable none = null;
        public const string emptyChoice = "None";

        public SGUIContent defaultOption = new SGUIContent(emptyChoice);

        protected List<NodeFieldPair> unassigned
        {
            get { return references[none]; }
        }

        public SharedVariableCollection(NodeBehaviorTree nodeGraph)
        {
            none = ScriptableObject.CreateInstance<NullSharedVariable>();
            none.hideFlags = HideFlags.HideInHierarchy;
            none.name = emptyChoice;
            this.nodeGraph = nodeGraph;

            values[emptyChoice] = none;
            references.Add(none, new NodeFieldPairList());
        }

        /// <summary>
        /// Adds a new SharedVariable to the tracked collection of shared variables.
        /// </summary>
        /// <param name="name">The name of the new variable.</param>
        /// <param name="type">The type of the new variable.</param>
        public void AddVariable(string name, Type type)
        {
            string typeFullName = type.FullName;
            if (!Application.isEditor)
            {
                throw new System.InvalidOperationException("You cannot add variables during runtime.");
            }
            if (name == none.name)
            {
                Debug.LogError("For confusion reasons with the GUI, naming your variable \"None\" has been disabled." +
                    " Please choose a more verbose name.");
                return;
            }
            if (values.ContainsKey(name))
            {
                Debug.LogError("Error: a SharedVariable already exists in this tree with the given name.");
                return;
            }
            if (name.Length == 0)
            {
                Debug.LogError("Error: name must be at least one character long.");
                return;
            }

            if (dropdowns.ContainsKey(typeFullName))
            {
                //Add the name to the given type.
                dropdowns[typeFullName].Add(new SGUIContent(name));
            }
            else
            {
                //Create the GUIContent list for the given type.
                dropdowns[typeFullName] = new SGUIContentList() { new SGUIContent(name) };
            }


            System.Predicate<Type> findSharedType = varType => (varType.BaseType.GetGenericArguments()[0] == type);
            Type sharedVarType = System.Array.Find(sharedVariableDerivedTypes, findSharedType);

            if (sharedVarType == null)
            {
                Debug.LogError("Error: recieved a type that does not have an associated SharedVariable<> derived " +
                    "class. Type: " + type);
                return;
            }

            SharedVariable sharedVar = (SharedVariable)ScriptableObject.CreateInstance(sharedVarType);
            sharedVar.hideFlags = HideFlags.HideInHierarchy;
            sharedVar.name = name;
            values.Add(name, sharedVar);
            references.Add(sharedVar, new NodeFieldPairList());
            AssetDatabase.AddObjectToAsset(sharedVar, nodeGraph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return;
        }

        public void OnCreateAsset()
        {
            AssetDatabase.AddObjectToAsset(none, @"Assets/Resources/BehaviorTrees/" + nodeGraph.name + ".asset");
        }

        /// <summary>
        /// Removes a variable from the collection of shared variables.
        /// Prints out an error upon failure.
        /// </summary>
        /// <param name="name">The name of the variable we are trying to remove.</param>
        /// <param name="type"></param>
        public void RemoveVariable(string name, Type type)
        {
            string typeFullName = type.FullName;
            SharedVariable removedSharedVariable = values[name];
            if (!dropdowns.ContainsKey(typeFullName))
            {
                Debug.LogError("Error: tried to remove a value from a shared variable type that does not exist.");
                return;
            }

            List<SGUIContent> guiContents = dropdowns[typeFullName];
            SGUIContent match = guiContents.Find(x => x.text == name);

            if (references[removedSharedVariable].Count != 0)
            {
                Debug.LogWarning(string.Format("There nodes that are still currently using SharedVariable \"{0}\". " +
                    "These references will be cleared.", removedSharedVariable.name));
                foreach (NodeFieldPair reference in references[removedSharedVariable])
                {
                    BehaviorNodeBase node = reference.node;
                    FieldInfo choicesField = node.GetType().GetField("choices",
                                                                     BindingFlags.NonPublic | BindingFlags.Instance);
                    SerializableDictionary<string, SharedVariable> choices = 
                        (SerializableDictionary<string, SharedVariable>)choicesField.GetValue(node);

                    choices[reference.fieldName] = none;
                    unassigned.Add(reference);
                }
            }

            references.Remove(removedSharedVariable);

            if (guiContents.Count == 1)
            {
                dropdowns.Remove(typeFullName);
            }
            else
            {
                dropdowns[typeFullName].Remove(match);
            }

            values.Remove(name);

            Object.DestroyImmediate(removedSharedVariable, true);
            AssetDatabase.SaveAssets();
        }


        /// <summary>
        /// Renames a variable, keeping the same type it had before.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="type">The type the variable is.</param>
        /// <remarks>
        /// It's important to note that we don't want to remove the value and add it back in.
        /// If we did this, the nodes would lose their references to the SharedVariables, and
        /// all of them would have to be reset.
        /// </remarks>
        public void RenameVariable(string oldName, string newName, Type type)
        {
            string typeFullName = type.FullName;
            if (values.ContainsKey(newName))
            {
                Debug.LogError("Error: tried to rename variable to the same name as another variable.");
                return;
            }
            if (!dropdowns.ContainsKey(typeFullName))
            {
                Debug.LogError("Error: tried to rename a value from a shared variable type that does not exist.");
                return;
            }

            List<SGUIContent> sguiContents = dropdowns[typeFullName];
            SGUIContent sgui = sguiContents.Find(x => x.text == oldName);


            if (sgui == null)
            {
                //If we did not find and rename the value.
                Debug.LogError(string.Format("Error: could not find variable {0} of type {1} to rename.", oldName, type));
                return;
            }

            SharedVariable renamedVariable = values[oldName];
            renamedVariable.name = newName;
            values.Remove(oldName);
            values.Add(newName, renamedVariable);

            sgui.text = newName;
            sguiContents.OrderBy(x => x.text);
        }

        /// <summary>
        /// Changes the type of the SharedVariable from <paramref name="oldType"/> to <paramref name="newType"/>.
        /// </summary>
        /// <param name="oldType"></param>
        /// <param name="newType"></param>
        /// <param name="variableName"></param>
        /// <remarks>
        /// Because the type is changing, and the SharedVariable requires a specific type, we should clear out the 
        /// references in the nodes that contain this SharedVariable.
        /// </remarks>
        public void ChangeVariableType(Type oldType, Type newType, string variableName)
        {
            RemoveVariable(variableName, oldType);
            AddVariable(variableName, newType);
        }

        /// <summary>
        /// Gets a GUIContent[] populated with the names of each variable for that given type. Useful for calling 
        /// EditorGUI.Popup.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>All SharedVariable names with the given type in the form of a GUIContent[].</returns>
        public GUIContent[] GetDropdownOptions(Type type)
        {
            string typeFullName = type.FullName;
            if (dropdowns.ContainsKey(typeFullName))
            {
                List<GUIContent> list = new List<GUIContent> { new GUIContent(none.name) };
                list.AddRange(dropdowns[typeFullName].ConvertAll(sguiContent => (GUIContent)sguiContent));
                return list.ToArray();
            }
            else
            {
                return new GUIContent[1] { new GUIContent(none.name) };
            }
        }

        /// <summary>
        /// Sets a SharedVariable to the given variable associated with the GUIContent.
        /// </summary>
        /// <param name="node">The Node that is handling the behavior.</param>
        /// <param name="fieldName">The name of the field being set within the behavior.</param>
        /// <param name="option">The option selected in the dropdown menu. Must not be a copy.</param>
        public void SetReference(BehaviorNodeBase node, string fieldName, string prevOption, string currentOption)
        {
            NodeFieldPair sharedVarPair = new NodeFieldPair(node, fieldName);

            references[values[prevOption]].Remove(sharedVarPair);
            references[values[currentOption]].Add(sharedVarPair);

            Type nodeType = node.behaviorComponent.GetType();
            FieldInfo fieldInfo =
            nodeType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(node.behaviorComponent, values[currentOption]);
        }

        /// <summary>
        /// Clears a reference that corresponds to the given parameters.
        /// </summary>
        /// <param name="node">The Node that is handling the behavior.</param>
        /// <param name="fieldName">The name of the field being cleared within the behavior.</param>
        /// <param name="sharedVariableName">The previous option selected in the dropdown menu.
        ///                                  Must not be a copy.</param>
        public void RemoveReference(BehaviorNodeBase node, string fieldName, string sharedVariableName)
        {
            NodeFieldPair sharedVarPair = new NodeFieldPair(node, fieldName);
            references[values[sharedVariableName]].Remove(sharedVarPair);
            unassigned.Add(sharedVarPair);

            Type nodeType = node.behaviorComponent.GetType();
            nodeType.GetField(fieldName).SetValue(node.behaviorComponent, null);
        }

        /// <summary>
        /// Call this funciton when a node is added to the behavior tree.
        /// </summary>
        /// <param name="node">The new node.</param>
        public void AddBehavior(BehaviorNodeBase node)
        {
            Type behaviorType = node.behaviorComponent.GetType();

            foreach (FieldInfo fieldInfo in behaviorType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | 
                                                                   BindingFlags.Instance))
            {
                //If it is a shared variable and a SerializedField or isPublic
                if (fieldInfo.FieldType.IsSubclassOf(typeof(SharedVariable)) &&
                    (((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length > 0 ||
                        fieldInfo.IsPublic))
                {
                    unassigned.Add(new NodeFieldPair(node, fieldInfo.Name));
                }
            }
        }

        /// <summary>
        /// Call this function when a node is removed from the behavior tree.
        /// </summary>
        /// <param name="node">The node removed.</param>
        public void RemoveBehavior(BehaviorNodeBase node)
        {
            Type behaviorType = node.behaviorComponent.GetType();

            foreach (FieldInfo fieldInfo in behaviorType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | 
                                                                   BindingFlags.Instance))
            {
                //If it is a shared variable and a SerializedField or isPublic
                if (fieldInfo.FieldType.IsSubclassOf(typeof(SharedVariable)) &&
                    ((((Attribute[])fieldInfo.GetCustomAttributes(typeof(SerializeField), true)).Length > 0 &&
                        fieldInfo.IsPrivate) ||
                    (((Attribute[])fieldInfo.GetCustomAttributes(typeof(HideInInspector), true)).Length != 0 && 
                        fieldInfo.IsPublic)))
                {
                    object sharedVar = (fieldInfo.GetValue(node.behaviorComponent));
                    if (sharedVar == null)
                    {
                        unassigned.Remove(new NodeFieldPair(node, fieldInfo.Name));
                    }
                    else
                    {
                        Type varType = (Type)sharedVar.GetType().GetProperty("sharedType").GetGetMethod()
                                       .Invoke(sharedVar, new object[] { });

                        GUIContent guiContent = dropdowns[varType.FullName].ToList()
                                                .Find(x => x.text == fieldInfo.Name);
                        references[values[guiContent.text]].Remove(new NodeFieldPair(node, fieldInfo.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Call this function when the behavior within the node changes.
        /// </summary>
        /// <param name="node">The modified node.</param>
        public void NodeBehaviorChanged(BehaviorNodeBase node)
        {
            RemoveBehavior(node);
            AddBehavior(node);
        }

        /// <summary>
        /// Sets the value of the given SharedVariable option to the given value. Uses reflection.
        /// </summary>
        /// <param name="sharedVariableName">The option in the Popup dropdown menu.</param>
        /// <param name="value">The new value the SharedVariable should hold.</param>
        public void SetValue(string sharedVariableName, object value)
        {
            values[sharedVariableName].GetType().GetField("value").SetValue(values[sharedVariableName], value);
        }

        public IEnumerator<NodeFieldPair> GetAllUnAssigned()
        {
            List<NodeFieldPair>.Enumerator enumerator = unassigned.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public List<NodeFieldPair>.Enumerator GetUnAssignedEnumerator()
        {
            return unassigned.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, SharedVariable>> GetValueEnumerator()
        {
            return values.GetEnumerator();
        }

        public IDictionary<string, SharedVariable> GetValues()
        {
            return values;
        }

        public void FinishSetup()
        {
            if (unassigned.Any())
            {
                Debug.LogError("Error: Some nodes still have unassigned references. " +
                    "If this is intentional, set your variable as private/protected without a SerializedField " +
                    "attribute, or as public/internal with a HideInInspector attribute.");
                return;
            }

            foreach (KeyValuePair<string, SharedVariable> pair in values)
            {
                values[pair.Key].GetType().GetField("name").SetValue(values[pair.Key], pair);
            }
        }

        public void OnBeforeSerialize()
        {
            //Debug.Log("Before Serialize");
        }

        public void OnAfterDeserialize()
        {
            //Debug.Log("After DeSerialize");
        }


        //public void OnEnable()
        //{
        //    if (dropdowns.Keys == null) { dropdowns = new DropdownDictionary(); Debug.Log("Rebuilding Dropdowns."); }
        //    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(nodeGraph));
        //    foreach (Object asset in assets)
        //    {
        //        SharedVariable sharedVariable = asset as SharedVariable;
        //        if (sharedVariable != null)
        //        {
        //            if (asset is NullSharedVariable)
        //            {
        //                none = (NullSharedVariable)asset;
        //            }

        //            GUIContent content = new GUIContent(asset.name);
        //            Type sharedVarType = sharedVariable.sharedType;
        //            if (dropdowns.ContainsKey(sharedVarType))
        //            {
        //                dropdowns[sharedVarType].Add(content);
        //            }
        //            else
        //            {
        //                dropdowns[sharedVarType] = new GUIContentList { content };
        //            }
        //            //values.Add(asset.name, asset);
        //        }
        //    }
        //}
    }
}
