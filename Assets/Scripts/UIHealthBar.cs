using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Transform target;
    public Image backgroundImage;
    public Image foregroundImage;
    public Image borderImage;
    public Vector3 offset;
    public float maxVisibleDistance;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = (target.position - Camera.main.transform.position);
        bool isBehind = Vector3.Dot(direction.normalized, Camera.main.transform.forward) <= 0.0f;
        if (direction.magnitude > maxVisibleDistance || isBehind)
        {
            backgroundImage.enabled = false;
            foregroundImage.enabled = false;
            borderImage.enabled = false;
        }
        else
        {
            backgroundImage.enabled = true;
            foregroundImage.enabled = true;
            borderImage.enabled = true;
        }
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }

    public void SetHealthBarPercentage(float percentage)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * percentage;
        foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
