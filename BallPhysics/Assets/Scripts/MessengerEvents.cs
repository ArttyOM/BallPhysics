using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MessengerEvents
{
    public const string kBallClicked = "BallClicked";
    public const string kDoubleClick = "DoubleClickDetected";
    public const string kCameraOwnerChanged = "CameraOwnerChanged";

    public const string kLevelChanged = "LevelChanged";
    public enum Level { Base = 1, Mid = 2, Advanced = 3}
}
