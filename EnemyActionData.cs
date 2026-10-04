using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Action")]
public class EnemyActionData : ScriptableObject
{
    [Header("Info")]
    public string actionName;
    public int ID;
    [TextArea(3, 5)]
    public string description;                //Descripción de la accion

    [Header("Effects")]
    public List<Effect> effects;

    [Header("UI")]
    public Sprite icon;

    [Header("Sound")]
    public AudioClip soundEffect;
}