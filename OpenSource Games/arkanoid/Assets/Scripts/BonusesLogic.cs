using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BonusesLogic : MonoBehaviour
{
    [SerializeField]
    GameObject ballPrefab;

    [SerializeField]
    int multiballsNumber;

    [SerializeField]
    float multiballsSpawnRadius;

    public void SpawnMultiBalls()
    {
        // Find a ball (it can return any with that tag):
        GameObject ball = GameObject.FindWithTag("Ball");

        if (ball == null)
            return;

        // Needed to avoid spawning balls inside the bricks.
        TilemapCollider2D tileMapCollider = Object.FindObjectOfType<TilemapCollider2D>();

        // Spawn new N balls around it in a circular pattern
        for (int i = 0; i < multiballsNumber; i++)
        {
            float angle = 2 * Mathf.PI * i / multiballsNumber;

            float dirX = Mathf.Cos(angle);
            float dirY = Mathf.Sin(angle);

            Vector3 center = ball.transform.position;
            Vector3 where = center + new Vector3(dirX, dirY, 0f) * multiballsSpawnRadius;

            // This is some tricky business to prevent balls appearing within the bricks collider :)
            // Note we can do it differently: using OverlapCircle from Physics2D and check that
            // tileMap collider is not in the returned array of colliders.
            if (!tileMapCollider.composite.OverlapPoint(where))
            {
                Vector2 closestPoint = tileMapCollider.composite.ClosestPoint(where);
                float distance = Vector2.Distance(closestPoint, where);

                if (distance > 0.75f)
                {
                    GameObject newBallObj = Instantiate(ballPrefab, where, Quaternion.identity);
                    Ball newBall = newBallObj.GetComponent<Ball>();
                    Vector2 dir = new Vector2(dirX, dirY);
                    newBall.Fire(dir);
                }
            }
        }
    }
}
