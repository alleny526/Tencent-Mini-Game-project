using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    回复盾量不在此类中
*/
public class Buff : ScriptableObject
{
    [Header("Buff名称")]
    public string _name;

    public enum Nature
    {
        OpponentDamage,
        MyDefense,
        Stun,   //眩晕
        Tired,    //疲劳
    }
    public Nature _nature;

    public Cata_UpperOrLower _upperOrLower;

    [Header("攻击或防御百分比变化（小数化），加成为正，减少为负")]
    public float _changeInPercentage = 0.0f;

    public Buff() {}

    public Buff(string name, Nature nature, float changeinpercentage, Cata_UpperOrLower uol) {
        _name = name;
        _nature = nature;
        _changeInPercentage = changeinpercentage;
        _upperOrLower = uol;
    }
}
