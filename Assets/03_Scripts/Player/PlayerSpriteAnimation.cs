using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteAnimation : MonoBehaviour
{
    public SpriteRenderer sprite;   // 타겟 스프라이트 렌더러
    public Sprite[] idle_front, idle_left, idle_right, idle_up, run_front, run_left, run_right, run_up; // 각 방향&상태별 스프라이트
    public float framePerSecond = 12.0f;

    private bool isRunning = false; // 현재 뛰는 상태
    private int dirrection = 0;     // 현재 바라보는 방향, 상 = 0, 하 = 1, 좌 = 2, 우 = 3
    private Sprite[][] spriteLists_idle, spriteLists_run;
    private Sprite[] sprites;
    private int index = 0, length = 1;
    private float frame;
    private float time = 0;

    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;

    private void Awake()
    {
        // 스프라이트 리스트 등록, 연결
        spriteLists_idle = new Sprite[4][];
        spriteLists_idle[0] = idle_up;
        spriteLists_idle[1] = idle_front;
        spriteLists_idle[2] = idle_left;
        spriteLists_idle[3] = idle_right;

        spriteLists_run = new Sprite[4][];
        spriteLists_run[0] = run_up;
        spriteLists_run[1] = run_front;
        spriteLists_run[2] = run_left;
        spriteLists_run[3] = run_right;

        frame = 1.0f/ framePerSecond;
        sprites = spriteLists_idle[1];
        index = 0;
        length = sprites.Length;
    }

    private void Update()
    {
        time += Time.deltaTime;

        // 프레임에 도달 시
        if(time >= frame)
        {
            sprite.sprite = sprites[index];             // 스프라이트 교체

            index = index >= length -1 ? 0 : index + 1;    // 스프라이트 인덱스 이동
            time = 0;
        }
    }

    public void SetSpriteState(bool isRunning, int dirrection = 0)
    {
        // 멈춰있다 뛸 경우
        if (!this.isRunning && isRunning)
        {
            sprites = spriteLists_run[dirrection];
            index = 0;
            length = sprites.Length;
            this.isRunning = isRunning;
            this.dirrection = dirrection;
        }
        // 뛰다가 멈출 경우
        else if (this.isRunning && !isRunning)
        {
            sprites = spriteLists_idle[this.dirrection];
            index = 0;
            length = sprites.Length;
            this.isRunning = isRunning;
        }
        // 뛰다가 방향 바꿀 경우
        else if (this.isRunning && isRunning)
        {
            sprites = spriteLists_run[dirrection];
            this.dirrection = dirrection;
        }
    }    

    public void SetFramePerSecond(float value)
    {
        framePerSecond = value;
        frame = 1 / framePerSecond;
    }
}
