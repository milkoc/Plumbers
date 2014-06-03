using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	
	
	public float moveForce;				// Amount of force added to move the player left and right.
	public float maxSpeed;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce; 			// Amount of force added when the player jumps.
	public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.
	
	public float gravityForce;
	
	private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.
	
	
	// zmienne do synchronizacji
	public double m_InterpolationBackTime = 0.10;
	public double m_ExtrapolationLimit = 0.5;
	public bool jumped = false; //used to serialize
	
	internal struct  State
	{
		internal double timestamp;
		internal Vector3 position;
		internal Vector3 velocity;
		internal bool jumped;
	}
	
	// We store twenty states with "playback" information
	State[] m_BufferedState = new State[60];
	// Keep track of what slots are used
	int m_TimestampCount;
	
	
	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
	}
	
	
	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));  
		
		if (networkView.isMine) {
			
			// If the jump button is pressed and the player is grounded then the player should jump.
			if (Input.GetButtonDown ("Jump") && grounded)
			{
				jump = true;
				jumped = true;
			}
		}
		
	}
	
	
	void FixedUpdate ()
	{
		if (networkView.isMine) {
			// Cache the horizontal input.
			float h = Input.GetAxis ("Horizontal");
			//grawitacja
			rigidbody2D.AddForce ( -1.0f * Vector2.up * gravityForce);
			
			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat ("Speed", Mathf.Abs (h));
			
			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if (h * rigidbody2D.velocity.x < maxSpeed)
				// ... add a force to the player.
				rigidbody2D.AddForce (Vector2.right * h * moveForce);
			
			// If the player's horizontal velocity is greater than the maxSpeed...
			if (Mathf.Abs (rigidbody2D.velocity.x) >= maxSpeed)
				// ... set the player's velocity to the maxSpeed in the x axis.
				rigidbody2D.velocity = new Vector2 (Mathf.Sign (rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
			
			// If the input is moving the player right and the player is facing left...
			if (h > 0 && !facingRight)
				// ... flip the player.
				Flip ();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (h < 0 && facingRight)
				// ... flip the player.
				Flip ();
			
			// If the player should jump...
			if (jump) {
				// Set the Jump animator trigger parameter.
				anim.SetTrigger ("Jump");
				
				// Play a random jump audio clip.
				int i = Random.Range (0, jumpClips.Length);
				AudioSource.PlayClipAtPoint (jumpClips [i], transform.position);
				
				// Add a vertical force to the player.
				rigidbody2D.AddForce (new Vector2 (0f, jumpForce));
				
				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				jump = false;
			}
		} 
		
		
		else { //gdy jest to cudzy obiekt
			// This is the target playback time of the rigid body
			double interpolationTime = Network.time - m_InterpolationBackTime;
			
			// Use interpolation if the target playback time is present in the buffer
			if (m_BufferedState[0].timestamp > interpolationTime)
			{
				// Go through buffer and find correct state to play back
				for (int i=0;i<m_TimestampCount;i++)
				{
					if (m_BufferedState[i].timestamp <= interpolationTime || i == 
					    m_TimestampCount-1)
					{
						// The state one slot newer (<100ms) than the best playback state
						State rhs = m_BufferedState[Mathf.Max(i-1, 0)];
						// The best playback state (closest to 100 ms old (default time))
						State lhs = m_BufferedState[i];
						
						// Use the time between the two slots to determine if interpolation is necessary
						double length = rhs.timestamp - lhs.timestamp;
						float t = 0.0F;
						// As the time difference gets closer to 100 ms t gets closer to 1 in 
						// which case rhs is only used
						// Example:
						// Time is 10.000, so sampleTime is 9.900 
						// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
						// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
						if (length > 0.0001)
							t = (float)((interpolationTime - lhs.timestamp) / length);						
						// if t=0 => lhs is used directly
						
						
						
						// ---INTERPOLACJA---
						transform.position = Vector3.Lerp(lhs.position, rhs.position, t);
						rigidbody2D.velocity = Vector2.Lerp(lhs.velocity, rhs.velocity, t);
						
						
						if(rhs.jumped == true)
						{
							jump = true;
							m_BufferedState[Mathf.Max(i-1, 0)].jumped = false;
						}
						
						break;
					}
				}
			}
			
			
			// Use extrapolation
			else
			{
				State latest = m_BufferedState[0];
				float extrapolationLength = (float)(interpolationTime - latest.timestamp);
				// Don't extrapolation for more than 500 ms, you would need to do that carefully
				if (extrapolationLength < m_ExtrapolationLimit)
				{
					//brak ekstrapolacji - postać stoi w miejscu
					rigidbody2D.velocity = new Vector3(0F, 0F);
					
					// --różne sposoby na ekstrapolację
					
					//transform.position = latest.position + latest.velocity * extrapolationLength;
					
					//rigidbody2D.velocity = latest.velocity;
					
					//rigidbody2D.velocity = latest.velocity + (latest.velocity - m_BufferedState[1].velocity)/(extrapolationLength);
				}
			}
			
			
			//----ANIMACJA
			if (rigidbody2D.velocity.x > 0 && !facingRight)
				Flip ();
			else if(rigidbody2D.velocity.x < 0 && facingRight)
				Flip ();
			
			if(rigidbody2D.velocity.x != 0 && grounded)
				anim.SetFloat ("Speed", 1.0f);
			else
				anim.SetFloat ("Speed", 0.0f);
			
			if(jump)
			{
				anim.SetTrigger ("Jump");
				
				// Play a random jump audio clip.
				int i = Random.Range (0, jumpClips.Length);
				AudioSource.PlayClipAtPoint (jumpClips [i], transform.position);
				
				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				jump = false;
			}
		}
	}
	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	
	public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds(tauntDelay);
			
			// If there is no clip currently playing.
			if(!audio.isPlaying)
			{
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom();
				
				// Play the new taunt.
				audio.clip = taunts[tauntIndex];
				audio.Play();
			}
		}
	}
	
	
	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);
		
		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector2.zero;
		bool syncJumped = false;
		
		if (stream.isWriting)
		{
			syncPosition = transform.position;
			syncVelocity = rigidbody2D.velocity;
			syncJumped = jumped;
			jumped = false;
			
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncJumped);
		}
		else 
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);
			stream.Serialize(ref syncJumped);
			
			/*  // wpisanie bez interpolacji
			transform.position = syncPosition;
			rigidbody2D.velocity = syncVelocity;*/
			
			// Shift the buffer sideways, deleting state 20
			for (int i=m_BufferedState.Length-1;i>=1;i--)
				m_BufferedState[i] = m_BufferedState[i-1];
			
			// Record current state in slot 0
			State state;
			state.timestamp = info.timestamp;
			state.position = syncPosition;
			state.velocity = syncVelocity;
			state.jumped = syncJumped;
			//state.isFacedRight = syncIsFacedRight;
			m_BufferedState[0] = state;
			// update used slots in m_BufferedState
			m_TimestampCount = Mathf.Min(m_TimestampCount + 1, 
			                             m_BufferedState.Length);
			
			// check if states are in the proper order
			for (int i=0;i<m_TimestampCount-1;i++)
			{
				if (m_BufferedState[i].timestamp < m_BufferedState[i+1].timestamp)
					Debug.Log("State inconsistent");
			} 
			
		}
	}
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        //Network.Destroy (GetComponent(NetworkView).viewID);
        Network.Destroy(((NetworkView)GetComponent("NetworkView")).viewID);
    }

	
}
