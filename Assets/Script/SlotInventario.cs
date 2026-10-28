using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image), typeof(Button))]
public class SlotInventario : MonoBehaviour
{
    [SerializeField] Image _layout;

    [SerializeField] Image _selecao;

    [SerializeField] Image _icone;

    [SerializeField] TMP_Text _quantidade;

    void Awake()
    {
        _layout = GetComponent<Image>();
        _selecao.enabled = false;

        if (_quantidade != null)
        {
            _quantidade.outlineColor = Color.black;
            _quantidade.outlineWidth = 0.2f;
            _quantidade.raycastTarget = false;
        }

        SetItem(null);
    }

    public void SetLayout(Sprite layout, Sprite selecionado)
    {
        _layout.sprite = layout;
        _selecao.sprite = selecionado;
    }

    public void SetSelecionado(bool selecionado)
    {
        _selecao.enabled = selecionado;
    }

    public void SetItem(Sprite sprite, int quantidade = 1)
    {
        _icone.sprite = sprite;
        _icone.enabled = sprite != null;
        SetQuantidade(sprite != null ? quantidade : 0);
    }

    public void SetQuantidade(int quantidade)
    {
        if (_quantidade == null) return;

        _quantidade.text = quantidade > 1 ? quantidade.ToString() : "";
    }
}
