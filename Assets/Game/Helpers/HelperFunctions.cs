﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
    public void GoBack()
    {
        GameManager.instance.sceneChanger.GoBack();
    }
}