using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIVehicle : CharacterVehicle
{
    private AIStateMachine _stateMachine = null;

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = GetComponent<AIStateMachine>();
    }

    public override void RegainControl()
    {
        _stateMachine.Agent.enabled = true;
    }

    protected override void LoseControl()
    {
        _stateMachine.Agent.enabled = false;
    }
}
