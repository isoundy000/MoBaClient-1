﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据类
/// </summary>
public class GameData
{
    //游戏管理器
    public static GameManager m_GameManager;
    //当前角色id
    public static int m_CurrentRoleId;
    //当前选择英雄id
    public static int m_HeroId = 201991000;
    //当前阵营
    public static CampType m_CampId;
    //当前匹配位置
    public static int m_MatchPos;
    //当前匹配Key值
    public static string m_MatchKey;
    //当前MobaKey值
    public static string m_MobaKey;
    //匹配用户列表
    public static List<MatchPlayerData> m_MatchPlayerDataList;
    //当前显示角色
    public static Player m_CurrentPlayer;
    //每帧时间长度
    public static Fix64 m_FixFrameLen = Fix64.FromRaw(33);
    //随机数
    public static SRandom m_Srandom = new SRandom(1000);
    //游戏逻辑帧数
    public static int m_GameFrame;
    //Ping累计时长
    public static Fix64 m_PingTime = Fix64.Zero;
    //所有游戏物体列表(角色、小兵、箭塔、水晶)
    public static List<BaseObject> m_ObjectList = new List<BaseObject>();
    //所有操作事件的列表
    public static List<FrameKeyData> m_OperationEventList = new List<FrameKeyData>();
    //技能特效路径
    public static string m_EffectPath = "Effect/Prefabs";
    //是否执行显示层逻辑
    public static bool m_IsExecuteViewLogic = true;
    //游戏是否开始
    public static bool m_IsGame = false;
    //客户端帧数
    public static int m_ClientGameFrame;
    //战斗结果
    public static bool m_GameResult = false;
    //死亡次数
    public static int m_DieCount = 0;
    //是否正在查看小地图
    public static bool m_IsDragMinMap;
    //IP
    public static string m_IP = "192.168.31.254";
    //端口
    public static int m_Port = 6666;
    //UDPIP
    public static string m_UdpIP = "192.168.31.254";
    //UDP端口
    public static int m_UdpPort = 8888;
    //普攻按钮点击次数
    public static int m_AttackClickIndex = 0;
    //日志列表
    public static List<string> m_LogList = new List<string>();
    //日志写入路径
    public static string m_LogFilePath = string.Empty;
}
