﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗管理
/// </summary>
public class BattleLogicManager
{
    /// <summary>
    /// 根据不同的操作调用状态机
    /// </summary>
    /// <param name="data"></param>
    public void OnOperation(KeyData data)
    {
        Player m_OperationPlayer = GetOperationPlayer(data.m_RoleId);
        if (m_OperationPlayer == null)
            return;
        BaseState state = null;
        switch ((Cmd)data.m_Cmd)
        {
            case Cmd.MoveEnd:
                state = new MoveEndState();
                break;
            case Cmd.Move:
                state = new MoveState();
                break;
            case Cmd.UseSkill:
                state = new SkillState();
                break;
            case Cmd.Attack:
                state = new AttackState();
                break;
            case Cmd.Turn:
                break;
            default:
                break;
        }
        if (state == null)
            return;
        m_OperationPlayer.ChangeState(state, data.m_Parameter);
    }

    /// <summary>
    /// 每帧处理游戏物体状态逻辑
    /// </summary>
    public void UpdateLogic()
    {
        for (int i = 0; i < GameData.m_ObjectList.Count; i++)
        {
            if (GameData.m_ObjectList[i] == null || GameData.m_ObjectList[i].m_Data == null)
                continue;
            GameData.m_ObjectList[i].UpdateLogic();
        }
    }

    /// <summary>
    /// 获取当前操作对象
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public Player GetOperationPlayer(int roleId)
    {

        for (int i = 0; i < GameData.m_ObjectList.Count; i++)
        {
            if (!(GameData.m_ObjectList[i] is Player))
                continue;
            Player player = GameData.m_ObjectList[i] as Player;
            if (player.m_PlayerData.m_RoleId == roleId)
            {
                return player;
            }
        }
        return null;
    }
}
