using UnityEngine;

namespace Code.Isolation
{
    [System.Serializable]
    public abstract class BaseType : ScriptableObject { }

    [System.Serializable]
    public class GenericType<T> : BaseType { /*base stuff*/ }

    [System.Serializable]
    public class ConcreteNonGenericType : GenericType<int> { /*concrete class for reflection and serialization purposes.*/ }

    public class TestScript : MonoBehaviour
    {
        [System.Serializable]
        public class ValueDictionary : SerializableDictionary<string, ScriptableObject> { }

        [SerializeField]
        protected ValueDictionary testDictionary = new ValueDictionary();

        void Awake()
        {
            testDictionary.Add("one", ScriptableObject.CreateInstance<ConcreteNonGenericType>());
            testDictionary.Add("two", ScriptableObject.CreateInstance<ConcreteNonGenericType>());
            testDictionary.Add("three", ScriptableObject.CreateInstance<ConcreteNonGenericType>());

            testDictionary.Remove("two");

            foreach (string key in testDictionary.Keys)
            {
                Debug.Log(key);
            }
            // output:
            // one
            // Null

            foreach (string key in testDictionary.AsDictionary.Keys)
            {
                Debug.Log(key);
            }
            // output:
            // one
            // three
        }

    }
}
