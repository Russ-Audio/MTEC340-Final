using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
    using UnityEditor;
#endif


public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;

    [SerializeField] GameObject fpsPlayer;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Slider fpsHealthBar;
    [SerializeField] TextMeshProUGUI fpsHealthCount;
    [SerializeField] TextMeshProUGUI killCount;
    [SerializeField] TextMeshProUGUI instructions;
    [SerializeField] TextMeshProUGUI pause;
    [SerializeField] GameObject resume;
    [SerializeField] GameObject restart;
    [SerializeField] GameObject quit;
    [SerializeField] GameObject crossHair;
    [SerializeField] TextMeshPro highScore;

    [SerializeField] GameObject Rifle;
    [SerializeField] GameObject Pistol;

    [SerializeField] AudioClip StartVO;
    [SerializeField] AudioClip pickupPistolVO;
    [SerializeField] AudioClip pickupRifleVO;
    [SerializeField] AudioClip WeaponSwitch;
    [SerializeField] AudioClip DoorOpenVO;
    [SerializeField] AudioClip[] HitVO;
    [SerializeField] AudioClip[] RandomVO;

    [SerializeField] AudioClip DeathVOSFX;

    [SerializeField] AudioClip UIPause;
    [SerializeField] AudioClip UISelect;
    [SerializeField] AudioClip KillStreakSFX;
    [SerializeField] AudioClip[] KillSFX;
    [SerializeField] AudioClip Stinger;
    [SerializeField] AudioClip HealSFX;
    [SerializeField] AudioClip DeathSFX;




    public Animator sceneFade;


    [SerializeField] Transform[] spawnVector = new Transform[6];

    public List<EnemyBehaviour> enemy = new List<EnemyBehaviour>();

    private int KillCount;
    private int HighScore;

    private bool RifleCollected = false;

    private bool spawnEnemy;
    private bool setState;
    private bool EnemyInstantiated;
    private bool inputDisabled = false;

    private int randVOIndex;
    private int prevVOIndex;
    private int RandHurtVO;
    private int PrevHurtVO;
    private bool FightVOisPlaying;

    public bool isPaused = false;

    public bool PlayerDead;

    public int MaxPlayerHealth = 100;
    private int PlayerHealth;

    public GameState _state;

    public DoorBehavior doorOpen;


    private AudioSource audioSource;
    public AudioSource voSource;


    private void Start()
    {
        instructions.text = "Press [E] to Pickup Weapon";
        instructions.enabled = true;
        pause.enabled = false;
        resume.SetActive(false);
        restart.SetActive(false);
        quit.SetActive(false);

        isPaused = false;

        PlayerDead = false;

        crossHair.SetActive(true);

        RifleCollected = false;

        highScore.text = $"{HighScore} Kills";

        sceneFade.Play("Fade In");

        PlayerHealth = MaxPlayerHealth;

        KillCount = 0;

        spawnEnemy = false;
        setState = true;
        inputDisabled = false;

        _state = GameState.Start;

        audioSource = GetComponent<AudioSource>();

        //PlayOneShot StartingVO
        voSource.Stop();
        voSource.PlayOneShot(StartVO);

        SetPlayerRef();
    }

    public void SetPlayerRef()
    {
        EnemyBehaviour[] enemyObjects = FindObjectsOfType<EnemyBehaviour>();

        foreach(EnemyBehaviour p in enemyObjects)
        {
            if (!enemy.Contains(p))
            {
                enemy.AddRange(enemyObjects);
            }
        }

        //enemy.AddRange(enemyObjects);

        foreach (EnemyBehaviour p in enemyObjects)
        {
            p.OnDestroyed += RemoveEnemyFromList;
            //enemy.Add(p);
            //Debug.Log("List added, enemy removed");
        }
    }

    private void RemoveEnemyFromList(EnemyBehaviour deadEnemy)
    {
        enemy.Remove(deadEnemy);
    }

    private void Update()
    {
        if(KillCount != 0 && IsEven(KillCount) && !EnemyInstantiated)
        {
            InstantiateEnemy();
            EnemyInstantiated = true;
        }

        if(KillCount != 0 && IsOdd(KillCount))
        {
            EnemyInstantiated = false;
        }

        if(doorOpen.Open == true && setState)
        {
            //Debug.Log("START");
            //PlayOneShot DoorOpenVO
            voSource.Stop();
            voSource.PlayOneShot(DoorOpenVO);
            audioSource.PlayOneShot(Stinger, 0.3f);
            instructions.text = "Press [R] to Reload... Tip: Find the Rifle";
            StartCoroutine(InstructEnd());
            _state = GameState.Combat;
            setState = false;
        }

        if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            //PlayOneShot UIPause;
            audioSource.PlayOneShot(UIPause, 0.3f);

            pause.enabled = true;
            resume.SetActive(true);
            restart.SetActive(true);
            quit.SetActive(true);

            crossHair.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0;
            isPaused = true;
        }

        if (inputDisabled)
        {
            Input.ResetInputAxes();
        }

        if(!RifleCollected && Rifle.gameObject.activeSelf)
        {
            //Play One Shot RifleCollected
            audioSource.PlayOneShot(WeaponSwitch, 0.7f);
            voSource.Stop();
            voSource.PlayOneShot(pickupRifleVO);
            RifleCollected = true;
        }

    }

    IEnumerator InstructEnd()
    {
        yield return new WaitForSeconds(6.0f);
        instructions.enabled = false;
    }

    void InstantiateEnemy()
    {
        spawnEnemy = true;

        SendValue();

        if (spawnEnemy)
        {
            int SpawnIndex1 = Random.Range(0, 3);
            int SpawnIndex2 = Random.Range(3, 6);

            //Instantiate 2 new enemies
            Instantiate(enemyPrefab, spawnVector[SpawnIndex1].position, Quaternion.identity);
            Instantiate(enemyPrefab, spawnVector[SpawnIndex2].position, Quaternion.identity);

            SetPlayerRef();

            spawnEnemy = false;
        }


    }

    public void EnemyKilled()
    {
        KillCount++;

        //PlayOneShot Kill UI

        int Probability = Random.Range(0, 4);

        if(Probability == 2)
        {
            do
            {
                randVOIndex = Random.Range(0, 7);
            }
            while (randVOIndex == prevVOIndex);

            voSource.Stop();
            voSource.PlayOneShot(RandomVO[randVOIndex]);

            FightVOisPlaying = true;
            Invoke(nameof(HurtCanPlay), 5f);
        }

        int KillIndex = Random.Range(0, 3);

        audioSource.PlayOneShot(KillSFX[KillIndex], 0.7f);

        SendValue();
        SetPlayerRef();

        if(KillCount % 5 == 0)
        {
            audioSource.PlayOneShot(KillStreakSFX, 0.55f);
        }

        if(KillCount > HighScore)
        {
            HighScore++;
        }

        UpdatePlayerUI();
        prevVOIndex = randVOIndex;
    }

    private void HurtCanPlay()
    {
        FightVOisPlaying = false;
    }

    public int GetUpdatedKillCount()
    {
        return KillCount;
    }

    void SendValue()
    {
        //enemy = FindObjectOfType<EnemyBehaviour>();

        foreach(EnemyBehaviour p in enemy)
        {
            p.GetKillCount(KillCount);
        }
    }

    public void PlayerHit(int Damage)
    {
        PlayerHealth -= Damage;
        PlayerHealth = Mathf.Max(PlayerHealth, 0);

        //PlayOneShot HitVO

        if(!FightVOisPlaying && !PlayerDead)
        {
            do
            {
                RandHurtVO = Random.Range(0, 5);
            }
            while (RandHurtVO == PrevHurtVO);

            voSource.Stop();
            voSource.PlayOneShot(HitVO[RandHurtVO]);
        }

        UpdatePlayerUI();

        if(PlayerHealth <= 0 && !PlayerDead)
        {
            PlayerDead = true;

            //PlayOneShot DeathVO
            audioSource.PlayOneShot(DeathSFX, 0.5f);
            audioSource.PlayOneShot(Stinger, 0.3f);

            voSource.Stop();
            voSource.PlayOneShot(DeathVOSFX);

            inputDisabled = true;
            Rifle.SetActive(false);
            Pistol.SetActive(false);
            crossHair.SetActive(false);
            Death();
        }

        PrevHurtVO = RandHurtVO;
    }

    public void PlayerHeal(int HealAmount)
    {
        if(PlayerHealth >= 0)
        {
            PlayerHealth += HealAmount;
            PlayerHealth = Mathf.Min(PlayerHealth, MaxPlayerHealth);

            //PlayOneShot HealAudioSFX
            audioSource.PlayOneShot(HealSFX, 0.5f);

            UpdatePlayerUI();
        }
        else
        {
            return;
        }
    }

    void UpdatePlayerUI()
    {
        //Update both TMPro & Slider for Player Health
        fpsHealthBar.value = PlayerHealth;
        fpsHealthCount.text = $"Health: {PlayerHealth.ToString()}/100";

        //Update TMPro for Kill Count
        killCount.text = $"Kills: {KillCount.ToString()}";

        //Update TMPro for High Score
        highScore.text = $"{HighScore} Kills";

    }

    void Death()
    {
        if(KillCount >= HighScore)
        {
            HighScore = KillCount;
            highScore.text = $"{HighScore} Kills";
            //Congratulation & New HighScore Text Appear
        }

        _state = GameState.End;
        sceneFade.Play("Fade Out");
        Invoke(nameof(RestartButton), 4.0f);
        
        //Death Scenario
    }

    private bool IsEven(int Number)
    {
        return Number % 2 == 0;
    }

    private bool IsOdd(int Number)
    {
        return Number % 2 == 1;
    }

    public void WeaponPickedup()
    {
        instructions.text = "Use [LMouse] to Shoot the Door";
        audioSource.PlayOneShot(WeaponSwitch, 0.7f);
        voSource.PlayOneShot(pickupPistolVO);
    }

    public void ResumeButton()
    {
        //PlayOneShot UISelect
        audioSource.PlayOneShot(UISelect, 0.2f);
        pause.enabled = false;
        resume.SetActive(false);
        restart.SetActive(false);
        quit.SetActive(false);

        crossHair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void RestartButton()
    {
        //PlayOneShot UISelect
        if (!PlayerDead)
        {
            audioSource.PlayOneShot(UISelect, 0.2f);
        }

        pause.enabled = false;
        resume.SetActive(false);
        restart.SetActive(false);
        quit.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ResetGame();
        isPaused = false;
    }

    public void QuitButton()
    {
        QuitGame();
    }


    void ResetGame()
    {
        HighScore = KillCount;


        SceneManager.LoadScene(0);
        highScore.text = $"{HighScore} Kills";
        pause.enabled = false;
        resume.SetActive(false);
        restart.SetActive(false);
        quit.SetActive(false);

        

        Time.timeScale = 1;
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
    EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
