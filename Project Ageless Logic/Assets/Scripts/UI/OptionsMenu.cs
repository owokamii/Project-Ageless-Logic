using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public Canvas confirmationPrompt;

    public void LogoutAccount()
    {
        confirmationPrompt.enabled = true;
    }

    public void YesButton()
    {
        SceneManager.LoadScene("LoginMenu");
    }

    public void NoButton()
    {
        confirmationPrompt.enabled = false;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
