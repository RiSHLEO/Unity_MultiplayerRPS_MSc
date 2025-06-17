using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Player : MonoBehaviourPunCallbacks
{
    [Header("Forms")]
    public FormData CurrentForm { get; private set; }
    private int _currentFormIndex;
    private PlayerInputSet _input;
    private PlayerStateMachine _stateMachine;
    public Rigidbody2D rb {  get; private set; }
    public PlayerIdleState IdleState {  get; private set; }
    public PlayerMoveState MoveState {  get; private set; }
    public Vector2 MoveInput { get; private set; }
    public float MoveSpeed {  get; private set; }

    private FormDatabase _formDatabase;

    private void Awake()
    {
        Debug.developerConsoleVisible = true;

        rb = GetComponent<Rigidbody2D>();
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
            _stateMachine.UpdateActiveState();
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
        GetComponent<SpriteRenderer>().color = CurrentForm.FormColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!photonView.IsMine) return;

        Player otherPlayer = collision.GetComponent<Player>();
        if(otherPlayer == null) return;

        if(Beats(_currentFormIndex, otherPlayer._currentFormIndex))
        {
            PhotonNetwork.LocalPlayer.AddScore(10);
            otherPlayer.photonView.RPC(nameof(MutateToForm), RpcTarget.AllBuffered, _currentFormIndex);
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

    [PunRPC]
    private void MutateToForm(int currentFormIndex)
    {
        FormData form = _formDatabase.allForms[currentFormIndex];
        ApplyForm(form, currentFormIndex);
    }
}