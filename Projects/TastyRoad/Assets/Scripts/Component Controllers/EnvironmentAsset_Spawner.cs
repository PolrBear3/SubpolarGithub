using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentAsset
{
    [Header("")]
    [SerializeField][Range(0, 100)] private int _spawnAmount;
    public int spawnAmount => _spawnAmount;

    [Header("")]
    [SerializeField][Range(0, 10)] private float _collapseDistance;
    public float collapseDistance => _collapseDistance;

    [SerializeField][Range(-100f, 100)] private float _maxVerticalValue;
    public float maxVerticalValue => _maxVerticalValue;

    [SerializeField][Range(-100f, 100)] private float _minVerticalValue;
    public float minVerticalValue => _minVerticalValue;

    [Header("")]
    [SerializeField] private int _layerOrderNum;
    public int layerOrderNum => _layerOrderNum;

    [Header("")]
    [SerializeField] private Sprite[] _assetSprites;
    public Sprite[] assetSprites => _assetSprites;
}

public class EnvironmentAsset_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _assetObject;
    [SerializeField] private SpriteRenderer _environmentBoundsSR;

    [Header("")]
    [SerializeField] private EnvironmentAsset[] _assets;


    // UnityEngine
    private void Start()
    {
        Spawn(0);
    }


    //
    private bool Is_Collapsing(float distance, List<Vector2> comparePositions, Vector2 targetPosition)
    {
        if (comparePositions.Count <= 0) return false;

        for (int i = 0; i < comparePositions.Count; i++)
        {
            if (Vector2.Distance(comparePositions[i], targetPosition) >= distance) continue;
            return true;
        }

        return false;
    }


    // Spawn Control
    private void Spawn(int listNum)
    {
        EnvironmentAsset targetAsset = _assets[listNum];
        Bounds envirbounds = _environmentBoundsSR.bounds;

        List<Vector2> spawnedPositions = new();

        for (int i = 0; i < targetAsset.spawnAmount; i++)
        {
            Vector2 spawnPosition = new();

            do
            {
                float randXPoint = Random.Range(envirbounds.min.x, envirbounds.max.x);
                float randYPoint = Random.Range(targetAsset.minVerticalValue, targetAsset.maxVerticalValue);

                spawnPosition = new Vector2(randXPoint, randYPoint);
            }
            while (Is_Collapsing(targetAsset.collapseDistance, spawnedPositions, spawnPosition));

            spawnedPositions.Add(spawnPosition);

            GameObject spawnAsset = Instantiate(_assetObject, spawnPosition, Quaternion.identity);

            spawnAsset.transform.SetParent(_environmentBoundsSR.transform);

            SpriteRenderer assetSR = spawnAsset.GetComponent<SpriteRenderer>();
            Sprite randAssetSprite = targetAsset.assetSprites[Random.Range(0, targetAsset.assetSprites.Length)];

            assetSR.sortingOrder = targetAsset.layerOrderNum;
            assetSR.sprite = randAssetSprite;
        }
    }

    private void Spawn_All()
    {

    }
}
