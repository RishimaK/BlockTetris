using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // public BlockCreator.BlockType BlockType { get; set; }
    // public AudioManager audioManager;
    // public AdsManager adsManager;
    private Vector2 FirstBlockPosition;
    private Vector2 LimitHeightTouch;
    private float blockSize;
    private Camera mainCamera;

    public bool enabledTouch = false;

    public bool allowMoveDown = false;
    [SerializeField] private BlockCreator blockCreator;
    [SerializeField] private GameManager gameManager;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(Vector2 firstBlockPosition, float size, float MapFrameY)
    {
        FirstBlockPosition = firstBlockPosition;
        LimitHeightTouch = new Vector2(-FirstBlockPosition.y + MapFrameY * 2 + size / 2, firstBlockPosition.y - size / 2);
        // blockSize = GetMeshSize(transform.GetChild(0)).x;
        blockSize = size;
    }

    // Vector3 GetMeshSize(Transform block)
    // {
    //     MeshFilter meshFilter = block.GetComponent<MeshFilter>();
    //     if (meshFilter != null && meshFilter.mesh != null)
    //     {
    //         // Lấy kích thước nguyên bản của mesh
    //         Vector3 size = meshFilter.mesh.bounds.size;
    //         // Debug.Log("Kích thước mesh: " + size);

    //         // Kích thước thực tế với scale
    //         Vector3 scaledSize = Vector3.Scale(size, block.transform.localScale);
    //         // Debug.Log("Kích thước thực tế: " + scaledSize);
    //         return scaledSize;
    //     }
    //     return Vector3.zero;
    // }

    public void StartMoveDown()
    {
        speed = 5;
        allowMoveDown = true;
        enabledTouch = true;
        currentPosX = transform.position.x;
        // CoroutineMoveBlock = StartCoroutine(MoveBlock(blockSize));
    }

    public void StopAllAction()
    {
        allowMoveDown = false;
        enabledTouch = false;
    }

    // Xoay khối 90 độ theo trục Y
    public void RotateBlock()
    {
        float angle = -90f * Mathf.Deg2Rad;
        float cosAngle = Mathf.Cos(angle); // 0 cho 90 độ
        float sinAngle = Mathf.Sin(angle); // 1 cho 90 độ

        List<Vector3> listNewPosition = new List<Vector3>();
        int length = transform.childCount;
        for (int i = 0; i < length; i++)
        {
            // Vị trí hiện tại của con so với cha (local position)
            Vector3 currentLocalPos = transform.GetChild(i).localPosition;

            // Tính vị trí mới sau khi xoay
            float newX = currentLocalPos.x * cosAngle - currentLocalPos.y * sinAngle;
            float newY = currentLocalPos.x * sinAngle + currentLocalPos.y * cosAngle;

            // Vị trí mới trong không gian cục bộ của cha
            listNewPosition.Add(new Vector3(newX, newY, currentLocalPos.z));
        }

        if (!blockCreator.CheckBlockTouchWhenRotate(listNewPosition, transform, -speed * Time.deltaTime)) return;
        // audioManager.PlaySFX("RotateBlock");
        for (int i = 0; i < listNewPosition.Count; i++)
        {
            transform.GetChild(i).localPosition = listNewPosition[i];
        }
        blockCreator.SetGhostBLockPosition();
    }

    bool CheckWallKick(float nextPositionX)
    {
        float min = FirstBlockPosition.x;
        float max = -FirstBlockPosition.x;
        int length = transform.childCount;

        float xx = currentPosX - transform.position.x;
        for (int i = 0; i < length; i++)
        {
            float childPositionX = transform.GetChild(i).position.x + nextPositionX + xx;
            if (Math.Round(childPositionX - min) < 0 ||
                Math.Round(max - childPositionX) < 0) return false;
        }

        return blockCreator.CheckBlockTouch(nextPositionX, transform, -speed * Time.deltaTime);
    }


    // private float timer = 0.01f;
    public float speed = 5;
    private float waitTime = 0.15f;

    void CheckBlockMove()
    {
        // if (!allowMoveDown) return;
        // float timer = Time.fixedDeltaTime;
        float timer = Time.deltaTime;
        float distance = speed * timer;
        float trueDistance = blockCreator.CheckNextPosition(distance);
        if (trueDistance == distance)
        {
            waitTime = 0.15f;
            transform.position += new Vector3(0, trueDistance, 0);
            // transform.DOMove(new Vector3(transform.position.x, transform.position.y + trueDistance, transform.position.z), timer);
            // transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + trueDistance, 0), 0.1f);
            // transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x, transform.position.y + trueDistance, 0), 0.1f);
        }
        else
        {
            if (waitTime == 0.15f)
            {
                // audioManager.PlaySFX("PutDownBlock");
                transform.position += new Vector3(0, trueDistance, 0);
                // transform.DOMove(new Vector3(transform.position.x, transform.position.y + trueDistance, transform.position.z), timer);
                if (speed != 5) waitTime = 0;
            }
            if (waitTime > 0)
            {
                waitTime -= timer;
                waitTime -= Time.deltaTime;
                return;
            }
            else
            {
                allowMoveDown = false;
                // StopTrailRenderer();
                if (speed == 1500) ShakeMap();
                blockCreator.LockPlayerBlock();
            }
        }
    }

    // void MovePlayerBlock()
    // {
    //     blockCreator.MovePlayerBlock();
    // }

    void ShakeMap()
    {
        Transform gameManager = transform.parent;
        gameManager.transform.DOMove(new Vector3(0, -0.5f, 0), 0.1f).SetEase(Ease.OutQuad);
        gameManager.transform.DOMove(new Vector3(0, 0, 0), 0.1f).SetDelay(0.1f).SetEase(Ease.InQuad);

        // adsManager.Vibrate(100, 40);
    }

    private Vector2 PastTouch = Vector2.zero;
    private Vector2 CurrentTouch = Vector2.zero;
    private bool TouchMove = false;
    private float countTimetouch = 0;
    float sensitivity = 2.85f;
    private bool isTouchBegan = false;

    void FixedUpdate()
    {
        // if (allowMoveDown) CheckBlockMove();
    }

    void Update()
    {
        if (allowMoveDown) CheckBlockMove();

        if (enabledTouch) CheckForInput(); 
    }

    void CheckForInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPhase touchPhase = touch.phase;
            // CurrentTouch = ConvertPixelToWorldUnit(touch.position);
            // if (speed == 5 && (CurrentTouch.y > LimitHeightTouch.x || CurrentTouch.y < LimitHeightTouch.y)) return;

            switch (touchPhase)
            {
                case TouchPhase.Began: HandleTouchBegan(touch); break;
                case TouchPhase.Moved: HandleTouchMoved(touch); break;
                case TouchPhase.Stationary: HandleTouchStationary(); break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled: HandleTouchEnded(); break;
            }
        }
        else if (TouchMove)
        {
            HandleTouchEnded();
        }
    }

    void HandleTouchBegan(Touch touch)
    {
        CurrentTouch = ConvertPixelToWorldUnit(touch.position);
        if (speed == 5 && (CurrentTouch.y > LimitHeightTouch.x || CurrentTouch.y < LimitHeightTouch.y)) return;

        PastTouch = CurrentTouch;
        isTouchBegan = true;
        // TouchMove = false;
        // speed = 5;
    }

    void HandleTouchMoved(Touch touch)
    {
        if (!isTouchBegan) HandleTouchBegan(touch);
        CurrentTouch = ConvertPixelToWorldUnit(touch.position);
        Vector2 TouchDelta = CurrentTouch - PastTouch;
        float absX = Mathf.Abs(TouchDelta.x);
        float absY = Mathf.Abs(TouchDelta.y);
        if (absX >= sensitivity && absX > absY)
        {
            TouchMove = true;
            countTimetouch = 0;
            speed = 5;
            // Debug.Log((int)(TouchDelta.x / sensitivity));
            if (TouchDelta.x < 0 && (MandatoryDirection == "" || MandatoryDirection == "left"))
            {
                // move left
                if (CheckWallKick(-blockSize))
                {
                    MoveBlockToSides(-blockSize);
                    if (MandatoryDirection == "left") CheckTutorialChildPosition(); 
                } else PastTouch = CurrentTouch + new Vector2(sensitivity + 0.01f, 0);
            }
            else if (TouchDelta.x > 0 && (MandatoryDirection == "" || MandatoryDirection == "right"))
            {
                // move right
                if (CheckWallKick(blockSize))
                {
                    MoveBlockToSides(blockSize);
                    if (MandatoryDirection == "right") CheckTutorialChildPosition();
                } else PastTouch = CurrentTouch - new Vector2(sensitivity + 0.01f, 0);
            }
        }
        else if (TouchDelta.y > 0 && MandatoryDirection == "")
        {
            // move down
            if(speed == 70) countTimetouch += Time.deltaTime;
            if (absY >= 2.5f)
            {
                speed = 70;
                TouchMove = true;

                // enabledTouch = false;
                // MovePlayerBlock();
                PastTouch = CurrentTouch;
            }
        }
    }

    void HandleTouchStationary()
    {
        if (TouchMove && MandatoryDirection == "")
        {
            if (Mathf.Abs(PastTouch.x - CurrentTouch.x) >= sensitivity)
            {
                speed = 5;
                // TryMoveHorizontal(TouchDelta.x);
                if (PastTouch.x > CurrentTouch.x && CheckWallKick(-blockSize))
                {
                    // move left
                    MoveBlockToSides(-blockSize);
                }
                else if (PastTouch.x < CurrentTouch.x && CheckWallKick(blockSize))
                {
                    // move right
                    MoveBlockToSides(blockSize);
                }
            }
        }
    }

    public float currentPosX;
    private Coroutine CoroutineMoveBlock;

    void MoveBlockToSides(float xOffset)
    {
        if(CoroutineMoveBlock != null) StopCoroutine(CoroutineMoveBlock);
        CoroutineMoveBlock = StartCoroutine(MoveBlock(xOffset));


        // transform.position += new Vector3(xOffset, 0, 0);
        // blockCreator.SetGhostBLockPosition();
        // PastTouch = CurrentTouch;
    }

    IEnumerator MoveBlock(float xOffset)
    {
        currentPosX += xOffset;
        float elapsed = 0f;
        float duration = 0.1f;
        float firstPositionX = transform.position.x;
        PastTouch = CurrentTouch;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(
                new Vector3(firstPositionX, transform.position.y, transform.position.z),
                new Vector3(currentPosX, transform.position.y, transform.position.z),
                elapsed / duration);
            yield return null;
        }
        transform.position = new Vector3(currentPosX, transform.position.y, transform.position.z);
        // blockCreator.SetGhostBLockPosition();
    }

    void HandleTouchEnded()
    {
        Vector2 TouchDelta = CurrentTouch - PastTouch;
        float absY = Mathf.Abs(TouchDelta.y);

        if (countTimetouch <= 0.15f && (speed == 70 ||
            ((MandatoryDirection == "down" || MandatoryDirection == "toolDown") &&
            absY >= 2.5 && PastTouch.y < CurrentTouch.y && Mathf.Abs(TouchDelta.x) < Mathf.Abs(TouchDelta.y))))
        {
            speed = 1500;
            enabledTouch = false;
            // PlayTrailRenderer();
            if (MandatoryDirection == "down")
            {
                allowMoveDown = true;
                CheckEndTutorialChildPosition();
            }
            else if (MandatoryDirection == "toolDown")
            {
                allowMoveDown = true;
                MandatoryDirection = "";
            }
        }
        else speed = 5;

        if (!TouchMove && absY <= 0.1f && (MandatoryDirection == "" || MandatoryDirection == "rotate"))
        {
            RotateBlock();
            if (MandatoryDirection == "rotate") SetTutorial(3);
        }
        TouchMove = false;
        countTimetouch = 0;
        isTouchBegan = false;
    }

    public void DisableTouch()
    {
        enabledTouch = false;
    }

    public void EnabledTouch()
    {
        enabledTouch = true;
    }

    Vector2 ConvertPixelToWorldUnit(Vector2 pixelPosition)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(
            new Vector3(pixelPosition.x, pixelPosition.y, 10 + transform.position.z)
        );

        return worldPoint;
    }

    void PlayTrailRenderer()
    {
        foreach (Transform child in transform)
        {
            bool haveBlockUpper = CheckBlock(child);
            if (!haveBlockUpper)
            {
                TrailRenderer trailRenderer = child.GetComponent<TrailRenderer>();
                trailRenderer.enabled = true;
            }
        }
    }

    bool CheckBlock(Transform child)
    {
        foreach (Transform item in transform)
        {
            if (item.position.x == child.position.x && item.position.y > child.position.y)
            {
                return true;
            }
        }

        return false;
    }

    void StopTrailRenderer()
    {
        foreach (Transform child in transform)
        {
            // yield return new WaitForSeconds(0.15f);
            // child.GetComponent<TrailRenderer>().enabled = false;
            StartCoroutine(TurnOffTrailRenderer(child.GetComponent<TrailRenderer>()));
        }
    }

    IEnumerator TurnOffTrailRenderer(TrailRenderer block)
    {
        yield return new WaitForSeconds(0.15f);
        block.enabled = false;
    }

    string MandatoryDirection = "";
    int countRotate = 0;
    public void SetTutorial(int num)
    {
        // allowMoveDown = false;
        // if (num == 1)
        // {
        //     countRotate = 0;
        //     MandatoryDirection = "left";
        // }
        // else if (num == 2)
        // {
        //     MandatoryDirection = "rotate";
        // }
        // else if (num == 3)
        // {
        //     countRotate++;
        //     if (countRotate == 2)
        //     {
        //         MandatoryDirection = "right";
        //         gameManager.PlayTutorialAnimation("Right");
        //     }
        // }
        // else if (num == 4)
        // {
        //     allowMoveDown = false;
        //     MandatoryDirection = "toolDown";
        // }
    }

    void CheckTutorialChildPosition()
    {
        // int i = Mathf.RoundToInt((transform.GetChild(0).position.x - FirstBlockPosition.x) / blockSize);
        // // TouchMove = false;
        // PastTouch = CurrentTouch;
        // if (i == 1 || i == 7)
        // {
        //     MandatoryDirection = "down";
        //     gameManager.PlayTutorialAnimation("Down");
        // }
    }

    void CheckEndTutorialChildPosition()
    {
        // int i = Mathf.RoundToInt((transform.GetChild(0).position.x - FirstBlockPosition.x) / blockSize);
        // if (i == 7)
        // {
        //     MandatoryDirection = "";
        //     countRotate = 0;
        //     gameManager.TurnOffTutorial();
        // }
    }
}
