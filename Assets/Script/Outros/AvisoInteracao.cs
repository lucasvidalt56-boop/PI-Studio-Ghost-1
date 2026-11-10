
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TMPro;

public class AvisoInteracao : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject aviso;
    public Transform botaoX;
    public Transform teclaE;
    public TMP_Text texto;

    [Header("Personalizacao")]
    public string mensagem = "ENTRAR";

    [Header("Animacao")]
    public float velocidadePulso = 2f;
    public float intensidadePulso = 0.04f;

    private bool jogadorPerto;
    private bool usandoControle;
    private Vector3 escalaX;
    private Vector3 escalaE;
    private IDisposable escutaInput;

    void OnEnable()
    {
        escutaInput = InputSystem.onAnyButtonPress.Call(botao =>
        {
            if (botao.device is Gamepad)
                usandoControle = true;
            else if (botao.device is Keyboard)
                usandoControle = false;
            else
                return;

            AtualizarIcones();
        });
    }

    void OnDisable()
    {
        escutaInput?.Dispose();
        escutaInput = null;
    }

    void Start()
    {
        if (botaoX != null)
            escalaX = botaoX.localScale;

        if (teclaE != null)
            escalaE = teclaE.localScale;

        if (texto != null)
            texto.text = mensagem;

        if (aviso != null)
            aviso.SetActive(false);
    }

    void Update()
    {
        // Detecta movimento pelo analogico do controle
        if (Gamepad.current != null &&
            Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.25f)
        {
            if (!usandoControle)
            {
                usandoControle = true;
                AtualizarIcones();
            }
        }

        if (!jogadorPerto || aviso == null)
            return;

        float pulso = 1f +
            Mathf.Sin(Time.time * velocidadePulso)
            * intensidadePulso;

        Transform icone = usandoControle ? botaoX : teclaE;

        if (icone != null)
        {
            Vector3 escala = usandoControle ? escalaX : escalaE;
            icone.localScale = escala * pulso;
        }
    }

    void AtualizarIcones()
    {
        if (!jogadorPerto || aviso == null)
            return;

        if (botaoX != null)
        {
            botaoX.gameObject.SetActive(usandoControle);
            botaoX.localScale = escalaX;
        }

        if (teclaE != null)
        {
            teclaE.gameObject.SetActive(!usandoControle);
            teclaE.localScale = escalaE;
        }
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (!EhJogador(outro)) return;

        jogadorPerto = true;

        if (aviso != null)
            aviso.SetActive(true);

        AtualizarIcones();
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        if (!EhJogador(outro)) return;

        jogadorPerto = false;

        if (aviso != null)
            aviso.SetActive(false);

        if (botaoX != null)
            botaoX.localScale = escalaX;

        if (teclaE != null)
            teclaE.localScale = escalaE;
    }

    bool EhJogador(Collider2D outro)
    {
        return outro != null &&
            (outro.CompareTag("Player") ||
             outro.GetComponentInParent<GrupoJogador>() != null);
    }
}
