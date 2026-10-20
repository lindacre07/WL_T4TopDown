using Unity.Scripting.LifecycleManagement;

public partial class Bomb : MonoBehaviour
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

    [AutoStaticsCleanup]
    private static WaitForSeconds _explosionDelay;

    [AutoStaticsCleanup]
    private static WaitForSeconds _flashTime;

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
    }

    IEnumerator Start()
    {
        var renderer = GetComponent<SpriteRenderer>();

        Color color = renderer.color;

        yield return _explosionDelay;

        for (int i = 0; i < numberOfFlashes; i++)
        {
            renderer.color = flashColor;
            yield return _flashTime;

            renderer.color = color;
            yield return _flashTime;
        }

        Instantiate(
                explosion,
                transform.position +
                explosionOffset,
                explosion.transform.rotation);

        Destroy(gameObject);
    }
}
