using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D m_rigidbody2D;

    [SerializeField]
    private float m_moveVelocity;

    /// <summary>
    /// 控制移动的协程
    /// </summary>
    private Coroutine m_moveCoroutine;

    /// <summary>
    /// 说明：控制移动时，战机红色x轴的旋转角度
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    [SerializeField]
    private float m_moveRotationAngle = 40f;

    /// <summary>
    /// 算上战机自身的宽度，设置可移动距离的偏移
    /// </summary>
    [SerializeField]
    public float m_leftOffset;

    [SerializeField]
    public float m_rightOffset;

    [SerializeField]
    public float m_bottomOffset;

    [SerializeField]
    public float m_topOffset;

    /// <summary>
    /// 说明：战机加速度时间
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    [SerializeField]
    public float m_accelerationTime = 3f;

    /// <summary>
    /// 说明：减速时间
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    public float m_decelerationTime = 3f;

    /// <summary>
    /// 输入控制
    /// </summary>
    [SerializeField]
    PlayerInputController m_playerInputController;

    /// <summary>
    /// 玩家子弹
    /// </summary>
    [SerializeField]
    private GameObject m_playerProjectilePreferb1;

    [SerializeField]
    private GameObject m_playerProjectilePreferb2;

    [SerializeField]
    private GameObject m_playerProjectilePreferb3;


    /// <summary>
    /// 子弹位置
    /// </summary>
    [SerializeField]
    private Transform m_projectileMuzzleMiddle;

    [SerializeField]
    private Transform m_projectileMuzzleTop;

    [SerializeField]
    private Transform m_projectileMuzzleBottom;


    /// <summary>
    /// 玩家发射间隔
    /// </summary>
    [SerializeField]
    private float m_playerFireInterval = 0.3f;

    private WaitForSeconds m_waitForSeconds;

    /// <summary>
    /// 玩家攻击威力
    /// </summary>
    [SerializeField,Range(0,3)]
    private int m_weaponPower = 1;

    [SerializeField]
    public int WeaponPower
    {
        get { return m_weaponPower; }
        set { m_weaponPower = value; }
    }


    private void OnEnable()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();

        m_playerInputController.OnPlayerMove += DoMove;
        m_playerInputController.OnPlayerStopMove += DoStop;
        m_playerInputController.OnPlayerFire += DoFire;
        m_playerInputController.OnPlayerStopFire += DoStopFire;
    }

    private void Start()
    {
        //初始重力设置为0
        m_rigidbody2D.gravityScale = 0;
        //设置间隔
        m_waitForSeconds = new WaitForSeconds(m_playerFireInterval);
        m_playerInputController.EnablePlayInput();
    }

    private void OnDisable()
    {
        m_playerInputController.OnPlayerMove -= DoMove;
        m_playerInputController.OnPlayerStopMove -= DoStop;
    }

    #region 移动

    /// <summary>
    /// 说明：持续移动
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    /// <param name="moveInput">输入的移动</param>
    private void DoMove(Vector2 moveInput)
    {
        //开始移动前先关闭上次的协程，否则会抽搐
        if (m_moveCoroutine != null)
        {
            StopCoroutine(m_moveCoroutine);
        }

        //moveInput.y在键盘上的上下操作时，-1到1的区间应该时随时间逐渐增大的
        //（参数1为 moveInput.y范围在-1到1，战机x轴要旋转的角度* moveInput.y，就会产生不同方向的旋转角度） 参数2为旋转轴，Vector3.right为红色的x轴
        Quaternion moveRotation = Quaternion.AngleAxis(m_moveRotationAngle * moveInput.y, Vector3.right);

        //normalized 对moveInput进行归一化处理，让键盘和手柄输入的二维向量将会保持一致
        m_moveCoroutine = StartCoroutine(DoMoveCoroutine(m_accelerationTime, moveInput.normalized * m_moveVelocity, moveRotation));

        StartCoroutine(DoLimitPositionCoroutine());
    }

    /// <summary>
    /// 说明：停止移动
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    private void DoStop()
    {
        //开始移动前先关闭上次的协程，否则会抽搐
        if (m_moveCoroutine != null)
        {
            StopCoroutine(m_moveCoroutine);
        }

        m_moveCoroutine = StartCoroutine(DoMoveCoroutine(m_decelerationTime, Vector2.zero, Quaternion.identity));
        StopCoroutine(DoLimitPositionCoroutine());
    }

    /// <summary>
    /// 说明：通过协程限制移动，避免update持续检测
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    /// <returns></returns>
    IEnumerator DoLimitPositionCoroutine()
    {
        while (true)
        {
            transform.position = ViewPortController.Instance.LimitMovementRange(transform.position, m_leftOffset
                , m_rightOffset, m_bottomOffset, m_topOffset);
            yield return null;
        }
    }

    /// <summary>
    /// 说明：开始移动，线性插值，形成加速度的效果
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    /// <param name="moveVelocity">想要达到的速度</param>
    /// <returns></returns>
    IEnumerator DoMoveCoroutine(float targetTime, Vector2 moveVelocity, Quaternion moveRotation)
    {
        float time = 0;

        while (time < targetTime)
        {
            time += Time.fixedDeltaTime;

            //设置速度
            m_rigidbody2D.velocity = Vector2.Lerp(m_rigidbody2D.velocity, moveVelocity, time / m_accelerationTime);

            //设置战机上下移动时的旋转
            transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, time / m_accelerationTime);

            yield return null;
        }
    }

    #endregion 移动

    #region 攻击

    /// <summary>
    /// 说明：开火
    /// 作者：wk
    /// 日期：2023-08-05
    /// </summary>
    private void DoFire()
    {
        StartCoroutine(nameof(DoFireCoroutine));
    }

    /// <summary>
    /// 说明：停止开火
    /// 作者：wk
    /// 日期：2023-08-05
    /// </summary>
    private void DoStopFire()
    {
        //unity老问题，直接传入函数，不会停止协程
        //StopCoroutine(DoFireCoroutine());
        StopCoroutine(nameof(DoFireCoroutine));
    }

    IEnumerator DoFireCoroutine()
    {
        while (true)
        {
            //Instantiate(m_playerProjectilePreferb, m_projectileMuzzle.position, Quaternion.identity);
            switch (WeaponPower)
            {
                case 1:
                    PoolManager.Release(m_playerProjectilePreferb1, m_projectileMuzzleMiddle.position, Quaternion.identity);
                    break;
                case 2:
                    PoolManager.Release(m_playerProjectilePreferb1, m_projectileMuzzleTop.position, Quaternion.identity);
                    PoolManager.Release(m_playerProjectilePreferb1, m_projectileMuzzleBottom.position, Quaternion.identity);
                    break;
                case 3:
                    PoolManager.Release(m_playerProjectilePreferb1, m_projectileMuzzleMiddle.position, Quaternion.identity);
                    PoolManager.Release(m_playerProjectilePreferb2, m_projectileMuzzleTop.position, Quaternion.identity);
                    PoolManager.Release(m_playerProjectilePreferb3, m_projectileMuzzleBottom.position, Quaternion.identity);
                    break;
                default:
                    PoolManager.Release(m_playerProjectilePreferb1, m_projectileMuzzleMiddle.position, Quaternion.identity);
                    break;
            }
            yield return m_waitForSeconds;
        }
    }

    #endregion 攻击

}
