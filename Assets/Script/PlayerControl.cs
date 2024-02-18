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
        ////////////////////////////////////////////////////////控制摄像机位置////////////////////////////////////////////////////
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mouseX += Input.GetAxis("Mouse X") * mouseXSpeed * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * mouseYSpeed * Time.deltaTime;
        //限制数值
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        mouseX = Mathf.Repeat(mouseX + 180f, 360f) - 180f;
        mouseY = Mathf.Clamp(mouseY, -12, 60);
        //计算坐标
        float x = Mathf.Sin(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        float y = Mathf.Sin(mouseY * Mathf.Deg2Rad);
        float z = -Mathf.Cos(-mouseX * Mathf.Deg2Rad) * Mathf.Cos(mouseY * Mathf.Deg2Rad);
        //获得当前玩家角色的角度
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles);
        followCameraPos.transform.position = rotation * new Vector3(x, y, z) * distance + transform.position;
        followCameraPos.transform.LookAt(transform.position, transform.up);
        Camera.main.transform.position = followCameraPos.transform.position;
        Camera.main.transform.eulerAngles = followCameraPos.transform.eulerAngles;
        ////////////////////////////////////////////////////////控制角色朝向//////////////////////////////////////////////////////
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        //计算在不同摄像机角度、不同人物角度下人物四个方向的朝向
        Vector3 localRight = -Vector3.Cross(Camera.main.transform.forward, transform.up).normalized;
        Vector3 localForward = Vector3.Cross(localRight, transform.up).normalized;
        Debug.DrawRay(transform.position, localForward, Color.blue);
        Debug.DrawRay(transform.position, localRight, Color.red);
        if (verticalInput != 0)
        {
            //控制摄像机初始位置
            float angle = Vector3.SignedAngle(transform.forward, localForward * verticalInput, transform.up);
            Debug.Log("角度为" + angle);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        if (horizontalInput != 0)
        {
            //控制摄像机初始位置
            float angle = Vector3.SignedAngle(transform.forward, localRight * horizontalInput, transform.up);
            transform.GetChild(0).localEulerAngles = new Vector3(0, angle, 0);
        }
        ////////////////////////////////////////////////////////控制角色移动////////////////////////////////////////////////////////
        Vector3 moveVector = transform.GetChild(0).forward * Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput)) * moveSpeed * Time.deltaTime;
        transform.position += (moveVector);
        //设置动画
        animator.SetBool("IsRun", verticalInput != 0 || horizontalInput != 0);
        //绘制
        Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.blue * 0.5f);
        Debug.DrawRay(transform.position, transform.right * 0.5f, Color.red * 0.5f);
        Debug.DrawRay(transform.position, moveVector * 10, Color.white);
        ////////////////////////////////////////////////////////让人物跟随地面角度///////////////////////////////////////////////////
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, 2))
        {
            if (hitInfo.collider != null)
            {
                var normal = hitInfo.normal;
                gravity = -normal * 10;
                //错误写法，直接设置up朝向
                //transform.up = Vector3.Lerp(transform.up, normal, Time.deltaTime * 5);
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normal);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation * transform.rotation, Time.deltaTime * 5);
            }
        }
        GetComponent<Rigidbody>().AddForce(gravity, ForceMode.Acceleration);
        Debug.DrawRay(transform.position, gravity, Color.cyan);
    }
}