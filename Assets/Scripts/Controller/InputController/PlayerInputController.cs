using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static MainInputActions;

[CreateAssetMenu(menuName = "PlayerInput")]
public class PlayerInputController : ScriptableObject, IPlayInputActions
{
    MainInputActions m_mainInputActions;

    /// <summary>
    /// ��ʼ�ƶ��¼�
    /// </summary>
    public event Action<Vector2> OnPlayerMove = delegate { };

    /// <summary>
    /// ֹͣ�ƶ��¼�
    /// </summary>
    public event Action OnPlayerStopMove = delegate { };

    /// <summary>
    /// ��ʼ�����¼�
    /// </summary>
    public event Action OnPlayerFire = delegate { };

    /// <summary>
    /// ֹͣ�����¼�
    /// </summary>
    public event Action OnPlayerStopFire = delegate { };

    // Start is called before the first frame update
    void OnEnable()
    {
        m_mainInputActions = new MainInputActions();
        m_mainInputActions.PlayInput.SetCallbacks(this);
    }

    /// <summary>
    /// ˵���������������
    /// ���ߣ�wk
    /// ���ڣ�2023-07-31
    /// </summary>
    public void EnablePlayInput()
    {
        m_mainInputActions.PlayInput.Enable();

        //�������
        Cursor.visible = false;

        //�������
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// ˵����ȡ�������������
    /// ���ߣ�wk
    /// ���ڣ�2023-07-31
    /// </summary>
    public void DisablePlayInput()
    {
        m_mainInputActions.PlayInput.Disable();
    }

    /// <summary>
    /// ˵����ʵ�ֽӿ�
    /// ���ߣ�wk
    /// ���ڣ�2023-07-31
    /// </summary>
    /// <param name="context">context</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //���붯����ִ��ʱ
        if (context.phase == InputActionPhase.Performed)
        {
            OnPlayerMove.Invoke(context.ReadValue<Vector2>());
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            OnPlayerStopMove.Invoke();
        }
    }

    /// <summary>
    /// ˵����ս������
    /// ���ߣ�wk
    /// ���ڣ�2023-08-05
    /// </summary>
    /// <param name="context">context</param>
    public void OnFire(InputAction.CallbackContext context)
    {
        //���붯����ִ��ʱ
        if (context.phase == InputActionPhase.Performed)
        {
            OnPlayerFire.Invoke();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            OnPlayerStopFire.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        DisablePlayInput();
    }
}
