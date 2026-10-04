using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Base_Stat_Enemy",
    menuName = "Enemy/Base_Stat_Enemy"
)]
public class Base_Stat_Enemy : ScriptableObject
{
    [Header("Identificar")]
    public int ID;
    public string Nombre;

    public enum Tipo_Enemigo { Normal, Elite, Jefe }
    public Tipo_Enemigo tipo;

    [Header("Main Stats")]
    public int Max_HP;
    public int Fuerza;
    public int Magia;
    public int Robustez;
    public int dinero;
    public int XP;

    [Header("Exploration")]
    public float Velocidad;
    public float Fuerza_Salto;
    public float Acelerar_Tierra;
    public float Acelerar_Aire;
    public float Freno_Tierra;
    public float Freno_Aire;
    public float Gravedad_base;
    public float MultiplicadorCaida;
    public float Vision;
    public float DistanciaDeteccionBorde;
    public int Num_Saltos;
    public int Limite_Saltos;
    public bool TeVio;
    public Comportamiento QueHace;

    [Header("Decision")]
    public int Atacar;
    public int Curar;
    public int Buffear;
    public int Debuffear;
    public int Invocar;
    public int Huir;

    [Header("Acciones en combate")]
    public List<EnemyActionData> Ataques;
    public List<EnemyActionData> Curacion;
    public List<EnemyActionData> Buffs;
    public List<EnemyActionData> Debuffs;
    public List<EnemyActionData> Invocaciones;

    [Header("Escape")]
    public EnemyActionData Run;

    [Header("Invocacion")]
    public int Num_Invocaciones;

    [Header("Status Modifiers")]
    public int Potenciar_Duracion_Estado;
    public int Resistir_Estado;

    public enum Comportamiento
    {
        Nada,
        Esconderse,
        Seguir,
        Atacar,
        Teletransportar,
        Hechizo,
        Boss
    }
}