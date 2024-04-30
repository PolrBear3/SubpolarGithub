# Brackys Smooth Movement Camera


### Attach this to Camera
```C#
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
	[SerializeField] private Transform playerTarget;
    
    // speed for smoothness
	[SerializeField] private float lerpValue = 0.125f;
	
    // adjust for custom center position of target
    [SerializeField] private public Vector2 offSetPositon;

	void FixedUpdate ()
	{
		Vector2 desiredPosition = target.position + offSetPositon;
		Vector2 smoothedPosition = Vector2.Lerp(transform.position, desiredPosition, lerpValue);
		
        transform.position = smoothedPosition;
		
        // for 3d use
        transform.LookAt(target);
	}

}
```