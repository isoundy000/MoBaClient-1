﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 显示层对象
/// </summary>
public class Player
{
    #region 逻辑层
    //位置
    public FixVector3 m_Pos;
    //转向
    public FixVector3 m_Rotation;
    //缩放
    public FixVector3 m_Scale;
    //攻击目标
    public Player m_TargetPlayer;
    //攻击目标
    public Tower m_TargetTower;
    //显示对象角度
    public FixVector3 m_Angles;
    //对象数据
    public PlayerData m_PlayerData;
    //角色AI
    public PlayerAI m_PlayerAI;
    //是否移动
    public bool m_IsMove = false;
    //是否普攻
    public bool m_IsAttack = false;
    //是否技能
    public bool m_IsSkill = false;
    //是否死亡
    public bool m_IsDie = false;
    //是否后仰
    public bool m_IsHit = false;
    //当前状态
    public BaseState m_State;
    //技能间隔时间
    public Fix64 m_IntervalTime = Fix64.Zero;
    //移动速度
    public Fix64 m_Speed = Fix64.FromRaw(1000);
    //技能索引
    public int m_SkillIndex = 0;
    //当前技能信息
    public SkillNode m_SkillNode;
    //销毁延迟时间
    private Fix64 m_DestoryDelayTime = Fix64.FromRaw(1000);

    #endregion
    #region 显示层

#if IS_EXECUTE_VIEWLOGIC
    //显示对象
    public GameObject m_VGo;
    //显示对象血条组件
    public PlayerHealth m_Health;
    //显示对象飘血组件
    public PlayerHudText m_HudText;
    //选中对象
    public GameObject m_SelectedGo;
    //角色控制器
    public CharacterController m_Controller;
    //小地图Icon
    public UISprite m_MapIcon;
    //小地图销毁回调
    public delegate void DestoryMinMapCallback(Player player);
    public DestoryMinMapCallback m_DestoryMinMapCallback;
#endif
    #endregion
    public Player()
    {
        m_Pos = FixVector3.Zero;
        m_Rotation = FixVector3.Zero;
        m_Angles = FixVector3.Zero;
        m_Scale = FixVector3.Zero;
        m_TargetPlayer = null;
        m_TargetTower = null;
        m_PlayerData = null;
        m_IsMove = false;
        m_IsAttack = false;
        m_IsSkill = false;
        m_IsDie = false;
        m_IsHit = false;
        m_State = null;
        m_IntervalTime = Fix64.Zero;
        m_SkillIndex = 0;
        m_SkillNode = null;
        m_PlayerAI = null;
    }
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="charData">对象数据</param>
    public void Create(PlayerData playerData)
    {
        #region 逻辑层
        m_PlayerData = new PlayerData(playerData.m_Id, playerData.m_HeroId, playerData.m_Name, playerData.m_CampId, playerData.m_Type);
        GameObject posGo = null;
        if (playerData.m_Type == 1)
            posGo = GameObject.Find(string.Format("203_SceneCtrl_Moba_1/Pos{0}", m_PlayerData.m_CampId));
        if (playerData.m_Type == 2)
            posGo = GameObject.Find(string.Format("203_SceneCtrl_Moba_1/JinZhan{0}", m_PlayerData.m_CampId));
        if (playerData.m_Type == 3)
            posGo = GameObject.Find(string.Format("203_SceneCtrl_Moba_1/YuanCheng{0}", m_PlayerData.m_CampId));
        if (playerData.m_Type == 4)
            posGo = GameObject.Find(string.Format("203_SceneCtrl_Moba_1/PaoChe{0}", m_PlayerData.m_CampId));
        m_Pos = (FixVector3)posGo.transform.position;
        m_Rotation = (FixVector3)(posGo.transform.rotation.eulerAngles);
        m_Scale = (FixVector3)(posGo.transform.localScale);
        m_Angles = (FixVector3)(new Vector3(posGo.transform.forward.normalized.x, 0, posGo.transform.forward.normalized.z));
        #endregion
        #region 显示层
        //是否执行显示层逻辑
        if (GameData.m_IsExecuteViewLogic)
        {
            GameObject go = Resources.Load<GameObject>(m_PlayerData.m_ModelPath);
            m_VGo = GameObject.Instantiate(go);
            m_VGo.transform.parent = posGo.transform.parent;
            m_VGo.transform.localPosition = m_Pos.ToVector3();
            m_VGo.transform.localRotation = Quaternion.Euler(m_Rotation.ToVector3());
            m_VGo.transform.localScale = m_Scale.ToVector3();
            m_SelectedGo = m_VGo.transform.Find("Effect_targetselected01").gameObject;
            m_SelectedGo.SetActive(false);
            m_Health = m_VGo.GetComponent<PlayerHealth>();
            m_HudText = m_VGo.GetComponent<PlayerHudText>();
            m_VGo.name = playerData.m_Id.ToString();
            m_Health.m_Health = m_PlayerData.m_HP;
            if (playerData.m_Type == 1 && playerData.m_Id == GameData.m_CurrentRoleId)
            {
                GameData.m_CampId = playerData.m_CampId;
                GameData.m_CurrentPlayer = this;
                m_VGo.tag = "Player";
                GameObject cameraPosGo = GameObject.Find(string.Format("CameraPos{0}", m_PlayerData.m_CampId));
                Camera.main.transform.localPosition = cameraPosGo.transform.localPosition;
                Camera.main.transform.localRotation = cameraPosGo.transform.localRotation;
                Camera.main.transform.localScale = cameraPosGo.transform.localScale;
                //显示主界面UI，关闭复活UI
                GameData.m_GameManager.m_UIManager.m_UpdatePlayerDieUICallback(false);
                //更新技能图标
                GameData.m_GameManager.m_UIManager.m_UpdateSkillUICallback(m_PlayerData.m_SkillList);
            }
            //关闭敌方复活UI
            if (playerData.m_Type == 1 && playerData.m_Id != GameData.m_CurrentRoleId)
                GameData.m_GameManager.m_UIManager.m_UpdateEnemyDieUICallback(false, m_PlayerData);
            MobaMiniMap.instance.AddMapIconByType(this);
        }
        #endregion
        m_PlayerAI = new PlayerAI();
        m_PlayerAI.OnInit(this);
        m_PlayerAI.OnEnter();
    }

