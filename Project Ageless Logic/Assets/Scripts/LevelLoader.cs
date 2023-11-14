using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1.0f;

    private bool difficultySelected = false;
    private int levelToLoad;

    public void FadeToLevel(int levelIndex)
    {   
        if (difficultySelected)
        {
            transition.SetTrigger("Start");
        }
        else
        {
            levelToLoad = levelIndex + 1;
        }
    }

    public void SelectDifficulty(int difficultyIndex)
    {
        Debug.Log(levelToLoad);
        difficultySelected = true;
        FadeToLevel(levelToLoad);
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
