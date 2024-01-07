using UnityEngine;
using UnityEngine.Tilemaps;

public class Explosion : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psRenderer = GetComponent<ParticleSystemRenderer>();
    }

    public void Play(Tile tile, Vector3 hitNormal)
    {
        // Orient the explosion direction so that it's facing in direction opposite
        // to the direction of hitting object.
        var shape = ps.shape;
        float zRotation = Vector2.SignedAngle(Vector2.right, -hitNormal) - shape.arc / 2f;
        shape.rotation = new Vector3(shape.rotation.x, shape.rotation.y, zRotation);

        // Create a material based on the texture of the sprite of the broken tile,
        // so that the exploded pieces are of the same color as the original brick.
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetTexture("_MainTex", tile.sprite.texture);
        psRenderer.material = mat;

        ps.Play();
    }
}
