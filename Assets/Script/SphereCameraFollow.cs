using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCameraFollow : MonoBehaviour
{
    public Transform player; // 玩家的Transform组件  
    public float sensitivityX = 15f; // 鼠标水平灵敏度，用于旋转摄像机  
    public float sensitivityY = 15f; // 鼠标垂直灵敏度，用于旋转摄像机  
    public float minYAngle = -80f; // 摄像机仰角的最小限制  
    public float maxYAngle = 80f; // 摄像机仰角的最大限制  

    private float xRotation = 0f; // 摄像机水平旋转角度  
    private float yRotation = 0f; // 摄像机垂直旋转角度  
    private Quaternion originalRotation; // 摄像机的初始旋转  

    void Start()
    {
        // 保存摄像机的初始旋转  
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // 获取鼠标输入  
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        // 更新摄像机的旋转角度  
        xRotation -= mouseX;
        yRotation += mouseY;

        // 限制摄像机的垂直旋转角度  
        yRotation = Mathf.Clamp(yRotation, minYAngle, maxYAngle);

        // 计算旋转  
        Quaternion xQuaternion = Quaternion.AngleAxis(xRotation, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(yRotation, Vector3.right);

        // 应用旋转到摄像机，注意旋转顺序可能会影响最终结果  
        transform.rotation = originalRotation * xQuaternion * yQuaternion;

        // 确保摄像机始终指向玩家（可选，取决于你想要的摄像机行为）  
        // 如果你想要摄像机完全由鼠标控制旋转，可以注释掉下面的LookAt调用  
        // transform.LookAt(player.position + (player.up * someOffsetToKeepPlayerInFrame));  
    }
}
