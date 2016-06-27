using System;
using UnityEngine;
using System.Collections.Generic;

public class CSVManager
{
    static Dictionary<uint, CSVEntity> m_EntityData = new Dictionary<uint, CSVEntity>();
    static Dictionary<uint, CSVSkill> m_SkillData = new Dictionary<uint, CSVSkill>();
    static Dictionary<int, CSVBuff> m_BuffData = new Dictionary<int, CSVBuff>();
    static Dictionary<string, CSVMap> m_MapData = new Dictionary<string, CSVMap>();
    public static void LoadAllCsv()
    {
        ClearAllCsv();
        CSVReader.ReadCsvDic<uint, CSVEntity>("Config/Entity", m_EntityData, "ID");
        CSVReader.ReadCsvDic<uint, CSVSkill>("Config/Skill", m_SkillData, "ID");
        CSVReader.ReadCsvDic<int, CSVBuff>("Config/Buff", m_BuffData, "ID");
        CSVReader.ReadCsvDic<string, CSVMap>("Config/Map", m_MapData, "name");
        Log.Info("配置表加载完成");
    }

    public static void ClearAllCsv()
    {
        m_EntityData.Clear();
        m_SkillData.Clear();
        m_BuffData.Clear();
        m_MapData.Clear();
    }

    public static CSVEntity GetEntityCfg(uint ID)
    {
        CSVEntity data = null;
        m_EntityData.TryGetValue(ID, out data);
        return data;
    }

    public static CSVSkill GetSkillCfg(uint ID)
    {
        CSVSkill data = null;
        m_SkillData.TryGetValue(ID, out data);
        return data;
    }

    public static CSVBuff GetBuffCfg(int ID)
    {
        CSVBuff data = null;
        m_BuffData.TryGetValue(ID, out data);
        return data;
    }

    public static CSVMap GetMapCfg(string mapName)
    {
        CSVMap data = null;
        m_MapData.TryGetValue(mapName, out data);
        return data;
    }
}