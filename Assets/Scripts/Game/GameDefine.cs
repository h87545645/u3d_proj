using System;
/// <summary>
/// ui��ʾ�㼶, ��ֵС����ʾ��ǰ������������Ҫ������չ����㼶��
/// </summary>
public enum E_UI_Layer
{
    // ��ʾ��
    Tip = 0,
    // ����
    Top = 100,
    // �ײ�
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