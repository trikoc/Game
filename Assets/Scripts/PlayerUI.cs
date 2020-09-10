using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject crossHair;

    [SerializeField]
    RectTransform healthBarFill;

    [SerializeField]
    RectTransform healthBarFillLoss;

    public PlayerInfo playerInfo;

    [SerializeField]
    GameObject winnerMenu;

    [SerializeField]
    GameObject loserMenu;



    // Start is called before the first frame update
    void Start()
    {
        PauseMenuScript.IsOn = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(loserMenu.activeSelf || winnerMenu.activeSelf)
        {
            return;
        }
        setHealtAmmount(playerInfo.getHealthPct());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }


    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenuScript.IsOn = pauseMenu.activeSelf;
        crossHair.SetActive(!pauseMenu.activeSelf);
    }

    void setHealtAmmount(float _ammount)
    {
        healthBarFill.localScale = new Vector3(_ammount, 1f, 1f);
        StartCoroutine(HealthLoss());
    }

    public void setPlayerInfo(PlayerInfo _playerInfo)
    {
        playerInfo = _playerInfo;
    }

    IEnumerator HealthLoss()
    {
        healthBarFillLoss.gameObject.SetActive(true);
        while (healthBarFill.localScale.x != healthBarFillLoss.localScale.x)
        {
            if(healthBarFill.localScale.x + .01f > healthBarFillLoss.localScale.x)
            {
                healthBarFillLoss.localScale = healthBarFill.localScale;
                break;
            }
            healthBarFillLoss.localScale = Vector3.Lerp(healthBarFillLoss.localScale, healthBarFill.localScale, .001f);
            yield return new WaitForSeconds(.05f);
        }
        healthBarFillLoss.gameObject.SetActive(false);
    }


    public void ActivateWinnerOrLoserMenu(bool winORlose)
    {
        setHealtAmmount(playerInfo.getHealthPct());
        pauseMenu.SetActive(false);
        crossHair.SetActive(false);
        // win = true, lose = false
        if (winORlose)
        {
            winnerMenu.SetActive(true);
        }
        else
        {
            loserMenu.SetActive(true);
        }
        

    }
}
