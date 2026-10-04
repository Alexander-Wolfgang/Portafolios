[System.Serializable]
public class ActiveStatus
{
    public StatusEffect effect;
    public int value;
    public int duration;
    public int burstValue;
    public int type;

    // 🔥 NUEVO: para UI / intención
    public bool isPerTurn;   // veneno, regen
    public bool hasBurst;    // hemorragia

    public ActiveStatus(StatusEffect effect, int value, int duration)
    {
        this.effect = effect;
        this.value = value;
        this.duration = duration;
    }
}