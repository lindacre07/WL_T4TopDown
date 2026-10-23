using System.Linq;

public class AH_ItemDropper : MonoBehaviour
{
    [SerializeField]
    List<AH_ItemDropperItem> items = new List<AH_ItemDropperItem>();
    bool isQuitting = false;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (isQuitting)
        {
            return;
        }

        float totalWeight = 0;
        
        foreach (var item in items)
        {
            totalWeight += item.weight;
        }
        
        var rand = Random.Range(0, totalWeight);

        float cursor = 0;
        
        foreach (var item in items)
        {
            cursor += item.weight;
            
            if (cursor >= rand)
            {
                Instantiate(item.itemPrefab, transform.position, transform.rotation);
                return;
            }
        }
    }
}
