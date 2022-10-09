using System;
/// <summary>
/// ui显示层级, 数值小的显示在前（根据自身需要，可扩展所需层级）
/// </summary>
public enum E_UI_Layer
{
    // 提示层
    Tip = 0,
    // 顶层
    Top = 100,
    // 底层
    Bot = 1000,
}

public enum Game_Event 
{
    SceneLoading,
    FragGameChargeCancel,
    FragGameCharge,
    FragGameJump,
    FragGameDirection,
}

public enum Game_Direction
{
    Left = -1,
    Right = 1,
}