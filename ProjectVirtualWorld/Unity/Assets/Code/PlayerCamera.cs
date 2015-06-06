using Sleepy.UnityTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class PlayerCamera : UnityObject
{
    protected PlayerCamera(PrefabAsset prefab)
        : base(prefab)
    {
        
    }

    public abstract Vector3 LookDirection { get;}
    public abstract Vector3 LookSource { get; }

}
