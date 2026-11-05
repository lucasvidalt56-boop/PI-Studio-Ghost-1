using UnityEngine;

public class PortaCena : MonoBehaviour
{
    [SerializeField] string _cenaDestino;

    public void Entrar(GrupoJogador jogador)
    {
        jogador.TrocarCena(_cenaDestino);
    }
}