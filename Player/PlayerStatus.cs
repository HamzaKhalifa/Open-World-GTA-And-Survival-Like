using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField][Range(0, 100)] private float _energy = 100f;
    [SerializeField] private float _energyRecoverRate = 20f;
    [SerializeField] private float _eneryRunningDepletionRate = 20f;
    [SerializeField] private float _enerySpritingDepletionRate = 30f;
    [SerializeField] private float _eneryNinjaSprintingDepletionRate = 40f;
    [SerializeField] private float _energyWallClimbingDepletionRate = 40f;
    [SerializeField] private float _energyWallRunningDepletionRate = 50f;

    [Header("Hunger")]
    [SerializeField] [Range(0, 100)] private float _hunger = 100f;
    [SerializeField] private float _hungerNormalDepletionRate = 1f;
    [SerializeField] private float _hungerRunningDepletionRate = 2f;
    [SerializeField] private float _hungerSpritingDepletionRate = 3f;
    [SerializeField] private float _hungerNinjaSprintingDepletionRate = 4f;
    [SerializeField] private float _hungerWallClimbingDepletionRate = 4f;
    [SerializeField] private float _hungerWallRunningDepletionRate = 5f;

    public float Enery => _energy;
    public float Hunger => _hunger;
    public float Health => _player.PlayerHealth.HealthValue;
    public bool IsRecoveringEnergy => _isRecoveringEnergy;

    #region Cache Fields

    private Player _player = null;

    #endregion

    private bool _isRecoveringEnergy = false;
    private bool _isRecoveringHunger = false;

    #region Monobehavior Callbacks

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        #region Energy depletion

        float energyDepletionRate = 0f;

        if (!_isRecoveringEnergy)
        {
            if (GameManager.Instance.InputManager.IsTryingToMove)
            {
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.Running)
                {
                    energyDepletionRate = _eneryRunningDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.Sprinting)
                {
                    energyDepletionRate = _enerySpritingDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.NinjaSprint)
                {
                    energyDepletionRate = _eneryNinjaSprintingDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.WallClimbing)
                {
                    energyDepletionRate = _energyWallClimbingDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.WallRunning)
                {
                    energyDepletionRate = _energyWallRunningDepletionRate;
                }
            }

            // Enegy depletion rate is also gonna depend on how hungry we are:
            energyDepletionRate += energyDepletionRate * (100 - Hunger) / 100;

            _energy -= energyDepletionRate * Time.deltaTime;
        }

        #endregion

        #region Energy Recovery

        if (_energy <= 0)
        {
            _isRecoveringEnergy = true;
        }

        if (_isRecoveringEnergy && _energy >= 100)
        {
            _isRecoveringEnergy = false;
        }

        // we only recover when we aren't losing energy
        if (energyDepletionRate == 0)
        {
            _energy += _energyRecoverRate * Time.deltaTime;
            _energy = Mathf.Min(100, _energy);
        }

        #endregion

        #region Hunger Increase

        if (!_isRecoveringHunger)
        {
            float hungerDepletionRate = _hungerNormalDepletionRate;

            if (GameManager.Instance.InputManager.IsTryingToMove)
            {
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.Running)
                {
                    hungerDepletionRate = _hungerRunningDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.Sprinting)
                {
                    hungerDepletionRate = _hungerSpritingDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.NinjaSprint)
                {
                    hungerDepletionRate = _hungerNinjaSprintingDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.WallClimbing)
                {
                    hungerDepletionRate = _hungerWallClimbingDepletionRate;
                }
                if (_player.PlayerState.PlayerMoveState == PlayerMoveState.WallRunning)
                {
                    hungerDepletionRate = _hungerWallRunningDepletionRate;
                }
            }

            _hunger -= hungerDepletionRate * Time.deltaTime;
        }

        #endregion
    }

    #endregion

    public void RegainEnergy(float amount)
    {
        _energy += amount;
        _energy = Mathf.Min(100, _energy);
    }

    public void RegainHunger(float amount)
    {
        _hunger += amount;
        _hunger = Mathf.Min(100, _hunger);
    }
}
