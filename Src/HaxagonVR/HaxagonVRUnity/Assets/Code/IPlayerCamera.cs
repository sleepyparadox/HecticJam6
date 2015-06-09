using Sleepy.UnityTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IPlayerCamera
{
    Vector3 LookDirection { get;}
    Vector3 LookSource { get; }
    Quaternion LookRotation { get; }
}
