# Pointer Event Systems
- works on both UI elements and normal gameobjects
- for normal gameobjects, Physics2DRaycaster component for camera, box collider for the gameobject, is needed
```C#
using UnityEngine.EventSystems;

public class UI_EventSystem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // EventSystems
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
```

## Vector3 Camera Position
```C#
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
```