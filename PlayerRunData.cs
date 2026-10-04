using UnityEngine;

public class PlayerRunData : MonoBehaviour
{
    public System.Action<int, int> OnHealthChanged;
    public System.Action<int, int> OnManaChanged;
    public static PlayerRunData Instance;

    public bool Combatiendo; //Indica si el jugador esta en combate o no, para que el HUD sepa que mostrar

    [Header("Health")]
    public int Limit_HP;
    public int Max_HP;
    public int current_HP;

    [Header("Energy")]
    public bool UsaEnergia;
    public int Energia;

    [Header("Mana")]
    public int Limit_MP;
    public int Max_MP;
    public int current_MP;

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
    public float Min_Vel_Ataque;        //El tiempo mínimo que puede alcanzar el personaje para volver a atacar
    public int Num_Saltos;              //Saltos maximos que puede hacer actualmente
    public int Limite_Saltos;           //Saltos maximos que puede alcanzar a tener este personaje
    public int Num_Bloqueo;             //Bloqueos que puede hacer el personaje actualmente
    public int Limite_Bloqueos;         //Bloqueos maximos que puede alcanzar a tener este personaje
    public bool Nadar;                  //¿Puede nadar? si o no
    public bool Bucear;                 //¿Puede Bucear? si o no
    public bool Escalar;                //¿Puede Escalar? si o no

    [Range(0f, 1f)]                     //Facilidad para moverse en el aire
    public float AirControl;            //Sirve para redireccionar tu movimiento en el aire, 0 es sin control y 1 es control total

    [Header("Posición")]
    public Vector2 posicion;

    [Header("Combat")]
    public int roboBase;
    public int manoMaxima;
    public int energiaConservadaPorTurno;
    public int Cost_Reshuffle;

    [Header("Resource")]
    public bool UsaMana;
    public int manaMaxima;
    public int manaLimit;
    public int manaRegenPorTurno;

    [Header("Combat – Start Bonus")]
    public int energiaInicialBonus;         //Al iniciar el combate, el jugador gana x de energia adicional
    public int danoInicialBonus;            //Al iniciar el combate, el jugador hace x daño a todos los enemigos
    public int roboInicialBonus;            //Al iniciar el combate, el jugador roba x cartas adicionales
    public int escudoInicial;               //Al iniciar el combate, el jugador gana x de escudo
    public int CuracionBonus;               //Al iniciar el combate, el jugador gana x de Fuerza Inicial
    public int FuerzaBonus;                 //Al iniciar el combate, el jugador gana x de Fuerza Inicial
    public int RobustezBonus;               //Al iniciar el combate, el jugador gana x de Robustez Inicial
    public int MagiaBonus;                  //Al iniciar el combate, el jugador gana x de Magia Inicial
    public int RegenManaInicial;            //Al iniciar el combate, el jugador regenera x de mana
    public int RegenManaBonus;              //Al iniciar el combate, el jugador regenera x de mana adicional por turno
    public bool BarreraInicial;             //Al iniciar el combate, el jugador gana 1 barrera protectora

    [Header("Status Modifiers")]
    public int Potenciar_Duracion_Estado;   //+X turno la duracion de los estados beneficiosos del jugador
    public int Resistir_Estado;             //-X turno la duracion de los estados perjudiciales del jugador

    [Header("Run Data")]
    public int gold;                        //Oro Normal, se usa en las tiendas de la RUN
    public int gold_Moai;                   //Oro Rapanui, se usa con el moai en el HUB
    public int experience;                  //Experiencia para tu personaje
    public int floor;                       //Piso Actual de la RUN
    private int FloorMax;                   //Piso mas alto que podras alcanzar en esta RUN, se establece al iniciar la RUN dependiendo de la dificultad elegida
    public int profundidad;                 //Profundidad actual en la RUN, se muestra en el HUB
    public int ID_GrupoEnemigo;             //Determina el grupo de enemigos que enfrentaras ahora
    public int Tamaño_Inventario;           //Determina el tamaño del inventario de objetos que tendras en esta RUN
    public Bonus BonusActual;               //Determina quien tiene bonus

