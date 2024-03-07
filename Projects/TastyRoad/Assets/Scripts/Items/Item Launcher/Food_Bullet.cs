using UnityEngine;
using System.Collections;

public class Food_Bullet : ItemLauncher_Bullet
{
    private SpriteRenderer _sr;

    private Food_ScrObj _currentFood;
    public Food_ScrObj currentFood => _currentFood;



    // UnityEngine
    protected override void Awake()
    {
        base.Awake();

        _sr = gameObject.GetComponent<SpriteRenderer>();
    }



    //
    public void Set_Food(Food_ScrObj food)
    {
        _currentFood = food;
        _sr.sprite = food.sprite;
    }
}