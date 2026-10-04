using UnityEngine;

[CreateAssetMenu(
    fileName = "NewCharacterStats",
    menuName = "Characters/Character Stats"
)]
public class CharacterStats : ScriptableObject
{
    [Header("Main_Stats")]
    public int HPInicial;
    public int HPLimit;
    public bool usaEnergia;
    public int energiaBase;

    [Header("Exploration")]
    public float Velocidad;             //Velocidad maxima de movimiento del personaje
    public float Fuerza_Salto;          //Que tanta fuerza de salto tiene el personaje
    public float Acelerar_Tierra;       //que tan rapido acelerar el personaje en tierra
    public float Acelerar_Aire;         //que tan rapido acelerar el personaje en  aire
    public float Freno_Tierra;          //que tan rapido desacelerar el personaje en Tierra
    public float Freno_Aire;            //que tan rapido desacelerar el personaje en Aire
    public float Gravedad_base;         //Gravedad normal del personaje
    public float MultiplicadorCaida;    //multiplicador de gravedad al caer, para hacer caidas mas rapidas
    public float Vel_Ataque;            //El tiempo que tarda el personaje en volver a atacar
    public float Min_Vel_Ataque;        //El tiempo mínimo que puede tener el personaje entre ataques, para evitar bugs de velocidad de ataque negativa
    public int Num_Saltos;              //Saltos maximos que puede hacer actualmente
    public int Limite_Saltos;           //Saltos maximos que puede alcanzar a tener este personaje
    public int Num_Bloqueo;             //Bloqueos que puede hacer el personaje actualmente
    public int Limite_Bloqueos;         //Bloqueos maximos que puede alcanzar a tener este personaje
    public bool Nadar;                  //¿Puede nadar? si o no
    public bool Bucear;                 //¿Puede Bucear? si o no
    public bool Escalar;                //¿Puede Escalar? si o no

    [Range(0f, 1f)]
    public float AirControl;            //Sirve para redireccionar tu movimiento en el aire, 0 es sin control y 1 es control total

    [Header("Combat")]
    public int roboBase;                //Cantidad de cartas que el jugador roba al inicio de su turno
    public int manoMaxima;              //Cantidad máxima de cartas que el jugador puede tener en su mano
    public int energiaConservar;        //Energia sobrante se suma para el siguiente turno
    public int Cost_Reshuffle;           //Costo de energia para hacer un reshuffle del mazo

    [Header("Resource")]
    public bool usaMana;
    public int manaMaxima;
    public int manaLimit;
    public int manaRegenPorTurno;

    [Header("Combat – Start Bonus")]
    public int energiaInicialBonus;     //Al iniciar el combate, el jugador gana x de energia adicional
    public int danoInicialBonus;        //Al iniciar el combate, el jugador hace x daño a todos los enemigos
    public int roboInicialBonus;        //Al iniciar el combate, el jugador roba x cartas adicionales
    public int escudoInicial;           //Al iniciar el combate, el jugador gana x de escudo
    public int CuracionBonus;           //Al iniciar el combate, el jugador se cura x de HP al inicio del combate
    public int FuerzaBonus;             //Al iniciar el combate, el jugador gana x de Fuerza por el resto del combate
    public int RobustezBonus;           //Al iniciar el combate, el jugador gana x de Robustez por el resto del combate
    public int MagiaBonus;              //Al iniciar el combate, el jugador gana x de Magia por el resto del combate

    [Header("Status Modifiers")]
    public int Potenciar_Duracion_Estado;   //+X turno la duracion de los estados beneficiosos del jugador
    public int Resistir_Estado;             //-X turno la duracion de los estados perjudiciales del jugador
}