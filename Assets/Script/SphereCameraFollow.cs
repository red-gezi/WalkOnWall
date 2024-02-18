using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCameraFollow : MonoBehaviour
{
    public Transform player; // ��ҵ�Transform���  
    public float sensitivityX = 15f; // ���ˮƽ�����ȣ�������ת�����  
    public float sensitivityY = 15f; // ��괹ֱ�����ȣ�������ת�����  
    public float minYAngle = -80f; // ��������ǵ���С����  
    public float maxYAngle = 80f; // ��������ǵ��������  

    private float xRotation = 0f; // �����ˮƽ��ת�Ƕ�  
    private float yRotation = 0f; // �������ֱ��ת�Ƕ�  
    private Quaternion originalRotation; // ������ĳ�ʼ��ת  

    void Start()
    {
        // ����������ĳ�ʼ��ת  
        originalRotation = transform.rotation;
    }

    void Update()
    {
        // ��ȡ�������  
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        // �������������ת�Ƕ�  
        xRotation -= mouseX;
        yRotation += mouseY;

        // ����������Ĵ�ֱ��ת�Ƕ�  
        yRotation = Mathf.Clamp(yRotation, minYAngle, maxYAngle);

        // ������ת  
        Quaternion xQuaternion = Quaternion.AngleAxis(xRotation, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(yRotation, Vector3.right);

        // Ӧ����ת���������ע����ת˳����ܻ�Ӱ�����ս��  
        transform.rotation = originalRotation * xQuaternion * yQuaternion;

        // ȷ�������ʼ��ָ����ң���ѡ��ȡ��������Ҫ���������Ϊ��  
        // �������Ҫ�������ȫ����������ת������ע�͵������LookAt����  
        // transform.LookAt(player.position + (player.up * someOffsetToKeepPlayerInFrame));  
    }
}
