
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LuzMenu : MonoBehaviour
{
    [Header("Imagens")]
    [SerializeField] private Image meioEscuro;
    [SerializeField] private Image escuro;

    [Header("Intervalos")]
    [SerializeField] private float esperaInicial = 1.5f;
    [SerializeField] private float esperaMinima = 6f;
    [SerializeField] private float esperaMaxima = 12f;

    [Header("Animacao")]
    [SerializeField] private float duracaoTransicao = 0.6f;
    [SerializeField] private float tempoEscurecido = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float chanceEscuroForte = 0.3f;

    private void Start()
    {
        DefinirAlpha(meioEscuro, 0f);
        DefinirAlpha(escuro, 0f);

        StartCoroutine(AnimarLuzes());
    }

    private IEnumerator AnimarLuzes()
    {
        yield return new WaitForSeconds(esperaInicial);

        bool primeiraAnimacao = true;

        while (true)
        {
            // Escurece levemente
            yield return Transicao(meioEscuro, 0f, 1f);
            yield return new WaitForSeconds(tempoEscurecido);

            // Escurecimento forte apenas nas animacoes seguintes
            if (!primeiraAnimacao && Random.value < chanceEscuroForte)
            {
                yield return Transicao(escuro, 0f, 1f);
                yield return new WaitForSeconds(0.7f);
                yield return Transicao(escuro, 1f, 0f);
            }

            // Volta ao normal
            yield return Transicao(meioEscuro, 1f, 0f);

            primeiraAnimacao = false;

            // Espera aleatoria ate a proxima animacao
            float espera = Random.Range(esperaMinima, esperaMaxima);
            yield return new WaitForSeconds(espera);
        }
    }

    private IEnumerator Transicao(Image imagem, float inicio, float fim)
    {
        float tempo = 0f;

        while (tempo < duracaoTransicao)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(tempo / duracaoTransicao);
            progresso = Mathf.SmoothStep(0f, 1f, progresso);

            DefinirAlpha(imagem, Mathf.Lerp(inicio, fim, progresso));

            yield return null;
        }

        DefinirAlpha(imagem, fim);
    }

    private void DefinirAlpha(Image imagem, float alpha)
    {
        Color cor = imagem.color;
        cor.a = alpha;
        imagem.color = cor;
    }
}
