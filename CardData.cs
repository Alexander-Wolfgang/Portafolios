using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    [Header("Información básica")]
    public string cardID;                     // Ej: 001, 002…
    public string cardName;                   // Nombre visible
    public Rarity rarity;                     // Rareza (enum)
    public CharacterType character;           // Personaje que puede usarla
    public ObjetivoCarta ObjetivoCarta;
    public TipoCarta TipoCarta;
    [TextArea(3, 5)]
    public string description;                //Descripción de la carta
    public CostType costType;
    public ULTIMATE Ultimate;                 // Si es una carta normal o Ultimate

    [Header("Audio")]
    public AudioClip audioClip;              // Clip de sonido asociado (si audio es Si)

    [Header("Visuales")]
    public Sprite artwork;                    // Imagen principal
    public Sprite border;                     // Borde según rareza

    [Header("Datos")]
    public bool desbloqueada = true;                            // Si está disponible
    public int cost = 1;                                        // Energía necesaria para jugar la carta
    public DamageType damageType = DamageType.Generico;         // Tipo de ataque (enum)
    public FamilyDamage familyDamage = FamilyDamage.Ninguno;    // Familia de daño (enum)

    [Header("Efectos de la carta")]
    public List<Effect> effects;              // Lista de efectos reales (ScriptableObjects)
}

public enum Rarity
{
    Common = 1,      // Blanco
    Uncommon = 2,    // Verde
    Rare = 3,        // Azul
    Epic = 4,        // Morado
    Legendary = 5    // Dorado
}

public enum CharacterType
{
    General,
    Capyguardian,
    Bullzerg,
    Catssassin,
    Frogzard,
    Cuymancer,
    Dogker,
    Turltank,
    Kangarage,
    Monkkey,
    Goatness,
    Sealed,
    Hawkshot,
    CrowMancer,
    Raccooneer,
    CorsShark,
    ArcanOwl,
    LionLord,
    BloodLeech
}

public enum DamageType
{
    Ninguno,
    Generico,
    Cortante,
    Contundente,
    Punzante,
    Agua,
    Fuego,
    Veneno,
    Trueno
}
public enum FamilyDamage
{
    Ninguno,
    Fisico,     //Escala con fuerza
    Magico,     //Escala con magia
    Mixto       //Escala con ambos
}
public enum CostType
{
    Energy,
    Mana,
    Health,
    Sacrifice
}
public enum ObjetivoCarta
{
    Self,           // Se aplica al jugador
    Enemy,          // Requiere elegir enemigo
    AllEnemies      // A todos los enemigos
}
public enum TipoCarta
{
    Attack,
    Skill,
    Hybrid,
    Power
}
public enum ULTIMATE
{
    Normal,
    Ultimate
}