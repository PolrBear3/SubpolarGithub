using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialLoader : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _tempObject;
    [SerializeField] private float _loadTime;

    [Header("")]
    [SerializeField] private Material[] materials;


    // MonoBehaviour
    private void Start()
    {
        Load_Materials();
    }


    //
    private void Load_Materials()
    {
        StartCoroutine(Load_Materials_Coroutine());
    }

    private IEnumerator Load_Materials_Coroutine()
    {
        List<GameObject> tempObjs = new();

        for (int i = 0; i < materials.Length; i++)
        {
            GameObject tempObj = Instantiate(_tempObject);
            tempObj.transform.SetParent(transform);
            tempObjs.Add(tempObj);

            SpriteRenderer tempSR = tempObj.GetComponent<SpriteRenderer>();
            tempSR.material = materials[i];
        }

        yield return new WaitForSeconds(_loadTime);

        foreach (var tempObj in tempObjs)
        {
            Destroy(tempObj);
        }
    }
}
