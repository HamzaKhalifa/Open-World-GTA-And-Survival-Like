using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Hands,
    Pistol,
    Rifle,
    Melee,
    Bow
}

[CreateAssetMenu(menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public WeaponType WeaponType = WeaponType.Pistol;
    public float Damage = 10f;
    public Projectile Projectile = null;
    public ParticleType DamageParticleType = ParticleType.Blood;
    public int MaxAmmo = 100;
    public int MaxMagazineAmmo = 10;
    public float TimeBetweenBullets = 1f;
    public AudioClip FireSound = null;
    public AudioClip MeleeSound = null;
    public GameObject BulletHole = null;
    public float Range = float.MaxValue;
    public float PushForce = 10f;
    public bool WithFireAnimation = false;
    public bool ChargingWeapon = false;
    public AudioClip ChargingSound = null;

    public bool IsRangeWeapon => (WeaponType == WeaponType.Pistol || WeaponType == WeaponType.Rifle || WeaponType == WeaponType.Bow);
}
