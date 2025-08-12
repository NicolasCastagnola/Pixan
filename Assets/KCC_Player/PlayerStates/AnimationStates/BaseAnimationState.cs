using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationState : StateMachineBehaviour
{
    [SerializeField] protected CharacterView _characterView;

    public virtual void Initialize(CharacterView characterView)
    {
       _characterView = characterView;
    }
}
