using UnityEngine;

[DisallowMultipleComponent]
public class DanoContato : MonoBehaviour
{
    [SerializeField, Min(1)] int _dano = 10;
    [SerializeField, Min(0.1f)] float _intervaloDano = 1f;

    float _proximoDano;

    void OnTriggerEnter2D(Collider2D outro)
    {
        AplicarDano(outro);
    }

    void OnTriggerStay2D(Collider2D outro)
    {
        AplicarDano(outro);
    }

    void AplicarDano(Collider2D outro)
    {
        if (!isActiveAndEnabled || outro.isTrigger || Time.time < _proximoDano) return;

        CombatePlayer jogador = outro.GetComponentInParent<CombatePlayer>();
        if (jogador == null || !jogador.isActiveAndEnabled || jogador.EstaMorto) return;

        _proximoDano = Time.time + Mathf.Max(0.1f, _intervaloDano);
        jogador.ReceberDano(_dano);
    }
}