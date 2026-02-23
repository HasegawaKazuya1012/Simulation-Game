using UnityEngine;
using UnityEngine.SceneManagement; 
public class ResultManager : MonoBehaviour
{

    public void OnBackToTitleClicked()
    {

        Time.timeScale = 1.0f;


        SceneManager.LoadScene("Title");
    }
}