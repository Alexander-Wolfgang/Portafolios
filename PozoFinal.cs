using UnityEngine;

public class PozoFinal : MonoBehaviour
{
    [SerializeField] private GameObject panelFinDemo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            panelFinDemo.SetActive(true);
        }
    }
}