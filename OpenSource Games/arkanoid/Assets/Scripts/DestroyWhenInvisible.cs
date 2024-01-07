using UnityEngine;

public class DestroyWhenInvisible : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
