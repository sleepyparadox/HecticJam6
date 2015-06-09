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

public class UnityObject
{
    public readonly GameObject GameObject;

    //Exposed functionality
    public Vector3 WorldPosition { get { return GameObject.transform.position; } set { GameObject.transform.position = value; } }
    public Vector3 LocalPosition { get { return GameObject.transform.localPosition; } set { GameObject.transform.localPosition = value; } }
    public Transform Transform { get { return GameObject.transform; } }
    public Transform Parent { get { return Transform.parent; } set { Transform.parent = value; } }

    //Exposed events
    public UnityEventAction<UnityObject> UnityMouseEnter;
    public UnityEventAction<UnityObject> UnityMouseExit;
    public UnityEventAction<UnityObject> UnityMouseUp;
    public UnityEventAction<UnityObject> UnityMouseOver;
    public UnityEventAction<UnityObject> UnityUpdate;
    public UnityEventAction<UnityObject> UnityFixedUpdate;
    public UnityEventAction<UnityObject> UnityStart;
    public UnityEventAction<UnityObject> OnDispose;
    public UnityEventAction<UnityObject> UnityGUI;
    public UnityEventAction<UnityObject> UnityDrawGizmos;
    public UnityEventAction<UnityObject> UnityInspectorGUI;
    public UnityEventAction<UnityObject, Collision> UnityOnCollisionEnter;

    private UnityObjectBehaviour _behaviour;

    public UnityObject()
        : this(new GameObject("Unnamed UnityObject"))
    {
        GameObject.name = GetType().ToString();
    }

    public UnityObject(string name)
        : this(new GameObject(name))
    {
    }

    public UnityObject(PrefabAsset asset)
        : this(asset.Prefab.Clone())
    {

    }

    public UnityObject(GameObject sceneObject)
    {
        if (sceneObject == null)
            throw new Exception("UnityObject sceneObject is null");

        GameObject = sceneObject;

        //Try to get the existing behaviour
        _behaviour = sceneObject.GetComponent<UnityObjectBehaviour>();

        if (_behaviour == null)
            _behaviour = sceneObject.AddComponent<UnityObjectBehaviour>();

        _behaviour.UnityObject = this;
    }
    public static GameObject CreatePrefabInstance(string prefabPath)
    {
        return CreatePrefabInstance(prefabPath, Vector3.zero, Quaternion.identity);
    }
    public static GameObject CreatePrefabInstance(string prefabPath, Vector3 position, Quaternion rotation)
    {
        var prefab = Resources.Load(prefabPath);
        return (GameObject)UnityEngine.Object.Instantiate(prefab, position, rotation);
    }
    public static GameObject FindInScene(string name)
    {
        return GameObject.Find(name);
    }

    public bool Alive
    {
        get
        {
            return GameObject != null;
        }
    }

    public void SetActive(bool active)
    {
        GameObject.SetActive(active);
    }

    public virtual void Dispose()
    {
        if (OnDispose != null)
            OnDispose.Fire(this);
        if (GameObject != null)
            UnityEngine.Object.Destroy(GameObject);
    }
        
    public GameObject FindChild(string child)
    {
        return GameObject.transform.FindChild(child).gameObject;
    }

    public T0 FindChildComponent<T0>(string child) where T0 : Component
    {
        return GameObject.transform.FindChild(child).GetComponent<T0>();
    }

    public void SetLayer(int layer, bool recursive)
    {
        if (recursive)
            RecursiveTransform(t => t.gameObject.layer = layer);
        else
            GameObject.layer = layer;
    }

    public void RecursiveTransform(Action<Transform> operation)
    {
        RecursiveTransform(Transform, operation);
    }

    public static void RecursiveTransform(Transform target, Action<Transform> operation)
    {
        operation(target);
        for (var i = 0; i < target.childCount; ++i)
        {
            RecursiveTransform(target.GetChild(i), operation);
        }
    }

    public static implicit operator Transform(UnityObject unityObject)
    {
        return unityObject.Transform;
    }
}
