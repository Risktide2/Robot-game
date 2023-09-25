using System;
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

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}