using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScanningUI : MonoBehaviour
{
    [SerializeField] private ScanningVision scanningVision;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI hackableText;

    private void Awake()
    {
    }

    private void Start()
    {
        scanningVision.OnScanningObjectChanged += ScanningVision_OnScanningObjectChanged;

        Hide();
    }

    private void ScanningVision_OnScanningObjectChanged(object sender, System.EventArgs e)
    {
        if (sender != null)
        {
            Show();
            ScannableObject scannableObject = ((GameObject)sender).GetComponent<ScannableObject>();
            nameText.text = scannableObject.scanName;
            statusText.text = scannableObject.status;
            hackableText.text = scannableObject.hackable;
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
