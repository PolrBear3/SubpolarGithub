using UnityEngine;
using UnityEngine.Systems;
using UnityEngine.UI;

public class Hover_Element : MonoBehavior, IPointerEnterHandler, IPointerExitHandler
{
    public ToolTip_Panel toolTip;
    public Element_ScrObj element;
    
    private bool timerStart;
    private float timer;
    public float onHoverTime;
    
    private void Update()
    {
        Timer();
        Show_ToolTip();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        timerStart = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        timerStart = false;
        toolTipController.ToolTip_Off();
    }
    
    private void Timer()
    {
        if (timerStart)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
    }

    private void Show_ToolTip()
    {
        if (timer >= onHoverTime)
        {
            toolTip.Update_ToolTip_Info(element);
            toolTip.gameObject.SetActive(true);
        }
        else
        {
            toolTip.gameObject.SetActive(false);
        }
    }
}