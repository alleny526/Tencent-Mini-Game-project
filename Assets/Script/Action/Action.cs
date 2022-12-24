using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Cata_UpperOrLower {
    Upper,
    Lower,
    Null
}

// [CreateAssetMenu(fileName="New Action", menuName="Inventory/Action", order=1)]
public class Action : ScriptableObject {
    [Header("行动名称")]
    public string _name;

    [Header("是否为防御（否则默认为攻击）")]
    public bool _isDefense = false;

    [Header("伤害点数（防御牌不用填）")]
    public int _hitPoints = 0;

    [Header("防御百分比（小数化，0.0~1.0）")]
    public float _defensePercentage = 0.0f;

    [Header("攻击敌方/防御我方部位")]
    public Cata_UpperOrLower _upperOrLower = Cata_UpperOrLower.Null;

    public Action() {}
    
    public Action(string name, bool isdefense, int hitpoints, float defensepercentage, Cata_UpperOrLower uol) {
        _name = name;
        _isDefense = isdefense;
        _hitPoints = hitpoints;
        _defensePercentage = defensepercentage;
        _upperOrLower = uol;
    }

    public virtual void OnUse(Role who, bool hitIncrement) {    //hitIncrement: 1.1倍加成
        if (_isDefense) {
            who.Defense(_defensePercentage, _upperOrLower);
        } else {    // damage
            if (hitIncrement)
                who._opponent.BeHit((int)(_hitPoints * 1.1f), _upperOrLower);
            else
                who._opponent.BeHit(_hitPoints, _upperOrLower);
        }
    }
}
