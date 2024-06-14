using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBox : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Image _iconImage;

    [Header("")]
    [SerializeField] private GameObject _infoPanel;
    //[SerializeField] private TextMeshProUGUI _infoText;


    // Functions
    public void Update_Box(DialogData data)
    {
        _iconImage.sprite = data.icon;
        //_infoText.text = data.info;
    }


    public void InfoPanel_Toggle(bool toggleOn)
    {
        _infoPanel.SetActive(toggleOn);
    }
}
