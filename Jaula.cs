using UnityEngine;
using UnityEngine.Tilemaps;

public class Jaula : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool rota = false;

    [Header("Tilemap")]
    public Tilemap detallesTilemap;

    [Header("Tiles soporte")]
    public TileBase soporteIzquierdo;
    public TileBase soporteDerecho;

    [Header("Busqueda")]
    public int radioBusqueda = 2;

    [Header("Reward")]
    [SerializeField] private GameObject chestReward;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // La jaula empieza flotando
        rb.gravityScale = 0;
    }

    // 🔥 Llamar esto cuando la espada golpee la jaula
    public void RomperSoporte()
    {
        if (rota)
            return;

        rota = true;

        rb.gravityScale = 5f;

        Vector3Int centerCell = detallesTilemap.WorldToCell(transform.position);

        // Buscar tiles cerca de la jaula
        for (int x = -radioBusqueda; x <= radioBusqueda; x++)
        {
            for (int y = -radioBusqueda; y <= radioBusqueda; y++)
            {
                Vector3Int cell = new Vector3Int(
                    centerCell.x + x,
                    centerCell.y + y,
                    0
                );

                TileBase currentTile = detallesTilemap.GetTile(cell);

                // Si es uno de los soportes → borrarlo
                if (currentTile == soporteIzquierdo ||
                    currentTile == soporteDerecho)
                {
                    detallesTilemap.SetTile(cell, null);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Cuando golpea el piso
        if (collision.gameObject.CompareTag("IsGrounded"))
        {
            Debug.Log("💥 La jaula se rompió");

            // 🔥 mover cofre al lugar de impacto
            if (chestReward != null)
            {
                chestReward.transform.position = transform.position;
                chestReward.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}