    public enum Bonus
    {
        None = 0,
        Player = 1,
        Enemy = 2
    }

    [Header("Modifiers")]
    public int Vida_Extra;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========================
    // INIT RUN
    // ========================
    public bool IsInitialized = false;

    public void InitializeFromBaseStats(CharacterStats stats)
    {
        Debug.Log("INICIALIZANDO RUN DATA");

        Debug.Log("stats.energiaBase = " + stats.energiaBase);
        Debug.Log("stats.usaEnergia = " + stats.usaEnergia);

        //HP
        Max_HP = stats.HPInicial;
        current_HP = Max_HP;
        Limit_HP = stats.HPLimit;

        //Energia
        Energia = stats.energiaBase;
        UsaEnergia = stats.usaEnergia;

        //MP
        UsaMana = stats.usaMana;
        Max_MP = stats.manaMaxima;
        current_MP = Max_MP;
        Limit_MP = stats.manaLimit;

        //Stats de Exploracion
        Velocidad = stats.Velocidad;
        Fuerza_Salto = stats.Fuerza_Salto;
        Acelerar_Tierra = stats.Acelerar_Tierra;
        Acelerar_Aire = stats.Acelerar_Aire;
        Freno_Tierra = stats.Freno_Tierra;
        Freno_Aire = stats.Freno_Aire;
        Gravedad_base = stats.Gravedad_base;
        MultiplicadorCaida = stats.MultiplicadorCaida;
        Vel_Ataque = stats.Vel_Ataque;
        Min_Vel_Ataque = stats.Min_Vel_Ataque;
        Num_Saltos = stats.Num_Saltos;
        Limite_Saltos = stats.Limite_Saltos;
        Num_Bloqueo = stats.Num_Bloqueo;
        Limite_Bloqueos = stats.Limite_Bloqueos;
        Nadar = stats.Nadar;
        Bucear = stats.Bucear;
        Escalar = stats.Escalar;

        //Control en el aire
        AirControl = stats.AirControl;

        //Stats de Combate
        roboBase = stats.roboBase;
        manoMaxima = stats.manoMaxima;
        energiaConservadaPorTurno = stats.energiaConservar;
        Cost_Reshuffle = stats.Cost_Reshuffle;

        //Start Bonus
        energiaInicialBonus = stats.energiaInicialBonus;
        danoInicialBonus = stats.danoInicialBonus;
        roboInicialBonus = stats.roboInicialBonus;
        escudoInicial = stats.escudoInicial;
        CuracionBonus = stats.CuracionBonus;
        FuerzaBonus = stats.FuerzaBonus;
        RobustezBonus = stats.RobustezBonus;
        MagiaBonus = stats.MagiaBonus;

        //Modificadores de estados
        Potenciar_Duracion_Estado = stats.Potenciar_Duracion_Estado;
        Resistir_Estado = stats.Resistir_Estado;
        Vida_Extra = 0;

        //Run Data
        gold = 0;
        floor = 1;
        experience = 0;

        IsInitialized = true;
        OnHealthChanged?.Invoke(current_HP, Max_HP);
        if (UsaMana)
        {
            //Falta un script para que la barra de mana quede activa (Enabled=true)
            OnManaChanged?.Invoke(current_MP, Max_MP);
        }
    }

    // =======================================
    // GESTIONAR SALUD, PERDER Y GANAR SALUD
    // =======================================
    public void Heal(int amount)
    {
        current_HP += amount;
        current_HP = Mathf.Min(current_HP, Max_HP);

        OnHealthChanged?.Invoke(current_HP, Max_HP);        //Actualiza Barra de salud
    }
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        current_HP -= amount;

        if (current_HP <= 0)
        {
            TryRevive();
        }

        current_HP = Mathf.Clamp(current_HP, 0, Max_HP);

