using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public TextMeshProUGUI Skin_01;
    //Skin 2
    public Image Cartel_02;
    public TextMeshProUGUI Precio_02;
    public TextMeshProUGUI Skin_02;
    //Skin 3
    public Image Cartel_03;
    public TextMeshProUGUI Precio_03;
    public TextMeshProUGUI Skin_03;
    //Skin 4
    public Image Cartel_04;
    public TextMeshProUGUI Precio_04;
    public TextMeshProUGUI Skin_04;
    //Skin 5
    public Image Cartel_05;
    public TextMeshProUGUI Precio_05;
    public TextMeshProUGUI Skin_05;
    //Skin 6
    public Image Cartel_06;
    public TextMeshProUGUI Precio_06;
    public TextMeshProUGUI Skin_06;
    //Skin 7
    public Image Cartel_07;
    public TextMeshProUGUI Precio_07;
    public TextMeshProUGUI Skin_07;
    //Skin 8
    public Image Cartel_08;
    public TextMeshProUGUI Precio_08;
    public TextMeshProUGUI Skin_08;
    //Skin 9
    public Image Cartel_09;
    public TextMeshProUGUI Precio_09;
    public TextMeshProUGUI Skin_09;
    //Skin 10
    public Image Cartel_10;
    public TextMeshProUGUI Precio_10;
    public TextMeshProUGUI Skin_10;

    [Header("Sonidos")]
    public AudioClip sonido_Equipar;
    public AudioClip sonido_Comprar;

    //Texto que muestra la cantidad de monedas actuales del jugador en la interfaz de la tienda
    public TextMeshProUGUI Puntos_actuales;
    private int SkinsCompradas = 0;

    public void Awake()
    {
        ActualizarAlIniciar();
    }

    public void Equipar_01()    //Equipar Skin 1
    {
        PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 1);
        PlayerPrefs.Save();
        AudioManager.Instance.ReproducirSFX(sonido_Equipar);
        ActualizarTienda();
    }
    public void Equipar_02()
    {
        if (Skin_02.text == "BUY")
        {
            Comprar(5, "2");
        }

        if (PlayerPrefs.GetInt("RollingMaze_Skin02", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 2);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_03()    //Equipar Skin 3, se llama con el click del boton de la skin 3
    {
        if(Skin_03.text == "BUY")
        {
            Comprar(5,"3");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin03", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 3);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_04()    //Equipar Skin 4, se llama con el click del boton de la skin 4
    {
        if(Skin_04.text == "BUY")
        {
            Comprar(10,"4");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin04", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 4);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_05()    //Equipar Skin 5, se llama con el click del boton de la skin 5
    {
        if (Skin_05.text == "BUY")
        {
            Comprar(10, "5");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin05", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 5);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_06()    //Equipar Skin 6, se llama con el click del boton de la skin 6
    {
        if (Skin_06.text == "BUY")
        {
            Comprar(15, "6");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin06", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 6);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_07()    //Equipar Skin 7, se llama con el click del boton de la skin 7
    {
        if (Skin_07.text == "BUY")
        {
            Comprar(15, "7");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin07", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 7);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_08()    //Equipar Skin 8, se llama con el click del boton de la skin 8
    {
        if (Skin_08.text == "BUY")
        {
            Comprar(20, "8");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin08", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 8);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_09()    //Equipar Skin 9, se llama con el click del boton de la skin 9
    {
        if (Skin_09.text == "BUY")
        {
            Comprar(30, "9");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin09", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 9);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    public void Equipar_10()    //Equipar Skin 10, se llama con el click del boton de la skin 10
    {
        if (Skin_10.text == "BUY")
        {
            Comprar(70, "10");
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin10", 0) == 1)
        {
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", 10);
            PlayerPrefs.Save();
            AudioManager.Instance.ReproducirSFX(sonido_Equipar);
            ActualizarTienda();
        }
    }
    void Comprar(int precio, string NumSkin)
    {
        int puntos = PlayerPrefs.GetInt("RollingMaze_Puntos", 0);
        int Numero_Skin = int.Parse(NumSkin);
        AudioManager.Instance.ReproducirSFX(sonido_Comprar);

        if (precio <= puntos)
        {
            // Quitar puntos
            PlayerPrefs.SetInt("RollingMaze_Puntos", puntos - precio);


            // Equipar inmediatamente la skin recién comprada
            PlayerPrefs.SetInt("RollingMaze_SkinSeleccionada", Numero_Skin);

            if(Numero_Skin < 10)
            {
                // Marcar skin como comprada, con un 0 adelante, pues es menor a 10
                PlayerPrefs.SetInt("RollingMaze_Skin0" + NumSkin, 1);
            }
            else
            {
                // Marcar skin como comprada, sin el 0 adelante, pues es mayor a 10
                PlayerPrefs.SetInt("RollingMaze_Skin" + NumSkin, 1);
            }

            PlayerPrefs.Save();

            // Cambiar cartel de compra
            UpdateCartel(Numero_Skin);
                        
            return;
        }
        else
        {
            Debug.Log("No tienes suficientes monedas");
        }
        // Actualizar toda la tienda
        ActualizarTienda();
    }
    private void ActualizarTienda()
    {
        Puntos_actuales.text = PlayerPrefs.GetInt("RollingMaze_Puntos", 0).ToString();
        SkinsCompradas = 0;

        // Skin 2
        if (PlayerPrefs.GetInt("RollingMaze_Skin02", 0) == 1)
        {
            Skin_02.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_02.text = "BUY";

        // Skin 3
        if (PlayerPrefs.GetInt("RollingMaze_Skin03", 0) == 1)
        {
            Skin_03.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_03.text = "BUY";

        // Skin 4
        if (PlayerPrefs.GetInt("RollingMaze_Skin04", 0) == 1)
        {
            Skin_04.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_04.text = "BUY";

        // Skin 5
        if (PlayerPrefs.GetInt("RollingMaze_Skin05", 0) == 1)
        {
            Skin_05.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_05.text = "BUY";

        // Skin 6
        if (PlayerPrefs.GetInt("RollingMaze_Skin06", 0) == 1)
        {
            Skin_06.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_06.text = "BUY";

        // Skin 7
        if (PlayerPrefs.GetInt("RollingMaze_Skin07", 0) == 1)
        {
            Skin_07.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_07.text = "BUY";

        // Skin 8
        if (PlayerPrefs.GetInt("RollingMaze_Skin08", 0) == 1)
        {
            Skin_08.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_08.text = "BUY";

        // Skin 9
        if (PlayerPrefs.GetInt("RollingMaze_Skin09", 0) == 1)
        {
            Skin_09.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_09.text = "BUY";

        // Skin 10
        if (PlayerPrefs.GetInt("RollingMaze_Skin10", 0) == 1)
        {
            Skin_10.text = "EQUIP";
            SkinsCompradas++;
        }
        else
            Skin_10.text = "BUY";

        // Skin 1 siempre está disponible
        if(SkinsCompradas == 0)
            Skin_01.text = "EQUIPED";
        else
            Skin_01.text = "EQUIP";

        ActualSkinEquipada();
    }
    private void OnEnable()
    {
        ActualizarTienda();
    }

    private void ActualSkinEquipada()
    {
        int skinEquipada = PlayerPrefs.GetInt("RollingMaze_SkinSeleccionada");

        switch (skinEquipada)
        {
            case 1:
                Skin_01.text = "EQUIPED";
                break;
            case 2:
                Skin_02.text = "EQUIPED";
                break;
            case 3:
                Skin_03.text = "EQUIPED";
                break;
            case 4:
                Skin_04.text = "EQUIPED";
                break;
            case 5:
                Skin_05.text = "EQUIPED";
                break;
            case 6:
                Skin_06.text = "EQUIPED";
                break;
            case 7:
                Skin_07.text = "EQUIPED";
                break;
            case 8:
                Skin_08.text = "EQUIPED";
                break;
            case 9:
                Skin_09.text = "EQUIPED";
                break;
            case 10:
                Skin_10.text = "EQUIPED";
                break;
        }
    }

    private void UpdateCartel(int NumSkin)
    {
        switch (NumSkin)
        {
            case 2:
                Cartel_02.sprite = Resources.Load<Sprite>("Sold");
                Precio_02.text = "";
                break;
            case 3:
                Cartel_03.sprite = Resources.Load<Sprite>("Sold");
                Precio_03.text = "";
                break;
            case 4:
                Cartel_04.sprite = Resources.Load<Sprite>("Sold");
                Precio_04.text = "";
                break;
            case 5:
                Cartel_05.sprite = Resources.Load<Sprite>("Sold");
                Precio_05.text = "";
                break;
            case 6:
                Cartel_06.sprite = Resources.Load<Sprite>("Sold");
                Precio_06.text = "";
                break;
            case 7:
                Cartel_07.sprite = Resources.Load<Sprite>("Sold");
                Precio_07.text = "";
                break;
            case 8:
                Cartel_08.sprite = Resources.Load<Sprite>("Sold");
                Precio_08.text = "";
                break;
            case 9:
                Cartel_09.sprite = Resources.Load<Sprite>("Sold");
                Precio_09.text = "";
                break;
            case 10:
                Cartel_10.sprite = Resources.Load<Sprite>("Sold");
                Precio_10.text = "";
                break;
        }
    }
    private void ActualizarAlIniciar()
    {
        if (PlayerPrefs.GetInt("RollingMaze_Skin02", 0) == 1)
        {
            Cartel_02.sprite = Resources.Load<Sprite>("Sold");
            Precio_02.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin03", 0) == 1)
        {
            Cartel_03.sprite = Resources.Load<Sprite>("Sold");
            Precio_03.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin04", 0) == 1)
        {
            Cartel_04.sprite = Resources.Load<Sprite>("Sold");
            Precio_04.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin05", 0) == 1)
        {
            Cartel_05.sprite = Resources.Load<Sprite>("Sold");
            Precio_05.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin06", 0) == 1)
        {
            Cartel_06.sprite = Resources.Load<Sprite>("Sold");
            Precio_06.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin07", 0) == 1)
        {
            Cartel_07.sprite = Resources.Load<Sprite>("Sold");
            Precio_07.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin08", 0) == 1)
        {
            Cartel_08.sprite = Resources.Load<Sprite>("Sold");
            Precio_08.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin09", 0) == 1)
        {
            Cartel_09.sprite = Resources.Load<Sprite>("Sold");
            Precio_09.text = "";
        }
        if (PlayerPrefs.GetInt("RollingMaze_Skin10", 0) == 1)
        {
            Cartel_10.sprite = Resources.Load<Sprite>("Sold");
            Precio_10.text = "";
        }
    }
}
