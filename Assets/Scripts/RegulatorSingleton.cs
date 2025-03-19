using UnityEngine;


/// <summary>
/// Persistent Regulator singleton, will destroy any older components of type It finds on Awake
/// </summary>
/// <typeparam name="T"></typeparam>
public class RegulatorSingleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;

    public static bool HasInstance => instance != null;

    public float InitializationTime { get; private set; }

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    var go = new GameObject(typeof(T).Name + " Auto-Generated");
                    go.hideFlags = HideFlags.HideAndDontSave;
                    instance = go.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// Make sur to call base.Awake() in override if yo need awake.
    /// </summary>

    protected virtual void Awake()
    {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if (Application.isPlaying) return;

        InitializationTime = Time.time;
        DontDestroyOnLoad(gameObject);

        T[] oldinstances = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach(T old in oldinstances)
        {
            if(old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime)
            {
                Destroy(old.gameObject);
            }
        }

        if (instance == null)
        {
            instance = this as T;
        }
    }
}