        OnHealthChanged?.Invoke(current_HP, Max_HP);        //Actualiza Barra de salud
    }
    public void TryRevive()
    {
        if (Vida_Extra > 0)
        {
            Vida_Extra--;

            current_HP = Mathf.CeilToInt(Max_HP * 0.5f);

            Debug.Log("Ha Revivido! Remaining extra lives: " + Vida_Extra);

            OnHealthChanged?.Invoke(current_HP, Max_HP);
        }
        else
        {
            HandleDeath();
        }
    }
    private void HandleDeath()
    {
        current_HP = 0;
        Debug.Log("El jugador esta muerto.");

        // Aquí llamas a GameOver o evento
    }
    // =====================================
    // GESTIONA MANÁ, USAR Y RECUPERAR MANÁ
    // =====================================
    public void UseMana(int amount)
    {
        current_MP -= amount;
        current_MP = Mathf.Max(current_MP, 0);

        OnManaChanged?.Invoke(current_MP, Max_MP);
    }

    public void RestoreMana(int amount)
    {
        current_MP += amount;
        current_MP = Mathf.Min(current_MP, Max_MP);

        OnManaChanged?.Invoke(current_MP, Max_MP);
    }

    //============================================
    //BUFF DE STATS
    //============================================
    public void AddMaxHP(int amount)
    {
        Max_HP += amount;

        if (Max_HP > Limit_HP)
            Max_HP = Limit_HP;      // Asegura que no se exceda el límite

        current_HP += amount;       // Incrementa la salud actual en la misma cantidad

        if (current_HP > Max_HP)
            current_HP = Max_HP;    // Asegura que la salud actual no exceda el nuevo máximo

        //OnHealthChanged?.Invoke(current_HP, Max_HP);    //Actualiza la barra de salud
    }
    public void AddMaxHPPercent(int percent)
    {
        int increase = (Max_HP * percent) / 100;

        AddMaxHP(increase);
    }
    public void AddEnergy(int amount)
    {
        Energia += amount;      // Incrementa la energía base

        if (Energia > 10)
            Energia = 10;    // Asegura que NO pases de 10 de energía, que es el máximo permitido en el juego
    }
    public void AddMaxMP(int amount)
    {
        Max_MP += amount;

        if (Max_MP > Limit_MP)
            Max_MP = Limit_MP;

        current_MP += amount;

        if (current_MP > Max_MP)
            current_MP = Max_MP;

        OnManaChanged?.Invoke(current_MP, Max_MP);
    }
    public void AddMaxMPPercent(int percent)
    {
        int increase = (Max_MP * percent) / 100;

        AddMaxMP(increase);
    }
    public void AddVelocidad(float amount)
    {
        Velocidad += amount;
    }
    public void AddFuerzaSalto(float amount)
    {
        Fuerza_Salto += amount;
    }
    public void AddAceleracionTierra(float amount)
    {
        Acelerar_Tierra += amount;
    }
    public void AddAceleracionAire(float amount)
    {
        Acelerar_Aire += amount;
    }
    public void AddVelAtaque(float amount)
    {
        Vel_Ataque -= amount;

        if (Vel_Ataque < Min_Vel_Ataque)
            Vel_Ataque = Min_Vel_Ataque;    // Asegura que no se exceda el límite mínimo
    }
    public void AddSaltos(int amount)
    {
        Num_Saltos += amount;

        if (Num_Saltos > Limite_Saltos)
            Num_Saltos = Limite_Saltos;     // Asegura que no se exceda el límite
    }
    public void AddBloqueos(int amount)
    {
        Num_Bloqueo += amount;

        if (Num_Bloqueo > Limite_Bloqueos)
            Num_Bloqueo = Limite_Bloqueos;  // Asegura que no se exceda el límite
    }
    public void AddAirControl()
    {
        AirControl += 0.2f;
        if (AirControl > 1f)
            AirControl = 1f;    // Asegura que no se exceda el límite
    }
    public void AddRoboBase(int amount)
    {
        roboBase += amount;
    }
    public void AddManoMaxima(int amount)
    {
        manoMaxima += amount;
    }
    public void AddEnergyConservada(int amount)
    {
        energiaConservadaPorTurno += amount;
    }
    public void AddEnergiaInicialBonus(int amount)
    {
        energiaInicialBonus += amount;
    }
    public void AddDanoInicialBonus(int amount)
    {
        danoInicialBonus += amount;
    }
    public void AddRoboInicialBonus(int amount)
    {
        roboInicialBonus += amount;
    }
    public void AddEscudoInicial(int amount)
    {
        escudoInicial += amount;
    }
    public void AddDuracionEstadosBeneficiosos(int amount)
    {
        Potenciar_Duracion_Estado += amount;
    }
    public void AddDuracionEstadosPerjudiciales(int amount)
    {
        Resistir_Estado += amount;
    }
    public void AddGold(int amount)
    {
        gold += amount;
    }
    public void AddGoldRapanui(int amount)
    {
        gold_Moai += amount;
    }
    public void AddPercentGold(int Percent)
    {
        int increase = (gold * Percent) / 100;      //Ganas un porcentaje del oro que tienes actualmente

        AddGold(increase);
    }
    //public void AddExperience(int amount)
    //{
    //    experience += amount;
    //}
    public void NextFloor()
    {
        floor += 1;
    }
    public void AddVidaExtra(int amount)
    {
        Vida_Extra += amount;
    }
    //============================
    //DEBUFF DE STATS
    //============================
    public void SubtractMaxHP(int amount)
    {
        Max_HP -= amount;
        if (Max_HP < 1)
            Max_HP = 1;        // Asegura que el máximo de HP no sea menor a 1
        if (current_HP > Max_HP)
            current_HP = Max_HP;    // Ajusta la salud actual si excede el nuevo máximo

        OnHealthChanged?.Invoke(current_HP, Max_HP);   //Actualiza la barra de salud
    }
    public void SubtractMaxHPPercent(int percent)
    {
        int decrease = (Max_HP * percent) / 100;
        SubtractMaxHP(decrease);
    }
    public void SubtractEnergy(int amount)
    {
        Energia -= amount;
        if (Energia < 0)
            Energia = 0;       // Asegura que la energía no sea negativa
    }
    public void SubtractMaxMP(int amount)
    {
        Max_MP -= amount;

        if (Max_MP < 1)
            Max_MP = 1;

        if (current_MP > Max_MP)
            current_MP = Max_MP;

        OnManaChanged?.Invoke(current_MP, Max_MP);
    }
    public void SubtractMaxMPPercent(int percent)
    {
        int decrease = (Max_MP * percent) / 100;
        SubtractMaxMP(decrease);
    }
    public void SubtractVelocidad(float amount)
    {
        Velocidad -= amount;
        if (Velocidad < 0.1f)
            Velocidad = 0.1f;   // Asegura que la velocidad no sea demasiado baja
    }
    public void SubtractFuerzaSalto(float amount)
    {
        Fuerza_Salto -= amount;
        if (Fuerza_Salto < 0.1f)
            Fuerza_Salto = 0.1f;   // Asegura que la fuerza de salto no sea demasiado baja
    }
    public void SubtractAceleracionTierra(float amount)
    {
        Acelerar_Tierra -= amount;
        if (Acelerar_Tierra < 0.1f)
            Acelerar_Tierra = 0.1f;   // Asegura que la aceleración en tierra no sea demasiado baja
    }
    public void SubtractAceleracionAire(float amount)
    {
        Acelerar_Aire -= amount;
        if (Acelerar_Aire < 0.1f)
            Acelerar_Aire = 0.1f;   // Asegura que la aceleración en aire no sea demasiado baja
    }
    public void SubtractVelAtaque(float amount)
    {
        Vel_Ataque += amount;   // Aumenta el tiempo de ataque, lo que es una penalización
    }
    public void SubtractSaltos(int amount)
    {
        Num_Saltos -= amount;
        if (Num_Saltos < 0)
            Num_Saltos = 0;     // Asegura que el número de saltos no sea negativo
    }
    public void SubtractBloqueos(int amount)
    {
        Num_Bloqueo -= amount;
        if (Num_Bloqueo < 0)
            Num_Bloqueo = 0;   // Asegura que el número de bloqueos no sea negativo
    }
    public void SubtractAirControl()
    {
        AirControl -= 0.2f;
        if (AirControl < 0f)
            AirControl = 0f;   // Asegura que el control en el aire no sea negativo
    }
    public void SubtractRoboBase(int amount)
    {
        roboBase -= amount;
        if (roboBase < 0)
            roboBase = 0;      // Asegura que el robo base no sea negativo
    }
    public void SubtractManoMaxima(int amount)
    {
        manoMaxima -= amount;
        if (manoMaxima < 1)
            manoMaxima = 1;    // Asegura que la mano máxima no sea menor a 1
    }
    public void SubtractEnergyConservada(int amount)
    {
        energiaConservadaPorTurno -= amount;
        if (energiaConservadaPorTurno < 0)
            energiaConservadaPorTurno = 0;    // Asegura que la energía conservada por turno no sea negativa
    }
    public void SubtractEnergiaInicialBonus(int amount)
    {
        energiaInicialBonus -= amount;
        if (energiaInicialBonus < 0)
            energiaInicialBonus = 0;   // Asegura que el bono de energía inicial no sea negativo
    }
    public void SubtractDanoInicialBonus(int amount)
    {
        danoInicialBonus -= amount;
        if (danoInicialBonus < 0)
            danoInicialBonus = 0;     // Asegura que el bono de daño inicial no sea negativo
    }
    public void SubtractRoboInicialBonus(int amount)
    {
        roboInicialBonus -= amount;
        if (roboInicialBonus < 0)
            roboInicialBonus = 0;     // Asegura que el bono de robo inicial no sea negativo
    }
    public void SubtractEscudoInicial(int amount)
    {
        escudoInicial -= amount;
        if (escudoInicial < 0)
            escudoInicial = 0;       // Asegura que el bono de escudo inicial no sea negativo
    }
    public void SubtractDuracionEstadosBeneficiosos(int amount)
    {
        Potenciar_Duracion_Estado -= amount;
        if (Potenciar_Duracion_Estado < 0)
            Potenciar_Duracion_Estado = 0;   // Asegura que el potenciador de duración de estados beneficiosos no sea negativo
    }
    public void SubtractDuracionEstadosPerjudiciales(int amount)
    {
        Resistir_Estado -= amount;
        if (Resistir_Estado < 0)
            Resistir_Estado = 0;   // Asegura que el resistir estado no sea negativo
    }
    public void SubtractGold(int amount)
    {
        gold -= amount;
        if (gold < 0)
            gold = 0;      // Asegura que el oro no sea negativo
    }
    public void SubtractExperience(int amount)
    {
        experience -= amount;
        if (experience < 0)
            experience = 0;       // Asegura que la experiencia no sea negativa
    }
    public void SubtractFloor(int amount)
    {
        floor -= amount;
        if (floor < 1)
            floor = 1;           // Asegura que el piso no sea menor a 1
    }
    public void SubtractVidaExtra(int amount)
    {
        Vida_Extra -= amount;
        if (Vida_Extra < 0)
            Vida_Extra = 0;      // Asegura que las vidas extra no sean negativas
    }
    public void Estas_Combatiendo(bool combate)
    {
        if (combate)
        {
            Combatiendo = true;
        }
        else
        {
            Combatiendo = false;
        }
    }
    public void SaveCombatResults(CombatManager CM)
    {
        current_HP = CM.currentHealth;
        current_MP = CM.manaCurrent;
        //Max_HP = CM.maxHealth;
        //Max_MP = CM.manaMaxima;

        OnHealthChanged?.Invoke(current_HP, Max_HP);
        OnManaChanged?.Invoke(current_MP, Max_MP);

        AddGold(CM.Oro_Enemigos_Acum);
        AddGoldRapanui(CM.Oro_Moai);
        //AddExperience(CM.Exp_Actual);
        Debug.Log("Exp_Actual: " + CM.Exp_Actual);
        Debug.Log("experience: " + experience);
        experience = CM.Exp_Actual;
        Debug.Log("experience: " + experience);
    }
}