﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MapShip : MapMobileUnit
{
    public Sprite image;

    public SpriteRenderer render;

    private Ship ship;
        
    protected override void Start()
    {
        base.Start();

        if (render != null)
        {
            render.sprite = image;
        }

        ship = GameManager.instance.gameStateManager.gameState.myShip;
    }

    public override bool SetMovePosition(Vector3Int pos)
    {
        var hasPath = base.SetMovePosition(pos);

        if (this == unitManager.myShip)
        {
            if (hasPath)
            {
                if (unitManager.actionBar.myDestinationFlag != null)
                {
                    Destroy(unitManager.actionBar.myDestinationFlag);
                }

                var worldPos = tilemap.CellToWorld(targetPos);

                unitManager.actionBar.myDestinationFlag = Instantiate(unitManager.actionBar.flagPrefab, MapSceneManager.instance.overlayMap.transform);
                unitManager.actionBar.myDestinationFlag.transform.position = worldPos;
            }
            else
            {
                var worldPos = tilemap.CellToWorld(pos);

                var xMark = Instantiate(unitManager.actionBar.xMarkPrefab, MapSceneManager.instance.overlayMap.transform);
                xMark.transform.position = worldPos;
            }
        }

        return hasPath;
    }

    protected override void OnDestinationReached()
    {
        if (this == unitManager.myShip)
        {
            if (unitManager.actionBar.myDestinationFlag != null)
            {
                Destroy(unitManager.actionBar.myDestinationFlag);
            }
        }
    }

    public override void OnClickEvent()
    {
        if (!unitManager.inputManager.isMultiTouch())
        {
            unitManager.actionBar.OnUnitClick(this);

            Debug.Log("click");
        }
    }
}