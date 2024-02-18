using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseXSpeed = 100f;
    public float mouseYSpeed = 200f;
    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    public float distance = 5f;
    public float mouseX;
    public float mouseY;
    Vector3 gravity;

    public GameObject followCameraPos;
    private Animator animator;
    void Start() => animator = transform.GetChild(0).GetComponent<Animator>();
    void Update()
    {
        ////////////////////////////////////////////////////////���������λ��////////////////////////////////////////////////////
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mouseX += Input.GetAxis("Mouse X") * mouseXSpeed * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * mouseYSpeed * Time.deltaTime;
        //������ֵ
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        mouseX = Mathf.Repeat(mouseX + 180f, 360f) - 180f;
        mouseY = Mathf.Clamp(mouseY, -12, 60);
        //��������
        float x = Mathf.Sin(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        float y = Mathf.Sin(mouseY * Mathf.Deg2Rad);
        float z = -Mathf.Cos(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        //��õ�ǰ��ҽ�ɫ�ĽǶ�
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles);
        followCameraPos.transform.position = rotation * new Vector3(x, y, z) * distance + transform.position;
        followCameraPos.transform.LookAt(transform.position, transform.up);
        Camera.main.transform.position = followCameraPos.transform.position;
        Camera.main.transform.eulerAngles = followCameraPos.transform.eulerAngles;
        ////////////////////////////////////////////////////////���ƽ�ɫ����//////////////////////////////////////////////////////
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        //�����ڲ�ͬ������Ƕȡ���ͬ����Ƕ��������ĸ�����ĳ���
        Vector3 localRight = -Vector3.Cross(Camera.main.transform.forward, transform.up).normalized;
        Vector3 localForward = Vector3.Cross(localRight, transform.up).normalized;
        Debug.DrawRay(transform.position, localForward, Color.blue);
        Debug.DrawRay(transform.position, localRight, Color.red);
        if (verticalInput != 0)
        {
            //�����������ʼλ��
            float angle = Vector3.SignedAngle(transform.forward, localForward * verticalInput, transform.up);
            Debug.Log("�Ƕ�Ϊ" + angle);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        if (horizontalInput != 0)
        {
            //�����������ʼλ��
            float angle = Vector3.SignedAngle(transform.forward, localRight * horizontalInput, transform.up);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        ////////////////////////////////////////////////////////���ƽ�ɫ�ƶ�////////////////////////////////////////////////////////
        Vector3 moveVector = transform.GetChild(0).forward * Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput)) * moveSpeed * Time.deltaTime;
        transform.position += (moveVector);
        //���ö���
        animator.SetBool("IsRun", verticalInput != 0 || horizontalInput != 0);
        //����
        Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.blue * 0.5f);
        Debug.DrawRay(transform.position, transform.right * 0.5f, Color.red * 0.5f);
        Debug.DrawRay(transform.position, moveVector * 10, Color.white);
        ////////////////////////////////////////////////////////������������Ƕ�///////////////////////////////////////////////////
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, 2))
        {
            if (hitInfo.collider != null)
            {
                var normal = hitInfo.normal;
                gravity = -normal * 10;
                //����д����ֱ������up����
                //transform.up = Vector3.Lerp(transform.up, normal, Time.deltaTime * 5);
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normal);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation * transform.rotation, Time.deltaTime * 5);
            }
        }
        GetComponent<Rigidbody>().AddForce(gravity, ForceMode.Acceleration);
        Debug.DrawRay(transform.position, gravity, Color.cyan);
    }
}