using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class Minerio : MonoBehaviour
{
    [SerializeField, Min(1)] int _golpesParaQuebrar = 3;
    [SerializeField] GameObject _MinerioPrefab;
    [SerializeField] Transform _pontoDrop;

    int _golpesRestantes;
    bool _quebrado;

    void Awake()
    {
        _golpesRestantes = Mathf.Max(1, _golpesParaQuebrar);

        if (_MinerioPrefab != null) return;
        Debug.LogError("Preencha MinerioPrefab com o prefab do Minerio coletavel.", this);
        enabled = false;
    }

    public void ReceberGolpe()
    {
        if (_quebrado || !isActiveAndEnabled) return;
        _golpesRestantes--;
        if (_golpesRestantes > 0) return;

        _quebrado = true;
        Vector3 posicao = _pontoDrop != null ? _pontoDrop.position : transform.position;
        gameObject.SetActive(false);
        Instantiate(_MinerioPrefab, posicao, Quaternion.identity);
        Destroy(gameObject);
    }
}