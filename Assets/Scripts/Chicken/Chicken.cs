using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Chicken : MonoBehaviour
{
    [SerializeField] float speed = 100f;
    [SerializeField] float runSpeed = 200f;
    [Space, SerializeField] float rotationSpeed = 200f;
    [Space, SerializeField] float jumpForce = 4;
    [SerializeField] float bootsJumpForce = 8;
    [Space] public bool canMove = true;
    [Space, SerializeField] RandomSound chickenSound;
    [SerializeField] RandomSound stepSound;

    Animator anim;
    public Light lantern;


    private Rigidbody _myRB;
    private float _currentSpeed;
    private bool _isMoving;
    private bool _isJumping;

    private Collider _CollisionObj;
    private Quaternion _toRotation;
    private float _randomTime;
    public float _walkStepCooldown = 0.25f;
    public float _runStepCooldown = 0.15f;

    [Header("hit")]
    public ShowTutorial hitTutorial;
    public HitController chickenHit;
    public bool HitEnabled => chickenHit.gameObject.activeInHierarchy;

    [Header("Jump Boots")]
    public ShowTutorial bootsTutorial;
    public bool bootsEnabled = false;

    private float _stepCooldown;
    private float _currentStepCooldown = 0;
    private Vector3 _moveDirection;

    private void OnEnable()
    {
        TimeController.TimeChange += ToggleLantern;
    }

    private void OnDisable()
    {
        TimeController.TimeChange -= ToggleLantern;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        _myRB = GetComponent<Rigidbody>();

        _isMoving = false;
        _isJumping = false;
        _currentSpeed = speed;
        chickenHit.gameObject.SetActive(false);

        StartCoroutine(RandomAnimations());
    }

    IEnumerator RandomAnimations()
    {
        while (true)
        {
            if (!_isMoving)
            {
                _randomTime = Random.Range(4, 20);
                yield return new WaitForSeconds(_randomTime);
                if (!_isMoving)
                {
                    float randomaux = Random.value;
                    if (randomaux >= 0.5f)
                    {
                        anim.SetTrigger("Eat");
                    }
                    else
                    {
                        anim.SetTrigger("TurnHead");
                    }
                    if(chickenSound) chickenSound.PlayRandom();
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _toRotation, rotationSpeed * Time.deltaTime);
        if(_isMoving && _currentStepCooldown <= 0 && !_isJumping){
            if(stepSound)stepSound.PlayRandom();
            _currentStepCooldown = _stepCooldown;
        }else if(_isMoving){
            _currentStepCooldown -= Time.deltaTime;
        }
    }

    public void ToggleLantern()
    {
        bool val = TimeController.IsDay;
        if(val)
            lantern.intensity = 0;
        else
            lantern.intensity = 2;
    }

    private void FixedUpdate()
    {

        Vector3 directionAux = _moveDirection * _currentSpeed * Time.fixedDeltaTime;
        directionAux.y = _myRB.velocity.y;
        _myRB.velocity = directionAux;

    }
    public void OnMove(InputValue value)
    {
        if (value.Get<Vector2>() != Vector2.zero && canMove && !GameManager.onPause)
        {
            _isMoving = true;
            Vector3 forwardProj = (Vector3.Project(Vector3.forward, Camera.main.transform.forward)+ Camera.main.transform.forward).normalized * value.Get<Vector2>().y;
            Vector3 rightProj = (Vector3.Project(Vector3.right, Camera.main.transform.right)+ Camera.main.transform.right).normalized * value.Get<Vector2>().x;

            _moveDirection = forwardProj + rightProj;
            _moveDirection.y = _myRB.velocity.y;

            _toRotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z));

            if(_currentSpeed == speed)
            {
                _stepCooldown = _walkStepCooldown;
                anim.SetBool("Walk", true);
            }
            else
            {
                _stepCooldown = _runStepCooldown;
                anim.SetBool("Run", true);
            }
        }
        else
        {
            _isMoving = false;
            _moveDirection = new Vector3(0, _myRB.velocity.y, 0);
            _stepCooldown = _walkStepCooldown;
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
        }
    }

    public void OnRun(InputValue value)
    {
        if (value.isPressed)
        {
            _currentSpeed = runSpeed;
            if(_isMoving)
            {
                anim.SetBool("Run", true);
            }
        }
        else
        {
            _currentSpeed = speed;
            if (_isMoving)
            {
                anim.SetBool("Run", false);
            }
        }
    }
    public void OnInteract()
    {
        if (!_CollisionObj) return;

        var npc = _CollisionObj.GetComponent<InteractiveNPC>();
        if (chickenSound) chickenSound.PlayRandom();
        if (npc)
        {
            npc.ActivateDialog();
        }
        else
        {
            var item = _CollisionObj.GetComponent<ItemCollectableBase>();
            if (item)
            {
                item.Collect();
            }
            else
            {
                var consume = _CollisionObj.GetComponent<ConsumeItem>();
                if (consume)
                {
                    consume.CheckItem();
                }
            }
        }
    }

    public void OnJump()
    {
        if (!_isJumping && canMove && !GameManager.onPause)
        {
            if (chickenSound) chickenSound.PlayRandom();
            anim.SetTrigger("Jump");
            _isJumping = true;
            _myRB.AddForce(Vector2.up * jumpForce * 100);
        }
    }
    public void OnMenu()
    {
        GameManager.Instance.TogglePause();
    }

    public void OnHit()
    {
        if (!HitEnabled) return;

        anim.SetTrigger("MakeHit");
        if (chickenSound) chickenSound.PlayRandom();

        chickenHit.Hit();
    }

    // Active PowerUps
    public void ActiveHit()
    {
        if(hitTutorial)hitTutorial.Show();
        chickenHit.gameObject.SetActive(true);
    }
    public void ActiveBoot()
    {
        if(bootsTutorial)bootsTutorial.Show();
        bootsEnabled = true;
        jumpForce = bootsJumpForce;
    }
    //--------------------------


    public void ChangeIsJump(bool val)
    {
        _isJumping = val;
    }

    public void ChangeCanMove(bool val)
    {
        canMove = val;
    }


    public void ResetInteractiveCollider()
    {
        _CollisionObj = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            _CollisionObj = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("interactable"))
        {
            ResetInteractiveCollider();
        }
    }
}
