using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class TemporizadorCaixa : MonoBehaviour
{
    [SerializeField] TMP_Text _nome;
    [SerializeField] TMP_Text _segundos;
    [SerializeField] Image _icone;
    [SerializeField] RectTransform _preenchimento;

    int _ultimoSegundo = -1;

    public void Configurar(string nome, Sprite icone)
    {
        if (_nome != null && _nome.text != nome)
            _nome.text = nome;

        if (_icone != null)
        {
            _icone.sprite = icone;
            _icone.enabled = icone != null;
        }
    }

    public void AtualizarTempo(float restante, float duracao)
    {
        restante = Mathf.Max(0f, restante);

        int segundos = Mathf.CeilToInt(restante);

        if (_segundos != null && segundos != _ultimoSegundo)
        {
            _ultimoSegundo = segundos;
            _segundos.text = segundos + "s";
        }

        if (_preenchimento != null)
        {
            float proporcao = duracao > 0f
                ? Mathf.Clamp01(restante / duracao)
                : 0f;

            Vector2 ancora = _preenchimento.anchorMax;
            ancora.x = proporcao;
            _preenchimento.anchorMax = ancora;
        }
    }
}