using System.Collections;
using UnityEngine;

public class PortaCena : MonoBehaviour
{
    [SerializeField] string _cenaDestino;
    [Tooltip("Espera apenas quando esta porta aciona um objetivo, para dar tempo de ver a marcacao.")]
    [SerializeField, Min(0f)] float _tempoMostrarObjetivo = 0.8f;

    bool _entrando;

    public void Entrar(GrupoJogador jogador)
    {
        if (_entrando || jogador == null || !isActiveAndEnabled) return;
        if (string.IsNullOrWhiteSpace(_cenaDestino) ||
            !Application.CanStreamedLevelBeLoaded(_cenaDestino))
        {
            Debug.LogError("PortaCena: confira a Cena Destino e se ela esta incluida no Build Profiles.", this);
            return;
        }
        _entrando = true;
        StartCoroutine(EntrarNaCena(jogador));
    }

    private IEnumerator EntrarNaCena(GrupoJogador jogador)
    {
        GatilhoObjetivo objetivo = GetComponent<GatilhoObjetivo>();
        if (objetivo != null)
        {
            objetivo.ConfirmarEntradaPorta();
            if (objetivo.ObjetivoAcionado && _tempoMostrarObjetivo > 0f)
                yield return new WaitForSecondsRealtime(_tempoMostrarObjetivo);
        }
        if (jogador != null) jogador.TrocarCena(_cenaDestino);
    }
}
