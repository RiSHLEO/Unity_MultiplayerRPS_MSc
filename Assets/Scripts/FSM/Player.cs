using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using System.Collections;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("Form Data")]
    public FormData CurrentForm { get; private set; }
    private int _currentFormIndex;
    private FormDatabase _formDatabase;
    [Header("State Machine")]
    private PlayerStateMachine _stateMachine;
    public PlayerIdleState IdleState {  get; private set; }
    public PlayerMoveState MoveState {  get; private set; }
    [Header("Movement")]
    private PlayerInputSet _input;
    public Rigidbody2D rb {  get; private set; }
    public Vector2 MoveInput { get; private set; }
    public float MoveSpeed {  get; private set; }

    private XPManager _xpManager;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _cooldown = 4f;
    private bool _mutationReady = true;

    private void Awake()
    {
        Debug.developerConsoleVisible = true;

        _xpManager = XPManager.Instance;

        rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _formDatabase = FindFirstObjectByType<NetworkManager>().FormDatabase;
        _input = new PlayerInputSet();
        _stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, _stateMachine, "idle");
        MoveState = new PlayerMoveState(this, _stateMachine, "move");
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Movement.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _input.Player.Movement.canceled += ctx => MoveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Start()
    {
        if (!photonView.IsMine)
            return;

        _stateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            _stateMachine.UpdateActiveState();
            OnPressMutate();
        }
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if(photonView.IsMine)
            rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

    [PunRPC]
    public void AssignInitialForm(int formIndex)
    {
        FormData form = _formDatabase.allForms[formIndex];
        ApplyForm(form, formIndex);
    }

    public void ApplyForm(FormData form, int formIndex)
    {
        CurrentForm = form;
        _currentFormIndex = formIndex;
        MoveSpeed = CurrentForm.MoveSpeed;
        _spriteRenderer.color = CurrentForm.FormColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!photonView.IsMine) return;

        Player otherPlayer = collision.GetComponent<Player>();
        if(otherPlayer == null) return;

        if(Beats(_currentFormIndex, otherPlayer._currentFormIndex))
        {
            PhotonNetwork.LocalPlayer.AddScore(10);
            _xpManager.AddEnergy(20);
            otherPlayer.photonView.RPC(nameof(MutateToForm), RpcTarget.AllBuffered, _currentFormIndex);
            otherPlayer.photonView.RPC(nameof(LoseEnergy), otherPlayer.photonView.Owner, 10);
        }
    }

    bool Beats(int myIndex, int theirIndex)
    {
        // Stone(0) beats Blade(2)
        // Blade(2) beats Veil(1)
        // Veil(1) beats Stone(0)

        return (myIndex == 0 && theirIndex == 2)
            || (myIndex == 2 && theirIndex == 1)
            || (myIndex == 1 && theirIndex == 0);
    }

    private void OnPressMutate()
    {
        if(_xpManager.currentEnergy == 0 || _mutationReady == false) return;

        int formIndex = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            formIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            formIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            formIndex = 2;

        if (formIndex != -1)
        {
            _xpManager.SpendEnergy(10);
            photonView.RPC(nameof(MutateToForm), RpcTarget.AllBuffered, formIndex);
            StartCoroutine(AbilityCooldownRoutine(_cooldown));
        }
    }

    [PunRPC]
    private void MutateToForm(int currentFormIndex)
    {
        if (_currentFormIndex == currentFormIndex) return;
        FormData form = _formDatabase.allForms[currentFormIndex];
        ApplyForm(form, currentFormIndex);
    }

    [PunRPC]
    private void LoseEnergy(int amount)
    {
        _xpManager.SpendEnergy(amount);
    }

    IEnumerator AbilityCooldownRoutine(float cooldown)
    {
        _mutationReady = false;
        yield return new WaitForSeconds(cooldown);
        _mutationReady = true;
    }
}