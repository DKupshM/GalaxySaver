using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	[Header ("Health Bar")]
	public float cooldownTimer = 1f;
	public float maxHealth = 100;
	public GameObject healthBar;

	[Header ("Positions Relative to Player")]
	public float closePosition = 2.5f;
	public float farPosition = 5f;

	[Header ("Attack")]
	public float attackCooldownTimerMin = 1f;
	public float attackCooldownTimerMax = 4f;

	[Header ("Speed")]
	public float xSpeed = 1.5f;
	public float ySpeed = 3;


	[HideInInspector]
	public bool facingRight = true;

	private float cooldown;
	private float attackCooldown;
	private Vector2 lastPos;
	private GameObject player;
	private AttackCollider weaponCollider;
	//	private Transform groundCheck;
	private Rigidbody2D rb;
	//	private bool grounded = false;
	private Animator bodyAnim;
	private Animator handAnim;
	private float currentHealth;

	void Start ()
	{
		currentHealth = maxHealth;
		//groundCheck = transform.Find ("GroundCheck");
		weaponCollider = transform.Find ("WeaponCollider").GetComponent<AttackCollider> ();
		bodyAnim = GetComponent<Animator> ();
		handAnim = transform.GetChild (0).GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		player = GameObject.FindWithTag ("Player");
		lastPos = rb.position;
	}

	void Update ()
	{
		if (currentHealth <= 0) {
			Destroy (gameObject);
		}
		if (cooldown > 0) {
			cooldown -= Time.deltaTime;
		}
		if (attackCooldown > 0) {
			attackCooldown -= Time.deltaTime;
		}
		healthBar.transform.localScale = new Vector3 (currentHealth / maxHealth, 1, 1);
	}

	void FixedUpdate ()
	{
		if (rb.position.x == lastPos.x) {
			bodyAnim.SetBool ("Moving", false);
		} else {
			bodyAnim.SetBool ("Moving", true);
		}
		lastPos = rb.position;

		if (transform.position.x - player.transform.position.x > 0) {
			if (!facingRight) {
				Flip ();
			}
		} else {
			if (facingRight) {
				Flip ();
			}
		}
		float distance = Mathf.Abs (transform.position.x - player.transform.position.x);
		if (distance > closePosition && distance < farPosition) {
			//within min and far distance
			Vector2 velocity = new Vector2 ((transform.position.x - player.transform.position.x) * xSpeed, (transform.position.y - player.transform.position.y) * ySpeed);
			rb.velocity = -velocity;
	
		} else if (distance < farPosition) {
			//close to player
			if (attackCooldown <= 0) {
				attackCooldown = Random.Range (attackCooldownTimerMin, attackCooldownTimerMax);
				Attack ();
			}
		}
	}

	void Flip ()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void Attack ()
	{
		StartCoroutine (AttackLoop ());
	}

	public IEnumerator AttackLoop ()
	{
		handAnim.Play ("Attack");
		weaponCollider.isAttacking = true;
		yield return new WaitForSeconds (.33f);
		weaponCollider.isAttacking = false;
	}

	public void Damage (float amount)
	{
		if (cooldown <= 0) {
			currentHealth -= amount;
			cooldown += cooldownTimer;
		}
	}
}
