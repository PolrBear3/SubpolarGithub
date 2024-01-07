using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameUI : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Levels");
        };
    }
}
