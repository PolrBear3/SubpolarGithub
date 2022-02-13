using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highscoreFirework : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (PlayerPrefs.GetFloat("Highscore") < scoreManager.score)
        {
            anim.SetTrigger("active");
            Destroy(gameObject, 1.1f);
        }
    }
}
