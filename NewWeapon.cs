using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWeapon",
    menuName = "Characters/Weapon"
)]
public class WeaponData : ScriptableObject
{
    public Sprite sprite;

    [Header("Hitbox")]
    public Vector2 colliderSize;
    public Vector2 colliderOffset;

    [Header("Animation")]
    public float startAngle = -50f;
    public float endAngle = 110f;
    public float duration = 0.2f;

    [Header("Stats")]
    public int damage = 10;
}