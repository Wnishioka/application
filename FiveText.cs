using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiveText : MonoBehaviour
{
    public Image Main_material;
    public Image material1;
    public Image material2;
    public Image material3;
    public Image material4;
    public Toggle Main_toggle;
    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    public Toggle toggle4;

    public void PointerEnter()
    {
        if (Main_toggle.isOn == true)
        {
            Main_material.gameObject.SetActive(true);
            Main_material.color = new Color32(50, 50, 50, 100);
        }
        else
        {
            Main_material.gameObject.SetActive(true);
            Main_material.color = new Color32(200, 200, 200, 128);
        }
    }
    public void PointerExit()
    {
        Main_material.gameObject.SetActive(false);
        material1.gameObject.SetActive(false);
        material2.gameObject.SetActive(false);
        material3.gameObject.SetActive(false);
        material4.gameObject.SetActive(false);
    }
    public void PointerClick()
    {
        if (Main_toggle.isOn == true)
        {
            Main_toggle.isOn = false;
            Main_material.gameObject.SetActive(false);
        }
        else if (toggle1.isOn == true)
        {
            Main_toggle.isOn = true;
            Main_material.gameObject.SetActive(true);
            Main_material.color = new Color32(50, 50, 50, 100);
            material1.gameObject.SetActive(false);
            material2.gameObject.SetActive(false);
            material3.gameObject.SetActive(false);
            material4.gameObject.SetActive(false);
        }
        else if (toggle2.isOn == true)
        {
            Main_toggle.isOn = true;
            Main_material.gameObject.SetActive(true);
            Main_material.color = new Color32(50, 50, 50, 100);
            material1.gameObject.SetActive(false);
            material2.gameObject.SetActive(false);
            material3.gameObject.SetActive(false);
            material4.gameObject.SetActive(false);
        }
        else if (toggle3.isOn == true)
        {
            Main_toggle.isOn = true;
            Main_material.gameObject.SetActive(true);
            Main_material.color = new Color32(50, 50, 50, 100);
            material1.gameObject.SetActive(false);
            material2.gameObject.SetActive(false);
            material3.gameObject.SetActive(false);
            material4.gameObject.SetActive(false);
        }
        else
        {
            Main_toggle.isOn = true;
            Main_material.gameObject.SetActive(true);
            Main_material.color = new Color32(50, 50, 50, 100);
            material1.gameObject.SetActive(false);
            material2.gameObject.SetActive(false);
            material3.gameObject.SetActive(false);
            material4.gameObject.SetActive(false);

        }
    }
}
