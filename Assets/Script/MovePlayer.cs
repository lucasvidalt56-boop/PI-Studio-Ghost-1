using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] float _velocidadeX = 5;

    [SerializeField] Vector2 _posicao;

    [SerializeField] Rigidbody2D _rig2d;

    [SerializeField] bool _checkGround;

    [SerializeField] bool _isface;

    [SerializeField] float _velocidadeCorrida = 8f;

    [SerializeField] Animator _animator;

    [SerializeField] float _forcaPulo = 150f;

    bool _correndo;

    
    bool _correndoNoAr;

    void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        _rig2d.angularVelocity = 0;
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
        if (value.performed && _checkGround)
        {
            _correndoNoAr = _correndo;
            _checkGround = false;
            _rig2d.AddForceY(_forcaPulo);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && _rig2d.linearVelocityY <= 0.01f)
        {
            _checkGround = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && _rig2d.linearVelocityY <= 0.01f)
            _checkGround = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (_checkGround)
            {
                _correndoNoAr = _correndo;
            }
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
        
        if (_checkGround)
        {
            _correndoNoAr = _correndo;
        }

        bool usarCorrida = _checkGround ? _correndo : _correndoNoAr;
        _rig2d.linearVelocityX = _posicao.x * (usarCorrida ? _velocidadeCorrida : _velocidadeX);
        _animator.SetBool("Andando", _posicao.x != 0);
        _animator.SetBool("Correndo", _checkGround && _posicao.x != 0 && usarCorrida);
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
