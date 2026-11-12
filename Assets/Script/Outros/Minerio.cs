using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class Minerio : MonoBehaviour
{
    [SerializeField, Min(1)] int _golpesParaQuebrar = 3;
    [SerializeField] GameObject _pedraPrefab;
    [SerializeField] Transform _pontoDrop;

    int _golpesRestantes;
    bool _quebrado;

    void Awake()
    {
        _golpesRestantes = Mathf.Max(1, _golpesParaQuebrar);
    }

    public void ReceberGolpe()
    {
        if (_quebrado || !isActiveAndEnabled) return;
        _golpesRestantes--;
        if (_golpesRestantes > 0) return;

        _quebrado = true;
        Vector3 posicao = _pontoDrop != null ? _pontoDrop.position : transform.position;
        gameObject.SetActive(false);
        if (_pedraPrefab != null)
        {
            Instantiate(_pedraPrefab, posicao, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}