using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Combo", menuName="Inventory/Card Combo", order=1)]
public class Combo : ScriptableObject
{
    [Header("连携技组合，列表为有序优先匹配")]
    [Header("填写卡片名")]
    public List<string> _cardClassNames;

    [Header("连携技额外伤害（整数）")]
    public Cata_UpperOrLower _upperOrLower;
    public int _value;

    [Header("是否造成眩晕")]
    public bool _stun;
    [Header("是否造成虚弱")]
    public bool _exhaust;

    [HideInInspector]
    public Action action;

    public void OnUse(Role who) {   // run bonus action only
        action = new Action(
            "Combo",
            false,
            _value,
            0.0f,
            _upperOrLower
        );
        
        if (_stun) {
            who._opponent._delayAction = true;
        }
        if (_exhaust) {
            int res = Random.Range(0, 100);
            if (res < 50) {
                who.AddBuff(new Buff("连携技虚弱上身", Buff.Nature.OpponentDamage, -0.30f, Cata_UpperOrLower.Upper));
                who.AddBuff(new Buff("连携技虚弱下盘", Buff.Nature.OpponentDamage, -0.30f, Cata_UpperOrLower.Lower));
            }
        }
        action.OnUse(who, false);
    }
}
