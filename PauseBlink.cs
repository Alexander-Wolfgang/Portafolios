using UnityEngine;
using TMPro;

public class PauseBlink : MonoBehaviour
{
    private TMP_Text text;
    public float speed = 2f;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.unscaledTime * speed));

        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
