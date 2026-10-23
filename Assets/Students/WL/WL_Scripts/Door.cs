using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private float timer = 20f;

    [SerializeField]
    private float slideSpeed = 2f;

    [SerializeField]
    private float slideDistance = 3f;

    private Vector3 targetPos;

    private bool _isOpen;
    private GameObject _closedDoor;
    private GameObject _openedDoor;
    
    
    void Start()
    {
        _closedDoor = transform.Find("Closed Door").gameObject;
        _openedDoor = transform.Find("Open Door").gameObject;
    }

    void Update()
    {
       if (!_isOpen)
       {
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            Open();
        }
       }
    }

    void Open()
    {
        _isOpen = true;
        targetPos = transform.position + Vector3.up * slideDistance;
        _closedDoor.SetActive(false);
    }
}
