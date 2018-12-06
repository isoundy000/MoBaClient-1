﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭塔
/// </summary>
public class Tower
{
    //位置
    public FixVector3 m_Pos;
    //攻击目标
    public Player m_TargetPlayer;
    //攻击距离
    public Fix64 m_AttackDistince = Fix64.FromRaw(2500);
    //销毁延迟时间
    private Fix64 m_DestoryDelayTime = Fix64.FromRaw(1000);
    //显示对象
    public GameObject m_VGo;
    //塔攻击范围
    public GameObject m_Quan;
    //攻击特效
    public GameObject m_Attack;
    //死亡特效
    public GameObject m_Die;
    //选择特效
    public GameObject m_SelectedGo;
    //显示对象血条组件
    public PlayerHealth m_Health;
    //技能间隔时间
    public Fix64 m_IntervalTime = Fix64.Zero;
    //所属阵营
    public int m_CampId;
    public int m_HP;
    public int m_Type;
    //子弹
    public TowerAttack m_TowerAttack;
    //小地图Icon
    public UISprite m_MapIcon;
    //小地图销毁回调
    public delegate void DestoryMinMapCallback(Tower player);
    public DestoryMinMapCallback m_DestoryMinMapCallback;
    public Tower() { }
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(int campId, int hp, int type)
    {
        m_CampId = campId;
        m_HP = hp;
        m_Type = type;
        m_TowerAttack = new TowerAttack();
        if (m_CampId == 1 && type == 1)
            m_VGo = GameObject.Find("Tower_Blue");
        if (m_CampId == 2 && type == 1)
            m_VGo = GameObject.Find("Tower_Red");
        if (m_CampId == 1 && type == 2)
            m_VGo = GameObject.Find("Camp_Blue");
        if (m_CampId == 2 && type == 2)
            m_VGo = GameObject.Find("Camp_Red");
        m_Quan = m_VGo.transform.Find("quan_hero").gameObject;
        m_Attack = m_VGo.transform.Find("attack0").gameObject;
        m_Die = m_VGo.transform.Find("death").gameObject;
        m_SelectedGo = m_VGo.transform.Find("Effect_targetselected").gameObject;
        m_Health = m_VGo.GetComponent<PlayerHealth>();
        m_Health.m_Health = hp;
        m_Pos = new FixVector3((Fix64)m_VGo.transform.position.x, (Fix64)m_VGo.transform.position.y, (Fix64)m_VGo.transform.position.z);
        MobaMiniMap.instance.AddMapIconByTower(this);
    }

    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        if (m_TowerAttack == null)
            return;
        m_TargetPlayer = FindTarget(m_AttackDistince);
        if (m_TargetPlayer == null)
            m_Quan.SetActive(false);
        else
            m_Quan.SetActive(true);

        Fix64 temp = m_IntervalTime / Fix64.One;
        if (temp > Fix64.One)
            m_IntervalTime = Fix64.Zero;

        if (m_TargetPlayer != null && m_IntervalTime == Fix64.Zero)
            m_TowerAttack.Create(m_VGo, m_TargetPlayer);

        m_TowerAttack.UpdateLogic();
        m_IntervalTime += GameData.m_FixFrameLen;
    }

    /// <summary>
    /// 查找目标
    /// </summary>
    /// <param name="skillNode"></param>
    /// <returns></returns>
    public Player FindTarget(Fix64 attackDistince)
    {
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i].m_PlayerData.m_CampId == m_CampId)
                continue;
            Fix64 distance = FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos);
            if (distance <= attackDistince)
            {
                return GameData.m_PlayerList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="playerAttack">攻击者</param>
    /// <param name="skillNode">攻击技能</param>
    public void FallDamage(int damage)
    {
        if (m_HP <= 0 || damage <= 0)
            return;
        m_HP -= damage;

        if (m_HP <= 0)
        {
            int campId = m_CampId;
            if (m_Type == 2)
                GameData.m_GameManager.GameOver(campId);
            Destroy();
        }
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
            m_Health.m_Health -= damage;
        #endregion
    }

    public void DestoryTowerAttack()
    {
        m_TowerAttack = null;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        GameData.m_GameManager.m_GridManager.SetWalkable(this);
        GameData.m_TowerList.Remove(this);
        if (m_TowerAttack != null)
            m_TowerAttack.Destroy();
        m_IntervalTime = Fix64.Zero;
        m_Pos = FixVector3.Zero;
        m_TowerAttack = null;
        m_TargetPlayer = null;
        #region 显示层
        if (m_Die != null)
            m_Die.SetActive(true);
        if (GameData.m_IsExecuteViewLogic)
        {
            GameObject.Destroy(m_VGo, (float)m_DestoryDelayTime);
        }
        if (m_DestoryMinMapCallback != null)
            m_DestoryMinMapCallback(this);
        #endregion
        m_Die = null;
        m_Quan = null;
        m_VGo = null;
    }
}