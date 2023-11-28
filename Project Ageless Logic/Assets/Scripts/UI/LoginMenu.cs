using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{
    public string username;
    public string password;

    public TMP_Text errorText;
    public Canvas LoggedIn;

    private string typedUsername;
    private string typedPassword;

    public void VerifyUsername(string username)
    {
        typedUsername = username;
        Debug.Log(username);
        Debug.Log(typedUsername);
    }

    public void VerifyPassword(string password)
    {
        typedPassword = password;
        Debug.Log(password);
        Debug.Log(typedPassword);
    }

    public void VerifyCredentials()
    {
        if(typedUsername != username)
        {
            Debug.Log("username is not correct");
            errorText.enabled = true;
            Invoke("CloseErrorText", 2f);
        }

        if(typedPassword != password)
        {
            Debug.Log("password is not correct");
            errorText.enabled = true;
            Invoke("CloseErrorText", 2f);
        }

        if(typedUsername == username && typedPassword == password)
        {
            Debug.Log("correct credentials");
            LoggedIn.enabled = true;
            Invoke("CloseLogInPrompt", 3f);
            Invoke("LoggingIn", 5f);
        }
    }

    public void CloseErrorText()
    {
        errorText.enabled = false;
    }

    public void CloseLogInPrompt()
    {
        LoggedIn.enabled = false;
    }

    public void LoggingIn()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
