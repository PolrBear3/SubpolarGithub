using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform target2;
    public Vector3 offset; 
    [Range(1,20)]   // range of smooth factor
    public float smoothFactor;
    public Vector3 minValue, maxValue;   // camera boundary  

    private void FixedUpdate()
    {
        Follow();
    }

    void Follow()
    {
        Vector3 targetPosition = target.position + offset;

        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValue.x, maxValue.x),
            Mathf.Clamp(targetPosition.y, minValue.y, maxValue.y),
            Mathf.Clamp(targetPosition.z, minValue.z, maxValue.z));

        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }  
}
