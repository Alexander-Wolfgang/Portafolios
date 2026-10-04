using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    //Barra salud
    public event System.Action<int, int> OnHealthChanged;

    //public abstract IEnumerator PerformAction();
    public CombatManager PCM;
    public Base_Stat_Enemy stats;
    protected EnemyActionData accionElegida;
    public EnemyStatusUIManager statusUI;

    [Header("Icono")]
    public Image Icono;
    public TMP_Text Texto_Icono;

    [Header("Stats")]
    public GameObject HPBar;
    public List<ActiveStatus> activeStatuses;
    public bool IsDead;

    [HideInInspector]
    public int currentHP;
    [HideInInspector]
    public int MaxHP;
    [HideInInspector]
    public int Dinero;
    [HideInInspector]
    public int XP;

    [HideInInspector]
    public int Fuerza;
    [HideInInspector]
    public int Magia;
    [HideInInspector]
    public int Robustez;
    [HideInInspector]
    public int AtaqueFinal;

    protected int armorShield;
    protected int armorTurnsRemaining;
    [HideInInspector]
    public int ProbAtacar_Base;
    [HideInInspector]
    public int ProbCurar_Base;
    [HideInInspector]
    public int ProbBuffear_Base;
    [HideInInspector]
    public int ProbDebuffear_Base;
    [HideInInspector]
    public int ProbInvocar_Base;
    [HideInInspector]
    public int ProbEscapar_Base;
    [HideInInspector]
    public int ProbAtacar;
    [HideInInspector]
    public int ProbCurar;
    [HideInInspector]
    public int ProbBuffear;
    [HideInInspector]
    public int ProbDebuffear;
    [HideInInspector]
    public int ProbInvocar;
    [HideInInspector]
    public int ProbEscapar;
    [HideInInspector]
    public int ProbTotal;
    [HideInInspector]
    public int ID_Ataque;
    [HideInInspector]
    public int cont_Ataques;
    [HideInInspector]
    public int ID_Curacion;
    [HideInInspector]
    public int cont_Curacion;
    [HideInInspector]
    public int ID_Buff;
    [HideInInspector]
    public int ID_Debuff;
    [HideInInspector]
    public int ID_Invocar;
    [HideInInspector]
    public int Num_Invocaciones;

    [Range(1, 6)]
    public int eleccion; //1 Atacar, 2 Curar, 3 Buffear, 4 Debuffear, 5 Invocar, 6 Huir
    [Range(1, 6)]
    public int eleccionActual;

    [HideInInspector]
    public Enemy enemy;

    [Header("Feedback")]
    public Transform visualRoot; // el sprite del enemigo (o el padre visual)
    public SpriteRenderer sprite;
    private Vector3 baseScale;
    private Color originalSpriteColor;
    private Coroutine flashRoutine;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    public CombatManager.CombatState currentState;
    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => MaxHP;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        Transform stat = transform.Find("Stats");

        if (stat != null)
        {
            sprite = stat.GetComponent<SpriteRenderer>();
            visualRoot = stat;
        }
    }
    protected virtual void Start()
    {
        if (sprite != null)
            originalSpriteColor = sprite.color;

        currentHP = stats.Max_HP;
        MaxHP = stats.Max_HP;
        OnHealthChanged?.Invoke(currentHP, MaxHP);
        Fuerza = stats.Fuerza;
        Magia = stats.Magia;
        Robustez = stats.Robustez;
        Dinero = stats.dinero;
        XP = stats.XP;

        armorShield = 0;
        armorTurnsRemaining = 0;

        cont_Ataques = 0;
        cont_Curacion = 0;
        Num_Invocaciones = stats.Num_Invocaciones;

        if (visualRoot != null)
            baseScale = visualRoot.localScale;
        Crear_Prob();
        statusUI = GetComponentInChildren<EnemyStatusUIManager>();

        if (statusUI == null)
        {
            Debug.LogError($"NO se encontró EnemyStatusUIManager en {gameObject.name}");
        }
        else
        {
            Debug.Log($"EnemyStatusUIManager encontrado en {gameObject.name}");
        }
    }

    public void Initialize(CombatManager combatManager)
    {
        PCM = combatManager;
    }

    //public List<EnemyActiveStatus> activeBuffs = new List<EnemyActiveStatus>();
    public void Crear_Prob()
    {
        ProbAtacar_Base = stats.Atacar;
        ProbCurar_Base = stats.Curar;
        ProbBuffear_Base = stats.Buffear;
        ProbDebuffear_Base = stats.Debuffear;
        ProbInvocar_Base = stats.Invocar;
        ProbEscapar_Base = stats.Huir;

        ProbAtacar = ProbAtacar_Base;
        ProbCurar = ProbAtacar + ProbCurar_Base;
        ProbBuffear = ProbCurar + ProbBuffear_Base;
        ProbDebuffear = ProbBuffear + ProbDebuffear_Base;
        ProbInvocar = ProbDebuffear + ProbInvocar_Base;
        ProbEscapar = ProbInvocar + ProbEscapar_Base;
    }
    public void Agregar_Iconos()
    {
        eleccionActual = ElegirIntencion();
        accionElegida = Identificar_Accion();

        ActualizarIcono();
    }
    public int ElegirIntencion()
    {
        int intencion = Random.Range(1, 101);

        if (intencion <= ProbAtacar)
            return 1;

        if (intencion <= ProbCurar)
        {
            if (PCM.turnCounter == 1)
            {
                int MayorProb = ProbAtacar;
                if (ProbBuffear > MayorProb)
                {
                    MayorProb = ProbBuffear;
                    return 3;
                }
                if (ProbDebuffear > MayorProb)
                {
                    MayorProb = ProbDebuffear;
                    return 4;
                }
                if (ProbInvocar > MayorProb)
                {
                    MayorProb = ProbInvocar;
                    return 5;
                }
                if (ProbEscapar > MayorProb)
                {
                    MayorProb = ProbEscapar;
                    return 6;
                }
                else
                {
                    return 1;
                }
            }
            return 2;
        }

        if (intencion <= ProbBuffear)
            return 3;

        if (intencion <= ProbDebuffear)
            return 4;

        if (intencion <= ProbInvocar)
            return 5;

        if (intencion <= ProbEscapar)
            return 6;

        return 0;
    }
    public void ActualizarIcono()
    {
        if (accionElegida == null)
        {
            Debug.LogError("accionElegida es NULL");
            return;
        }

        if (Icono != null)
        {
            Icono.sprite = accionElegida.icon;
            Icono.enabled = accionElegida.icon != null;
        }

        string textoFinal = "";

        foreach (Effect effect in accionElegida.effects)
        {
            textoFinal += GenerarTextoIntento(effect) + "\n";
        }

        Texto_Icono.text = textoFinal;
    }
    string GenerarTextoIntento(Effect effect)
    {
        if (effect == null) return "";

        // Primero delegar al propio Effect (si está implementado)
        try
        {
            string intentText = effect.GetIntentText(this);
            if (!string.IsNullOrEmpty(intentText))
                return intentText;
        }
        catch
        {
            // Si algún Effect no implementa bien la función, continuamos con el fallback.
        }

        // 🟥 ATAQUE
        if (effect is EnemyDealDamageEffect dmgEffect)
        {
            int dañoFinal = CalcularDañoIntento(dmgEffect);
            return dañoFinal.ToString();
        }

        // 🟩 CURACIÓN
        if (effect is EnemyHealEffect healEffect)
        {
            if (healEffect.duration > 0)
                return $"{healEffect.value} ({healEffect.duration})";
            return healEffect.value.ToString();
        }

        // 🟦 BUFF (stats)
        if (effect is EnemyStatBuffEffect buffEffect)
        {
            if (buffEffect.duration > 0)
                return $"{buffEffect.value} ({buffEffect.duration})";
            return buffEffect.value.ToString();
        }

        // 🟪 DEBUFF (estadísticas / estados)
        if (effect is StatDebuffEffect statDebuff) // cubre StatDebuffEffect
        {
            if (statDebuff.duration > 0)
                return $"{statDebuff.value} ({statDebuff.duration})";
            return statDebuff.value.ToString();
        }
        if (effect is ApplyStatDebuffEffect applyDebuff) // si tienes otra clase distinta
        {
            if (applyDebuff.duration > 0)
                return $"{applyDebuff.value} ({applyDebuff.duration})";
            return applyDebuff.value.ToString();
        }

        // 🛡 ESCUDO
        if (effect is EnemyShieldEffect shieldEffect)
        {
            if (shieldEffect.duration > 0)
                return $"{shieldEffect.value} ({shieldEffect.duration})";
            return shieldEffect.value.ToString();
        }

        return "";
    }
    int CalcularDañoIntento(EnemyDealDamageEffect effect)
    {
        int baseDamage = effect.value;      //Extrae potencia del ataque        
        int total = baseDamage + Fuerza;    // Sumar fuerza del enemigo

        return Mathf.Max(0, total);
    }
    public virtual void TakeDamage(int damage)
    {
        bool hitShield = false;
        bool hitHP = false;

        if (armorShield > 0 && damage > 0)
        {
            int absorbed = Mathf.Min(damage, armorShield);
            armorShield -= absorbed;
            damage -= absorbed;

            hitShield = true;

            Debug.Log($"{gameObject.name} armadura absorbió {absorbed} daño. Armadura restante: {armorShield}");
        }

        if (damage > 0)
        {
            currentHP -= damage;
            OnHealthChanged?.Invoke(currentHP, MaxHP);

            hitHP = true;

            Debug.Log($"{gameObject.name} recibe {damage} daño. HP actual: {currentHP}/{MaxHP}");
        }

        // 🔥 Elegir SOLO un feedback
        if (hitHP)
            PlayFlash(Color.red);      // prioridad
        else if (hitShield)
            PlayFlash(Color.gray);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        currentHP = 0;
        IsDead = true;
        PCM.Sumar_Oro_y_XP(Dinero,XP);
        Debug.Log($"{gameObject.name} ha muerto.");

        // 🔥 Revisar victoria inmediatamente
        if (PCM != null)
        {
            PCM.CheckCombatEnd();
        }
        statusUI?.DestroyAllIcons();
        Destroy(gameObject);
    }

    public virtual void AddArmor(int duration)  //Añade armadura con valor por defecto (25% de MaxHP), pasa duration
    {
        int defaultAmount = Mathf.Max(1, MaxHP / 4);
        AddArmor(defaultAmount, duration);
    }

    public virtual void AddArmor(int amount, int duration)
    {
        if (amount <= 0) return;

        int finalAmount = Mathf.Max(0, amount + Robustez);
        armorShield += finalAmount;
        armorTurnsRemaining = duration;

        Debug.Log($"{gameObject.name} recibe {amount} de armadura por {duration} turnos.");
    }

    public virtual void OnTurnPassed()
    {
        if (armorTurnsRemaining > 0)
        {
            armorTurnsRemaining--;

            Debug.Log($"{gameObject.name} - armadura duración restante: {armorTurnsRemaining} turnos.");

            if (armorTurnsRemaining <= 0)
            {
                armorShield = 0;
                armorTurnsRemaining = 0;
                Debug.Log($"{gameObject.name} - armadura ha expirado.");
            }
        }
    }
    public void SetArmorDuration(int turns)
    {
        armorTurnsRemaining = turns;
    }
    public virtual EnemyActionData Identificar_Accion()
    {
        if (stats == null)
        {
            Debug.LogError($"{name}: stats es NULL en Identificar_Accion()");
            accionElegida = null;
            return null;
        }

        // Helper seguro para obtener elemento o fallback
        EnemyActionData SafeGet(List<EnemyActionData> list, int index = 0)
        {
            if (list == null || list.Count == 0)
                return null;
            if (index < 0) index = 0;
            if (index < list.Count)
                return list[index];
            // fallback: último elemento disponible
            return list[list.Count - 1];
        }

        EnemyActionData movimientoActual = SafeGet(stats.Ataques, 0);

        switch (eleccionActual)
        {
            case 1: // Ataque
                cont_Ataques++;

                // Si hay al menos 2 ataques, alternar entre 0 y 1
                if (stats.Ataques != null && stats.Ataques.Count >= 2)
                {
                    movimientoActual = (cont_Ataques % 2 != 0) ? stats.Ataques[0] : stats.Ataques[1];
                }
                else
                {
                    // Si solo hay 1 o 0, tomar el primero si existe
                    movimientoActual = SafeGet(stats.Ataques, 0);
                }

                // Si hay ataque especial en índice 2 y corresponde el contador, usarlo
                if (cont_Ataques == 5)
                {
                    if (stats.Ataques != null && stats.Ataques.Count >= 3)
                    {
                        movimientoActual = stats.Ataques[2];
                    }
                    else
                    {
                        Debug.LogWarning($"{name}: intento usar stats.Ataques[2] pero la lista no tiene 3 elementos. Usando fallback.");
                        movimientoActual = SafeGet(stats.Ataques, 0);
                    }
                    cont_Ataques = 0;
                }
                break;

            case 2: // Curación
                cont_Curacion++;

                if (stats.Curacion != null && stats.Curacion.Count >= 2)
                {
                    if (cont_Curacion % 4 == 0)
                    {
                        movimientoActual = stats.Curacion[0];
                        cont_Curacion = 0;
                    }
                    else
                    {
                        movimientoActual = stats.Curacion[1];
                    }
                }
                else
                {
                    movimientoActual = SafeGet(stats.Curacion, 0);
                    if (movimientoActual == null)
                        movimientoActual = SafeGet(stats.Ataques, 0); // fallback a ataque si no hay curaciones
                }
                break;

            case 3: // Buff
                if (stats.Buffs != null && stats.Buffs.Count > 0)
                    movimientoActual = stats.Buffs[Random.Range(0, stats.Buffs.Count)];
                else
                {
                    Debug.LogWarning($"{name}: stats.Buffs vacío, usando ataque por defecto.");
                    movimientoActual = SafeGet(stats.Ataques, 0);
                }
                break;

            case 4: // Debuff
                if (stats.Debuffs != null && stats.Debuffs.Count > 0)
                    movimientoActual = stats.Debuffs[Random.Range(0, stats.Debuffs.Count)];
                else
                {
                    Debug.LogWarning($"{name}: stats.Debuffs vacío, usando ataque por defecto.");
                    movimientoActual = SafeGet(stats.Ataques, 0);
                }
                break;

            case 5: // Invocar
                if (PCM != null && PCM.Hay_Espacio && stats.Invocaciones != null && stats.Invocaciones.Count > 0)
                    movimientoActual = stats.Invocaciones[0];
                else
                {
                    movimientoActual = SafeGet(stats.Ataques, 0);
                }
                break;

            case 6: // Huir
                movimientoActual = SafeGet(stats.Ataques, 0); // opcional, o null si prefieres
                break;

            default:
                movimientoActual = SafeGet(stats.Ataques, 0);
                break;
        }

        if (movimientoActual == null)
        {
            Debug.LogWarning($"{name}: No se encontró movimiento válido en Identificar_Accion(). Revisa el Base_Stat_Enemy del enemigo.");
        }

        accionElegida = movimientoActual;
        return movimientoActual;
    }

    protected virtual void EjecutarAccion(EnemyActionData accion)
    {
        if (accion == null) return;

        foreach (var effect in accion.effects)
        {
            // 🔥 Detectar si es efecto de enemigo
            if (effect is EnemyHealEffect healEffect)
            {
                healEffect.Execute(this, PCM);
            }
            else if (effect is EnemyStatBuffEffect strEffect)
            {
                strEffect.Execute(this, PCM);
            }
            else if (effect is EnemyCleanseEffect cleanseEffect)
            {
                cleanseEffect.Execute(this, PCM);
            }
            else if (effect is EnemyDealDamageEffect dmgEffect)
            {
                dmgEffect.Execute(this, PCM);
            }
            else if (effect is EnemyShieldEffect shieldEffect)
            {
                shieldEffect.Execute(this);
            }
            else if (effect is EnemyShieldAllEffect shieldAll)
            {
                shieldAll.Execute(PCM);
            }
            else if (effect is Escape escapeEffect)
            {
                escapeEffect.Execute(this, PCM);
            }
            else
            {
                effect.Apply(null, PCM);
            }
        }

        if (currentHP > MaxHP)
            currentHP = MaxHP;
    }
    public void EjecutarAccionElegida()
    {
        if (accionElegida == null) return;

        EjecutarAccion(accionElegida);
    }
    public virtual IEnumerator PerformAction()
    {
        if (accionElegida == null)
        {
            Debug.LogWarning($"{gameObject.name} no tenía acción, eligiendo una ahora.");
            Agregar_Iconos();
        }

        Debug.Log($"{gameObject.name} ejecuta acción");

        yield return StartCoroutine(FocoOn());          //Activa el Foco
        yield return new WaitForSeconds(0.1f);          // Pausa para que se note el foco

        // 🔊 SONIDO DE LA ACCIÓN
        if (accionElegida != null && accionElegida.soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(accionElegida.soundEffect);
        }

        EjecutarAccionElegida();

        switch (eleccionActual)
        {
            case 1:
                //Nada, combat manager maneja este
                break;
            case 2:
                yield return StartCoroutine(Flash(Color.green));    // Curar
                break;
            case 3:
                yield return StartCoroutine(Flash(Color.blue));     // Buff
                break;
            case 4:
                yield return StartCoroutine(Flash(Color.purple));   // Debuff
                break;
            case 5:
                yield return StartCoroutine(Flash(Color.black));    // Invocar 
                break;
            case 6:
                yield return StartCoroutine(Flash(Color.white));   // Escapar
                break;
        }

        yield return new WaitForSeconds(0.2f);          // Espera para que se vea el resultado
        yield return StartCoroutine(FocoOff());         // 🔴 FOCO OFF

        LimpiarIntencion();                        //Limpiar acción elegida para la próxima ronda
        //Agregar_Iconos();
        yield return new WaitForSeconds(0.2f);          // Pausa entre enemigos
    }
    public void ProcessStatuses()
    {
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            var status = activeStatuses[i];

            // =========================
            // 🟢 EFECTOS POR TURNO
            // =========================

            if (status.effect.effectName == "Veneno")
            {
                TakeDamage(status.value);
                Debug.Log($"{name} recibe {status.value} de veneno");
            }

            if (status.effect.effectName == "Hemorragia")
            {
                TakeDamage(status.value);
                Debug.Log($"{name} sangra {status.value}");

                if (status.duration <= 0)
                {
                    // 💥 Burst final
                    TakeDamage(status.burstValue);
                    Debug.Log($"{name} sufre hemorragia final de {status.burstValue}");

                    // 🔄 REVERT (si aplica)
                    if (status.effect.revertOnExpire)
                    {
                        AddStat(status.type, status.value);
                        Debug.Log($"{name} recupera {status.value} de stat tipo {status.type}");
                    }

                    activeStatuses.RemoveAt(i);
                    //RecalcularIntencion();
                }

                continue; // 🔥 IMPORTANTE: evita doble reducción
            }
            // =========================
            // ⏳ REDUCIR DURACIÓN
            // =========================
            status.duration--;

            // =========================
            // 💀 EXPIRACIÓN GENERAL
            // =========================
            if (status.duration <= 0)
            {
                if (status.effect.revertOnExpire)
                {
                    AddStat(status.type, status.value);
                    Debug.Log($"{name} recupera {status.value} de stat tipo {status.type}");
                }

                activeStatuses.RemoveAt(i);
            }
        }
        RecalcularIntencion();
        statusUI?.UpdateStatuses();
    }
    public void LimpiarIntencion()
    {
        accionElegida = null;

        Icono.enabled = false;
        Texto_Icono.text = "";
    }
    public void AddStat(int type, int amount)
    {
        switch (type)
        {
            case 1:
                Fuerza += amount;
                break;

            case 2:
                Magia += amount;
                break;

            case 3:
                Robustez += amount;
                break;

            default:
                Debug.LogWarning("Tipo de stat desconocido en Enemy");
                break;
        }

        Debug.Log($"{name} cambia stat {type} en {amount}. (F:{Fuerza} M:{Magia} R:{Robustez})");
    }
    public void AddStatus(StatusEffect effect, int value, int duration, int type)
    {
        AddStat(type, effect.category == StatusEffect.StatusCategory.Debuff ? -value : value);

        ActiveStatus status = new ActiveStatus(effect, value, duration);
        status.type = type;
        activeStatuses.Add(status);
        // 🔥 AÑADE ESTO
        statusUI?.UpdateStatuses();
    }
    void OnEnable()         //Actualizar barra salud al activar el enemigo
    {
        OnHealthChanged?.Invoke(currentHP, MaxHP);
    }
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHP = Mathf.Min(currentHP + amount, MaxHP);

        if (currentHP > MaxHP)
            currentHP = MaxHP;

        OnHealthChanged?.Invoke(currentHP, MaxHP);
    }
    IEnumerator FocoOn()
    {
        if (visualRoot == null) yield break;

        Vector3 originalScale = visualRoot.localScale;
        Vector3 targetScale = baseScale * 1.1f;

        float t = 0;
        float duration = 0.1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            visualRoot.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }
    }
    IEnumerator FocoOff()
    {
        if (visualRoot == null) yield break;

        Vector3 originalScale = visualRoot.localScale;
        Vector3 targetScale = baseScale;

        float t = 0;
        float duration = 0.1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            visualRoot.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }
    }
    IEnumerator Flash(Color flashColor)
    {
        if (sprite == null)
            yield break;

        sprite.color = flashColor;

        yield return new WaitForSeconds(0.1f);

        sprite.color = originalSpriteColor;

        flashRoutine = null;
    }
    private void PlayFlash(Color color)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(Flash(color));
    }
    public void RecalcularIntencion()
    {
        if (accionElegida == null)
            return;

        ActualizarIcono();
    }
    public int GetArmorShield() => armorShield;
    public int GetArmorTurnsRemaining() => armorTurnsRemaining;
}