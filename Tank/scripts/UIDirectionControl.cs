/*
 
控制血条环随着坦克转向（m_UseRelativeRotation = false;），
或者不随坦克转向（m_UseRelativeRotation = true;）
 
 */

using UnityEngine;
using System.Collections;

public class UIDirectionControl : MonoBehaviour {

    public bool m_UseRelativeRotation = true;

    private Quaternion m_RelativeRotation;

	// Use this for initialization
	void Start () {
        m_RelativeRotation = transform.parent.localRotation;    // HealthSlider 的父对象 Canvas 相对于 Tank 的旋转。

	}
	
	// Update is called once per frame
	void Update () {
	    if (m_UseRelativeRotation)
        {
            // 设置 HealthSlider 的世界空间中旋转保持为最初的 m_RelativeRotation，观察其Transform可以发现其相对旋转会随着Tank的旋转而改变
            transform.rotation = m_RelativeRotation;        
        }

        // 如果不使用相对旋转，则HealthSlider随着Tank一起旋转，观察其Transform可发现其相对旋转一直不变。
	}
}
