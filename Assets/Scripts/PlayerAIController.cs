using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class PlayerAIController : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 10.0f;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private GameObject spotLight;
	[SerializeField] private GameObject startPoint;
	private List<GameObject> destinations;
	private CharacterController characterController;
	private Animator anim;
	private BoxCollider[] swordColliders;
	private GameObject fireTrail;
	private ParticleSystem fireTrailParticles;
	private NavMeshAgent navigation;
	int visitingIndex = 0;
	bool inCollision = false;
	private bool attacking = false;
	Queue<GameObject> queue = new Queue<GameObject>();
	List<GameObject> destinationsVisited = new List<GameObject>();
	// Use this for initialization
	void Start()
	{
		fireTrail = GameObject.FindWithTag("Fire") as GameObject;
		fireTrail.SetActive(false);
		characterController = GetComponent<CharacterController>();
		anim = GetComponent<Animator>();
		swordColliders = GetComponentsInChildren<BoxCollider>();
		navigation = GetComponent<NavMeshAgent>();
		this.destinations = GameObject.FindGameObjectsWithTag("Destinations").ToList();
		StartCoroutine(navigateBFS(startPoint));
		Assert.IsNotNull(navigation);
	}

	IEnumerator navigateBFS(GameObject currNode)
    {
	
		navigation.SetDestination(currNode.transform.position);
		Vector3 moveDirection = navigation.velocity;
		GameObject temp = Instantiate(spotLight);
		temp.transform.position = new Vector3(currNode.transform.position.x, 1.2f, currNode.transform.position.z);
		temp.SetActive(true);
		characterController.SimpleMove(moveDirection * moveSpeed);
		yield return new WaitForSeconds(2f);
		float dis = Vector3.Distance(navigation.destination, transform.position);
		if (navigation.pathStatus == NavMeshPathStatus.PathComplete)
		{			
			print("Test Case Passed: Reached destination "+currNode.name);
		}
		else
		{
			print("Test Case Failed: Unreachable Destination");

		}
		StartCoroutine(navigateBFS(queue.Dequeue()));
	}




    // Update is called once per frame
    void Update()
	{
		
		if (!GameManager.instance.GameOver && navigation.destination!=null)
			{	
				Vector3 moveDirection = navigation.velocity;
				characterController.SimpleMove(moveDirection * moveSpeed);

				if (moveDirection == Vector3.zero)
				{
					anim.SetBool("IsWalking", false);
				}
				else
				{
					anim.SetBool("IsWalking", true);
				}
			navigation.updateRotation = true;
		}
		
	}
 

    public void BeginAIAttack()
	{
		foreach (var weapon in swordColliders)
		{
			weapon.enabled = true;
		}
	}

	public void EndAIAttack()
	{
		foreach (var weapon in swordColliders)
		{
			weapon.enabled = false;
		}
	}
	IEnumerator StartAttack(GameObject other)
    {
        if (!GameManager.instance.GameOver)
		{
			if (attacking)
			{
				navigation.SetDestination(other.transform.position); 
				Vector3 moveDirection = navigation.velocity;
				spotLight.transform.position = other.transform.position;
				int idx = UnityEngine.Random.Range(0, 2);
				if (idx == 0)
				{
					anim.Play("DoubleChop");
				}
				else if (idx == 1)
				{
					anim.Play("SpinAttack");
				}

			}
			yield return new WaitForSeconds(2f);
			StartCoroutine(StartAttack(other));

		}
	}
	public void SpeedPowerUp()
	{

		StartCoroutine(fireTrailRoutine());
	}

	IEnumerator fireTrailRoutine()
	{
		fireTrail.SetActive(true);
		moveSpeed = 10f;
		yield return new WaitForSeconds(5f);
		moveSpeed = 6f;
		fireTrailParticles = fireTrail.GetComponent<ParticleSystem>();
		var em = fireTrailParticles.emission;
		em.enabled = false;
		yield return new WaitForSeconds(3f);
		em.enabled = true;
		fireTrail.SetActive(false);

	}
    private void OnTriggerEnter(Collider other)
    {
		if(other.gameObject.tag == "Enemy")
        {
			Console.WriteLine("Enter Collision...");
			attacking = true;
			StartCoroutine(StartAttack(other.gameObject));

		}else if(other.gameObject.tag == "Destinations" && !destinationsVisited.Contains(other.gameObject))
        {
			//print("Here is something...");
			queue.Enqueue(other.gameObject);
			//print(queue.Count);
			destinationsVisited.Add(other.gameObject);
			//print(queue.Count);
        }
	}
    private void OnTriggerExit(Collider other)
    {
		attacking = false;
		//Console.WriteLine("Exit Collision...");
		StopCoroutine(StartAttack(other.gameObject));
		//StartCoroutine(setDestination());
	}
}
