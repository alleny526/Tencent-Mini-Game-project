using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Behavior", menuName="Inventory/Boss Behavior", order=1)]
public class BossBehavior : ScriptableObject
{
    public string _name;
    
    public enum Nature {
        Damage,
        Defense,
        RecoverDP,
        Vulnerable,
        Tired
    }
    [Header("类型：攻击/防御/回复盾条/脆弱/疲劳/蓄力")]
    public Nature _nature;
    
    [Header("若为攻击或防御，是否随机")]
    public bool _isRandom;

    [Header("若随机，填写以下概率（整数0~100），若上下概率相加不满100，剩余为回复盾条概率")]
    public int _probWithDPUpper = 100;
    public int _probWithDPLower = 100;
    public int _probWithoutDPUpper = 100;
    public int _probWithoutDPLower = 100;
    
    [Header("若为攻击，填写以下(0~100)")]
    public int _hitUpperValue = 0;
    public int _hitLowerValue = 0;

    [Header("若为防御，填写以下(0.0~1.0)")]
    public float _defenseUpperPercentage = 0.0f;
    public float _defenseLowerPercentage = 0.0f;

    [Header("若为回复盾条，填写百分比（小数化，0.0~1.0）")]
    public float _DPRecoverPercentage = 0;


    public void OnUse(Boss_Parent boss) {
        switch(_nature) {
            case Nature.Damage:
            case Nature.Defense:
                if (_isRandom) {
                    int probUpper = (boss._defensePoint > 0 ? _probWithDPUpper : _probWithoutDPUpper);
                    int probLower = (boss._defensePoint > 0 ? _probWithDPLower : _probWithoutDPLower);
                    int rand = Random.Range(0, 100);
                    if (rand < probUpper) {
                        Action act = new Action("随机攻击选择上身", (_nature==Nature.Defense), _hitUpperValue, _defenseUpperPercentage, Cata_UpperOrLower.Upper);
                        act.OnUse(boss, false);
                        // TODO: 随机攻击上身UI
                    }
                    else if (rand < probUpper + probLower) {
                        Action act = new Action("随机攻击选择下盘", (_nature==Nature.Defense), _hitLowerValue, _defenseLowerPercentage, Cata_UpperOrLower.Lower);
                        act.OnUse(boss, false);
                        // TODO: 随机攻击下盘UI
                    }
                    else {
                        boss.RecoverDP(_DPRecoverPercentage);
                        // TODO: 回盾UI
                    }
                }
                else {  // fixed full body hit
                    Action act1 = new Action("固定全身攻击上身", (_nature==Nature.Defense), _hitUpperValue, _defenseUpperPercentage, Cata_UpperOrLower.Upper);
                    Action act2 = new Action("固定全身攻击下盘", (_nature==Nature.Defense), _hitLowerValue, _defenseLowerPercentage, Cata_UpperOrLower.Lower);
                    act1.OnUse(boss, false);
                    act2.OnUse(boss, false);
                    // TODO: 全身攻击UI
                }
                break;
            case Nature.RecoverDP:
                boss.RecoverDP(_DPRecoverPercentage);
                // TODO: 回盾UI
                break;
            case Nature.Vulnerable:
                boss.AddBuff(new Buff("BOSS脆弱上身", Buff.Nature.MyDefense, -0.30f, Cata_UpperOrLower.Upper));
                boss.AddBuff(new Buff("BOSS脆弱下盘", Buff.Nature.MyDefense, -0.30f, Cata_UpperOrLower.Lower));
                // TODO: 脆弱UI
                break;
            case Nature.Tired:
                //疲劳无计算
                // TODO: 疲劳UI
                break;
            default:
                break;
        }

        
    }
}
