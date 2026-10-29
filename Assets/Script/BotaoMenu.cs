using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BotaoMenu : MonoBehaviour, ISelectHandler, IDeselectHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referências")]
    [SerializeField] private Image ornamento;
    [SerializeField] private TMP_Text texto;

    [Header("Sprites")]
    [SerializeField] private Sprite azul;
    [SerializeField] private Sprite amarelo;

    [Header("Cores")]
    [SerializeField] private Color corNormal = new Color32(133, 134, 155, 255);
    [SerializeField] private Color corSelecionada = new Color32(242, 180, 90, 255);

    private bool mouseSobre;
    private bool selecionadoPorNavegacao;

    private void Start()
    {
        AtualizarVisual();
    }

    public void OnSelect(BaseEventData eventData)
    {
        selecionadoPorNavegacao = true;
        AtualizarVisual();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selecionadoPorNavegacao = false;
        AtualizarVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseSobre = true;
        AtualizarVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseSobre = false;
        AtualizarVisual();
    }

    private void AtualizarVisual()
    {
        bool selecionado = mouseSobre || selecionadoPorNavegacao;

        ornamento.sprite = selecionado ? amarelo : azul;
        texto.color = selecionado ? corSelecionada : corNormal;
    }
}
