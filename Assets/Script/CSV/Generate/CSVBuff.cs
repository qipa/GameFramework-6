//此代码根据 Buff.csv 自动生成，不要手动修改！！！！
//工具菜单：Export/CSV_TO_C#    生成时间 ： 6/23/2016 4:18:44 PM
using System;
public class CSVBuff
{
	public int ID;    //buffid
	public int Type;    //类型(1.增益 2.减益 3.增益光环 4.减益光环)
	public int TiggerRate;    //触发概率（百分比）
	public int TiggerImpact;    //触发效果(0=无效果 1=血量 2=物攻 3=物防 4=法攻 5=法防 6=暴击 7=免暴 8=速度 9=攻速 10=持续回/掉血(中毒、灼烧) 11=眩晕(冰冻) 12=吸血 13=血上限护盾 14=(主)伤害提高/降低 15=(被)伤害加深/免除 16=嘲讽 17=召唤自爆 18=技能后，普攻伤害 19=使用治疗效果 20=接受治疗效果 21=反伤 22=溅射 23=多重施法 24=复活 25=杀敌回血 26=驱散减益 27=主动晕别人）
	public int ValueType;    //数据类型 1.固定值（绝对伤害） 2.百分比  3.物理攻击百分比 4.法术攻击百分比 5.BUFFID
	public float Value;    //数据
	public int KeepTime;    //存在时间(毫秒)
	public int Time;    //作用次数
	public int DeathToClear;    //死亡是否清除(1.清除 2.不清除）
	public int ResultMutex;    //效果互斥（此BUFF在 互斥表的BUFF释放无效）
	public int OverlayMode;    //效果清除（此BUFF可清除效果表里的BUFF）
	public int Buffhandle;    //一种buff标志(同种buff相同 不同种不同)
	public int BuffLevel;    //buff等级
	public string Icon;    //图标
	public string EffectName;    //特效
	public string BindBone;    //特效绑定骨骼
	public string Name;    //buff名称
	public string Intro;    //效果描述
}
