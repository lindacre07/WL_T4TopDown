public interface IItemPool
{
    private static List<AH_ItemDropperItem> _items { get; set; }

    public static void InitItemPool(List<AH_ItemDropperItem> items)
    {
        _items = items;
    }

    public static GameObject GetItemFromPool()
    {
        float totalWeight = 0;

        foreach (var item in _items)
        {
            totalWeight += item.weight;
        }

        var rand = Random.Range(0, totalWeight);

        float cursor = 0;

        foreach (var item in _items)
        {
            cursor += item.weight;

            if (cursor >= rand)
            {
                return item.itemPrefab;
            }
        }

        return null;
    }
}
