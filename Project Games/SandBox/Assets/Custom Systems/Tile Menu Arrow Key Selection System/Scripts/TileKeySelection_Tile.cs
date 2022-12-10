using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileKeySelection_Tile : MonoBehaviour
{
    public GameObject highlightBox;

    public void UnSelect()
    {
        highlightBox.SetActive(false);
    }
    public void Select()
    {
        highlightBox.SetActive(true);
    }
}
