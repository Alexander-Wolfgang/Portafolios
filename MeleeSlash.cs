using UnityEngine;
using System.Collections.Generic;

public class MeleeSlash : MonoBehaviour
{
    public Player_Control player;
    [Header("Weapon Visual")]
    public Sprite weaponSprite;

    [Header("Hitbox")]
    public Vector2 colliderSize = new Vector2(1f, 0.5f);
    public Vector2 colliderOffset = new Vector2(0.5f, 0);

    [Header("Animation")]
    public float startAngle = -80f;
    public float endAngle = 80f;
    public float duration = 0.2f;

    [Header("Stats")]
    public int damage = 10;

    float timer = 0f;
    SpriteRenderer sr;
    BoxCollider2D col;
    HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Intentar obtener el collider en el mismo GameObject, si no en hijos
        col = GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = GetComponentInChildren<BoxCollider2D>();
            if (col != null)
                Debug.Log("MeleeSlash: BoxCollider2D encontrado en un hijo.");
        }

        if (col == null)
        {
            Debug.LogWarning("MeleeSlash: No se encontró BoxCollider2D en el objeto ni en hijos. Añade uno al prefab.");
        }

        // Asegurar Rigidbody2D (necesario para que OnTriggerEnter2D se invoque)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Debug.Log("MeleeSlash: Rigidbody2D kinematic añadido automáticamente.");
        }
    }

    void Start()
    {
        // Aplicar datos
        if (sr != null)
            sr.sprite = weaponSprite;

        if (col != null)
        {
            col.size = colliderSize;
            col.offset = colliderOffset;
            col.isTrigger = true; // asegurar trigger
        }

        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
        timer = 0f;
    }
    void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / duration);
        t = t * t * (3f - 2f * t);

        float angle = Mathf.Lerp(startAngle, endAngle, t);
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        // 🔥 Detectar enemigos manualmente
        DetectEnemies();

        if (timer >= duration)
        {
            if (player != null)
                player.atacando = false;

            Destroy(gameObject);
        }
    }
    void DetectEnemies()
    {
        Vector2 hitboxCenter = transform.TransformPoint(colliderOffset);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            hitboxCenter,
            colliderSize,
            transform.eulerAngles.z
        );

        foreach (Collider2D hit in hits)
        {
            if (hitEnemies.Contains(hit.gameObject))
                continue;

            hitEnemies.Add(hit.gameObject);

            // ======================
            // ENEMIGOS
            // ======================

            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("🗡️ Golpeaste enemigo");

                IA_Mov_tierra enemigo = hit.GetComponent<IA_Mov_tierra>();

                if (enemigo != null)
                {
                    int idGrupo = enemigo.ID_GrupoEnemigos;

                    PlayerRunData.Instance.ID_GrupoEnemigo = idGrupo;
                    PlayerRunData.Instance.BonusActual = (PlayerRunData.Bonus)1; //Ventaja para el jugador

                    Player_Control pc = Object.FindFirstObjectByType<Player_Control>();

                    if (pc != null)
                    {
                        pc.GuardarDatos();
                        pc.Combatir(idGrupo, enemigo);
                    }
                }
            }

            // ======================
            // COFRES
            // ======================

            ChestReward chest = hit.GetComponent<ChestReward>();

            if (chest != null)
            {
                //player.reproducir_SFX_Chest();
                chest.Hit();
            }

            // ======================
            // JAULA
            // ======================

            if (hit.CompareTag("Jaula"))
            {
                //player.reproducir_SFX_Chest();
                Debug.Log("🪤 Golpeaste una jaula");

                Jaula jaula = hit.GetComponent<Jaula>();

                if (jaula != null)
                {
                    jaula.RomperSoporte();
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 hitboxCenter = transform.TransformPoint(colliderOffset);

        Gizmos.matrix = Matrix4x4.TRS(
            hitboxCenter,
            transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(Vector3.zero, colliderSize);
    }
}