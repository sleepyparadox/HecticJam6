using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GearVRCamera : PlayerCamera
{
    private GameObject _eyeCenter;
    public GearVRCamera()
        : base(Assets.Prefabs.OVRCameraRig_CopyPrefab)
    {
        var trackingSpace = new UnityObject(FindChild("TrackingSpace"));
        _eyeCenter = trackingSpace.FindChild("CenterEyeAnchor");
    }

    public override Vector3 LookDirection
    {
        get { return _eyeCenter.transform.forward; }
    }
    public override Vector3 LookSource
    {
        get { return WorldPosition; }
    }
    public override Quaternion LookRotation
    {
        get { return _eyeCenter.transform.rotation; }
    }
}
