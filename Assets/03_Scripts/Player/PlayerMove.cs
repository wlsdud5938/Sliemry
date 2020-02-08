using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameObject green, white;

    private PlayerSpriteAnimation greenAni, whiteAni;
    private int characterState = 0; // 현재 캐릭터 상태, 0:그린, 1:화이트, 2:그린 슬라임

    private bool isRunning = false; // 현재 뛰는 상태
    private int dirrection = 0;     // 현재 바라보는 방향, 상 = 0, 하 = 1, 좌 = 2, 우 = 3

    private bool isCharaChangeEnabled = true;   // 캐릭터 전환 가능한 상태인지

    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;

    // 이동 관련 변수들
    public float moveSpeed = 10.0f;
    private Vector3 upVec, downVec, leftVec, rightVec, upLeftVec, upRightVec, downLeftVec, downRightVec;


    private void Awake()
    {
        greenAni = green.GetComponent<PlayerSpriteAnimation>();
        whiteAni = white.GetComponent<PlayerSpriteAnimation>();

        // 방향 벡터 초기화
        upVec = new Vector3(0, 1, 0);
        downVec = new Vector3(0, -1, 0);
        leftVec = new Vector3(-1, 0, 0);
        rightVec = new Vector3(1, 0, 0);
        upLeftVec = new Vector3(-1, 1, 0).normalized;
        upRightVec = new Vector3(1, 1, 0).normalized;
        downLeftVec = new Vector3(-1, -1, 0).normalized;
        downRightVec = new Vector3(1, -1, 0).normalized;
    }

    private void Start()
    {
        
        if (DataManager.Instance.userData_status.GetPlayingChara() == 0)
        {
            green.SetActive(true);
            white.SetActive(false);
        }
        else
        {
            white.SetActive(true);
            green.SetActive(false);
        }

        /*
        if (DataManager.Instance.userData_status.GetPlayingChara() == 0) ChangeGreen();
        else ChangeWhite();
        */
    }

    private void Update()
    {
        // 캐릭터 변경
        if (Input.GetKeyDown("space"))
        {
            // 쿨타임 중이면 리턴
            if (!isCharaChangeEnabled)
            {
                MessageManager.Instance.ShowMessage(AlarmManager.charaChangeCoolTimeAlarm);
                return;
            }

            // 쿨다운 돌리기
            StartCoroutine(CharaChangeCoolDown());

            if      (characterState == 0) ChangeWhite();
            else if (characterState == 1) ChangeGreen();

            // 활성화된 캐릭터 정보 저장 및 UI 업데이트
            DataManager.Instance.userData_status.SetPlayingChara(characterState);
            StatusHudManager.Instance.SetCharacterHud();
        }

        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)   // 멈춤
        {
            isRunning = false;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 1)   // 위로 이동
        {
            transform.Translate(upVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = UP;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == -1)   // 아래로 이동
        {
            transform.Translate(downVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = DOWN;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == -1 && Input.GetAxisRaw("Vertical") == 0)   // 왼쪽으로 이동
        {
            transform.Translate(leftVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = LEFT;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == 0)   // 오른쪽으로 이동
        {
            transform.Translate(rightVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = RIGHT;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == -1 && Input.GetAxisRaw("Vertical") == 1)   // 왼쪽 위로 이동
        {
            transform.Translate(upLeftVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = LEFT;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == 1)   // 오른쪽 위로 이동
        {
            transform.Translate(upRightVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = RIGHT;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == -1 && Input.GetAxisRaw("Vertical") == -1)   // 왼쪽 아래로 이동
        {
            transform.Translate(downLeftVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = LEFT;
            SetMovingState();
        }
        else if (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == -1)   // 오른쪽 아래로 이동
        {
            transform.Translate(downRightVec * moveSpeed * Time.deltaTime);

            isRunning = true;
            dirrection = RIGHT;
            SetMovingState();
        }
        
    }

    private void SetMovingState()
    {
        if      (characterState == 0) greenAni.SetSpriteState(isRunning, dirrection);
        else if (characterState == 1) whiteAni.SetSpriteState(isRunning, dirrection);
    }

    // 그린으로 변경
    public void ChangeGreen()
    {
        characterState = 0;
        green.SetActive(true);
        white.SetActive(false);

        greenAni.SetInitialState(isRunning, dirrection);
    }

    // 화이트로 변경
    public void ChangeWhite()
    {
        characterState = 1;
        green.SetActive(false);
        white.SetActive(true);

        whiteAni.SetInitialState(isRunning, dirrection);
    }

    public void ChangeState()
    {
        if (characterState == 0) ChangeWhite();
        else if (characterState == 1) ChangeGreen();
    }

    // 캐릭터 전환 쿨타임 쿨다운
    private IEnumerator CharaChangeCoolDown()
    {
        isCharaChangeEnabled = false;

        yield return new WaitForSeconds(StaticValueManager.CharacterChangeCoolTime);

        isCharaChangeEnabled = true;
    }
}
