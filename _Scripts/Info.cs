using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    public GameObject grid;
    public Text[] Titles;
    public Text[] Values;
    bool toggleOn;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if (toggleOn == false)
            {
                grid.gameObject.SetActive(true);
                toggleOn = true;
            }
            else
            {
                grid.gameObject.SetActive(false);
                toggleOn = false;
            }
        }

        Values[0].text = Inventory.instance.numberOfRolls.ToString();
        Values[1].text = Inventory.instance.isFlavored ? "True" : "False";
    }
}
