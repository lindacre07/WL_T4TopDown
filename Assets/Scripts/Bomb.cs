using Cysharp.Threading.Tasks;
using Unity.Scripting.LifecycleManagement;

public partial class Bomb : MonoBehaviour, IItemPool
{
    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private Vector3 explosionOffset = new Vector3(0.1f, -0.25f, 0);

    [SerializeField]
    private float explosionDelay = 1;

    [SerializeField]
    private int numberOfFlashes = 10;

    [SerializeField]
    private float flashTime = 0.05f;

    [SerializeField]
    private Color flashColor = Color.red;

    [SerializeField]
    private List<AH_ItemDropperItem> items = new();

    [AutoStaticsCleanup]
    private static WaitForSeconds _explosionDelay;

    [AutoStaticsCleanup]
    private static WaitForSeconds _flashTime;

    [AutoStaticsCleanup]
    private static GameObject _cachedExplosion;

    private void Awake()
    {
        if (_explosionDelay == null)
        {
            _explosionDelay = new WaitForSeconds(explosionDelay);
        }

        if (_flashTime == null)
        {
            _flashTime = new WaitForSeconds(flashTime);
        }

        if (!_cachedExplosion)
        {
            _cachedExplosion = Instantiate(explosion);
            _cachedExplosion.SetActive(false);
        }
    }

    async UniTaskVoid Start()
    {
        Debug.Log("Ran start script");

        IItemPool.InitItemPool(items);

        var renderer = GetComponent<SpriteRenderer>();

        Color color = renderer.color;
        
        await UniTask.WaitForSeconds(explosionDelay);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            renderer.color = flashColor;
            await UniTask.WaitForSeconds(flashTime);

            renderer.color = color;
            await UniTask.WaitForSeconds(flashTime);
        }

        _cachedExplosion.SetActive(true);
        _cachedExplosion.transform.position = transform.position + explosionOffset;

        Instantiate(
                IItemPool.GetItemFromPool(),
                transform.position,
                Quaternion.identity);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Start();
    }
}
