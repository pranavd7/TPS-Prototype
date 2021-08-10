using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonChangeColor : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color colorOnSelected;
    [SerializeField] Color colorOnDeselected;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnSelect()
    {
        text.color = colorOnSelected;
    }
    
    public void OnDeselect()
    {
        text.color = colorOnDeselected;
    }
}
