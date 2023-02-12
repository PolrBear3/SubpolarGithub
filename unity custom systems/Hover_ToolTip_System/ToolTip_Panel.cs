using UnityEngine;
using UnityEngine.Systems;
using UnityEngine.UI;

public class ToolTip_Panel : MonoBehavior
{
    public ToolTip_Panel toolTipPanel;
    
    [SerializeField] private Text nameBox;
    [SerializeField] private Text description;
    
    public void Update_ToolTip_Info(Element_ScrObj elementData)
    {
        nameBox.text = elementData.elementName.ToString();
        description.text = elementData.elementDescription.ToString();
    }
}