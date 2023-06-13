using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Action_System : MonoBehaviour
{
    private Tile_Controller _tileController;

    [SerializeField] private GameObject _thinkCloud;

    private void Awake()
    {
        if (transform.parent.gameObject.TryGetComponent(out Tile_Controller tileController))
        {
            _tileController = tileController;
            _tileController.objectActionSystem = this;
        }
    }

    public void Activate_ThinkCloud()
    {
        _thinkCloud.SetActive(true);
    }
    public void Deactivate_ThinkCloud()
    {
        _thinkCloud.SetActive(false);
    }
}
