using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("���x���A�b�v���")]
    public GameObject _levelUpCanvas;
    [Header("�o�g�������")]
    public GameObject _battleCanvas;
    [Header("�G�L����")]
    public GameObject[] _enemy;
    [Header("�G�L����UI")]
    public GameObject[] _enemyUI;

    public float[] _playerAttackDamage;
    public float[] _enemyAttackDamage;

    [Header("Player�U���N�[���^�C��")]
    public float _playerWaitTime;
    [Header("Enemy�U���N�[���^�C��")]
    public float _enemyWaitTime;

    public int _enemyDethCount = 0;

    private bool isPlayerAttack = false;
    private bool isEnemyAttack = false;
    private bool isEnemyDeth = false;
    [Header("�G��Ȋ댯")]
    public bool isSpawnEnemy = false;

    [SerializeField] PlayerStatus _playerStatus;
    [SerializeField] EnemyStatus _enemyStatus;
    [SerializeField] SkillManager _skillManager;

    [SerializeField]
    private GameObject _playerChar;
    [SerializeField]
    private GameObject _playerHpUI;
    [SerializeField]
    private Animator _playerAnim;

    [SerializeField]
    private GameObject _nowEnemy;
    [SerializeField]
    private Animator _enemyAnim;

    [SerializeField]
    private GameObject _playerAttackTextObj;
    [SerializeField]
    private GameObject _enemyAttackTextObj;

    [SerializeField]
    private TextMeshProUGUI _playerAttackText;
    [SerializeField]
    private TextMeshProUGUI _enemyAttackText;

    [SerializeField]
    private SEManager _seManager;

    [SerializeField]
    private GameObject _clearCanvas;
    [SerializeField]
    private GameObject _overCanvas;

    public bool isGameOver = false;
    public bool isGameClear = false;

    private void Start()
    {
        _playerAttackDamage = new float[2];
        _playerAttackDamage[0] = _playerStatus._currentSTR;
        _playerAttackDamage[1] = _playerStatus._currentCRIDamage;

        _playerChar.SetActive(false);
        _playerHpUI.SetActive(false);
        _playerAttackTextObj.SetActive(false);
        _enemyAttackTextObj.SetActive(false);

        _levelUpCanvas.SetActive(false);
    }

    private void Update()
    {
        if (isSpawnEnemy == true && !isGameOver)
        {
            if (isPlayerAttack == false )
            {
                StartCoroutine(PlayerAttack());
            }
            if (isEnemyAttack == false)
            {
                StartCoroutine(EnemyAttack());
            }

            if (_playerStatus != null && _playerStatus._currentHp <= 0 && !isGameOver)
            {
                _playerChar.SetActive(false);
                _playerHpUI.SetActive(false);
                _playerAttackTextObj.SetActive(false);
                if (!_overCanvas.activeSelf)
                {
                    _overCanvas.SetActive(true);
                }
                isGameOver = true;
            }
        }

        if (_enemyStatus != null && _enemyStatus._currentHp <= 0)
        {
            _battleCanvas.SetActive(false);
            //
            Time.timeScale = 0;

            if (isEnemyDeth == false)
            {
                //
                _enemyDethCount += 1;

                isEnemyDeth = true;
            }

            if (_enemyDethCount == 1)
            {
                Destroy(_enemy[0]);
                _enemyUI[0].SetActive(false);
                _levelUpCanvas.SetActive(true);
            }
            else if (_enemyDethCount == 2)
            {
                Destroy(_enemy[1]);
                _enemyUI[1].SetActive(false);
                _levelUpCanvas.SetActive(true);
            }
            else if (_enemyDethCount == 3 && !isGameClear)
            {
                if(!_clearCanvas.activeSelf)
                {
                    _enemy[2].SetActive(false);
                    _clearCanvas.SetActive(true);
                }
                isGameClear = true;
            }
        }


    }

    public void OnButtonClick(GameObject copiedButton)
    {
        TextMeshProUGUI btnText = copiedButton.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText == null)
        {
            return;
        }

        string text = btnText.text;


        switch (text)
        {
            case "AttackBuff":
                _skillManager.skillLevel_AttackBuff += 1;
                break;
            case "AttackCoolTimeDecrease":
                _skillManager.skillLevel_AttackCoolTimeDecrease += 1;
                break;
            case "CriticalDamage":
                _skillManager.skillLevel_CriticalDamage += 1;
                break;
            case "CriticalPar":
                _skillManager.skillLevel_CriticalPar += 1;
                break;
            case "DefenseBuff":
                _skillManager.skillLevel_DefenseBuff += 1;
                break;
            case "DodgePar":
                _skillManager.skillLevel_DodgePar += 1;
                break;
            case "Heal":
                _skillManager.skillLevel_Heal += 1;
                break;
            case "InstantKillPar":
                _skillManager.skillLevel_InstantKillPar += 1;
                break;
            case "ResetCoolTime":
                _skillManager.skillLevel_ResetCoolTime += 1;
                break;
            case "Shield":
                _skillManager.skillLevel_Shield += 1;
                break;
        }
        


        if (_enemyDethCount == 1)
        {
            isEnemyDeth = false;
            _enemy[1].SetActive(true);
            _enemyUI[1].SetActive(true);
            GameObject newEnemy = _enemy[1]; 
            _enemyStatus = newEnemy.GetComponent<EnemyStatus>();
            _nowEnemy = newEnemy;
            _enemyAnim = _nowEnemy.GetComponent<Animator>();

            _levelUpCanvas.SetActive(false);
            _battleCanvas.SetActive(true);
        }
        else if (_enemyDethCount == 2)
        {
            isEnemyDeth = false;
            _enemy[2].SetActive(true);
            _enemyUI[2].SetActive(true);
            GameObject newEnemy = _enemy[2];
            _enemyStatus = newEnemy.GetComponent<EnemyStatus>();
            _nowEnemy = newEnemy;
            _enemyAnim = _nowEnemy.GetComponent<Animator>();

            _levelUpCanvas.SetActive(false);
            _battleCanvas.SetActive(true);
        }

        Time.timeScale = 1;
    }

    IEnumerator PlayerAttack()
    {
        float elapsedTime = 0;
        isPlayerAttack = true;
        _playerAttackDamage[0] = _playerStatus._currentSTR;
        _playerAttackDamage[1] = _playerStatus._currentCRIDamage;
        _playerWaitTime = _playerStatus._currentNormalAttackCoolTime;
        int i = Random.Range(0, _playerAttackDamage.Length);
        while(_playerWaitTime > elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = Mathf.Max(0, _playerWaitTime - elapsedTime);
            _playerAttackText.text = $"���� {remainingTime:F1} �b�ōU���I";
            yield return null;
        }

        _playerAnim.SetTrigger("attack");
        _enemyStatus._currentHp -= _playerAttackDamage[i];
        _seManager.LightAttackSE();
        isPlayerAttack = false;
    }
    IEnumerator EnemyAttack()
    {
        float elapsedTime = 0;
        isEnemyAttack = true;
        _enemyAttackDamage[0] = _enemyStatus._currentSTR;
        _enemyAttackDamage[1] = _enemyStatus._currentCRIDamage;
        _enemyWaitTime = _enemyStatus._currentNormalAttackCoolTime;
        //
        int i = Random.Range(0, _enemyAttackDamage.Length);

        while (_enemyWaitTime > elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = Mathf.Max(0, _enemyWaitTime - elapsedTime);
            _enemyAttackText.text = $"���� {remainingTime:F1} �b�ōU���I";
            yield return null;
        }

        switch (_enemyDethCount)
        {
            case 0:
                _enemyAnim.SetTrigger("AttackTrigger");
                break;
            case 1:
                _enemyAnim.SetTrigger("AttackTrigger");
                break;
            case 2:
                _enemyAnim.SetTrigger("AttackTrigger");
                break;
        }
        _playerAnim.SetTrigger("hurt");
        _playerStatus._currentHp -= _enemyAttackDamage[i];
        _seManager.StrongAttackSE();

        isEnemyAttack = false;
    }

    public void SpawnEnemyAndChar()
    {
        _enemy[0].SetActive(true);
        _enemyUI[0].SetActive(true );
        _enemyAttackTextObj.SetActive(true);
        _playerChar.SetActive(true);
        _playerHpUI.SetActive(true);
        _playerAttackTextObj.SetActive(true);

        GameObject newEnemy = _enemy[0];
        _enemyStatus = newEnemy.GetComponent<EnemyStatus>();
        _nowEnemy = newEnemy;
        _enemyAnim = _nowEnemy.GetComponent<Animator>();

        isSpawnEnemy = true;

        _enemyAttackDamage = new float[2];

        _enemyAttackDamage[0] = _enemyStatus._currentSTR;
        _enemyAttackDamage[1] = _enemyStatus._currentCRIDamage;
    }
}
