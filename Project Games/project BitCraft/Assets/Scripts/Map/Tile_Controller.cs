using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Controller : MonoBehaviour
{
    private SpriteRenderer sr;

    private int _rowNum;
    public int rowNum { get => _rowNum; set => _rowNum = value; }

    private int _columnNum;
    public int columnNum { get => _columnNum; set => _columnNum = value; }

    private bool _selectReady = false;
    public bool selectReady { get => _selectReady; set => _selectReady = value; }

    private List<GameObject> _currentPrefabs = new List<GameObject>();
    public List<GameObject> currentPrefabs { get => _currentPrefabs; set => _currentPrefabs = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { this.sr = sr; }
    }

    public bool Found(int rowNum, int columnNum)
    {
        if (rowNum != this.rowNum) return false;
        if (columnNum != this.columnNum) return false;
        return true;
    }

    public bool Has_Prefab(Prefab_Type type)
    {
        if (currentPrefabs.Count <= 0) return false;
        if (type == Prefab_Type.all) return true;

        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (!currentPrefabs[i].TryGetComponent(out Prefab_Tag tag)) continue;
            if (type != tag.prefabType) continue;

            return true;
        }

        return false;
    }
    public bool Has_Prefab_ID(Prefab_Type type, int searchID)
    {
        Has_Prefab(type);

        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (!currentPrefabs[i].TryGetComponent(out Prefab_Tag tag)) continue;
            if (searchID != tag.prefabID) continue;

            return true;
        }

        return false;
    }

    public void Set_Data(int row, int column)
    {
        rowNum = row;
        columnNum = column;
    }
    public void Track_Current_Prefabs(Transform prefabTransform)
    {
        prefabTransform.parent = transform;

        currentPrefabs.Clear();

        if (transform.childCount <= 0) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            currentPrefabs.Add(transform.GetChild(i).gameObject);
        }
    }

    public void Click()
    {
        if (!selectReady) return;
        Debug.Log("available tile clicked");
    }

    public void Highlight_Tile()
    {
        selectReady = true;
        sr.color = Color.green;
    }
    public void UnHighlight_Tile()
    {
        selectReady = false;
        sr.color = Color.white;
    }
}