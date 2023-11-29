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
    // �� ��ȯ�� �̱��� ������Ʈ�� ���� ���θ� �����ϴ� �÷���
    [SerializeField] private bool _isPersistent = true;

    // �Ϲ������� �̱����� �� ����� ���ŵ˴ϴ�. �̶� ����Ƽ�� ������ ������ ������Ʈ�� �����ϱ�
    // ������ �̱��� ��ü�� �̹� ���ŵ� ������ �ܺο��� �̱������� �����ϸ� �̱��� ������Ʈ�� 
    // �ٽ� �����˴ϴ�. ���� �̸� �����ϱ� ���� �̱��� ������Ʈ�� ������������ üũ�ϴ� 
    // �÷��׸� �߰��մϴ�.
    protected static bool _isQuitting = false;

    // ������ �������� ���� �ڵ��Դϴ�.
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
        // �� ����� �ı����� �ʰ� �����Ǵ� ������Ʈ�� Awake, Start �Լ��� �ٽ� ȣ������ 
        // �ʱ� ������ ����� ������ �̱��� Ŭ������ �ʱ�ȭ�� �ʿ��� ��� 
        // SceneManager.sceneLoaded ��������Ʈ�� �̿��մϴ�.
        // Awake -> OnEnable -> sceneLoaded -> Start �Լ������� ����Ǳ⿡ 
        // Awake �Լ� ������ ��������Ʈ ����� ���� �������� OnSceneLoaded �Լ��� ȣ��˴ϴ�.
        // �̸� �����ϱ� ���� Awake �Լ��� �ƴ϶� Start �Լ� ������ ��������Ʈ�� �����մϴ�.
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