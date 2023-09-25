using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static bool _paused = false;
    
    public UnityEvent<bool> pause;
    public UnityEvent<bool> pauseNot;

    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePaused();
    }

    public void Unpause()
    {
        TogglePaused(); //Assuming this is called only when is paused
    }
    //Pause or unpause
    private void TogglePaused()
    {
        _paused = !_paused;
        Time.timeScale = _paused ? 0 : 1;
        
        //Lock/unlock mouse
        if(MouseLocker.Instance != null)
            MouseLocker.Instance.enabled = !_paused;
        
        pause?.Invoke(_paused);
        pauseNot?.Invoke(!_paused);
    }
    
    //Load the main menu scene
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}