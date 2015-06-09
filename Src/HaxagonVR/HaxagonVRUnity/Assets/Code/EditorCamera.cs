using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EditorCamera : UnityObject, IPlayerCamera
{
    UnityObject VerticalCamera;
    public EditorCamera()
        : base(Assets.Prefabs.EditorCameraPrefab)
    {
        VerticalCamera = new UnityObject(Assets.Prefabs.EditorCameraPrefab);
        VerticalCamera.Parent = this;
        VerticalCamera.LocalPosition = Vector3.zero;
        VerticalCamera.Transform.localRotation = Quaternion.identity;

        UnityUpdate += Update;
    }

    void Update(UnityObject me)
    {
        var rotateAmount = 1000f;
        Transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * rotateAmount * Time.deltaTime, Space.World);
        VerticalCamera.Transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * rotateAmount * Time.deltaTime * -1f, Space.Self);
    }

    public Vector3 LookDirection
    {
        get { return VerticalCamera.Transform.forward; }
    }

    public Vector3 LookSource
    {
        get { return VerticalCamera.Transform.position; }
    }

    public Quaternion LookRotation
    {
        get { return VerticalCamera.Transform.rotation; }
    }
}
