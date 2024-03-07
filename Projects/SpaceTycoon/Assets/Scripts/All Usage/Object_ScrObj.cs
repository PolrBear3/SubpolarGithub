using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { ground,wall };

[CreateAssetMenu(menuName = "Create New Object")]
public class Object_ScrObj : ScriptableObject
{
    public GameObject gameObjectPrefab;
    public ObjectType objectType;
    public Sprite objectSprite;
    [TextArea(2, 5)]
    public string objectDescription;

    public Ingredient[] ingredients;
}
