using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponListItem : MonoBehaviour
{

    
    [SerializeField]
    private Text WeaponName;
    [SerializeField]
    private Image WeaponImage;


    public void Setup(string _name, Sprite _image)
    {
        WeaponImage.sprite = _image;
        WeaponName.text = _name;
    }


    public bool ToggleIsOn()
    {
        return GetComponent<Toggle>().isOn;
    }

    public bool isActive()
    {
        return GetComponent<Toggle>().IsActive();
    }


    public void Disactivate()
    {
        if (!ToggleIsOn())
        {
            
            GetComponent<Toggle>().enabled = false;
        }
    }

    public void Activate()
    {
        GetComponent<Toggle>().enabled = true;
    }


    public string getName()
    {
        
        return WeaponName.text;
        
    }


}
