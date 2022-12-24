using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Role
{
    // Start is called before the first frame update
    void Start()
    {
        _opponent = GameObject.FindGameObjectWithTag("Boss").GetComponent<Role>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("update Role");
        if (_pendingAnimations.Count > 0) {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_pendingAnimations[0])) {
                Debug.Log("PLAYER count =" + _pendingAnimations.Count + " @ " + _animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
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
}
