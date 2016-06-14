//此代码根据 Skill.csv 自动生成，不要手动修改！！！！
//工具菜单：Export/CSV_TO_C#    生成时间 ： 6/14/2016 2:19:54 PM
using System;
public class CSVSkill
{
	public uint ID;    //技能ID
	public uint type;    //技能类型(1.普攻 2.物理 3.法术 4.保护 5.治疗 6.辅助 7.召唤 8.被动)
	public string name;    //技能名称
	public uint cameraEffect;    //屏幕效果(1.震屏 2.变暗 3.缩放)
	public string skillIcon;    //技能图标
	public string skillIntro;    //技能描述
	public uint damageCoefficient;    //技能系数(伤害 / 召唤物ID)
	public uint level;    //技能等级 
	public uint targetTpye;    //选择目标类型 1.指定目标 2.自己 3.所有友方 4.血量最少友方
	public uint attackType;    //施法方式 1.绑定技能(近战)2.范围技能 3.子弹(无目标) 4.子弹(有目标)
	public uint miagcType;    //施法范围类型 1.点 2.线形(以人为起点 打向鼠标位置)3.扇形(以自身为起点 默认120°)4.圆形(鼠标为原点或自身为原点)
	public float attackDistance;    //施法距离(单位厘米)
	public float guideTime;    //引导时间
	public float hitTime;    //击中时间(闪避技能战士、弩手不能有击中时间否则模型会消失)
	public float coolTime;    //冷却时间
	public uint attackBuff;    //附加效果(BUFF)
	public uint studyCost;    //学习所需金钱
	public string studyMat;    //学习所需材料 （多个材料格式150112*1|150112*3）
	public uint InitSkill;    //初始技能
	public uint preSkill;    //前置技能
	public uint nextSkill;    //下级技能
	public float flySpeed;    //轨迹飞行速度
	public string guideEffect;    //指引特效（100/101）
	public float guideEffectBeginTime;    //指引特效开始时间（从技能触发时间开始算）
	public float guideEffectDuration;    //指引特效持续时间
	public string castEffect;    //释放特效
	public float castEffectBeginTime;    //释放特效开始时间
	public float castEffectDuration;    //释放特效持续时间
	public string castEffectBindBone;    //释放特效绑定骨骼
	public string flyEffect;    //子弹特效
	public uint beattackEffect;    //受击特效(buff)
	public string guideAction;    //指引动作
	public float guideActionBeginTime;    //指引动作开始时间
	public float guideActionDuration;    //指引动作持续时间
	public string castAction;    //释放动作
	public float castActionBeginTime;    //释放动作开始时间
	public float castActionDuration;    //释放动作持续时间
	public string beattackAction;    //受击动作
	public float beattackActionBeginTime;    //受击动作开始时间
	public float beattackActionDuration;    //受击动作持续时间
	public float comboBeginTime;    //连击命令接收开始时间
	public float comboEndTime;    //连击命令接收持续时间
	public uint sound;    //声音
	public int isCD;    //是否CD 初始状态处在CD状态:1.不进入CD状态 2.进入CD状态
}
