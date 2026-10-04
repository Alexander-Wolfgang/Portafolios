using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionData", menuName = "Potions/PotionData")]
public class PotionData : ScriptableObject
{
    public string potionName;
    public Sprite icon;
    [TextArea] public string description;

    public List<PotionEffect> effects = new();
}
