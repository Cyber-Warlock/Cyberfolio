using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBroker : MonoBehaviour
{
    static Vector3 axisInput;

    static bool jump;

    static float mouseX;
    static float mouseY;
    static float xRot;

    static Vector3 mouseOutput;

    float sensitivity;

    // Start is called before the first frame update
    void Start()
    {
        axisInput = Vector3.zero;
        jump = false;
        xRot = 0f;
        sensitivity = GetComponent<Player>().MouseSensitivityFactor;
    }

    // Update is called once per frame
    void Update()
    {
        axisInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    public static Vector3 GetAxis()
    {
        return axisInput;
    }
    public static void SetAxis(Vector3 fakeAxis)
    {
        axisInput = fakeAxis;
    }

    public static Vector3 GetMouseInput()
    {
        xRot = Mathf.Clamp(xRot -= mouseY, -80f, 60f);
        mouseOutput = new Vector3(xRot, mouseX);
        return mouseOutput;
    }
    /// <summary>
    /// Simulates mouse GetAxis input
    /// </summary>
    /// <param name="fakeMouse">fakeMouse.x represents MouseX and is for horizontal rotation. fakeMouse.y represents MouseY and is for vertical rotation</param>
    public static void SetMouseInput(Vector3 fakeMouse)
    {
        mouseX = fakeMouse.x;
        mouseY = fakeMouse.y;
    }

    public static bool GetJump()
    {
        return jump;
    }

    public static void SetJump(bool jumpState)
    {
        jump = jumpState;
    }
}
