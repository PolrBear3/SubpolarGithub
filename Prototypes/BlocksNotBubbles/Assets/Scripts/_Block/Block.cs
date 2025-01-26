using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block
{
    private Block_ScrObj _blockType;
    public Block_ScrObj blockType => _blockType;


    // Block
    public Block(Block setBlock)
    {
        _blockType = setBlock.blockType;
    }

    public Block(Block_ScrObj setType)
    {
        _blockType = setType;
    }
}
