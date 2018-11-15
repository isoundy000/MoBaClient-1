﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏入口
/// </summary>
public class MainBehaviour : MonoBehaviour
{
    //普攻按钮
    public GameObject m_AttackGo;
    //技能一
    public GameObject m_Skill1Go;
    //技能二
    public GameObject m_Skill2Go;
    //技能三
    public GameObject m_Skill3Go;
    //技能四
    public GameObject m_Skill4Go;
    //技能索引
    private int m_Index = 0;
    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        //修改每帧时间长度
        Time.timeScale = 1f;
        //允许后台运行
        Application.runInBackground = true;
        //固定50帧
        Application.targetFrameRate = 50;
    }
    /// <summary>
    /// 开始准备游戏数据以及网络连接
    /// 初始化UI
    /// </summary>
    private void Start()
    {
        GameData.m_GameManager = new GameManager();
        GameData.m_GameManager.InitGame();
        GameData.m_GameManager.InputReady();

        UIEventListener.Get(m_AttackGo).onClick = OnAttackClick;
        UIEventListener.Get(m_Skill1Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill2Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill3Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill4Go).onClick = OnSkillClick;
        GameData.m_GameManager.m_UIManager.m_UpdateSkillUICallback = OnUpdateSkillUI;
    }

    /// <summary>
    /// 每帧更新游戏逻辑
    /// </summary>
    private void FixedUpdate()
    {
        if (GameData.m_GameManager == null)
            return;
        GameData.m_GameManager.UpdateGame();
    }

    /// <summary>
    /// 销毁游戏数据
    /// </summary>
    private void OnDestroy()
    {
        if (GameData.m_GameManager == null)
            return;
        GameData.m_GameManager.DestoryGame();
    }

    /// <summary>
    /// 点击普攻
    /// </summary>
    /// <param name="go"></param>
    private void OnAttackClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsSkill)
            return;
        if (GameData.m_CurrentPlayer.m_IsDie)
            return;
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        GameData.m_CurrentPlayer.m_IsAttack = true;
        GameData.m_GameManager.InputCmd(Cmd.Attack);
    }

    /// <summary>
    /// 点击技能
    /// </summary>
    /// <param name="go"></param>
    private void OnSkillClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsSkill)
            return;
        if (GameData.m_CurrentPlayer.m_IsAttack)
            return;
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        if (!int.TryParse(go.name.Substring(go.name.Length - 1, 1), out m_Index))
            return;
        GameData.m_GameManager.InputCmd(Cmd.UseSkill, m_Index.ToString());
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    /// <param name="skillNodeList"></param>
    private void OnUpdateSkillUI(List<SkillNode> skillNodeList)
    {
        m_Skill1Go.GetComponent<UISprite>().spriteName = skillNodeList[3].skill_icon;
        m_Skill2Go.GetComponent<UISprite>().spriteName = skillNodeList[4].skill_icon;
        m_Skill3Go.GetComponent<UISprite>().spriteName = skillNodeList[5].skill_icon;
        m_Skill4Go.GetComponent<UISprite>().spriteName = skillNodeList[6].skill_icon;

        m_Skill1Go.GetComponent<UIButton>().normalSprite = skillNodeList[3].skill_icon;
        m_Skill2Go.GetComponent<UIButton>().normalSprite = skillNodeList[4].skill_icon;
        m_Skill3Go.GetComponent<UIButton>().normalSprite = skillNodeList[5].skill_icon;
        m_Skill4Go.GetComponent<UIButton>().normalSprite = skillNodeList[6].skill_icon;
    }
}
