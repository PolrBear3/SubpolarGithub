using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private SpriteRenderer _sr;

    
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;
    public Detection_Controller detection => _detection;
    
    [SerializeField] private IInteractable_Controller _interactable;
    public IInteractable_Controller interactable => _interactable;
    
    [Space(20)]
    [SerializeField][Range(0, 100)] private int _destroyTikCount;
    [SerializeField][Range(0, 1)] private float _transparencyStep;
    
    
    private int _currentTikCount;
    
    
    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        globaltime.instance.OnTimeTik += Activate_DestroyTimeTik;
    }

    public void OnDestroy()
    {
        globaltime.instance.OnTimeTik -= Activate_DestroyTimeTik;
    }
    
    
    //
    private void Activate_DestroyTimeTik()
    {
        _currentTikCount++;
        Main_Controller.instance.Change_SpriteAlpha(_sr, _sr.color.a - _transparencyStep);

        if (_currentTikCount < _destroyTikCount) return;
        Destroy(gameObject, 0.1f);
    }
}
