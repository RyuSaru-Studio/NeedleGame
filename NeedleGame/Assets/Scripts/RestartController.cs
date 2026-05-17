using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartController : MonoBehaviour
{
    public bool restartEnabled = true;

    private void Update()
    {
        if (!restartEnabled)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentScene();
        }
    }

    public void RestartCurrentScene()
    {
        if (!restartEnabled)
        {
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
