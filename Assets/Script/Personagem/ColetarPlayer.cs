using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColetarPlayer : MonoBehaviour
{
    [SerializeField] Inventario _inventario;

    [SerializeField] Key _teclaColeta = Key.E;

    [SerializeField, Min(0.1f)] float _distanciaColeta = 1.5f;

    [SerializeField] LayerMask _camadaItens;

    List<Collider2D> _resultados = new List<Collider2D>();

    void Start()
    {
        if (_inventario != null) return;

        Debug.LogError("Preencha Inventario no ColetarPlayer com Canvas/Inventario.", this);
        enabled = false;
    }

    void Update()
    {
        bool teclado = Keyboard.current != null && _teclaColeta != Key.None &&
                       Keyboard.current[_teclaColeta].wasPressedThisFrame;

        bool controle = Gamepad.current != null &&
                        Gamepad.current.buttonWest.wasPressedThisFrame;

        if (teclado || controle)
        {
            Coletar();
        }
    }

    public void Coletar()
    {
        if (_inventario == null) return;

        ContactFilter2D filtro = new ContactFilter2D();
        filtro.SetLayerMask(_camadaItens);
        filtro.useTriggers = true;

        Physics2D.OverlapCircle(transform.position, _distanciaColeta, filtro, _resultados);

        Coletavel maisProximo = null;
        float menorDistancia = _distanciaColeta;

        foreach (Collider2D resultado in _resultados)
        {
            Coletavel item = resultado.GetComponentInParent<Coletavel>();
            if (item == null || !item.isActiveAndEnabled) continue;

            float distancia = Vector2.Distance(transform.position, item.transform.position);
            if (distancia > menorDistancia) continue;

            menorDistancia = distancia;
            maisProximo = item;
        }

        if (maisProximo != null)
        {
            maisProximo.Coletar(_inventario);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _distanciaColeta);
    }
}
