[Serializable]
public class Item
{
    public string name;
    public string description;
    public Sprite itemImage;
}

public class PassiveItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Item item;
    public int healthChange;
    public float moveSpeedChange;
    public float tearSizeChange;
    public float tearRateChange;
    public float tearSpeedChange;
    public float damageChange;
    public float rangeChange;
    public int tearAmountChange;




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayer.MoveSpeedChange(moveSpeedChange);
            HandlePlayer.HealthChange(healthChange);
            HandlePlayer.TearSizeChange(tearSizeChange);
            HandlePlayer.FireRateChange(tearRateChange);
            HandlePlayer.tearRangeChange(rangeChange);
            HandlePlayer.tearSpeedChange(tearSpeedChange);
            HandlePlayer.tearAmountChange(tearAmountChange);
            Destroy(gameObject);
        }
    }
}
