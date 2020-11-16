using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent nav;
    private Animator anim;
    EnemyHealth enemy;

    // Start is called before the first frame update
    void Start()
    {
        player = (GameManager.instance.Player as GameObject).transform;
        enemy = GetComponent<EnemyHealth>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemy.IsAlive)
        {
            nav.enabled = false;
        }else 
        if (GameManager.instance.GameOver)
        {
            nav.enabled = false;
            anim.Play("Idle");
        }
        else
        {
            nav.SetDestination(player.position);
        }
        
    }
}
