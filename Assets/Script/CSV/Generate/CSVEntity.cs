//此代码根据 Entity.csv 自动生成，不要手动修改！！！！
//工具菜单：Export/CSV_TO_C#    生成时间 ： 6/14/2016 2:19:54 PM
using System;
public class CSVEntity
{
	public uint ID;    //角色ID
	public string ResPath;    //角色模型资源路径
	public uint EntityType;    //分类（0=主角 1=将领 2=NPC友军 3=NPC敌军 4=BOSS  5=MobaMonster  6=MobaPlayer 7=Moba基地  8=Moba箭塔  9=MobaBoss(基地) 10=木桩 11=假人）
	public uint Sex;    //性别（0=女 1=男）
	public int Level;    //默认等级
	public string Name;    //名字
	public string AIName;    //AI名字
	public int StarLevel;    //星级（品质）
	public int Type;    //将领属性类型(1=物理输出  2=法术输出 3=物理防御 4=法术防御)
	public int ForceGrowth;    //武力成长
	public int IntelligenceGrowth;    //智力成长
	public int AgileGrowth;    //敏捷成长
	public int EnduranceGrowth;    //耐力成长
	public int PhysicalAttack;    //物理攻击
	public int MagicAttack;    //法术攻击
	public int PhysicalDefense;    //物理防御
	public int MagicDefense;    //法术防御
	public int HealthPoint;    //血量
	public int CriticalRate;    //暴击率（万分）
	public int FreeCriticalRate;    //免暴率（万分）
	public int AttackSpeed;    //攻速（万分）
	public int MoveSpeed;    //移速
	public int RotSpeed;    //转速
	public string Skill;    //初始技能
	public int WarninngRange;    //警戒范围（出生点坐标半径）
	public int PursuitRange;    //追击范围（出生点坐标半径）
	public float NameHeight;    //名字高度
	public string shows;    //SID
	public float Size;    //大小（人物碰撞体型）
	public float Scale;    //缩放（人物体型放大或者缩小）
	public uint body;    //身体
	public string handl;    //左手
	public string handr;    //右手
	public int reviveTime;    //复活时间（秒）
	public uint showname;    //显示名字
	public uint DeadTime;    //死亡时间(毫秒)
}
