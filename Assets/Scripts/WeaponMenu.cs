using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMenu : MonoBehaviour
{
    
    [SerializeField]
    private GameObject weaponListItemPrefab;

    [SerializeField]
    private Transform weaponListParent;

    public static string[] weaponsToGet = { "L96-Sniper", "AK-47","Pistol" };

    [SerializeField]
    Sprite[] imageToLoad;

    bool equip3 = false;
    bool toActive = false;
    bool loaded = false;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (!loaded)
        {
            AddOnList();
        }
        else
        {
            refresh();
            
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        int num = getNumberOfActiveWeaponToggle();
        if (num > 2 && !equip3)
        {
            
            disableToggle();
            equip3 = true;
            
        }
        if (num < 3 && equip3)
        {
            
            activateToggle();
            equip3 = false;
        }
    }


    public void AddOnList()
    {
        loaded = true;
        foreach (Sprite image in imageToLoad)
        {
            GameObject _weaponListItemGO = Instantiate(weaponListItemPrefab);
            _weaponListItemGO.transform.SetParent(weaponListParent);
            _weaponListItemGO.name = image.name;

            WeaponListItem _weaponListItem = _weaponListItemGO.GetComponent<WeaponListItem>();
            if (_weaponListItem != null)
            {
                _weaponListItem.Setup(image.name, image);
            }

        }
    }

    public void WeaponsToGet()
    {
        int i = 0;
        foreach (Transform child in weaponListParent)
        {
            WeaponListItem _weaponListItem = child.GetComponent<WeaponListItem>();
            if (_weaponListItem != null && _weaponListItem.ToggleIsOn())
            {
                weaponsToGet[i]=_weaponListItem.getName();
                
                i++;
            }

            
        }
        
        
    }

       
    public int getNumberOfActiveWeaponToggle() {
        int count = 0;
        foreach (Transform child in weaponListParent)
        { 
            WeaponListItem _weaponListItem = child.GetComponent<WeaponListItem>();
            if (_weaponListItem != null)
            {
                if (_weaponListItem.ToggleIsOn())
                {
                    count++;
                }
            }

        }
        

        return count;

    }

    public void refresh()
    {

        foreach (Transform child in weaponListParent)
        {
            WeaponListItem _weaponListItem = child.GetComponent<WeaponListItem>();
            if (!_weaponListItem.isActive())
            {
                _weaponListItem.Activate();
                _weaponListItem.Disactivate();

            }
        }

    }

    public void disableToggle()
    {

        foreach (Transform child in weaponListParent)
        {
            WeaponListItem _weaponListItem = child.GetComponent<WeaponListItem>();
            if (_weaponListItem != null)
            {
                _weaponListItem.Disactivate();
                
            }
        }
        
    }

    public void activateToggle()
    {

        foreach (Transform child in weaponListParent)
        {
            WeaponListItem _weaponListItem = child.GetComponent<WeaponListItem>();
            if (_weaponListItem != null)
            {
                _weaponListItem.Activate();

            }
        }

    }

}
