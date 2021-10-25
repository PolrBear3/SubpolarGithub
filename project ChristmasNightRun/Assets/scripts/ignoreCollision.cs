using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ignoreCollision : MonoBehaviour
{
    public BoxCollider2D buildingCollider;
    public BoxCollider2D buildingBLockerCollider;

    void Start()
    {
        Physics2D.IgnoreCollision(buildingCollider, buildingBLockerCollider, true);
    }
}
