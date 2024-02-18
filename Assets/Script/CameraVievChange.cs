using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVievChange : MonoBehaviour
{
    //是否正在改变中
    private bool isChanging = false;
    //触发视野改变
    public bool isChangeProjection = false;
    public float projectionChangeTime = 0.5f;

    private float currentT = 0.0f;
    //存放当前摄像机的透视和正视矩阵信息
    /// <summary>
    /// 透视矩阵
    /// </summary>
    private Matrix4x4 persMat;
    /// <summary>
    /// 正交矩阵
    /// </summary>
    private Matrix4x4 orthoMat;
    private void Update()
    {
        if (isChanging)
        {
            isChangeProjection = false;
        }
        else if (isChangeProjection)
        {
            isChanging = true;
            currentT = 0.0f;
        }
    }

    private void LateUpdate()
    {
        if (!isChanging) return;
        //将当前的 是否正视图值 赋值给currentlyOrthographic变量
        bool currentlyOrthographic = Camera.main.orthographic;
        if (currentlyOrthographic)
        {
            orthoMat = Camera.main.projectionMatrix;
            Camera.main.orthographic = false;
            Camera.main.ResetProjectionMatrix();
            persMat = Camera.main.projectionMatrix;
        }
        else //否则当前摄像机为透视状态
        {
            persMat = Camera.main.projectionMatrix;
            Camera.main.orthographic = true;
            Camera.main.ResetProjectionMatrix();
            orthoMat = Camera.main.projectionMatrix;
        }
        Camera.main.orthographic = currentlyOrthographic;

        currentT += (Time.deltaTime / projectionChangeTime);
        if (currentT < 1.0f)
        {
            if (currentlyOrthographic)
            {
                Camera.main.projectionMatrix = MatrixLerp(orthoMat, persMat, currentT * currentT);
            }
            else
            {
                Camera.main.projectionMatrix = MatrixLerp(persMat, orthoMat, Mathf.Sqrt(currentT));
            }
        }
        else
        {
            isChanging = false;
            Camera.main.orthographic = !currentlyOrthographic;
            Camera.main.ResetProjectionMatrix();
        }
    }

    private Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float t)
    {
        t = Mathf.Clamp(t, 0.0f, 1.0f);
        Matrix4x4 newMatrix = new Matrix4x4();
        newMatrix.SetRow(0, Vector4.Lerp(from.GetRow(0), to.GetRow(0), t));
        newMatrix.SetRow(1, Vector4.Lerp(from.GetRow(1), to.GetRow(1), t));
        newMatrix.SetRow(2, Vector4.Lerp(from.GetRow(2), to.GetRow(2), t));
        newMatrix.SetRow(3, Vector4.Lerp(from.GetRow(3), to.GetRow(3), t));
        return newMatrix;
    }
}
