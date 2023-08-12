using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 视口管理
/// </summary>
public class ViewPortController : Singleton<ViewPortController>
{
    /// <summary>
    /// 视口XY上最小、最大值
    /// </summary>
    private float m_minXViewPort;

    private float m_minYViewPort;

    private float m_maxXViewPort;

    private float m_maxYViewPort;

    void Start()
    {
        Vector3 leftBottom = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightTop = Camera.main.ViewportToWorldPoint(Vector3.one);

        m_minXViewPort = leftBottom.x;
        m_minYViewPort = leftBottom.y;

        m_maxXViewPort = rightTop.x;
        m_maxYViewPort = rightTop.y;
    }

    /// <summary>
    /// 说明：限制移动距离
    /// 作者：wk
    /// 日期：2023-08-01
    /// </summary>
    /// <param name="palyPosition">palyPosition</param>
    /// <param name="leftOffset">leftOffset</param>
    /// <param name="rightOffset">rightOffset</param>
    /// <param name="bottomOffset">bottomOffset</param>
    /// <param name="topOffset">topOffset</param>
    /// <returns></returns>
    public Vector3 LimitMovementRange(Vector3 palyPosition, float leftOffset, float rightOffset, float bottomOffset, float topOffset)
    {
        Vector3 position = Vector3.zero;
        position.x = Mathf.Clamp(palyPosition.x, m_minXViewPort + leftOffset, m_maxXViewPort - rightOffset);
        position.y = Mathf.Clamp(palyPosition.y, m_minYViewPort + bottomOffset, m_maxYViewPort - topOffset);

        return position;
    }
}
