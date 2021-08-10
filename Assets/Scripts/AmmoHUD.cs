using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoHUD : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Image bgImage;

    public void Refresh(int ammo, int ammoLeft, bool equipped)
    {
        if (!equipped)
        {
            text.enabled = false;
            bgImage.enabled = false;
        }
        else
        {
            text.enabled = true;
            bgImage.enabled = true;
            text.text = ammo.ToString() + "/" + ammoLeft.ToString();
        }
    }
}
