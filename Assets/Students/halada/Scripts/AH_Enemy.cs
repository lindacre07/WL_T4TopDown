// ReSharper disable once InconsistentNaming
public abstract class AH_Enemy : MonoBehaviour
{
    private float _health = 10f;
    private ScoreHandler _scoreHandler;

    private void Awake()
    {
        _scoreHandler = FindAnyObjectByType<Canvas>().GetComponent<ScoreHandler>();
    }

    // ReSharper disable once InconsistentNaming
    public float Health
    {
        get => _health;
        set => _health = value;
    }

    public void Damage(float damage)
    {
        _health -= damage;
    }

    protected virtual void Update()
    {
        if (_health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _scoreHandler.Score += 100;
    }
}
