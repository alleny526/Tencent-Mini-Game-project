using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Parent : Role
{
    [System.Serializable]
    public class Phase {
        public int start;
        public int end;
    }

    [Header("每阶段起始行动index [start, end)")]
    public List<Phase> _phaseRange;

    [Header("BOSS行动列表")]
    public List<BossBehavior> behaviors;

    [HideInInspector]
    public int _currentBehavior;   // [0, num of behaviors)
    public int _currentPhase;   // [0, num of phase)

    // Start is called before the first frame update
    void Start()
    {
        _opponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Role>();
        // 第一阶段无盾条
        _defensePointCurrent = 0;
        _currentPhase = 0;
        _currentBehavior = -1;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("update Role");
        if (_pendingAnimations.Count > 0) {
            // Debug.Log("BOSS count =" + _pendingAnimations.Count + " @ " + _animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_pendingAnimations[0])) {
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f) {
                    _pendingAnimations.RemoveAt(0);
                    if (_pendingAnimations.Count > 0) {
                        Debug.Log("new animation");
                        _animator.CrossFade(_pendingAnimations[0], 0.05f);
                    }
                }
            }
        }
        else {
        }
    }

    public void NextBehavior() {
        if (_delayAction) {
            behaviors[_currentBehavior].OnUse(this);
            _delayAction = false;
            return;
        }
        
        // End Chapter
        if (_currentBehavior + 1 == behaviors.Count) {
            // TODO: 强制关卡结束
            return;
        }

        // New phase
        if (_currentBehavior + 1 == _phaseRange[_currentPhase].end) {
            _currentPhase += 1;
            _defensePointCurrent = _defensePoint;
            // TODO: UI changes on new phase
        }

        // next behavior
        _currentBehavior += 1;
        behaviors[_currentBehavior].OnUse(this);
    }

    public void RecoverDP(float percentage) {
        _defensePointCurrent += _defensePointCurrent * percentage;
        if (_defensePointCurrent > _defensePoint)
            _defensePointCurrent = _defensePoint;
    }
}
