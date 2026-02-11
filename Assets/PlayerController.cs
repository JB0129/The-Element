using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Camera camera;
    void Start()
    {
        camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        handleMovement();
        handleRotation();
        handleDash();
        handleSpeedUp();
    }

    // 1. 일단 WASD로 움직이게 하자
    [SerializeField]
    private float moveSpeed = 5f; // 기본 이동속도
    Vector3 moveDir;
    private void handleMovement()
    {
        if (isDashing) return;
        if (!controller.isGrounded) return;

        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        // // 캐릭 중심 이동
        // moveDir = transform.right * xInput + transform.forward * zInput;

        // 카메라 중심 이동
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();
        moveDir = cameraRight * xInput + cameraForward * zInput;

        moveDir = Vector3.ClampMagnitude(moveDir, 1f);
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // 중력 처리
    }


    // 2. 마우스 이동에 따라 카메라 방향 이동
    [SerializeField]
    private float sensitivity = 500f;
    private float xRotation = 0f;
    private void handleRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        // 좌우 회전 제어 (플레이어 몸체)
        transform.Rotate(Vector3.up * mouseX);

        // 상하 회전 제어 (카메라)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f); // 위 아래 70도 제한
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // 3. Space 누르면 일정 거리 빠른 이동
    [SerializeField]
    private float dashDistance = 5f;
    [SerializeField]
    private float dashDuration = 0.3f;
    private bool isDashing = false;
    private void handleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        // 대쉬 중인가?
        isDashing = true;

        // 대쉬 방향
        Vector3 dashDir;


        if (moveDir.magnitude != 0)
        {
            // 이동 중이면
            dashDir = moveDir.normalized;
        }
        else
        {
            // 멈춰 있으면
            dashDir = transform.forward;
        }

        // 대쉬 거리/시간/속도
        float dashSpeed = dashDistance / dashDuration;

        // 경과 시간
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            controller.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }

    // 4. Shift 누르면서 이동 시 이동속도 증가
    private float walkSpeed = 5f; // 걸을 때
    private float runSpeed = 5f; // 달릴 때
    private void handleSpeedUp()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }
}
