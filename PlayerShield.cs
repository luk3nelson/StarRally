using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShield : MonoBehaviour
{
    public float startingShield = 100.0f;
    public float currentShield;
    public Slider shieldSlider;
    public Image shieldBar;
    [Header("Debug")]
    //public Text shieldNumber;   

    
    bool noShield;

    void Awake()
    {
        currentShield = startingShield;
        //shieldSlider.value = currentShield;
        shieldBar.fillAmount = currentShield;
        //shieldNumber.text = "Shield" + shieldSlider.value;
    }

    void Update()
    {
        HealthChange(currentShield);

    }

    public void RegenShield(float amount)
    {

        if (currentShield <= 100.0f)
        {
            currentShield += amount;
            //shieldNumber.text = "Shield" + currentShield;
            //shieldSlider.value = currentShield;
            HealthChange(currentShield);
        }
        else if (currentShield > 100.0f)
            currentShield = 100.0f;
    }

    public void ReduceShield(float amount)
    {
        currentShield -= amount;
        //shieldSlider.value = currentShield;
        HealthChange(currentShield);

        //shieldNumber.text = "Shield" + shieldSlider.value;

        if (currentShield <= 0.0f && !noShield)
        {
            ShutDown();
        }
    }

    public void HealthChange(float healthValue)
    {
        float amount = (healthValue / 100.0f) * 180.0f / 360;
        shieldBar.fillAmount = amount;
        //Debug.Log(shieldBar.fillAmount);
    }

    void ShutDown()
    {

        //wait 5 seconds with reduced speed of .5f

    }



}