    /// <summary>
    ///切换状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="parameter"></param>
    public void ChangeState(BaseState state, string parameter)
    {
        if (m_PlayerData == null)
            return;
        if (state is SkillState && parameter.Equals("8"))
        {
            AddHp(500);
            if (m_PlayerData.m_Id == GameData.m_CurrentRoleId)
                GameData.m_GameManager.m_UIManager.m_UpdateSkillCDUICallback(30, 8);
        }
        else
        {
            if (m_State != null)
                m_State.OnExit();
            m_State = state;
            m_State.OnInit(this, parameter);
            m_State.OnEnter();
        }
    }

    /// <summary>
    /// 加血
    /// </summary>
    public void AddHp(int hp)
    {
        int addHp = m_PlayerData.m_HeroAttrNode.hp - m_PlayerData.m_HP;
        if (addHp <= 0)
            return;
        addHp = addHp >= hp ? hp : addHp;
        m_PlayerData.m_HP += addHp;
        m_Health.m_Health += addHp;
        if (m_PlayerData.m_Id == GameData.m_CurrentRoleId)
            m_HudText.PlayerHUDText.AddLocalized(string.Format("+{0}", hp), new Color(0.09F, 0.9F, 0.09F, 1), 1);
    }


    /// <summary>
    /// 遍历状态
    /// </summary>
    public void UpdateLogic()
    {
        if (m_PlayerAI == null)
            return;
        m_PlayerAI.UpdateLogic();
        if (m_State == null)
            return;
        m_State.UpdateLogic();

        if (m_PlayerData != null && m_PlayerData.m_Type == 1)
            GameData.m_GameManager.m_UIManager.m_UpdateAddHpCallback(this);
    }

