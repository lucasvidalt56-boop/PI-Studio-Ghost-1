
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject botaoInicial;

    [Header("Transicao")]
    [SerializeField] private Image fadePreto;
    [SerializeField] private float duracaoFade = 0.7f;

    private bool iniciandoJogo;

    private void Start()
    {
        Color cor = fadePreto.color;
        cor.a = 0f;
        fadePreto.color = cor;

        EventSystem.current.firstSelectedGameObject = botaoInicial;
        EventSystem.current.SetSelectedGameObject(botaoInicial);
    }

    public void Jogar()
    {
        if (!iniciandoJogo)
            StartCoroutine(IniciarJogo());
    }

    private IEnumerator IniciarJogo()
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

        SceneManager.LoadScene("Cena1");
    }

    public void Sair()
    {
        Debug.Log("Botao SAIR pressionado!");
        Application.Quit();
    }
}
