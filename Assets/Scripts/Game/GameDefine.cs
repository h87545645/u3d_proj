using System;
/// <summary>
/// ui?????, ????????????????????????????????????????
/// </summary>
public enum E_UI_Layer
{
    // ?????
    Tip = 0,
    // ????
    Top = 100,
    // ???
    Bot = 1000,
}

public enum Game_Event 
{
    SceneLoading,
    FragGameChargeCancel,
    FragGameCharge,
    FragGameJump,
    FragGameDirection,
    FragGameCameraMove,
    FragStanding,
    FragActiveAllUI,
    FragGameFinish,
}

public enum Game_Direction
{
    None = 0,
    Left = -1,
    Right = 1,
}

public static class GlobalValue
{
    public static float JumpMaxChargeTime = 1;
}