using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class AtaqueInimigo : MonoBehaviour
{
    [Header("Vida e ataque")]
    [SerializeField, Min(1)] private int _vidaMaxima = 40;
    [SerializeField, Min(1)] private int _dano = 10;
    [SerializeField, Min(0.1f)] private float _intervaloAtaques = 1f;
    [Tooltip("Distancia entre as bordas dos colliders para atacar.")]
    [SerializeField, Min(0f)] private float _alcanceAtaque = 0.15f;
    [SerializeField] private int _vidaAtual;

    [Header("Perseguicao")]
    [SerializeField, Min(0f)] private float _velocidade = 2.5f;
    [SerializeField, Min(0.1f)] private float _distanciaPerseguicao = 8f;
    [Tooltip("Opcional: paredes/chao que bloqueiam ataques. Nao inclua Player ou Inimigos.")]
    [SerializeField] private LayerMask _camadaObstaculos;
    [SerializeField] private LayerMask _camadaLimite;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private bool _spriteOriginalOlhaDireita = true;

    private Rigidbody2D _rig;
    private BoxCollider2D _corpo;
    private CombatePlayer _alvo;
    private Collider2D _corpoAlvo;
    private float _proximaBusca;
    private float _proximoAtaque;
    private bool _morta;
    private readonly Dictionary<Collider2D, float> _limites = new Dictionary<Collider2D, float>();

    public int VidaAtual => _vidaAtual;

    private void Awake()
    {
        _rig = GetComponent<Rigidbody2D>();
        _corpo = GetComponent<BoxCollider2D>();
        if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
        _rig.freezeRotation = true;
        _vidaAtual = Mathf.Max(1, _vidaMaxima);
    }

    private void FixedUpdate()
    {
        if (_morta) return;

        // A personagem pode ser criada pelo ChegadaIgreja depois da cobra.
        if (_alvo == null && Time.time >= _proximaBusca)
        {
            _proximaBusca = Time.time + 0.5f;
            _alvo = FindFirstObjectByType<CombatePlayer>();
            if (_alvo != null)
            {
                _corpoAlvo = _alvo.GetComponent<Collider2D>();
                // Prefira o collider fisico, nunca um sensor de chao/itens.
                foreach (Collider2D col in _alvo.GetComponents<Collider2D>())
                    if (col.enabled && !col.isTrigger) { _corpoAlvo = col; break; }
            }
        }

        if (_alvo == null || !_alvo.isActiveAndEnabled || _alvo.EstaMorto ||
            _corpoAlvo == null || !_corpoAlvo.enabled)
        {
            Parar();
            return;
        }

        Vector2 diferenca = (Vector2)_corpoAlvo.bounds.center - (Vector2)_corpo.bounds.center;
        if (diferenca.sqrMagnitude > _distanciaPerseguicao * _distanciaPerseguicao)
        {
            Parar();
            return;
        }

        if (_sprite != null && Mathf.Abs(diferenca.x) > 0.02f)
        {
            bool direita = diferenca.x > 0f;
            bool transformInvertido = transform.right.x * transform.lossyScale.x < 0f;
            _sprite.flipX = (direita != _spriteOriginalOlhaDireita) ^ transformInvertido;
        }

        ColliderDistance2D distancia = _corpo.Distance(_corpoAlvo);
        bool noAlcance = distancia.isValid && distancia.distance <= _alcanceAtaque;
        bool bloqueado = Physics2D.Linecast(_corpo.bounds.center, _corpoAlvo.bounds.center,
            _camadaObstaculos).collider != null;

        if (noAlcance)
        {
            Parar();
            if (!bloqueado && Time.time >= _proximoAtaque)
            {
                _proximoAtaque = Time.time + Mathf.Max(0.1f, _intervaloAtaques);
                _alvo.ReceberDano(_dano);
            }
            return;
        }

        float direcao = Mathf.Abs(diferenca.x) > 0.05f ? Mathf.Sign(diferenca.x) : 0f;
        if (TemLimite(direcao))
        {
            Parar();
            return;
        }

        _rig.linearVelocity = new Vector2(direcao * _velocidade, _rig.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D outro)
    {
        RegistrarLimite(outro);
    }

    private void OnTriggerStay2D(Collider2D outro)
    {
        RegistrarLimite(outro);
    }

    private void RegistrarLimite(Collider2D outro)
    {
        if (!isActiveAndEnabled || _morta || !outro.isTrigger ||
            (_camadaLimite.value & (1 << outro.gameObject.layer)) == 0 ||
            _limites.ContainsKey(outro)) return;

        // Guarda o lado de entrada para permitir recuar, sem atravessar o limite.
        float lado = Mathf.Abs(_rig.linearVelocity.x) > 0.01f
            ? _rig.linearVelocity.x : outro.bounds.center.x - _corpo.bounds.center.x;
        float direcao = Mathf.Abs(lado) > 0.01f ? Mathf.Sign(lado) : 0f;
        _limites.Add(outro, direcao);
        if (direcao == 0f || _rig.linearVelocity.x * direcao > 0f) Parar();
    }

    private bool TemLimite(float direcao)
    {
        foreach (var limite in _limites)
        {
            Collider2D trigger = limite.Key;
            if (trigger == null || !trigger.enabled || !trigger.gameObject.activeInHierarchy ||
                !trigger.isTrigger || (_camadaLimite.value & (1 << trigger.gameObject.layer)) == 0) continue;
            if (limite.Value == 0f || direcao * limite.Value > 0f) return true;
        }
        return false;
    }

    private void OnTriggerExit2D(Collider2D outro)
    {
        _limites.Remove(outro);
    }

    private void OnDisable()
    {
        _limites.Clear();
    }

    public void ReceberDano(int quantidade)
    {
        if (_morta || quantidade <= 0) return;
        _vidaAtual -= Mathf.Min(quantidade, _vidaAtual);
        if (_vidaAtual > 0) return;

        _morta = true;
        Parar();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void Parar()
    {
        _rig.linearVelocity = new Vector2(0f, _rig.linearVelocity.y);
    }
}