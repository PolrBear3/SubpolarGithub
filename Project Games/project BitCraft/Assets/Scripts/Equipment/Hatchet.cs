using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatchet : MonoBehaviour, IEquippable
{
    private Prefab_Controller _prefabController;

    [SerializeField] private int _damage;

    private bool _useReady;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _prefabController = controller; }
    }

    public void Use()
    {
        // target tiles highlight
        if (!_useReady)
        {
            TileMap_Action_System actionSystem = _prefabController.tilemapController.actionSystem;
            TileMap_Combination_System combiSystem = _prefabController.tilemapController.combinationSystem;

            actionSystem.Highlight_EquipmentUse_Tiles(combiSystem.Cross_Tiles(Prefab_Type.character, 0));

            _useReady = true;
        }
        // use hatchet
        else
        {
            Equipment_Controller equipmentController = _prefabController.equipmentController;
            Tile_Controller targetTile = equipmentController.equipmentUseTile;

            for (int i = 0; i < targetTile.currentPrefabs.Count; i++)
            {
                if (!targetTile.currentPrefabs[i].TryGetComponent(out IDamageable damageable)) continue;
                damageable.Damage(_damage);
            }

            _useReady = false;
        }
    }
}
