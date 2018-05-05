using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Transform[] m_Targets;

    public float m_MinSize = 6.5f;          // Camera 的最小 size
    public float m_ScreenEdgeBuffer = 4f;   // target到屏幕边缘的最小垂直距离
    public float m_SmoothTime = 0.2f;       // 相机平滑移动的速度

    private Camera m_Camera;                // 用来引用 Camera GameObject
    private Vector3 m_DesiredPosition;      // Camera Rig需要移动到的位置
    private Vector3 m_MoveVolicity = Vector3.zero;  // Vector3.SmoothDamp 用到的引用
    private float m_ZoomSpeed = 0f;                 // Mathf.SmoothDamp 用到的引用

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }
	
    // 由于摄像机跟随的 Tank 是具有物理属性的 GameObject，其运动是在 FixedUpdate 中进行更新的，
    // 故摄像机也需要随着 FixedUpdate 更新，否则（在Update中更新的话）相机会有迟滞感（因为 FixedUpdate 的更新频率往往大于 Update）
	void FixedUpdate () {
        Move();
        Zoom();
	}

    // 移动 Camera Rig
    private void Move()
    {
        FindAveragePosition();

        // 平滑的移动 Camera Rig
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVolicity, m_SmoothTime);
        //transform.position = m_DesiredPosition;
    }

    // 寻找 m_Targets 的平均位置，更新 Camera Rig 的 x，z 坐标，保持 y 坐标不变
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;     // 记录当前 active 的 target 数量
        for(int i = 0; i < m_Targets.Length; ++i)
        {
            if(!m_Targets[i].gameObject.activeSelf)
            {
                continue;
            }
            averagePos += m_Targets[i].position;
            ++numTargets;
        }

        if(numTargets > 0)
        {
            averagePos /= numTargets;
        }
        // 保持y坐标不变
        averagePos.y = transform.position.y;
        // 更新目标位置
        m_DesiredPosition = averagePos;
    }

    // 缩放相机
    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_SmoothTime);
    }

    // 确定 Camera 需要的 orghographic size 的大小
    private float FindRequiredSize()
    {
        // m_DesiredPosition 在 Camera Rig 局部坐标系下的位置
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0;

        for (int i = 0; i < m_Targets.Length; ++i)
        {
            if(!m_Targets[i].gameObject.activeSelf)
            {
                continue;
            }
            // m_Targets[i] 在 Camera Rig 局部坐标系下的位置
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].transform.position);
            // targetLocalPos 关于 desiredLocalPos 的相对位置
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
            // 比较当前 size 与 要满足 desiredPosToTarget 所需的 size，包括左右方向以及上下方向
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x / m_Camera.aspect));
        }
        // 添加边框宽度
        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);
        return size;
    }
}
