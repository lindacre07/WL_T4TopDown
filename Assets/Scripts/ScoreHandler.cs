public class ScoreHandler : MonoBehaviour
{
    private TMP_Text _scoreText;
    private int _score = 0;

    private void Awake()
    {
        _scoreText = GameObject.Find("ScoreDisplay").GetComponent<TMP_Text>();
    }

    public int Score
    {
        get => _score;
        set => ChangeScore(value);
    }

    private void ChangeScore(int value)
    {
        _score = value;
        _scoreText.text = _score.ToString();
    }
}
