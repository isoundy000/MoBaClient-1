﻿using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色AI
/// </summary>
public class PlayerAI
{
    private Player m_Player;
    //寻路参数
    private string m_Parameter;
    //寻路坐标
    private List<FixVector3> m_FixVectorPath;
    //寻路停止距离
    private Fix64 m_distince = Fix64.FromRaw(50);
    /// <summary>
    /// 初始化AI数据
    /// </summary>
    /// <param name="player"></param>
    public void OnInit(Player player)
    {
        m_Player = player;
        Path p = ABPath.Construct(m_Player.m_Pos.ToVector3(), m_Player.m_PlayerData.m_NaviPos.ToVector3(), OnPathComplete);
        AstarPath.StartPath(p);
    }

    public void OnPathComplete(Path p)
    {
        m_FixVectorPath = new List<FixVector3>();
        for (int i = 0; i < p.vectorPath.Count; i++)
        {
            FixVector3 fixV3 = (FixVector3)(p.vectorPath[i]);
            m_FixVectorPath.Add(fixV3);
        }
    }

    /// <summary>
    /// 进入AI
    /// </summary>
    public void OnEnter()
    {
        if (m_Player == null || m_Player.m_PlayerData == null || m_Player.m_PlayerData.m_Type == 1 || m_Player.m_PlayerData.m_NaviPos == null)
            return;
        FixVector3 relativePos = m_Player.m_PlayerData.m_CampId == 1 ? (m_Player.m_PlayerData.m_NaviPos - m_Player.m_Pos) : (m_Player.m_Pos - m_Player.m_PlayerData.m_NaviPos);
        Quaternion rotation = Quaternion.LookRotation(relativePos.ToVector3(), Vector3.up);
        m_Player.m_Rotation = (FixVector3)(rotation.eulerAngles);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Player.m_VGo.transform.rotation = rotation;
        m_Player.m_Angles = (FixVector3)(new Vector3(m_Player.m_VGo.transform.forward.normalized.x, 0, m_Player.m_VGo.transform.forward.normalized.z));
        #endregion

        if (m_Player.m_PlayerData.m_CampId == 1)
            m_Parameter = string.Format("{0}#{1}#{2}", m_Player.m_Angles.x, m_Player.m_Angles.y, m_Player.m_Angles.z);
        if (m_Player.m_PlayerData.m_CampId == 2)
            m_Parameter = string.Format("{0}#{1}#{2}", -m_Player.m_Angles.x, -m_Player.m_Angles.y, -m_Player.m_Angles.z);
    }

    /// <summary>
    /// 每帧更新AI逻辑
    /// </summary>
    public void UpdateLogic()
    {
        if (m_Player == null || m_Player.m_PlayerData == null || m_Player.m_PlayerData.m_Type == 1)
            return;
        if (m_Player.m_IsDie)
            return;
        if (m_Player.m_IsAttack)
            return;
        if (m_Player.m_IsSkill)
            return;
        if (m_Player.m_IsHit)
            return;
        m_Player.m_SkillNode = m_Player.m_PlayerData.GetSkillNode(1);
        Player targetPlayer = m_Player.FindTarget(m_Player.m_SkillNode);
        Tower targetTower = m_Player.FindTowerTarget(m_Player.m_SkillNode);
        if (targetPlayer != null || targetTower != null)
        {
            m_Player.m_State = new AttackState();
            m_Player.m_State.OnInit(m_Player, "1");
            m_Player.m_State.OnEnter();
        }
        else
        {
            m_Player.m_State = new MoveState();
            m_Player.m_State.OnInit(m_Player, m_Parameter);
            m_Player.m_State.OnEnter();
            Fix64 distince = FixVector3.Distance(m_Player.m_PlayerData.m_NaviPos, m_Player.m_Pos);
            if (distince <= m_distince)
            {
                m_Player.m_State = new MoveEndState();
                m_Player.m_State.OnInit(m_Player);
                m_Player.m_State.OnEnter();
            }
        }
    }
}