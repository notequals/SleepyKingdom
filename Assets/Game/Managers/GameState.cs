﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Linq;

using UnityEngine;

[Serializable]
public class GameState
{
    private bool initSuccess = false;

    [SerializeField]
    private Country[] countries = new Country[0];

    public MyCountry myCountry;

    public HouseLevel[] houseLevels = new HouseLevel[0];

    public House temporaryHouse;

    public Ship myShip;
        
    public string currentMapName;

    //public MapCastleSave[] mapCastles = new MapCastleSave[0];
    public MapResourceSave[] mapResources = new MapResourceSave[0];

    [NonSerialized]
    private Dictionary<int, Country> countryDictionary = new Dictionary<int, Country>();
    
    public int countryCounter = Country.countryCounter;

    public GameState(string countryName)
    {
        myCountry = new MyCountry(countryName);
        myShip = new Ship();
    }

    public void AddCountry(Country newCountry)
    {
        if (countryDictionary.ContainsKey(newCountry.countryID))
        {
            Debug.Log("Duplicate country id: " + newCountry.countryID);
            return;
        }

        countryDictionary.Add(newCountry.countryID, newCountry);

        countries = countryDictionary.Values.ToArray();

        Save();
    }

    public void DeleteCountry(Country country)
    {
        countryDictionary.Remove(country.countryID);

        countries = countryDictionary.Values.ToArray();

        Save();
    }

    public void AddHouseLevel()
    {
        if (houseLevels.Length < GameManager.instance.defaultHouseManager.houseLevels.Length)
        {
            var levelObject = GameManager.instance.defaultHouseManager.houseLevels[houseLevels.Length];

            var newLevel = new HouseLevel();
            newLevel.houses = new House[levelObject.numHouses];

            var tempList = houseLevels.ToList();
            tempList.Add(newLevel);

            houseLevels = tempList.ToArray();

            Save();
        }
    }

    public void AddHouse(House house, int index)
    {
        if(index >= 0 && index < houseLevels.Length)
        {
            var level = houseLevels[index];

            level.housesList.Add(house);
            level.houses = level.housesList.ToArray();

            Save();
        }
    }

    private void SaveCounters()
    {
        countryCounter = Country.countryCounter;
    }

    public void SaveMapUnits(MapSceneManager manager)
    {
        List<MapUnit> castleSaveUnits = new List<MapUnit>();

        foreach(var unit in manager.castleManager.castles.Values)
        {
            //if unit is in range of the castle
            castleSaveUnits.Add(unit);
        }
                
        mapResources = manager.resourceManager.resources.Select(resource=> resource.Save()).ToArray();

        Save();
    }

    public ICollection<Country> GetCountries()
    {
        return countryDictionary.Values;
    }

    public Country GetCountry(int id)
    {
        return countryDictionary.GetObject(id);
    }

    public bool CountryExists(int id)
    {
        return countryDictionary.ContainsKey(id);
    }

    public bool isInitialized()
    {
        return initSuccess;
    }

    private void Initialize()
    {
        if(countryDictionary == null)
        {
            countryDictionary = new Dictionary<int, Country>();
        }

        if(houseLevels == null || houseLevels.Length == 0)
        {
            //Debug.Log("null house");

            houseLevels = new HouseLevel[0];                    

            temporaryHouse = new House();
            temporaryHouse.SetHouseObject(GameManager.instance.defaultHouseManager.defaultHouseObject);
        }
        else
        {
            foreach(var level in houseLevels)
            {
                level.Initialize();
            }

            temporaryHouse.Load();
        }

        foreach(var country in countries)
        {
            if (countryDictionary.ContainsKey(country.countryID))
            {
                Debug.Log("Duplicate country id: " + country.countryID);
                continue;
            }

            countryDictionary.Add(country.countryID, country);
        }

        if (myCountry == null || myCountry.name == "")
        {
            initSuccess = false;
            return;
        }

        initSuccess = true;
    }

    public void Save()
    {
        SaveCounters();

        string str = JsonUtility.ToJson(this, true);

        if (!Directory.Exists(DataPath.savePath))
        {
            Directory.CreateDirectory(DataPath.savePath);
        }

        var filePath = DataPath.savePath + "save.json";

        File.WriteAllText(filePath, str);        
    }

    public static GameState Load()
    {
        var filePath = DataPath.savePath + "save.json";
        //var textAsset = Resources.Load<TextAsset>(filePath);

        if (File.Exists(filePath))
        {
            var file = File.ReadAllText(filePath);

            var savedState = JsonUtility.FromJson<GameState>(file);

            if (savedState != null)
            {
                savedState.Initialize();
                
                return savedState;
            }
            else
            {
                Debug.Log("Not a valid save file.");
            }
        }
        else
        {
            Debug.Log("Save file not found at " + filePath);
        }

        return null;
    }
}