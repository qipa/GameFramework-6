using UnityEngine;
using System.Collections;

public class BuffManager
{

    //buff工厂，根据buff配置信息来决定具体创建哪种buff
	public static BuffBase CreateBuff(int buffID,Entity owner)
    {
        BuffBase buff = null;
        CSVBuff buffInfo = CSVManager.GetBuffCfg(buffID);
        if(buffInfo == null)
            return null;

        //触发效果(0=无效果 1=血量 2=物攻 3=物防 4=法攻 5=法防 6=暴击 7=免暴 8=速度 9=攻速 10=持续回/掉血(中毒、灼烧) 
        //11=眩晕(冰冻) 12=吸血 13=血上限护盾 14=(主)伤害提高/降低 15=(被)伤害加深/免除 
        //16=嘲讽 17=召唤自爆 18=技能后，普攻伤害 19=使用治疗效果 20=接受治疗效果 21=反伤 22=溅射 23=多重施法 
        //24=复活 25=杀敌回血 26=驱散减益 27=主动晕别人）

        switch(buffInfo.TiggerImpact)
        {
            case 1:
                {

                }
                break;
            default:
                break;
        }

        return buff;
    }
}
