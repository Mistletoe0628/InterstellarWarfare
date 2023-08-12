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
    /// 开始移动事件
    /// </summary>
    public event Action<Vector2> OnPlayerMove = delegate { };

    /// <summary>
    /// 停止移动事件
    /// </summary>
    public event Action OnPlayerStopMove = delegate { };

    /// <summary>
    /// 开始攻击事件
    /// </summary>
    public event Action OnPlayerFire = delegate { };

    /// <summary>
    /// 停止攻击事件
    /// </summary>
    public event Action OnPlayerStopFire = delegate { };

    // Start is called before the first frame update
    void OnEnable()
    {
        m_mainInputActions = new MainInputActions();
        m_mainInputActions.PlayInput.SetCallbacks(this);
    }

    /// <summary>
    /// 说明：激活输入控制
    /// 作者：wk
    /// 日期：2023-07-31
    /// </summary>
    public void EnablePlayInput()
    {
        m_mainInputActions.PlayInput.Enable();

        //隐藏鼠标
        Cursor.visible = false;

        //锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 说明：取消激活输入控制
    /// 作者：wk
    /// 日期：2023-07-31
    /// </summary>
    public void DisablePlayInput()
    {
        m_mainInputActions.PlayInput.Disable();
    }

    /// <summary>
    /// 说明：实现接口
    /// 作者：wk
    /// 日期：2023-07-31
    /// </summary>
    /// <param name="context">context</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //输入动作已执行时
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
    /// 说明：战机攻击
    /// 作者：wk
    /// 日期：2023-08-05
    /// </summary>
    /// <param name="context">context</param>
    public void OnFire(InputAction.CallbackContext context)
    {
        //输入动作已执行时
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
