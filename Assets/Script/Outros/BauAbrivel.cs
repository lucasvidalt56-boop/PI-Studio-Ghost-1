
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BauAbrivel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animatorBau;
    [SerializeField] private AvisoInteracao avisoInteracao;
    [SerializeField] private Transform pontoSaida;

    [Header("Conteudo do Bau")]
    [SerializeField] private GameObject[] itensDentro;
    [SerializeField] private float tempoAbertura = 0.5f;

    private bool jogadorPerto;
    private bool aberto;

    private void Update()
    {
        if (!jogadorPerto || aberto) return;

        bool apertouE = Keyboard.current != null &&
                        Keyboard.current.eKey.wasPressedThisFrame;

        bool apertouX = Gamepad.current != null &&
                        Gamepad.current.buttonWest.wasPressedThisFrame;

        if (apertouE || apertouX)
            StartCoroutine(AbrirBau());
    }

    private IEnumerator AbrirBau()
    {
        aberto = true;

        animatorBau.SetTrigger("Abrir");

        if (avisoInteracao != null)
        {
            if (avisoInteracao.aviso != null)
                avisoInteracao.aviso.SetActive(false);

            avisoInteracao.enabled = false;
        }

        yield return new WaitForSeconds(tempoAbertura);

        if (pontoSaida == null) yield break;

        for (int i = 0; i < itensDentro.Length; i++)
        {
            if (itensDentro[i] == null) continue;

            Vector3 posicao = pontoSaida.position +
                new Vector3(i * 0.4f, 0, 0);

            GameObject item = Instantiate(itensDentro[i], posicao, Quaternion.identity);

            SaidaItemBau movimento = item.GetComponent<SaidaItemBau>();

            if (movimento != null)
                movimento.Iniciar();
        }
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (EhJogador(outro))
            jogadorPerto = true;
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        if (EhJogador(outro))
            jogadorPerto = false;
    }

    private bool EhJogador(Collider2D outro)
    {
        return outro.CompareTag("Player") ||
               outro.GetComponentInParent<GrupoJogador>() != null;
    }
}
