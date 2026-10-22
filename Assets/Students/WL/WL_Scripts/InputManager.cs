public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    
    [HideInInspector] public InputSystem_Actions controls;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
