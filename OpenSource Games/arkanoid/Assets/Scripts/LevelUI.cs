using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    TMP_Text levelText;

    [SerializeField]
    TMP_Text hintText;

    [SerializeField]
    GameObject heartPrefab;

    [SerializeField]
    IntVar score;

    [SerializeField]
    IntVar lives;

    [SerializeField]
    IntVar level;

    List<GameObject> hearts;
    Animator levelTextAnimator;
    Animator hintTextAnimator;

    HashSet<string> shownHints;

    void Start()
    {
        scoreText.text = "";
        hearts = new List<GameObject>();
        levelTextAnimator = levelText.GetComponent<Animator>();
        hintTextAnimator = hintText.GetComponent<Animator>();
        shownHints = new HashSet<string>();

        SetLevel(level.Value);
    }

    void Update()
    {
        SetScore(score.Value);
        SetLives(lives.Value);
    }

    private void OnEnable()
    {
        Pad.OnBonusPickup += HandleBonus;
    }

    private void OnDisable()
    {
        Pad.OnBonusPickup -= HandleBonus;
    }

    void SetLives(int lives)
    {
        if (lives < hearts.Count)
        {
            for (int i = hearts.Count - 1; i >= lives && i >= 0; i--)
            {
                Destroy(hearts[i]);
                hearts.RemoveAt(i);
            }
        }
        else if (lives > hearts.Count)
        {
            for (int i = 0; i < lives; i++)
            {
                Vector2 position = new Vector2(-16.5f + 1.1f * i, -9.2f);
                GameObject newHeart = Instantiate(heartPrefab, position, Quaternion.identity);
                hearts.Add(newHeart);
            }
        }
    }

    void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    void SetLevel(int level)
    {
        levelText.text = $"Level {level}";
        levelTextAnimator.Play("LevelAnim", -1, 0f);
     }

    void HandleBonus(string bonus)
    {
        if (bonus == "LaserBonus")
        {
            ShowHint("Press space to shoot");
        }
        else if (bonus == "StickyBonus")
        {
            ShowHint("Now your pad will be sticky");
        }
    }

    void ShowHint(string hint)
    {
        if (!shownHints.Contains(hint))
        {
            hintText.text = hint;
            hintTextAnimator.enabled = true;
            hintTextAnimator.Play("HintAnim", -1, 0f);
            shownHints.Add(hint);
        }
    }
}
