using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Role : MonoBehaviour
{
    [Header("填入初始生命值")]
    public float _hitPoint;
    [HideInInspector]
    public float _hitPointCurrent;
    [Header("填入初始盾条值(BOSS第二三阶段专有，玩家置零)")]
    public float _defensePoint = 0;
    [HideInInspector]
    public float _defensePointCurrent = 0;
    
    [HideInInspector]
    public Role _opponent;

    [Header("玩家or敌人")]
    public MainManager.Turn _camp;

    [Header("Buff列表，不用配")]
    public List<Buff> _buffList = new List<Buff>();

    [Header("手卡卡槽，不用配")]
    public List<GameObject> _cardList = new List<GameObject>();

    [Header("总cost，不用配")]
    public int _totalCost = 0;

    [HideInInspector]
    public Card.Cata_FistOrSword _latestCriticalComboOn = Card.Cata_FistOrSword.Null;
    [HideInInspector]
    public int _criticalComboProbability = 0;
    [HideInInspector]
    public bool _delayAction = false;

    [HideInInspector]
    public int _oppHitPointsBasicUpper = 0;
    [HideInInspector]
    public int _oppHitPointsBasicLower = 0;
    [HideInInspector]
    public float _oppHitPercentUpper = 1.0f;
    [HideInInspector]
    public float _oppHitPercentLower = 1.0f;
    [HideInInspector]
    public float _myDefensePercentUpper = 0.0f;
    [HideInInspector]
    public float _myDefensePercentLower = 0.0f;
    public GameObject _main;
    
    public Animator _animator;
    [HideInInspector]
    public List<string> _pendingAnimations = new List<string>();

    void OnEnable()
    {
        _cardList.Clear();
        _hitPointCurrent = _hitPoint;
    }
    // Start is called before the first frame update
    void Start()
    {
        _main = GameObject.Find("GameMainManager");
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public void PlayAnimations() {
        Debug.Log("PlayAnimations, List count =" +_pendingAnimations.Count);

        _pendingAnimations.Insert(0, "QiShi");
        _animator.CrossFade(_pendingAnimations[0], 0.05f);

    }

    //清除敌人的buff
    public void ClearOpponentBuff()
    {
        if(_opponent!= null && _opponent.gameObject.activeSelf)
        {
            foreach(Buff bf in _opponent._buffList)
            {
                if(bf._nature == Buff.Nature.OpponentDamage)
                {
                    _opponent._buffList.Remove(bf);
                }
            }
        }
    }

    //清除自身debuff
    public void ClearMyDebuff()
    {
        foreach(Buff debf in _buffList)
        {
            if(debf._nature == Buff.Nature.MyDefense)
            {
                _buffList.Remove(debf);
            }
        }
    }

    //用法：this.BeHit() by opponent
    public void BeHit(int points, Cata_UpperOrLower uol)
    {
        if (uol == Cata_UpperOrLower.Upper)
            _oppHitPointsBasicUpper += points;
        else
            _oppHitPointsBasicLower += points;
    }

    //用法：this.Defense() against opponent
    public void Defense(float p, Cata_UpperOrLower uol) {
        if (uol == Cata_UpperOrLower.Upper)
            _myDefensePercentUpper += p;
        else
            _myDefensePercentLower += p;
    }

    public void AddBuff(Buff b) {
        _buffList.Add(b);
    }

    public void OpponentCriticalComboAdd(int probability, Card.Cata_FistOrSword fos) {
        // function only called if fist or sword
        if (_latestCriticalComboOn == fos) {
            _criticalComboProbability += probability;
        }
        else {
            _criticalComboProbability = probability;
        }
    }

    public virtual void Calculate() {
        if (_opponent._delayAction) {
            // 清零
            _oppHitPointsBasicUpper = 0;
            _oppHitPointsBasicLower = 0;
            _oppHitPercentUpper = 1.0f;
            _oppHitPercentLower = 1.0f;
            _myDefensePercentUpper = 0.0f;
            _myDefensePercentLower = 0.0f;
            _buffList.Clear();
            _opponent._delayAction = false;
            return;
        }

        // BUFF/DEBUFF计算
        foreach (Buff b in _buffList) {
            if (b._nature == Buff.Nature.OpponentDamage) {
                if (b._upperOrLower == Cata_UpperOrLower.Upper) {
                    _oppHitPercentUpper += b._changeInPercentage;
                }
                else {
                    _oppHitPercentLower += b._changeInPercentage;
                }
            }
            else if (b._nature == Buff.Nature.MyDefense) {
                if (b._upperOrLower == Cata_UpperOrLower.Upper) {
                    _myDefensePercentUpper += b._changeInPercentage;
                }
                else {
                    _myDefensePercentLower += b._changeInPercentage;
                }
            }
        }
        // 边界情况处理
        if (_myDefensePercentUpper < 0) _myDefensePercentUpper = 0.0f;
        if (_myDefensePercentUpper > 1) _myDefensePercentUpper = 1.0f;
        if (_myDefensePercentLower < 0) _myDefensePercentLower = 0.0f;
        if (_myDefensePercentLower > 1) _myDefensePercentLower = 1.0f;
        if (_oppHitPercentUpper < 0) _oppHitPercentUpper = 0.0f;
        if (_oppHitPercentLower < 0) _oppHitPercentLower = 0.0f;

        // 伤害公式
        float FinalUpper = _oppHitPointsBasicUpper * _oppHitPercentUpper * (1.0f - _myDefensePercentUpper);
        float FinalLower = _oppHitPointsBasicLower * _oppHitPercentLower * (1.0f - _myDefensePercentLower);
        float FinalDamage = FinalUpper + FinalLower;

        // 暴击
        int rand = Random.Range(0, 100);
        if (rand < _criticalComboProbability)
            FinalDamage *= 1.5f;
        // TODO: UI暴击特效

        // 实现
        if (_defensePointCurrent > 0) {
            _defensePointCurrent -= FinalDamage;
            if (_defensePointCurrent < 0)
                _defensePointCurrent = 0;
        }
        else {
            _hitPointCurrent -= FinalDamage;
            _main.GetComponent<MainManager>().BurstText(this.gameObject,FinalDamage);
            if (_hitPointCurrent < 0) {
                _hitPointCurrent = 0;
                // TODO: this输，关卡结束
            }
        }

        // 清零
        _oppHitPointsBasicUpper = 0;
        _oppHitPointsBasicLower = 0;
        _oppHitPercentUpper = 1.0f;
        _oppHitPercentLower = 1.0f;
        _myDefensePercentUpper = 0.0f;
        _myDefensePercentLower = 0.0f;
        _buffList.Clear();
    }

}