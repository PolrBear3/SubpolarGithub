using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundScroll : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed;
    private Transform cameraTransform;
    private Vector3 lastCamPos;
    private float textureUnitSizeX;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCamPos = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 deltaMove = cameraTransform.position - lastCamPos;
        transform.position += new Vector3(deltaMove.x * scrollSpeed.x, deltaMove.y * scrollSpeed.y);
        lastCamPos = cameraTransform.position;

        if (cameraTransform.position.x - transform.position.x >= textureUnitSizeX)
        {
            float offSetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offSetPositionX, transform.position.y);
        }
    }
}
