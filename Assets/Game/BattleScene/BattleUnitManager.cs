﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{
    public BattleEffectsManager effectsManager;
    public ProjectileManager projectileManager;

    public BattleUnit battleUnitPrefab;
    public Transform unitParent;

    public Transform playerCastlePosition;
    public Transform enemyCastlePosition;

    [NonSerialized]
    public List<BattleUnit> playerUnits = new List<BattleUnit>();
    [NonSerialized]
    public List<BattleUnit> computerUnits = new List<BattleUnit>();
    [NonSerialized]
    public List<BattleUnit> allUnits = new List<BattleUnit>();

    [NonSerialized]
    public Queue<BattleUnit> removeUnits = new Queue<BattleUnit>();

    public void CreateUnit(BattleUnitObject unitObj, BattleUnitTeam team)
    {
        var startPos = team == BattleUnitTeam.Player ? playerCastlePosition.position : enemyCastlePosition.position;

        var newUnit = Instantiate(battleUnitPrefab, unitParent);
        newUnit.transform.position = startPos;

        if(team == BattleUnitTeam.Player)
        {
            playerUnits.Add(newUnit);                      
        }
        else
        {
            computerUnits.Add(newUnit);
        }

        newUnit.Initialize(unitObj, team, this);
        allUnits.Add(newUnit);
    }

    private void Update()
    {
        RemoveUnits();

        foreach (var unit in allUnits)
        {
            unit.PreAttack();
            unit.Attack();
            unit.Move();           
        }

        RemoveUnits();
    }

    public void RemoveUnits()
    {
        while(removeUnits.Count > 0)
        {
            var unit = removeUnits.Dequeue();

            if (unit != null)
            {
                playerUnits.Remove(unit);
                computerUnits.Remove(unit);
                allUnits.Remove(unit);

                Destroy(unit.gameObject);
            }
        }
    }

    public void Restart()
    {
        foreach(var unit in allUnits)
        {
            unit.Death();
        }
    }
}