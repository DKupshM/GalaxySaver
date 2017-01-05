using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


[System.Serializable]
public class Movement
{
	public float sensH = 10;
	public float smooth = 0.5f;
	
	public bool isDynamic;
	public float horizontal;
}

[RequireComponent (typeof(Rigidbody2D), typeof(Animator))]
public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;
	[HideInInspector]
	public bool jump = false;

	[Header ("Objects")]
	public GameObject healthBar;
	public RectTransform attackUi;

	[Header ("Tilt Controls")]
	public Movement move;

	[Header ("Movement")]
	public float moveForce = 365f;
	public float maxSpeed = 5f;
	public float jumpForce = 1000f;
	public float leftPosition;
	public float rightPosition;

	[Header ("Health")]
	public float maxHealth = 100;

	[Header ("Attack Modifiers")]
	public float cooldownTimer = 1f;

	[Header ("Audio")]
	public AudioClip[] jumpClips;
	public AudioClip[] attackClips;

	[HideInInspector]
	public float currentHealth;

	private Vector3 zeroAc;
	private Vector3 curAc;
	private AttackCollider weaponCollider;
	private Transform groundCheck;
	private Rigidbody2D rb;
	private bool grounded = false;
	private Animator bodyAnim;
	private Animator handAnim;

	private float cooldown;
	private Vector2 lastPos;

	void Start ()
	{
		currentHealth = maxHealth;
		groundCheck = transform.Find ("GroundCheck");
		weaponCollider = transform.Find ("WeaponCollider").GetComponent<AttackCollider> ();
		bodyAnim = GetComponent<Animator> ();
		handAnim = transform.GetChild (0).GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		ResetAxes ();
		lastPos = rb.position;
	}

	void Update ()
	{
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		if (currentHealth <= 0) {
			SceneManager.LoadScene (0);
		}
		healthBar.transform.localScale = new Vector3 (currentHealth / maxHealth, 1, 1);
		if (cooldown > 0) {
			cooldown -= Time.deltaTime;
		}
		if (Input.GetButtonDown ("Jump")) {
			if (SystemInfo.deviceType == DeviceType.Desktop) {
				if (!RectTransformUtility.RectangleContainsScreenPoint (attackUi, Input.mousePosition)) {
					if (grounded) {
						jump = true;
					}
				}
			} else {
				foreach (Touch touch in Input.touches) {
					if (!RectTransformUtility.RectangleContainsScreenPoint (attackUi, touch.position)) {
						if (grounded) {
							jump = true;
						}
					}
				}
			}
		}
	}


	void FixedUpdate ()
	{
		if (rb.position.x == lastPos.x) {
			bodyAnim.SetBool ("Moving", false);
		} else {
			bodyAnim.SetBool ("Moving", true);
		}
		lastPos = rb.position;
		float h = 0;
		if (SystemInfo.deviceType == DeviceType.Handheld) {
			curAc = Vector3.Lerp (curAc, Input.acceleration - zeroAc, Time.deltaTime / move.smooth);
			h = Mathf.Clamp (curAc.x * move.sensH, -1, 1);
		} else {
			h = Input.GetAxis ("Horizontal");
		}
		if (h * rb.velocity.x < maxSpeed) {
			rb.AddForce (Vector2.right * h * moveForce);
		}
		if (Mathf.Abs (rb.velocity.x) > maxSpeed) {
			rb.velocity = new Vector2 (Mathf.Sign (rb.velocity.x) * maxSpeed, rb.velocity.y);
		}
		if (h > 0 && !facingRight) {
			Flip ();
		} else if (h < 0 && facingRight)
			Flip ();
		if (jump) {
			int i = Random.Range (0, jumpClips.Length);
			AudioSource.PlayClipAtPoint (jumpClips [i], transform.position);
			GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f, jumpForce));
			jump = false;
		}
		if (rb.position.x > rightPosition - 2) {
			SceneManager.LoadScene (2);
		}
		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, leftPosition, rightPosition),
			rb.position.y,
			0
		);
		bodyAnim.SetFloat ("Speed", Mathf.Abs (h));

	}

	void Flip ()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void ResetAxes ()
	{
		curAc = Vector3.zero;
		if (move.isDynamic) {
			zeroAc = Input.acceleration;
		} else {
			zeroAc = new Vector3 (move.horizontal, 0, 0);
		}
		
	}

	public void Attack ()
	{
		StartCoroutine (AttackLoop ());
	}

	public IEnumerator AttackLoop ()
	{
		handAnim.Play ("Attack");
		if (attackClips.Length > 0) {
			int i = Random.Range (0, attackClips.Length);
			AudioSource.PlayClipAtPoint (attackClips [i], transform.position);
		}
		weaponCollider.isAttacking = true;
		while (handAnim.GetCurrentAnimatorStateInfo (0).IsName ("Attack")) {
			yield return null;
		}
		weaponCollider.isAttacking = false;
	}
	//
	public void Damage (float amount)
	{
		if (cooldown <= 0) {
			currentHealth -= amount;
			cooldown += cooldownTimer;
		}
	}
}