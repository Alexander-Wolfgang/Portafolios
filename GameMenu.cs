using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static MainMenu;

public class GameMenu : MonoBehaviour
{
    public const string MAIN_MENU = "1-MainMenu";
    public const string Ex = "3-Exploracion";

    [Header("Definir paneles de la escena")]
    public RectTransform MainMenuHub;       //Panel principal del hub
    public RectTransform Panel_Seleccionar; //Panel donde eliges personaje, baraja y reliquia, tras eso inicias la partida
    public RectTransform Panel_Personajes;  //Informacion sobre cada personaje del juego, en caso de no haberlo desbloqueado, estara una silueta
    public RectTransform Panel_Tienda;      //La tienda de objetos, lleno de cosas para comprar, incluido el cofre gacha de cartas
    public RectTransform Panel_Abrir_Cofre; //Aqui se abren los cofres que hayas comprado
    public RectTransform Panel_Barajas;     //Panel para ver tus cartas y armar barajas para tus personajes.
    public RectTransform Panel_Salir;       //Pregunta si de verdad quieres salir.

    public RectTransform Panel_Demo;        //"NO DISPONIBLE EN LA DEMO", texto que aparece al intentar entrar a ciertas

    [Header("Sonidos")]
    public AudioClip sonidoConfirmacion;        //Sonido generico de confirmacion 
    public AudioClip sonido_Iniciar_Partida;    //Sonido tras seleccionar todo para iniciar partida
    public AudioClip sonido_Entrar_Tienda;      //Sonido de entrar en tienda, your welcome
    public AudioClip sonido_Comprar_objeto;     //sonido de dinero callendo, se reproduce al comprar
    public AudioClip sonido_Caja_registradora;  //sonido de caja registradora al comprar algo, se reproduce a la mitad del sonido anterior
    public AudioClip sonido_Abrir_Cofre;        //Al abrir un cofre gacha, saltara el sonido, junto con uno de los sonidos de cartas
    public AudioClip sonido_Elegir_Carta;       //Sonido tipico al dar vuelta una carta o moverla rapidamente "swaaash"


    [Header("Sonidos Capyguardian")]
    public AudioClip sonidoCapyguardian;
    public AudioClip sonidoCapyguardian_Feliz;
    public AudioClip sonidoCapyguardian_Enojado;

    [Header("Sonidos Bullzerk")]
    public AudioClip sonidoBullzerk;
    public AudioClip sonidoBullzerk_Feliz;
    public AudioClip sonidoBullzerk_Enojado;

    [Header("Sonidos Catssassin")]
    public AudioClip sonidoCatssassin;
    public AudioClip sonidoCatssassin_Feliz;
    public AudioClip sonidoCatssassin_Enojado;

    [Header("Sonidos Frogzard")]
    public AudioClip sonidoFrogzard;
    public AudioClip sonidoFrogzard_Feliz;
    public AudioClip sonidoFrogzard_Enojado;

    [Header("Sonidos CuyMancer")]
    public AudioClip sonidoCuyMancer;
    public AudioClip sonidoCuyMancer_Feliz;
    public AudioClip sonidoCuyMancer_Enojado;

    [Header("Sonidos Dogker")]
    public AudioClip sonidoDogker;
    public AudioClip sonidoDogker_Feliz;
    public AudioClip sonidoDogker_Enojado;

    [Header("Sonidos Turktank")]
    public AudioClip sonidoTurktank;
    public AudioClip sonidoTurktank_Feliz;
    public AudioClip sonidoTurktank_Enojado;

    [Header("Sonidos Kangarage")]
    public AudioClip sonidoKangarage;
    public AudioClip sonidoKangarage_Feliz;
    public AudioClip sonidoKangarage_Enojado;

    [Header("Sonidos Monkkey")]
    public AudioClip sonidoMonkkey;
    public AudioClip sonidoMonkkey_Feliz;
    public AudioClip sonidoMonkkey_Enojado;

    [Header("Sonidos Sealed")]
    public AudioClip sonidoSealed;
    public AudioClip sonidoSealed_Feliz;
    public AudioClip sonidoSealed_Enojado;

    [Header("Sonidos Goatness")]
    public AudioClip sonidoGoatness;
    public AudioClip sonidoGoatness_Feliz;
    public AudioClip sonidoGoatness_Enojado;

    public void Entrar_Calabozo() //Clic
    {
        OcultarPanel(MainMenuHub);
        MostrarPanel(Panel_Seleccionar);

    }
    public void Personajes() //Clic
    {
        OcultarPanel(MainMenuHub);
        MostrarPanel(Panel_Personajes);
    }

    public void Entrar_Tienda() //Clic
    {
        OcultarPanel(MainMenuHub);
        //MostrarPanel(Panel_Tienda);
        MostrarPanel(Panel_Demo);
    }

    public void Abrir_Cofre() //Clic
    {
        OcultarPanel(MainMenuHub);
        //MostrarPanel(Panel_Abrir_Cofre);
        MostrarPanel(Panel_Demo);
    }

    public void Barajas() //Clic
    {
        OcultarPanel(MainMenuHub);
        //MostrarPanel(Panel_Barajas);
        MostrarPanel(Panel_Demo);
    }

    public void Horno_Cartas() //Clic
    {
        OcultarPanel(MainMenuHub);
        //MostrarPanel(Panel_Horno_Cartas);
        MostrarPanel(Panel_Demo);
    }

    public void Salir() //Clic
    {
        OcultarPanel(MainMenuHub);
        MostrarPanel(Panel_Salir);
    }

    public void Volver_MainMenu() //Clic
    {
        //Cargar Escena 1
        SceneManager.LoadSceneAsync(SceneNames.MAIN_MENU);
    }
    public void Cancelar_Salir() //Clic
    {
        OcultarPanel(Panel_Salir);
        MostrarPanel(MainMenuHub);
    }
    public void Demo() //Clic
    {
        OcultarPanel(Panel_Demo);
        MostrarPanel(MainMenuHub);
    }

    public void MostrarPanel(RectTransform panel)
    {
        if (panel != null)
        {
            // Centra el panel en el Canvas
            panel.anchoredPosition = Vector2.zero;
            panel.localScale = Vector3.one;
            panel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Se pasó un panel nulo a MostrarPanel.");
        }
    }
    public void OcultarPanel(RectTransform panel)
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Se pasó un panel nulo a MostrarPanel.");
        }
    }
}
