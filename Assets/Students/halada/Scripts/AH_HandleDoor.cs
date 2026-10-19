using UnityEngine;

public class AH_HandleDoor : MonoBehaviour
{
    [SerializeField]
    private Transform destinationDoor;
    [SerializeField]
    private float transitionDuration = 1.5f;
    private HandlePlayer _playerScript;
    private Transform _playerTransform;
    private Animator _playerAnim;
    private GameObject _nextRoom;
    private GameObject _currentRoom;

    private float _teleportOffset = 0.9f;


    private Transform _camera;
    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _playerAnim = _playerTransform.GetComponent<Animator>();
        _playerScript = _playerTransform.GetComponent<HandlePlayer>();
        _nextRoom = destinationDoor.parent.gameObject;
        _currentRoom = transform.parent.gameObject;
        _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        StartCoroutine(TransitionRoom(other.gameObject));
    }

    IEnumerator TransitionRoom(GameObject go)
    {
        var animator = go.GetComponent<Animator>();
        animator.SetTrigger("Enter Up");

        yield return new WaitForSeconds(1);

        animator.SetTrigger("Exit Down");

        Vector3 startPos = _camera.position;
        Vector3 endPos = new Vector3(startPos.x, destinationDoor.parent.transform.position.y, _camera.position.z);
        float elapsedTime = 0f;

        _playerScript.enabled = false;
        _playerAnim.enabled = false;
        Time.timeScale = 0;

        while (elapsedTime < transitionDuration)
        {
            _camera.position = Vector3.Lerp(startPos, endPos, elapsedTime / transitionDuration);
            if (elapsedTime >= transitionDuration / 2)
            {
                // Avoids checking rotation of door, has drawback of being different depending on door rotation as Isaac feet hitbox is further from door
                // Might keep, it's the best solution so far. Offset might not even be needed and looks kinda shitty
                _nextRoom.SetActive(true);
                _playerTransform.position = destinationDoor.position - destinationDoor.up * _teleportOffset;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        _playerScript.enabled = true;
        _playerAnim.enabled = true;
        Time.timeScale = 1;
        _currentRoom.SetActive(false);
        _camera.position = endPos;
    }
}
