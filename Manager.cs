using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    public List<CardData> unlockedCards;
    public List<CardData> playerDeck;
    public int maxCopiesPerCard = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Esto hace que persista al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
