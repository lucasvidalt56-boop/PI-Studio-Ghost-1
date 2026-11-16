using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TemporizadorEfeitos : MonoBehaviour
{
    [SerializeField] TemporizadorCaixa _modelo;

    class Efeito
    {
        public MonoBehaviour origem;
        public TemporizadorCaixa caixa;
        public float inicio;
        public float duracao;
    }

    readonly List<Efeito> _efeitos = new List<Efeito>();

    void Awake()
    {
        if (_modelo != null && _modelo.transform.IsChildOf(transform))
            _modelo.gameObject.SetActive(false);
    }

    public void Mostrar(
        MonoBehaviour origem,
        string nome,
        Sprite icone,
        float inicio,
        float duracao)
    {
        if (!isActiveAndEnabled || origem == null)
            return;

        if (_modelo == null)
        {
            Debug.LogError("Preencha o Modelo no TemporizadorEfeitos.", this);
            return;
        }

        if (!origem.isActiveAndEnabled ||
            duracao <= 0f ||
            float.IsNaN(duracao) ||
            float.IsInfinity(duracao) ||
            float.IsNaN(inicio) ||
            float.IsInfinity(inicio) ||
            Time.time >= inicio + duracao)
        {
            Ocultar(origem);
            return;
        }

        Efeito efeito = _efeitos.Find(e => e.origem == origem);

        if (efeito == null)
        {
            TemporizadorCaixa caixa = Instantiate(_modelo, transform);
            caixa.gameObject.SetActive(true);

            efeito = new Efeito
            {
                origem = origem,
                caixa = caixa
            };

            _efeitos.Add(efeito);
        }

        efeito.inicio = inicio;
        efeito.duracao = duracao;

        efeito.caixa.Configurar(nome, icone);
        efeito.caixa.AtualizarTempo(
            inicio + duracao - Time.time,
            duracao);
    }

    public void Ocultar(MonoBehaviour origem)
    {
        for (int i = _efeitos.Count - 1; i >= 0; i--)
        {
            if (_efeitos[i].origem == origem)
                Remover(i);
        }
    }

    void LateUpdate()
    {
        for (int i = _efeitos.Count - 1; i >= 0; i--)
        {
            Efeito efeito = _efeitos[i];

            if (efeito.origem == null ||
                !efeito.origem.isActiveAndEnabled ||
                efeito.caixa == null)
            {
                Remover(i);
                continue;
            }

            float restante = efeito.inicio + efeito.duracao - Time.time;

            if (restante <= 0f)
            {
                Remover(i);
                continue;
            }

            efeito.caixa.AtualizarTempo(restante, efeito.duracao);
        }
    }

    void Remover(int indice)
    {
        TemporizadorCaixa caixa = _efeitos[indice].caixa;

        if (caixa != null)
        {
            caixa.gameObject.SetActive(false);
            Destroy(caixa.gameObject);
        }

        _efeitos.RemoveAt(indice);
    }

    void OnDisable()
    {
        for (int i = _efeitos.Count - 1; i >= 0; i--)
            Remover(i);
    }
}