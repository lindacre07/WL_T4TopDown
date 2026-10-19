using Unity.Scripting.LifecycleManagement;

// ReSharper disable once InconsistentNaming
public partial class AH_HandleDoor : MonoBehaviour
{
    [SerializeField]
    private Transform destinationDoor;

    [SerializeField]
    private float transitionDuration = 1.5f;
    
    [AutoStaticsCleanup]
    private static HandlePlayer _playerScript;
    
    [AutoStaticsCleanup]
    private static Transform    _playerTransform;
    
    [AutoStaticsCleanup]
    private static Animator     _playerAnim;
    
    
    private GameObject   _nextRoom;
    private GameObject   _currentRoom;

    private const float TeleportOffset = 0.9f;

    private Transform _camera;

    private void Start()
    {
        if (!_playerTransform)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (!_playerAnim)
        {
            _playerAnim = _playerTransform.GetComponent<Animator>();
        }

        if (!_playerScript)
        {
            _playerScript = _playerTransform.GetComponent<HandlePlayer>();
        }

        if (!_camera)
        {
            _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        _nextRoom     = destinationDoor.parent.gameObject;
        _currentRoom  = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        StartCoroutine(TransitionRoom());
    }

    IEnumerator TransitionRoom()
    {
        Vector3 startPos = _camera.position;
        Vector3 endPos = new Vector3(startPos.x, _nextRoom.transform.position.y, _camera.position.z);
        float elapsedTime = 0f;

        _playerScript.enabled = false;
        _playerAnim.enabled   = false;
        Time.timeScale        = 0;
        _nextRoom.SetActive(true);
        
        while (elapsedTime < transitionDuration)
        {
            _camera.position = Vector3.Lerp(startPos, endPos, elapsedTime / transitionDuration);
            if (elapsedTime >= transitionDuration / 2)
            {
                // Avoids checking rotation of door, has drawback of being different depending on door rotation as Isaac feet hitbox is further from door
                // Might keep, it's the best solution so far. Offset might not even be needed and looks kinda shitty
                _playerTransform.position = destinationDoor.position - destinationDoor.up * TeleportOffset;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        _playerScript.enabled = true;
        _playerAnim.enabled   = true;
        Time.timeScale        = 1;
        _currentRoom.SetActive(false);
        _camera.position = endPos;
    }
}
