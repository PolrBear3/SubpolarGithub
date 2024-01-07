using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private IntVar score;

    [SerializeField]
    private IntVar lives;

    [SerializeField]
    private IntVar level;

    [SerializeField]
    private IntVar hiScore;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text hiScoreText;

    void Start()
    {
        if (score.Value > hiScore.Value)
            hiScore.Value = score.Value;

        scoreText.text = $"Your score: <color=red>{score.Value}</color>";
        hiScoreText.text = $"High score: {hiScore.Value}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            level.Reset();
            lives.Reset();
            score.Reset();
            SceneManager.LoadScene("Levels");
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
