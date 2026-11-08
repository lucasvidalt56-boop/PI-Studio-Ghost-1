
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PosteApaga : MonoBehaviour
{
    [Header("Referencias")]
    public Light2D luz;
    public SpriteRenderer imagemPoste;
    public Sprite spriteApagado;

    [Header("Suspense")]
    public float distancia = 2f;
    public float duracaoFalha = 1.5f;

    private Transform jogador;
    private float intensidadeOriginal;
    private bool apagou;

    void Start()
    {
        if (imagemPoste == null)
            imagemPoste = GetComponent<SpriteRenderer>();

        if (luz != null)
            intensidadeOriginal = luz.intensity;
    }

    void Update()
    {
        if (apagou || luz == null) return;

        if (jogador == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) jogador = p.transform;
            return;
        }

        if (Mathf.Abs(jogador.position.x -
            transform.position.x) <= distancia)
        {
            apagou = true;
            StartCoroutine(Falhar());
        }
    }

    IEnumerator Falhar()
    {
        float tempo = 0f;

        while (tempo < duracaoFalha)
        {
            float fator = Mathf.Lerp(
                1f, 0.2f, tempo / duracaoFalha
            );

            luz.intensity = intensidadeOriginal *
                Random.Range(0.05f, 1f) * fator;

            float intervalo = Random.Range(0.05f, 0.18f);
            tempo += intervalo;

            yield return new WaitForSeconds(intervalo);
        }

        luz.intensity = 0f;
        luz.enabled = false;

        if (imagemPoste != null && spriteApagado != null)
            imagemPoste.sprite = spriteApagado;
    }
}