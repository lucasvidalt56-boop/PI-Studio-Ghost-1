using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[AddComponentMenu("Jogo/Ponto de Renascimento")]
public class PontoRenascimento : MonoBehaviour
{
    [Tooltip("Vida da personagem ao retornar a este ponto.")]
    [SerializeField, Range(1, VidaPlayer.VidaMaxima)] int _vidaAoRenascer = VidaPlayer.VidaMaxima;

    public int VidaAoRenascer => Mathf.Clamp(_vidaAoRenascer, 1, VidaPlayer.VidaMaxima);

    public static PontoRenascimento Buscar(Scene cena)
    {
        foreach (var ponto in FindObjectsByType<PontoRenascimento>(FindObjectsSortMode.None))
            if (ponto.isActiveAndEnabled && ponto.gameObject.scene == cena)
                return ponto;

        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
