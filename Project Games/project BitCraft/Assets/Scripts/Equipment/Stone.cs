using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour, IEquippable
{
    private Prefab_Controller _prefabController;

    private bool _useReady;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _prefabController = controller; }
    }

    public void Use()
    {
        Stone_Use();
    }

    //
    private void Stone_Use()
    {
        TileMap_Action_System actionSystem = _prefabController.tilemapController.actionSystem;
        TileMap_Combination_System combiSystem = _prefabController.tilemapController.combinationSystem;
        Tile_Controller playerTile = _prefabController.tilemapController.Get_Tile(Prefab_Type.character, 0);

        // target tiles highlight
        if (!_useReady)
        {
            actionSystem.Highlight_EquipmentUse_Tiles(combiSystem.Cross_Tiles(playerTile, 2));
            _useReady = true;
        }
        // use
        else
        {
            Tile_Controller targetTile = _prefabController.equipmentController.equipmentUseTile;
            List<Tile_Controller> meleeRangeTiles = combiSystem.Cross_Tiles(playerTile);

            bool isThrow = false;
            for (int i = 0; i < meleeRangeTiles.Count; i++)
            {
                if (targetTile != meleeRangeTiles[i]) continue;
                isThrow = true;
                break;
            }

            if (isThrow) Melee(targetTile);
            else Throw(targetTile);

            _useReady = false;
        }
    }

    private void Melee(Tile_Controller targetTile)
    {
        for (int i = 0; i < targetTile.currentPrefabs.Count; i++)
        {
            if (!targetTile.currentPrefabs[i].TryGetComponent(out IDamageable damageable)) continue;

            int damage = Random.Range(0, 2);

            _prefabController.tilemapController.controller.inventoryController.Decrease_EquippedSlot_Item(1);
            damageable.Damage(damage);
        }
    }
    private void Throw(Tile_Controller targetTile)
    {
        for (int i = 0; i < targetTile.currentPrefabs.Count; i++)
        {
            if (targetTile.currentPrefabs[i].prefabTag.prefabType != Prefab_Type.character) continue;
            if (!targetTile.currentPrefabs[i].TryGetComponent(out IDamageable damageable)) continue;

            int damage = Random.Range(0, 2);

            _prefabController.tilemapController.controller.inventoryController.Decrease_EquippedSlot_Item(1);
            damageable.Damage(damage);
        }
    }
}
