﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MapShip : MapMobileUnit
{
    public Sprite image;

    public SpriteRenderer render;

    [NonSerialized]
    public Ship ship;
       
    protected override void Start()
    {
        ship = GameManager.instance.gameStateManager.gameState.myShip;

        position = ship.position;
        transform.rotation = ship.rotation;

        base.Start();

        if (render != null)
        {
            render.sprite = image;
        }
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

                unitManager.cameraController.setCenterOnShipWhenMoving = true;
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

            unitManager.actionBar.OnShipDestinationReached();
        }
    }

    protected override void OnPositionChanged(Vector3Int oldPos, Vector3Int newPos)
    {
        ship.position = newPos;

        unitManager.mapResourcesManager.OnShipMoved();
    }

    protected override void OnRotationChanged(Quaternion oldRot, Quaternion newRot)
    {
        ship.rotation = newRot;
    }
    
    public override void OnClickEvent()
    {
        if (!unitManager.inputManager.isMultiTouch())
        {
            unitManager.actionBar.OnUnitClick(this);
        }
    }
}