    /// <summary>
    /// 查找离玩家最近的可攻击角色目标
    /// </summary>
    /// <param name="skillNode"></param>
    /// <returns></returns>
    public Player FindTarget(SkillNode skillNode)
    {
        Fix64 preDistance = Fix64.Zero;
        Player prePlayer = null;
        for (int i = 0; i < GameData.m_PlayerList.Count; i++)
        {
            if (GameData.m_PlayerList[i] == null || GameData.m_PlayerList[i].m_PlayerData == null)
                continue;
            if (GameData.m_PlayerList[i].m_PlayerData.m_CampId == m_PlayerData.m_CampId)
                continue;
            Fix64 distance = GameData.m_PlayerList[i].m_PlayerData.m_Type == 1 ? (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.FromRaw(200)) : (FixVector3.Distance(GameData.m_PlayerList[i].m_Pos, m_Pos) - Fix64.FromRaw(100));
            if ((preDistance == Fix64.Zero || preDistance > distance) && (float)distance <= skillNode.aoe_long)
            {
                prePlayer = GameData.m_PlayerList[i];
                preDistance = distance;
            }
        }
        return prePlayer;
    }

    /// <summary>
    /// 查找离玩家最近的可攻击箭塔目标
    /// </summary>
    /// <param name="skillNode"></param>
    /// <returns></returns>
    public Tower FindTowerTarget(SkillNode skillNode)
    {
        Fix64 preDistance = Fix64.Zero;
        Tower preTower = null;
        for (int i = 0; i < GameData.m_TowerList.Count; i++)
        {
            if (GameData.m_TowerList[i] == null)
                continue;
            if (GameData.m_TowerList[i].m_CampId == m_PlayerData.m_CampId)
                continue;
            Fix64 distance = GameData.m_TowerList[i].m_Type == 1 ? (FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos) - Fix64.FromRaw(500)) : (FixVector3.Distance(GameData.m_TowerList[i].m_Pos, m_Pos) - Fix64.One);
            if ((preDistance == Fix64.Zero || preDistance > distance) && (float)distance <= skillNode.aoe_long)
            {
                preTower = GameData.m_TowerList[i];
                preDistance = distance;
            }
        }
        return preTower;
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="playerAttack">攻击者</param>
    /// <param name="skillNode">攻击技能</param>
    public void FallDamage(int damage)
    {
        if (m_PlayerData == null || m_PlayerData.m_HP <= 0 || damage <= 0)
            return;
        m_PlayerData.m_HP -= damage;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_Health != null)
                m_Health.m_Health -= damage;
            if (m_HudText != null && m_PlayerData != null && GameData.m_CurrentPlayer != null && GameData.m_CurrentPlayer.m_PlayerData != null && m_PlayerData.m_Id == GameData.m_CurrentPlayer.m_PlayerData.m_Id)
                m_HudText.PlayerHUDText.AddLocalized(string.Format("-{0}", damage), Color.red, 0f);
        }
        #endregion
        if (m_PlayerData.m_HP <= 0)
        {
            m_State = new DieState();
            m_State.OnInit(this);
            m_State.OnEnter();
            #region 显示层
            if (GameData.m_IsExecuteViewLogic)
            {
                if (m_PlayerData.m_Type == 1 && m_PlayerData.m_Id == GameData.m_CurrentRoleId)
                    GameData.m_GameManager.m_UIManager.m_UpdatePlayerDieUICallback(true);
                if (m_PlayerData.m_Type == 1 && m_PlayerData.m_Id != GameData.m_CurrentRoleId)
                    GameData.m_GameManager.m_UIManager.m_UpdateEnemyDieUICallback(true, m_PlayerData);
            }
            #endregion
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        GameData.m_PlayerList.Remove(this);
        m_PlayerData = null;
        m_IsMove = false;
        m_IsAttack = false;
        m_IsSkill = false;
        m_IsDie = false;
        m_IsHit = false;
        m_State = null;
        #region 显示层
        if (GameData.m_IsExecuteViewLogic)
        {
            if (m_TargetPlayer != null && m_TargetPlayer.m_SelectedGo != null)
                m_TargetPlayer.m_SelectedGo.SetActive(false);
            if (m_VGo != null)
                GameObject.Destroy(m_VGo, (float)m_DestoryDelayTime);
            m_VGo = null;
        }
        if (m_DestoryMinMapCallback != null)
            m_DestoryMinMapCallback(this);
        #endregion
    }
}
