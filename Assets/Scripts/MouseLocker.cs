using UnityEngine;

/*
 * Auto locks and hides the mouse while enabled
 * Disable (eg while paused) to free the mouse
 */
public class MouseLocker : MonoBehaviour
{
    public static MouseLocker Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    //Lock the cursor (mouse) and hide it
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    //Unlock the cursor (mouse) and show it
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}