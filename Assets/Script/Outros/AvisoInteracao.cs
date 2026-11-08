
using UnityEngine;
using TMPro;

public class AvisoInteracao : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject aviso;
    public Transform botaoX;
    public TMP_Text texto;

    [Header("Personalizacao")]
    public string mensagem = "ENTRAR";

    [Header("Animacao")]
    public float velocidadePulso = 2f;
    public float intensidadePulso = 0.04f;

    private Vector3 escalaOriginal;
    private bool jogadorPerto;

    void Start()
    {
        if (botaoX != null)
            escalaOriginal = botaoX.localScale;

        if (texto != null)
            texto.text = mensagem;

        if (aviso != null)
            aviso.SetActive(false);
    }

    void Update()
    {
        if (!jogadorPerto || aviso == null || botaoX == null)
            return;

        float pulso = 1f +
            Mathf.Sin(Time.time * velocidadePulso)
            * intensidadePulso;

        botaoX.localScale = escalaOriginal * pulso;
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (!EhJogador(outro))
            return;

        jogadorPerto = true;

        if (aviso != null)
            aviso.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        if (!EhJogador(outro))
            return;

        jogadorPerto = false;

        if (aviso != null)
            aviso.SetActive(false);

        if (botaoX != null)
            botaoX.localScale = escalaOriginal;
    }

    bool EhJogador(Collider2D outro)
    {
        if (outro == null)
            return false;

        return outro.CompareTag("Player") ||
            outro.GetComponentInParent<GrupoJogador>() != null;
    }
}
