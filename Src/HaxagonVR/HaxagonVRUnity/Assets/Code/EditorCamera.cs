using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EditorCamera : PlayerCamera
{
    public EditorCamera()
        : base(Assets.Prefabs.EditorCameraPrefab)
    {
        UnityUpdate += Update;
    }

    void Update(UnityObject me)
    {
        var rotateAmount = 1000f;
        Transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * rotateAmount * Time.deltaTime, Space.World);
        Transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * rotateAmount * Time.deltaTime * -1f, Space.Self);

    }

    public override Vector3 LookDirection
    {
        get { return Transform.forward; }
    }

    public override Vector3 LookSource
    {
        get { return Transform.position; }
    }

    public override Quaternion LookRotation
    {
        get { return Transform.rotation; }
    }
}
