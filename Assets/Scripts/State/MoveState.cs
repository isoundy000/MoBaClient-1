﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{
    Fix64 m_MoveSpeed;
    #region 显示层
#if IS_EXECUTE_VIEWLOGIC
    //动画状态机
    private Animator m_Animator;
    //动画名称
    private string m_StateParameter = "State";
    //插值时长
    private float m_Interpolation = 1;
#endif
    #endregion

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="viewPlayer"></param>
    /// <param name="parameter"></param>
    public override void OnInit(Player viewPlayer, string parameter = null)
    {
        base.OnInit(viewPlayer, parameter);
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        if (m_Parameter == null || !m_Parameter.Contains("#"))
            return;
        float x = float.Parse(m_Parameter.Split('#')[0]);
        float z = float.Parse(m_Parameter.Split('#')[2]);
        m_Player.m_Angles = (FixVector3)(new Vector3(x, 0, z));
        m_MoveSpeed = m_Player.m_Speed + m_Player.m_Speed;
        if (m_Player.m_PlayerData.m_Type == 1)
        {
            float posX = float.Parse(m_Parameter.Split('#')[3]);
            float posY = float.Parse(m_Parameter.Split('#')[4]);
            float posZ = float.Parse(m_Parameter.Split('#')[5]);
            Fix64 fixX = (Fix64)posX + m_MoveSpeed * (Fix64)x;
            Fix64 fixZ = (Fix64)posZ + m_MoveSpeed * (Fix64)z;
            m_Player.m_Pos = new FixVector3(fixX, (Fix64)4.8F, fixZ);
        }
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Animator == null)
                m_Animator = m_Player.m_VGo.GetComponent<Animator>();
        }
        #endregion
    }

    /// <summary>
    /// 开始状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        m_Player.m_IsMove = true;
        Quaternion targetRotation = Quaternion.LookRotation((m_Player.m_Pos + m_Player.m_Angles - m_Player.m_Pos).ToVector3(), Vector3.up);
        m_Player.m_Rotation = (FixVector3)(targetRotation.eulerAngles);
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            //插值旋转可以优化旋转抖动，不流畅等问题
            //m_Player.m_VGo.transform.rotation = Quaternion.Slerp(m_Player.m_VGo.transform.rotation, targetRotation, m_Interpolation);
            m_Player.m_VGo.transform.rotation = targetRotation;
            m_Animator.SetInteger(m_StateParameter, 11);
        }
        #endregion
    }

    /// <summary>
    /// 每帧刷新状态
    /// </summary>
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        if (!m_Player.m_IsMove)
            return;
        Fix64 speed = m_Player.m_Speed;
        if (m_Player.m_PlayerData.m_Type == 1)
            speed = m_MoveSpeed;
        FixVector3 pos = m_Player.m_Pos + (speed * m_Player.m_Angles);
        Vector2 gridPos = GameData.m_GameManager.m_GridManager.MapPosToGrid(pos.ToVector3());
        bool isWalk = GameData.m_GameManager.m_GridManager.GetWalkable(gridPos);
        if (!isWalk)
            return;
        m_Player.m_IntervalTime += GameData.m_FixFrameLen;
        m_Player.m_Pos = pos;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Player.m_VGo.transform.position = m_Player.m_Pos.ToVector3();
        #endregion
    }

    /// <summary>
    /// 退出状态
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        if (m_Player == null || m_Player.m_PlayerData == null)
            return;
        m_Player.m_IsMove = false;
        m_Player.m_IntervalTime = Fix64.Zero;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Animator.SetInteger(m_StateParameter, 0);
        #endregion
    }
}
