using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckInitializer", menuName = "Deck/Deck Initializer")]
public class DeckInitializer : ScriptableObject
{
    [SerializeField] private List<CardData> startingCards = new List<CardData>();

    public List<CardData> GetStartingCards() => new List<CardData>(startingCards);
}
