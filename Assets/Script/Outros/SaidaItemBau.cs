
using System.Collections;
using UnityEngine;

public class SaidaItemBau : MonoBehaviour
{
    [Header("Saida do Bau")]
    [SerializeField] private float distancia = 0.8f;
    [SerializeField] private float altura = 0.7f;
    [SerializeField] private float duracao = 0.6f;

    [Header("Flutuacao")]
    [SerializeField] private float amplitude = 0.12f;
    [SerializeField] private float velocidade = 3f;

    public void Iniciar()
    {
        StartCoroutine(AnimarItem());
    }

    private IEnumerator AnimarItem()
    {
        Collider2D[] colisores = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colisores)
            col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Vector3 inicio = transform.position;
        Vector3 destino = inicio + new Vector3(distancia, -0.25f, 0);

        float tempo = 0f;

        // Item sai do bau fazendo um arco
        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracao);

            float arco = Mathf.Sin(t * Mathf.PI) * altura;

            transform.position =
                Vector3.Lerp(inicio, destino, t) +
                Vector3.up * arco;

            yield return null;
        }

        transform.position = destino;

        foreach (Collider2D col in colisores)
            col.enabled = true;

        // Item fica flutuando ate ser coletado
        float tempoFlutuacao = 0f;

        while (true)
        {
            tempoFlutuacao += Time.deltaTime;

            float movimento =
                Mathf.Sin(tempoFlutuacao * velocidade) * amplitude;

            transform.position = destino + Vector3.up * movimento;

            yield return null;
        }
    }
}
