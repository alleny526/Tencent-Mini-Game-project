using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string _cardName = "卡牌名";
    public string _cardClassName = "Card_Name";

    [Header("费用")]
    public int _cardCost = 0;
    [HideInInspector]
    public int _cardCostToPointRatio = 100;
    
    public enum Cata_FistOrSword {  // used by Card only
        Fist,
        Sword,
        Null
    }
    [Header("拳或剑")]
    public Cata_FistOrSword FistOrSword;
    [Header("上身或下盘")]
    public Cata_UpperOrLower UpperOrLower;  // globally defined in Action.cs

    [Header("是否防御牌，否则默认攻击牌")]
    public bool _isDefense = false;
    
    [HideInInspector]
    public bool _drawn = false;
    [HideInInspector]
    public Action action;

    public Card() {}

    public Card(Card c) {
        _cardName = c._cardName;
        _cardClassName = c._cardClassName;
        _cardCost = c._cardCost;
        _cardCostToPointRatio = c._cardCostToPointRatio;
        FistOrSword = c.FistOrSword;
        UpperOrLower = c.UpperOrLower;
        _isDefense = c._isDefense;
        _drawn = c._drawn;
        action = c.action;
    }

    // Start is called before the first frame update
    void Start()
    {
        action = new Action("卡牌"+_cardName,
                            _isDefense,
                            _cardCost * _cardCostToPointRatio,
                            (_isDefense ? Random.Range(0.25f, 0.50f) : 0.0f),
                            UpperOrLower
                            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Draw()
    {
        if(!_drawn)
        {
            this.gameObject.GetComponent<RectTransform>().SetPositionAndRotation(
                new Vector3(this.gameObject.GetComponent<RectTransform>().position.x,
                this.gameObject.GetComponent<RectTransform>().position.y+0.3f,
                this.gameObject.GetComponent<RectTransform>().position.z),
                Quaternion.Euler(0,0,0));
            _drawn = true;
        }
        else
        {
            this.gameObject.GetComponent<RectTransform>().SetPositionAndRotation(
                new Vector3(this.gameObject.GetComponent<RectTransform>().position.x,
                this.gameObject.GetComponent<RectTransform>().position.y-0.3f,
                this.gameObject.GetComponent<RectTransform>().position.z),
                Quaternion.Euler(0,0,0));
            _drawn = false;

        }

    }

    public void OnUse(Role who, bool isCombo) {
        action = new Action("卡牌"+_cardName,
                            _isDefense,
                            _cardCost * _cardCostToPointRatio,
                            (_isDefense ? Random.Range(0.25f, 0.50f) : 0.0f),
                            UpperOrLower
                            );
        //拳剑criticalCombo
        if (!isCombo && FistOrSword != Cata_FistOrSword.Null) {
            who.OpponentCriticalComboAdd(1, FistOrSword);
        }
        //拳剑功能性区分
        if (who._camp == MainManager.Turn.Player) {
            if (who._opponent._defensePoint > 0) {
                action.OnUse(who, FistOrSword==Cata_FistOrSword.Sword); //攻击DP，是剑则加10%
            } else {
                action.OnUse(who, FistOrSword==Cata_FistOrSword.Fist); //攻击HP，是拳则加10%
            }
        } else {
            action.OnUse(who, false);
        }
        
    }

    public void OnNextRound(Role who) {
        //上身下盘功能性区分
        if (UpperOrLower == Cata_UpperOrLower.Upper) {
            who._oppHitPercentUpper = 0.95f;
            who._oppHitPercentLower = 0.95f;
        }
        else if (UpperOrLower == Cata_UpperOrLower.Lower) {
            who._opponent._myDefensePercentUpper = -0.05f;
            who._opponent._myDefensePercentLower = -0.05f;
        }
    }
}
