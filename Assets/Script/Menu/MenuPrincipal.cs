using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject botaoInicial;
    [SerializeField] private Button botaoContinuar;

    [Header("Transicao")]
    [SerializeField] private Image fadePreto;
    [SerializeField] private float duracaoFade = 0.7f;

    private bool iniciandoJogo;

    private void Start()
    {
        Time.timeScale = 1f;

        Color cor = fadePreto.color;
        cor.a = 0f;
        fadePreto.color = cor;

        botaoContinuar.interactable = SistemaSave.Ler() != null;

        if (EventSystem.current != null)
        {
            EventSystem.current.firstSelectedGameObject = botaoInicial;
            EventSystem.current.SetSelectedGameObject(botaoInicial);
        }
    }

    public void Jogar()
    {
        if (iniciandoJogo) return;

        SistemaSave.NovoJogo();
        StartCoroutine(IniciarJogo("Cena1"));
    }

    public void Continuar()
    {
        if (iniciandoJogo) return;

        DadosSave dados = SistemaSave.Ler();

        if (dados == null)
        {
            botaoContinuar.interactable = false;
            return;
        }

        SistemaSave.PrepararCarregamento(dados);
        StartCoroutine(IniciarJogo(dados.cena));
    }

    private IEnumerator IniciarJogo(string cena)
    {
        iniciandoJogo = true;

        float tempo = 0f;
        Color cor = fadePreto.color;

        while (tempo < duracaoFade)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duracaoFade);
            cor.a = Mathf.SmoothStep(0f, 1f, progresso);
            fadePreto.color = cor;

            yield return null;
        }

        cor.a = 1f;
        fadePreto.color = cor;

        Time.timeScale = 1f;
        SceneManager.LoadScene(cena);
    }

    public void Sair()
    {
        Debug.Log("Botao SAIR pressionado!");
        Application.Quit();
    }
}