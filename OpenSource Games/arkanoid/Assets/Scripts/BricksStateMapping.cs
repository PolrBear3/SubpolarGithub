using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BricksStateMapping : ScriptableObject
{
    [System.Serializable]
    public class BrickMapping
    {
        public TileBase goodBrick;
        public TileBase brokenBrick;
    }

    // This is what we set up from the Scriptable Object UI.
    // The mapping between good and broken tiles.
    public BrickMapping[] AllBricks;

    // Internal data.
    private Dictionary<TileBase, TileBase> data;

    public TileBase Break(TileBase brick)
    {
        Init();
        return data.ContainsKey(brick) ? data[brick] : null;
    }

    private void Init()
    {
        if (data != null)
            return;

        data = new Dictionary<TileBase, TileBase>();
        foreach (var mapping in AllBricks)
        {
            data[mapping.goodBrick] = mapping.brokenBrick;
        }
    }
}
