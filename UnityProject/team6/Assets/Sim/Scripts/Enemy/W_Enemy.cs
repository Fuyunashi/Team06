using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class W_Enemy : MonoBehaviour
{
    public enum EnemyState { idle, trace, attack, die };

    public EnemyState enemyState = EnemyState.idle;

    private Transform enemyTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;

    private bool isDie = false;

    //private Transform target;
    //private Vector3 targetPoint;

    //private float accelaration;

    //public float velocity = 2.0f;
    //public float accel;
    //public float detectionRange;

    // Use this for initialization
    void Start()
    {
        enemyTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();

        StartCoroutine(this.CheckEnemyState());
        StartCoroutine(this.EnemyAction());
    }
    // Update is called once per frame
    void Update()
    {
     //MoveToPlayer();
    }
    IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            if(dist <= attackDist)
            {
                enemyState = EnemyState.attack;
            }
            else if(dist <= traceDist)
            {
                enemyState = EnemyState.trace;
            }
            else
            {
                enemyState = EnemyState.idle;
            }
        }
    }

    IEnumerator EnemyAction()
    {
        while(!isDie)
        {
            switch (enemyState)
            {
                case EnemyState.idle:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsTrace", false);              
                    break;

                case EnemyState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.isStopped = false;
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                    break;

                case EnemyState.attack:
                    nvAgent.isStopped = true;
                    animator.SetBool("IsAttack", true);
                    break;
            }
            yield return null;
        }
    }
    //void MoveToPlayer()
    //{
    //    gameObject.GetComponent<EnemyNavi>().enabled = true;

    //    target = GameObject.FindGameObjectWithTag("Player").transform;
    //    targetPoint = target.position - transform.position;
    //    velocity += accel * Time.deltaTime;

    //    if (targetPoint.sqrMagnitude <= detectionRange * detectionRange)
    //    {
    //        gameObject.GetComponent<EnemyNavi>().enabled = false;
    //        //soundManager.instance.EnemyToPlayer();
             
    //        transform.LookAt(target);
    //        transform.Translate(Vector3.forward * velocity);
    //        Debug.Log("In");
    //    }
    //    else
    //    {
    //        velocity = 0.0f;
    //        Debug.Log("Out");            
    //    }
    //}
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //soundManager.instance.EnemyDead();
            //Destroy(gameObject);
        }
    }
}
