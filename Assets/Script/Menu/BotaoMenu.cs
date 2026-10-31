
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

    private void Start()
    {
        AtualizarVisual(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        AtualizarVisual(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        AtualizarVisual(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current == null)
            return;

        mouseSobre = true;
        EventSystem.current.SetSelectedGameObject(gameObject);
        AtualizarVisual(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseSobre = false;

        if (EventSystem.current == null)
        {
            AtualizarVisual(false);
            return;
        }

        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);

        AtualizarVisual(false);
    }

    private void AtualizarVisual(bool selecionado)
    {
        if (ornamento == null || texto == null)
            return;

        ornamento.sprite = selecionado ? amarelo : azul;
        texto.color = selecionado ? corSelecionada : corNormal;
    }
}