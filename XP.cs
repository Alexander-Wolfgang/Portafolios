using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class XP : MonoBehaviour
{
    public Image barraXP;
    public float velocidad = 0.5f;     // 0.05 era extremadamente lento
    public bool AnimandoXP { get; private set; }

    public void Actualizar_XP(int XP_Actual, int XP_Max, int XP_Ganado)
    {
        StopAllCoroutines();
        AnimandoXP = true;
        StartCoroutine(AnimarConNivel(XP_Actual, XP_Max, XP_Ganado));
    }

    public void Barra_XP_Inicio_Combate(int XP_Actual, int XP_Max)
    {
        barraXP.fillAmount = (float)XP_Actual / XP_Max;
    }

    IEnumerator AnimarBarra(float inicio, float fin)
    {
        float fill = inicio;

        while (Mathf.Abs(fill - fin) > 0.001f)
        {
            fill = Mathf.MoveTowards(fill, fin, velocidad * Time.deltaTime);
            barraXP.fillAmount = fill;
            yield return null;
        }

        barraXP.fillAmount = fin;
    }

    IEnumerator AnimarConNivel(int XP_Actual, int XP_Max, int XP_Ganado)
    {
        int xpActual = XP_Actual;
        int xpRestante = XP_Ganado;

        while (xpRestante > 0)
        {
            int xpParaNivel = XP_Max - xpActual;

            // Subes de nivel
            if (xpRestante >= xpParaNivel)
            {
                yield return StartCoroutine(
                    AnimarBarra(
                        (float)xpActual / XP_Max,
                        1f));

                LevelUp();

                xpRestante -= xpParaNivel;
                xpActual = 0;

                barraXP.fillAmount = 0f;

                yield return new WaitForSeconds(0.25f);
            }
            // No alcanzas el siguiente nivel
            else
            {
                xpActual += xpRestante;

                yield return StartCoroutine(
                    AnimarBarra(
                        (float)(xpActual - xpRestante) / XP_Max,
                        (float)xpActual / XP_Max));

                xpRestante = 0;
            }
        }

        // Guardar la XP restante para el siguiente combate
        CombatManager.Instance.Exp_Actual = xpActual;

        AnimandoXP = false;
    }

    public void LevelUp()
    {
        CombatManager.Instance.SubirNivel();
    }
}