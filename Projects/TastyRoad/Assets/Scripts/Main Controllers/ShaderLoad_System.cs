using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderLoad_System : MonoBehaviour
{
    [SerializeField] private float _loadTime;

    [Header("")]
    [SerializeField] private GameObject _materialBox;
    [SerializeField] private List<Material> materials = new();

    // UnityEngine
    private void Start()
    {
        LoadAll_ShaderPrefabs();
    }

    //
    private void LoadAll_ShaderPrefabs()
    {
        StartCoroutine(LoadAll_ShaderMaterials_Coroutine());
    }
    private IEnumerator LoadAll_ShaderMaterials_Coroutine()
    {
        List<GameObject> materialBoxes = new(); 

        for (int i = 0; i < materials.Count; i++)
        {
            GameObject box = Instantiate(_materialBox, transform);
            materialBoxes.Add(box);

            if (!box.TryGetComponent(out SpriteRenderer sr)) continue;
            sr.material = materials[i];
        }

        yield return new WaitForSeconds(_loadTime);

        for (int i = 0; i < materialBoxes.Count; i++)
        {
            Destroy(materialBoxes[i]);
        }
    }
}
