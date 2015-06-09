///
/// The latest version of SleepyUnityTools can be found at 
/// https://github.com/sleepyparadox/SleepyUnityTools
///
/// The MIT License (MIT)
///
/// Copyright (c) 2015 Don Logan (sleepyparadox)
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:

/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.

/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Sleepy.UnityTools;

public class UnityObjectBehaviour : MonoBehaviour
{
    public UnityObject UnityObject;

    public void Start()
    {
        //Normal process
        //new UnityObject, UnityObjectBehaviour, Awake(), set UnityObject, Step()

        //UnityObject should always be set
        if (UnityObject == null)
        {
            //Disable incase we continue in editor
            enabled = false;
            throw new Exception(gameObject.name + " has UnityObjectBehaviour but doesn't have UnityObject assigned, bad prefab?");
        }

        if (UnityObject.UnityStart != null)
            UnityObject.UnityStart.Fire(UnityObject);
    }

    private void OnMouseEnter()
    {
        if (UnityObject.UnityMouseEnter != null)
            UnityObject.UnityMouseEnter.Fire(UnityObject);
    }
    private void OnMouseExit()
    {
        if (UnityObject.UnityMouseExit != null)
            UnityObject.UnityMouseExit.Fire(UnityObject);
    }
    private void OnMouseUp()
    {
        if (UnityObject.UnityMouseUp != null)
            UnityObject.UnityMouseUp.Fire(UnityObject);
    }
    private void OnGUI()
    {
        if (UnityObject.UnityGUI != null)
            UnityObject.UnityGUI.Fire(UnityObject);
    }
    private void Update()
    {
        if (UnityObject.UnityUpdate != null)
            UnityObject.UnityUpdate.Fire(UnityObject);
    }
    private void FixedUpdate()
    {
        if (UnityObject.UnityFixedUpdate != null)
            UnityObject.UnityFixedUpdate.Fire(UnityObject);
    }
    private void OnMouseOver()
    {
        if (UnityObject.UnityMouseOver != null)
            UnityObject.UnityMouseOver.Fire(UnityObject);
    }
    private void OnDrawGizmos()
    {
        //Start has nice clean error
        if (UnityObject == null)
            return;

        if (UnityObject.UnityDrawGizmos != null)
            UnityObject.UnityDrawGizmos.Fire(UnityObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Start has nice clean error
        if (UnityObject == null)
            return;

        if (UnityObject.UnityOnCollisionEnter != null)
            UnityObject.UnityOnCollisionEnter.Fire(UnityObject, collision);
    }
}