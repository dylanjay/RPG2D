using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static GameObject _gameObject = null;
    private static Transform _transform = null;
    private static object _lock = new object();
    private static bool appIsQuitting = false;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        Debug.LogError("Singleton " + typeof(T).ToString() + " not found. I'm not going to create a new object, so you better do it.");
                    }
                }
                _gameObject = _instance.gameObject;
                _transform = _instance.transform;
                DontDestroyOnLoad(_gameObject);
                return _instance;
            }
        }

        //psuedo-singleton. Probably bad design, but it works well for C# and Unity's scene structure.
        protected set
        {
            if (_instance == null)
            {
                _instance = value;
                _gameObject = _instance.gameObject;
                _transform = _instance.transform;
                DontDestroyOnLoad(_gameObject);
            }
            else
            {
                Debug.Log("Yo, you shouldn't already have an instance, I think.");
            }
        }
    }

    void OnDestroy() { }

    public static GameObject GameObject
    {
        get { return _gameObject; }
    }

    public static Transform Transform
    {
        get { return _transform; }
    }

    void OnApplicationQuit()
    {
        appIsQuitting = true;
    }
}
