using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class EnemyBehaviour : MonoBehaviour
{
    public event System.Action<EnemyBehaviour> OnDestroyed;
    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

    public Animator anim;
    public Transform player;

    public LayerMask layertoignore;

    public Canvas healthBarCanvas;

    private NavMeshAgent agent;

    private float xVal;
    private float yVal;

    [SerializeField] Slider enemyHealthBar;
    [SerializeField] TextMeshProUGUI enemyHealthCount;

    [SerializeField] AudioClip[] EnemyGun;

    public float weaponRange;
    public float shootFreq;
    public float enemySpeed;
    private float enemyHealth;

    private BoxCollider enemyCollider;

    public float maxHealth;
    public int MaxDamage;
    public int CurrentKill;

    private int EnemyDamage;
    private int CurrentDamage;

    public GameBehavior gameBehavior;
    public DoorBehavior doorCheck;
    public GameState gameState;

    public GameObject PlayerTorso;

    public bool playerInSight;

    public ParticleSystem muzzleFlash;

    private bool Shoot = false;
    private bool PlayerDead = false;
    private bool isAiming;

    private AudioSource shootSource;
    public AudioSource loopSource;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("First Person Player").transform;
        agent = GetComponent<NavMeshAgent>();
        gameBehavior = GameObject.Find("GameBehavior").GetComponent<GameBehavior>();
        doorCheck = GameObject.Find("Door").GetComponent<DoorBehavior>();
        enemyCollider = GetComponent<BoxCollider>();
        shootSource = GetComponent<AudioSource>();

        float randPitch = Random.Range(0.7f, 1.2f);
        loopSource.pitch = randPitch;

        shootFreq = Random.Range(2.0f, 5.0f);

        layertoignore = LayerMask.GetMask("Enemy");

        EnemyDamage = 5;

        enemyHealth = maxHealth;

        if (doorCheck.Open == true)
        {
            EnemyAim();
        }

    }

    // Update is called once per frame
    void Update()
    {

        if(gameBehavior.PlayerDead == true)
        {
            loopSource.Stop();
            shootSource.Stop();
        }

        if(anim.GetBool("Aiming") == true)
        {
            transform.LookAt(player);
        }

        if (Shoot && anim.GetBool("Dead") == false && anim.GetBool("Roll") == false)
        {
            //PlayOneShot shooting audio
            //Play Fire Animation
            RandMove();
            shootFreq = Random.Range(2.0f, 5.0f);
            StartCoroutine(ShotCoolDown());
            ShootPlayer();
            Shoot = false;
        }

        if(doorCheck.Open == true && !isAiming)
        {
            EnemyAim();
        }

        if (PlayerDead)
        {
            agent.isStopped = true;
        }


        if(gameBehavior.isPaused == true)
        {
            loopSource.mute = true;
            shootSource.mute = true;
            //Debug.Log("PAUSE");
        }
        else
        {
            loopSource.mute = false;
            shootSource.mute = false;
        }


    }
    

    public void EnemyAim()
    {
        anim.SetBool("Aiming", true);
        isAiming = true;
        StartCoroutine(ShotCoolDown());
    }

    IEnumerator ShotCoolDown()
    {
        Shoot = false;

        yield return new WaitForSeconds(shootFreq);

        Shoot = true;
    }

    void ShootPlayer()
    {

        muzzleFlash.Play();
        //Debug.Log("SHOOT");

        int randInt = Random.Range(0, 3);

        float randPitch = Random.Range(0.93f, 1.05f);
        shootSource.pitch = randPitch;

        shootSource.PlayOneShot(EnemyGun[randInt]);

        //Raycast Shoot Here + Lookat
        RaycastHit Hit;
        if (Physics.Raycast(transform.position, transform.forward, out Hit, weaponRange, ~layertoignore))
        {
            //Debug.Log(Hit.transform.name);
            if(Hit.transform.name == "First Person Player")
            {
                gameBehavior.PlayerHit(EnemyDamage);
            }
            else
            {
                MoveEnemy();
            }

            Shoot = false;
            return;
        }
    }

    void RandMove()
    {

        xVal = Random.Range(-1, 2);        
        yVal = Random.Range(-1, 2);
        

        anim.SetFloat("X", xVal);
        anim.SetFloat("Y", yVal);
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        enemyHealth = Mathf.Max(enemyHealth, 0);

        UpdateEnemyUI();

        //Debug.Log($"Ouch {damage} I only have {enemyHealth} left");
        if (enemyHealth <= 0)
        {
            PlayerDead = true;
            healthBarCanvas.enabled = false;
            loopSource.Stop();
            enemyCollider.enabled = false;
            anim.SetBool("Dead", true);
            gameBehavior.EnemyKilled();
            Invoke(nameof(Cleanup), 5f);
        }
    }

    void Cleanup()
    {
        Destroy(this.gameObject);
    }

    public void GetKillCount(int KillCount)
    {
        CurrentKill = KillCount;
        //Debug.Log(CurrentKill);
    }

    public void EnemyBuff()
    {
        if(this.gameObject != null && !PlayerDead)
        {

            int AddBuff = CurrentKill / 2;

            if ((EnemyDamage += AddBuff) < MaxDamage)
            {
                EnemyDamage += AddBuff;
            }

            maxHealth += (AddBuff * 2);

            if (enemySpeed < 10)
            {
                enemySpeed += (float)(AddBuff / 10);
                agent.speed = enemySpeed;
            }
        }
    }

    public void EnemyReset()
    {
        EnemyDamage = 5;
    }

    void UpdateEnemyUI()
    {
        enemyHealthBar.value = enemyHealth / maxHealth;
    }

    void MoveEnemy()
    {
        if (!PlayerDead)
        {
            agent.destination = player.position;
            agent.stoppingDistance = 12.0f;
        }
    }
}
