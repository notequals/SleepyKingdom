﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CountryUI : MonoBehaviour
{    
    [NonSerialized]
    public Country country;
        
    public Text countryPopulation;

    public CastleWindowController castleWindow;

    private void Start()
    {
        if (GameManager.instance.sceneChanger)
        {
            country = GameManager.instance.sceneChanger.targetCountry;
        }

        var gameState = GameManager.instance.gameStateManager.gameState;

        if (gameState != null)
        {
            var list = gameState.GetCountries();
            if (list.Count > 0)
            {
                country = list[0];
            }
        }


        if (country == null || country.countryID == 0)
        {
            country = Country.Generate();

            GameManager.instance.globalCountryManager.AddCountry(country);
        }

        castleWindow.SetCountry(country);
    }

    public void OnWeatherIconClick()
    {
        GameManager.instance.weatherManager.ChangeWeather();
    }
        
    public void OnTowerButtonPress()
    {
        GameManager.instance.sceneChanger.ChangeScene(country.tower);
    }
}