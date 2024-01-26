using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveMenu_Controller : MonoBehaviour
{
    private VehiclePanel_Controller _controller;

    [SerializeField] private List<VechiclePanel_ItemBox> _itemBoxes = new();

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out VehiclePanel_Controller controller)) { _controller = controller; }
    }

    //
    public void Assign_ItemBoxes()
    {
        if (_controller.currentMenuNum != 1) return;

        _controller.itemBoxes = _itemBoxes;
        _controller.Assign_All_BoxNum();

        // set starting cursor at first box
        _controller.currentItemBox = _itemBoxes[0];
        _controller.currentItemBox.BoxSelect_Toggle(true);
    }
}
