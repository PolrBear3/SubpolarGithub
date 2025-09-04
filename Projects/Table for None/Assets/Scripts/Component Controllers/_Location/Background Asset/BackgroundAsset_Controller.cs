using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAsset_Controller : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private GameObject _assetPrefab;

    [Space(40)] 
    [SerializeField] private Sprite[] _assetSprites;
    
    [Space(20)] 
    [SerializeField] [Range(0, 100)] private int _startingSpawnAmount;
    [SerializeField] [Range(0, 100)] private int _maxSpawnAmount;
    [SerializeField] private Vector2 _spawnRange;
    public Vector2 spawnRange => _spawnRange;

    [Space(20)] 
    [SerializeField] [Range(0, 100)] private float _spawnCoolTimeRange;
    [SerializeField] private Vector2 _movementSpeedRange;
    
    
    private List<BackgroundAsset> _spawnAssets = new();
    public List<BackgroundAsset> spawnAssets => _spawnAssets;

    
    // MonoBehaviour
    private void Start()
    {
        Spawn_Assets();
        StartCoroutine(CycleSpawn_Assets());
    }


    // Spawn Control
    private void Spawn_Asset(float spawnPointX)
    {
        Vector2 spawnPoint = new Vector2(spawnPointX, transform.position.y);
        
        GameObject spawnedAsset = Instantiate(_assetPrefab, spawnPoint, Quaternion.identity);
        BackgroundAsset asset = spawnedAsset.GetComponent<BackgroundAsset>();

        spawnedAsset.transform.parent = transform;
        
        _spawnAssets.Add(asset);
        asset.Set_Controller(this);
        
        int randIndex = Random.Range(0, _assetSprites.Length);
        asset.spriteRenderer.sprite = _assetSprites[randIndex];

        float randSpeed = Random.Range(_movementSpeedRange.x, _movementSpeedRange.y);
        asset.Set_Movement(randSpeed, spawnPointX >= 0);
    }


    private void Spawn_Assets()
    {
        for (int i = 0; i < _startingSpawnAmount; i++)
        {
            float randStartPoint = Random.Range(_spawnRange.x, _spawnRange.y);
            Spawn_Asset(randStartPoint);
        }
    }
    
    private IEnumerator CycleSpawn_Assets()
    {
        while (true)
        {
            float randCoolTime = Random.Range(0, _spawnCoolTimeRange);
            yield return new WaitForSeconds(randCoolTime);

            if (_spawnAssets.Count >= _maxSpawnAmount) continue;
            
            float randSpawnPoint = Random.value > 0.5f ? _spawnRange.x : _spawnRange.y;
            Spawn_Asset(randSpawnPoint);
        }
    }
}