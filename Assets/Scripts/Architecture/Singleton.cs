using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> where T : class, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null) _instance = new T();

            return _instance;
        }
    }

    protected Singleton() { }
}

public abstract class MonoSingleton : MonoBehaviour
{
    // 씬 전환시 싱글턴 오브젝트의 유지 여부를 설정하는 플래그
    [SerializeField] private bool _isPersistent = true;

    // 일반적으로 싱글턴은 앱 종료시 제거됩니다. 이때 유니티가 임의의 순서로 오브젝트를 제거하기
    // 때문에 싱글턴 객체가 이미 제거된 시점에 외부에서 싱글턴으로 접근하면 싱글턴 오브젝트가 
    // 다시 생성됩니다. 따라서 이를 방지하기 위해 싱글턴 오브젝트가 제거중인지를 체크하는 
    // 플래그를 추가합니다.
    protected static bool _isQuitting = false;

    // 스레드 세이프를 위한 코드입니다.
    protected static readonly object _lock = new object();

    protected virtual void Awake()
    {
        if (_isPersistent)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        OnAwake();
    }

    private void Start()
    {
        // 씬 변경시 파괴되지 않고 유지되는 오브젝트는 Awake, Start 함수를 다시 호출하지 
        // 않기 때문에 변경된 씬에서 싱글턴 클래스의 초기화가 필요한 경우 
        // SceneManager.sceneLoaded 델리게이트를 이용합니다.
        // Awake -> OnEnable -> sceneLoaded -> Start 함수순으로 실행되기에 
        // Awake 함수 내에서 델리게이트 연결시 최초 씬에서도 OnSceneLoaded 함수가 호출됩니다.
        // 이를 방지하기 위해 Awake 함수가 아니라 Start 함수 내에서 델리게이트를 연결합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;

        OnStart();
    }

    private void OnDestroy()
    {
        _isQuitting = true;

        if (_isPersistent)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }
}

public abstract class MonoSingleton<T> : MonoSingleton where T : MonoBehaviour
{
    private static T mbInstance;
    public static T Instance
    {
        get
        {
            if (_isQuitting)
            {
                Debug.LogWarning($"[MonoSingleton] Instance '{typeof(T)}' is called while in Destroy Process");

                return null;
            }

            lock (_lock)
            {
                if (mbInstance != null)
                {
                    return mbInstance;
                }

                var instances = FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
                int count = instances.Length;

                if (count > 0)
                {
                    if (count == 1)
                    {
                        return mbInstance = instances[0];
                    }

                    Debug.LogWarning($"[{nameof(MonoSingleton)}<{typeof(T)}>] is Duplicated");

                    for (int i = 1; i < instances.Length; i++) {
                        Destroy(instances[i]);
                    }
                    return mbInstance = instances[0];
                }

                Debug.Log($"[{nameof(MonoSingleton)}<{typeof(T)}>] is Created");

                return mbInstance = new GameObject($"({nameof(MonoSingleton)}){typeof(T)}").AddComponent<T>();
            }
        }
    }

    protected sealed override void Awake()
    {
        if (mbInstance == null)
        {
            mbInstance = this as T;
        }
        else if (mbInstance != this)
        {
            Destroy(this.gameObject);
        }

        base.Awake();
    }
}