using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Panel_Caracteristicas_Objetos : MonoBehaviour
{
    public static Panel_Caracteristicas_Objetos Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text textoDescripcion;
    [SerializeField] private CanvasGroup canvasGroup;

    private Sequence secuenciaMensaje;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Hay otra instancia en " + Instance.gameObject.scene.name);
        }

        if (Instance != null && Instance.gameObject.scene == gameObject.scene)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        Debug.Log("ON DESTROY -> " + gameObject.scene.name);

        if (Instance == this)
            Instance = null;
    }

    public void Mostrar(RectTransform objetivo, string descripcion)
    {
        panel.SetActive(true);

        textoDescripcion.text = descripcion;
        if (PlayerRunData.Instance.Combatiendo)
        {
            //panel.transform.position = objetivo.position + new Vector3(0f, -50f, 0f);
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(objetivo.anchoredPosition.x - 50.6f, 155f);
            Debug.Log(gameObject.scene.name);
        }

        panel.transform.SetAsLastSibling();
    }
    public void MostrarCofre(int num, string Nombre, string descripcion)
    {
        switch (num)
        {
            case 1:
                textoDescripcion.text = "Carta obtenida: " + Nombre + " (" + descripcion + ")";
                break;
            case 2:
                textoDescripcion.text = "Objeto obtenido: " + Nombre + " (" + descripcion + ")";
                break;
            case 3:
                textoDescripcion.text = "Reliquia obtenida: " + Nombre + " (" + descripcion + ")";
                break;
        }
        panel.SetActive(true);
        //MostrarAnimado();
        //canvasGroup.alpha = 1f;
        //panel.transform.localScale = Vector3.one;
    }
    public void MensajeDirecto(string texto)
    {
        textoDescripcion.text = texto;
        panel.SetActive(true);
    }
    public void Ocultar()
    {
        panel.SetActive(false);
    }
    private void MostrarAnimado()
    {
        secuenciaMensaje?.Kill(true);

        panel.SetActive(true);

        canvasGroup.alpha = 0f;
        panel.transform.localScale = Vector3.one * 0.8f;

        secuenciaMensaje = DOTween.Sequence();

        // Fade In
        secuenciaMensaje.Append(
            canvasGroup.DOFade(1f, 0.2f)
        );

        // Escala con efecto "pop"
        secuenciaMensaje.Join(
            panel.transform
                .DOScale(1f, 0.25f)
                .SetEase(Ease.OutBack)
        );

        // Esperar
        secuenciaMensaje.AppendInterval(2.5f);

        // Fade Out
        secuenciaMensaje.Append(
            canvasGroup.DOFade(0f, 0.2f)
        );

        secuenciaMensaje.OnComplete(() =>
        {
            panel.SetActive(false);
        });
    }
    public void Registrar()
    {
        Instance = this;
        panel.SetActive(false);
    }
    public void Apagar()
    {
        panel.SetActive(false);
    }
}