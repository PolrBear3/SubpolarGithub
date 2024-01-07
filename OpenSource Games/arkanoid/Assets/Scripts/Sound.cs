using UnityEngine;
using UnityEngine.Tilemaps;

public class Sound : MonoBehaviour
{
    [SerializeField]
    AudioSource source;

    [SerializeField]
    AudioClip bonusSound;

    [SerializeField]
    AudioClip padHitsBallSound;

    [SerializeField]
    AudioClip brickHitSound;

    [SerializeField]
    AudioClip levelClearedSound;

    [SerializeField]
    AudioClip lifeLostSound;

    [SerializeField]
    AudioClip gameOverSound;

    [SerializeField]
    AudioClip gameWonSound;

    private void OnEnable()
    {
        Bricks.OnBrickHit += PlayBrickHitSound;
        Pad.OnBonusPickup += PlayBonusPickup;
        Pad.OnPadHitsBall += PlayPadHitsBall;
        GameLogic.OnLevelCleared += PlayLevelCleared;
        Pad.OnLostLife += PlayLifeLost;
        GameLogic.OnGameOver += PlayGameOver;
    }

    private void OnDisable()
    {
        Bricks.OnBrickHit -= PlayBrickHitSound;
        Pad.OnBonusPickup -= PlayBonusPickup;
        Pad.OnPadHitsBall -= PlayPadHitsBall;
        GameLogic.OnLevelCleared -= PlayLevelCleared;
        Pad.OnLostLife -= PlayLifeLost;
        GameLogic.OnGameOver -= PlayGameOver;
    }

    public void PlayPadHitsBall()
    {
        source.clip = padHitsBallSound;
        source.Play();
    }

    public void PlayBonusPickup(string _bonus)
    {
        source.clip = bonusSound;
        source.Play();
    }

    public void PlayBrickHitSound(TileBase _tile)
    {
        source.clip = brickHitSound;
        source.Play();
    }

    public void PlayLevelCleared(int level)
    {
        source.clip = levelClearedSound;
        source.Play();
    }

    public void PlayLifeLost()
    {
        source.clip = lifeLostSound;
        source.Play();
    }

    public void PlayGameOver()
    {
        source.clip = gameOverSound;
        source.Play();
    }

    public void PlayGameWon()
    {
        source.clip = gameWonSound;
        source.Play();
    }
}
