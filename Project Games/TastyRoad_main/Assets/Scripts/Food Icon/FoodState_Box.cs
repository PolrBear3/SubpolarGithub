using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodState_BoxSprite
{
    public FoodState_Type type;
    public List<Sprite> boxSprite = new();
}

public class FoodState_Box : MonoBehaviour
{
    private FoodState_Controller _controller;

    [SerializeField] private List<FoodState_BoxSprite> _boxSprites;

    private SpriteRenderer _sr;
    [HideInInspector] public State_Data currentData;

    [HideInInspector] public bool isTransparent;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }

    // State Controller Connection
    public void Assign_StateController(FoodState_Controller controller)
    {
        _controller = controller;
    }

    // Get
    private Sprite BoxSprite()
    {
        for (int i = 0; i < _boxSprites.Count; i++)
        {
            if (currentData.stateType == _boxSprites[i].type)
            {
                return _boxSprites[i].boxSprite[currentData.stateLevel - 1];
            }
        }

        return null;
    }

    // Transparency Toggle
    public void StateBox_Transparency(bool isTransparent)
    {
        this.isTransparent = isTransparent;

        if (this.isTransparent == true)
        {
            _sr.color = Color.clear;
        }
        else
        {
            _sr.color = Color.white;
        }
    }

    // State Control
    public void Assign_State(State_Data stateData)
    {
        currentData = stateData;

        if (currentData.stateLevel > 3)
        {
            currentData.stateLevel = 3;
        }

        _sr.sprite = BoxSprite();
    }
    public void Assign_State(FoodState_Type type, int level)
    {
        currentData.stateType = type;
        currentData.stateLevel = level;

        if (currentData.stateLevel > 3)
        {
            currentData.stateLevel = 3;
        }

        _sr.sprite = BoxSprite();
    }

    public void Update_State()
    {
        if (currentData.stateLevel > 0)
        {
            Assign_State(currentData);
        }
        else
        {
            Clear_State();
        }
    }

    public void Clear_State()
    {
        // remove from controller's current states list
        _controller.currentStateBoxes.Remove(this);

        Destroy(gameObject);
    }
}