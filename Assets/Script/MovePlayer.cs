using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] float _velocidadeX = 5;

    [SerializeField] Vector2 _posicao;

    [SerializeField] Rigidbody2D _rig2d;

    [SerializeField] bool _checkGround;

    [SerializeField] bool _isface;

    //Adicionados Recentemente

    [SerializeField] float _velocidadeCorrida = 8f;

    [SerializeField] Animator _animator;

    bool _correndo;

    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _rig2d.angularVelocity = 0;

        // Linha nova
        _animator = GetComponent<Animator>();
    }

    public void SetMove(InputAction.CallbackContext value)
    {
        _posicao = value.ReadValue<Vector2>();
    }

    public void SetRun(InputAction.CallbackContext value)
    {
        _correndo = value.ReadValueAsButton();
    }

    public void SetJump(InputAction.CallbackContext value)
    {
        if (_checkGround)
        {
            _rig2d.AddForceY(150);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _checkGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _checkGround = false;
        }
    }

    private void Flip()
    {
        _isface = !_isface;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

    }

    void Update()
    {
        // Adicionado recentemente
        _rig2d.linearVelocityX = _posicao.x * (_correndo ? _velocidadeCorrida : _velocidadeX);
        _animator.SetBool("Andando", _posicao.x != 0);
        _animator.SetBool("Correndo", _posicao.x != 0 && _correndo);
        _animator.SetBool("Pulando", !_checkGround);


        if (_posicao.x < 0 && _isface == false)
        {
            Flip();
        }
        else if (_posicao.x > 0 && _isface == true)
        {
            Flip();
        }
    }
}

