
using System.Collections;
using UnityEngine;

public class UrubuGuia : MonoBehaviour
{
    [Header("Pontos de parada")]
    [SerializeField] private Transform[] pontos;

    [Header("Configuracoes do voo")]
    [SerializeField] private float velocidade = 4f;
    [SerializeField] private float alturaCurva = 2f;
    [SerializeField] private float distanciaAtivacao = 3f;

    [Header("Balao do urubu")]
    [SerializeField] private GameObject balaoSom;
    [SerializeField] private float tempoBalao = 1f;

    private Transform jogador;
    private Animator anim;
    private Vector3 escalaOriginal;

    private void Start()
    {
        anim = GetComponent<Animator>();

        if (balaoSom != null)
        {
            escalaOriginal = balaoSom.transform.localScale;
            balaoSom.SetActive(false);
        }

        if (anim == null || pontos == null || pontos.Length == 0)
        {
            Debug.LogError("Configure o Animator e os pontos do urubu!");
            return;
        }

        StartCoroutine(Guiar());
    }

    private IEnumerator Guiar()
    {
        for (int i = 0; i < pontos.Length; i++)
        {
            if (pontos[i] == null)
            {
                Debug.LogError("Ponto do urubu nao configurado!");
                yield break;
            }

            if (i > 0)
            {
                yield return EsperarJogador();
                yield return MostrarBalao();
            }

            anim.SetBool("Voando", true);

            yield return VoarAte(pontos[i].position);

            anim.SetBool("Voando", false);

            if (i == pontos.Length - 1)
            {
                yield return EsperarJogador();
                yield return MostrarBalao();
            }
        }
    }

    private IEnumerator EsperarJogador()
    {
        while (true)
        {
            if (jogador == null)
            {
                GameObject player =
                    GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                    jogador = player.transform;
            }

            if (jogador != null &&
                Mathf.Abs(jogador.position.x - transform.position.x)
                <= distanciaAtivacao)
            {
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator MostrarBalao()
    {
        if (balaoSom == null)
            yield break;

        Transform alvo = balaoSom.transform;

        balaoSom.SetActive(true);

        // Aparece crescendo
        yield return AnimarEscala(alvo, 0.6f, 1.08f, 0.16f);

        // Volta ao tamanho normal
        yield return AnimarEscala(alvo, 1.08f, 1f, 0.08f);

        // Fica visivel
        yield return new WaitForSeconds(tempoBalao);

        // Diminui e desaparece
        yield return AnimarEscala(alvo, 1f, 0f, 0.16f);

        balaoSom.SetActive(false);
        alvo.localScale = escalaOriginal;
    }

    private IEnumerator AnimarEscala(
        Transform alvo, float inicio, float fim, float duracao)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float t = Mathf.Clamp01(tempo / duracao);
            t = Mathf.SmoothStep(0f, 1f, t);

            float escala = Mathf.Lerp(inicio, fim, t);

            alvo.localScale = escalaOriginal * escala;

            yield return null;
        }

        alvo.localScale = escalaOriginal * fim;
    }

    private IEnumerator VoarAte(Vector3 destino)
    {
        Vector3 inicio = transform.position;
        destino.z = inicio.z;

        float distancia = Vector2.Distance(inicio, destino);
        float duracao = distancia / Mathf.Max(velocidade, 0.01f);
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float t = Mathf.Clamp01(tempo / duracao);

            Vector3 posicao = Vector3.Lerp(inicio, destino, t);
            posicao.y += 4f * alturaCurva * t * (1f - t);

            transform.position = posicao;

            yield return null;
        }

        transform.position = destino;
    }
}
