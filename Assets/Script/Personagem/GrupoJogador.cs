using UnityEngine;
using UnityEngine.SceneManagement;

public class GrupoJogador : MonoBehaviour
{
    [SerializeField] Rigidbody2D _personagem;

    public Transform Personagem => _personagem.transform;

    public static GrupoJogador EmTransicao { get; private set; }

    public void TrocarCena(string cena)
    {
        if (EmTransicao != null) return;

        if (string.IsNullOrWhiteSpace(cena) ||
            !Application.CanStreamedLevelBeLoaded(cena))
        {
            Debug.LogError("Confira a cena de destino da porta.", this);
            return;
        }

        EmTransicao = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(cena);
    }

    public void Posicionar(Vector3 posicao)
    {
        _personagem.transform.position = posicao;
        _personagem.position = new Vector2(posicao.x, posicao.y);
        _personagem.linearVelocity = Vector2.zero;
        _personagem.angularVelocity = 0f;
    }

    public void ConcluirTransicao(Scene cena)
    {
        SceneManager.MoveGameObjectToScene(gameObject, cena);
        EmTransicao = null;
    }

    void OnDestroy()
    {
        if (EmTransicao == this)
            EmTransicao = null;
    }
}