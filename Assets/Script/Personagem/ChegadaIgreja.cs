using UnityEngine;
using Unity.Cinemachine;

public class ChegadaIgreja : MonoBehaviour
{
    [SerializeField] GrupoJogador _jogadorPrefab;
    [SerializeField] CinemachineCamera _cameraDaFase;

    GrupoJogador _jogadorRecebido;

    void Awake()
    {
        GrupoJogador jogador = GrupoJogador.EmTransicao;

        if (jogador != null)
        {
            _jogadorRecebido = jogador;
            jogador.Posicionar(transform.position);
        }
        else
        {
            jogador = Instantiate(_jogadorPrefab);

            if (SistemaSave.Carregamento == null)
                jogador.Posicionar(transform.position);
        }

        _cameraDaFase.Follow = jogador.Personagem;
        _cameraDaFase.PreviousStateIsValid = false;
    }

    void Start()
    {
        if (_jogadorRecebido != null)
            _jogadorRecebido.ConcluirTransicao(gameObject.scene);
    }
}