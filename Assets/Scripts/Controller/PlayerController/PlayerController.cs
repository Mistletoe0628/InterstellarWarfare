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
    /// �����ƶ���Э��
    /// </summary>
    private Coroutine m_moveCoroutine;

    /// <summary>
    /// ˵���������ƶ�ʱ��ս����ɫx�����ת�Ƕ�
    /// ���ߣ�wk
    /// ���ڣ�2023-08-01
    /// </summary>
    [SerializeField]
    private float m_moveRotationAngle = 40f;

    /// <summary>
    /// ����ս������Ŀ�ȣ����ÿ��ƶ������ƫ��
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
    /// ˵����ս�����ٶ�ʱ��
    /// ���ߣ�wk
    /// ���ڣ�2023-08-01
    /// </summary>
    [SerializeField]
    public float m_accelerationTime = 3f;

    /// <summary>
    /// ˵��������ʱ��
    /// ���ߣ�wk
    /// ���ڣ�2023-08-01
    /// </summary>
    public float m_decelerationTime = 3f;

    /// <summary>
    /// �������
    /// </summary>
    [SerializeField]
    PlayerInputController m_playerInputController;

    /// <summary>
    /// ����ӵ�
    /// </summary>
    [SerializeField]
    private GameObject m_playerProjectilePreferb1;

    [SerializeField]
    private GameObject m_playerProjectilePreferb2;

    [SerializeField]
    private GameObject m_playerProjectilePreferb3;


    /// <summary>
    /// �ӵ�λ��
    /// </summary>
    [SerializeField]
    private Transform m_projectileMuzzleMiddle;

    [SerializeField]
    private Transform m_projectileMuzzleTop;

    [SerializeField]
    private Transform m_projectileMuzzleBottom;


    /// <summary>
    /// ��ҷ�����
    /// </summary>
    [SerializeField]
    private float m_playerFireInterval = 0.3f;

    private WaitForSeconds m_waitForSeconds;

    /// <summary>
    /// ��ҹ�������
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
        //��ʼ��������Ϊ0
        m_rigidbody2D.gravityScale = 0;
        //���ü��
        m_waitForSeconds = new WaitForSeconds(m_playerFireInterval);
        m_playerInputController.EnablePlayInput();
    }

    private void OnDisable()
    {
        m_playerInputController.OnPlayerMove -= DoMove;
        m_playerInputController.OnPlayerStopMove -= DoStop;
    }

    #region �ƶ�

    /// <summary>
    /// ˵���������ƶ�
    /// ���ߣ�wk
    /// ���ڣ�2023-08-01
    /// </summary>
    /// <param name="moveInput">������ƶ�</param>
    private void DoMove(Vector2 moveInput)
    {
        //��ʼ�ƶ�ǰ�ȹر��ϴε�Э�̣������鴤
        if (m_moveCoroutine != null)
        {
            StopCoroutine(m_moveCoroutine);
        }

        //moveInput.y�ڼ����ϵ����²���ʱ��-1��1������Ӧ��ʱ��ʱ���������
        //������1Ϊ moveInput.y��Χ��-1��1��ս��x��Ҫ��ת�ĽǶ�* moveInput.y���ͻ������ͬ�������ת�Ƕȣ� ����2Ϊ��ת�ᣬVector3.rightΪ��ɫ��x��
        Quaternion moveRotation = Quaternion.AngleAxis(m_moveRotationAngle * moveInput.y, Vector3.right);

        //normalized ��moveInput���й�һ�������ü��̺��ֱ�����Ķ�ά�������ᱣ��һ��
        m_moveCoroutine = StartCoroutine(DoMoveCoroutine(m_accelerationTime, moveInput.normalized * m_moveVelocity, moveRotation));

        StartCoroutine(DoLimitPositionCoroutine());
    }

    /// <summary>
    /// ˵����ֹͣ�ƶ�
    /// ���ߣ�wk
    /// ���ڣ�2023-08-01
    /// </summary>
    private void DoStop()
    {
        //��ʼ�ƶ�ǰ�ȹر��ϴε�Э�̣������鴤
        if (m_moveCoroutine != null)
        {
            StopCoroutine(m_moveCoroutine);
        }

        m_moveCoroutine = StartCoroutine(DoMoveCoroutine(m_decelerationTime, Vector2.zero, Quaternion.identity));
        StopCoroutine(DoLimitPositionCoroutine());
    }

    /// <summary>
    /// ˵����ͨ��Э�������ƶ�������update�������
    /// ���ߣ�wk
    /// ���ڣ�2023-08-01
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
    /// ˵������ʼ�ƶ������Բ�ֵ���γɼ��ٶȵ�Ч��
    /// ���ߣ�wk
    /// ���ڣ�2023-08-01
    /// </summary>
    /// <param name="moveVelocity">��Ҫ�ﵽ���ٶ�</param>
    /// <returns></returns>
    IEnumerator DoMoveCoroutine(float targetTime, Vector2 moveVelocity, Quaternion moveRotation)
    {
        float time = 0;

        while (time < targetTime)
        {
            time += Time.fixedDeltaTime;

            //�����ٶ�
            m_rigidbody2D.velocity = Vector2.Lerp(m_rigidbody2D.velocity, moveVelocity, time / m_accelerationTime);

            //����ս�������ƶ�ʱ����ת
            transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, time / m_accelerationTime);

            yield return null;
        }
    }

    #endregion �ƶ�

    #region ����

    /// <summary>
    /// ˵��������
    /// ���ߣ�wk
    /// ���ڣ�2023-08-05
    /// </summary>
    private void DoFire()
    {
        StartCoroutine(nameof(DoFireCoroutine));
    }

    /// <summary>
    /// ˵����ֹͣ����
    /// ���ߣ�wk
    /// ���ڣ�2023-08-05
    /// </summary>
    private void DoStopFire()
    {
        //unity�����⣬ֱ�Ӵ��뺯��������ֹͣЭ��
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

    #endregion ����

}
