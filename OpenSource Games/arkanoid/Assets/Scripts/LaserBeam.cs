using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
    }
}
