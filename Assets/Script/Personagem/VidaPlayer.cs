using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VidaPlayer : MonoBehaviour
{
    [SerializeField] private Slider _barraVida;
    [SerializeField] private TMP_Text _textoVida;

    public const int VidaMaxima = 100;
    private int _vidaAtual = VidaMaxima;
    public int VidaAtual => _vidaAtual;

    private void Start()
    {
        if (_barraVida != null)
        {
            _barraVida.minValue = 0;
            _barraVida.maxValue = VidaMaxima;
            _barraVida.wholeNumbers = true;
            _barraVida.interactable = false;
            _barraVida.transition = Selectable.Transition.None;
            _barraVida.navigation = new Navigation { mode = Navigation.Mode.None };
            _barraVida.direction = Slider.Direction.LeftToRight;
        }

        AtualizarInterface();
    }

    public void ReceberDano(int quantidade)
    {
        if (quantidade <= 0) return;

        _vidaAtual -= Mathf.Min(quantidade, _vidaAtual);
        AtualizarInterface();
    }

    public void Curar(int quantidade)
    {
        if (quantidade <= 0) return;

        _vidaAtual += Mathf.Min(quantidade, VidaMaxima - _vidaAtual);
        AtualizarInterface();
    }

    private void AtualizarInterface()
    {
        if (_barraVida != null)
            _barraVida.value = _vidaAtual;

        if (_textoVida != null)
            _textoVida.text = $"{_vidaAtual} / {VidaMaxima}";
    }

    [ContextMenu("Testar dano de 10 (Play Mode)")]
    private void TestarDano()
    {
        if (Application.isPlaying) ReceberDano(10);
    }

    [ContextMenu("Testar cura de 10 (Play Mode)")]
    private void TestarCura()
    {
        if (Application.isPlaying) Curar(10);
    }
}
