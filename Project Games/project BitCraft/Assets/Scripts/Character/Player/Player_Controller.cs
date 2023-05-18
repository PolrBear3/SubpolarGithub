using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Prefab_Controller _prefabController;
    public Prefab_Controller prefabController { get => _prefabController; set => _prefabController = value; }

    private bool _interactReady = false;
    public bool interactReady { get => _interactReady; set => _interactReady = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller prefabController)) { this.prefabController = prefabController; }
    }

    // checks
    public bool Position_at_Crust()
    {
        List<Tile_Controller> crustTiles = prefabController.tilemapController.combinationSystem.Map_Crust_Tiles();

        for (int i = 0; i < crustTiles.Count; i++)
        {
            if (!crustTiles[i].Has_Prefab_ID(Prefab_Type.character, 0)) continue;
            return true;
        }
        return false;
    }
    public bool Position_at_Corner()
    {
        List<Tile_Controller> cornerTiles = prefabController.tilemapController.combinationSystem.Map_Corners_Tiles();

        for (int i = 0; i < cornerTiles.Count; i++)
        {
            if (!cornerTiles[i].Has_Prefab_ID(Prefab_Type.character, 0)) continue;
            return true;
        }
        return false;
    }

    // functions
    public void Click()
    {
        TileMap_Controller tilemapController = prefabController.tilemapController;

        if (!interactReady) 
        {
            interactReady = true;
            tilemapController.actionSystem.Highlight_Player_Interactable_Tiles();
            tilemapController.actionSystem.Set_NewMap_Directions();
        }
        else
        {
            interactReady = false;
            tilemapController.actionSystem.UnHighlight_All_tiles();
            tilemapController.actionSystem.Reset_NewMap_Directions();
        }
    }

    public void Move()
    {
        LeanTween.moveLocal(gameObject, Vector2.zero, prefabController.moveSpeed).setEase(LeanTweenType.easeInOutQuint);
    }
}
