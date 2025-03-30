using DG.Tweening;
using System.Collections;
using UnityEngine;
using DreamCho;
using System;


[RequireComponent(typeof(Rigidbody2D), typeof(DreamCho.Collision), typeof(Collider2D))]
// 其他所需组件：XMath, Object Pool(ghost), Key(input), GameManager(AppMode), CameraController(Shake), AudioManager(Fx)
public class PlayerController : MonoBehaviour
{
    struct DefaultParams // 控制参数的默认值，用 Parameter 初始化，之后如果需要恢复默认值可以用这个默认值
    {
        public float moveSpeed;
        public float move_deceleration;
        public float fallAcceleration;
        public float jumpForce;
        public float bounceJumpForce;
        public float jumpBufferTime;
        public float bounceBufferTime;
        public float fallMultiplier;
        public float lowJumpMultiplier;
        public float maxFallSpeed;
        public float coyoteTime;
        public float dashForce;
        public float dashTime;
        public float dashCold;
        public float dashBufferTime;
        public float dashFrameStop;
        public float dashGroundCold;
        public float dashShakeStrength;
        public float dashShakeFrequency;
        public float dashShakeDuration;
        public float hyperJumpForce;
        public float hyperJump_moveForce;
        public float superJumpTimeForDash;
        public float ghostInterval;
        public float ghostFadeOut;
        public int ghostCount;
        public AnimationCurve dashRotateCurve;
        public AnimationCurve dashForceCurve;
        public float maxDashEnergy;  // ★ 修改拼写
        public float defMaxDash;
        public float dashEnergyRestoreSpeed;  // ★ 补充
    }


    #region References
    [Header("Parameter")]
    [SerializeField] private PlayerControlParamater P;
    public PlayerControlParamater ControlParameter { get { return P; } }
    private DefaultParams defaultParams;

    [Header("Reference")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem par_dash, par_slide, par_walk;
    [SerializeField] private SpriteRenderer[] dirHint;
    [SerializeField] private SpriteRenderer sr;

    private DreamCho.Collision collision;
    private Rigidbody2D rb;
    private Transform ghost_parent;
    private ParticleSystem.MainModule main;
    #endregion

    #region State Variables
    private bool detectInput = true, hiddenDirHint = false;
    private bool canMove = true, canJump = true, canDash = true, canRoll;
    private bool isWalking, isFalling, isDashing, isDashFalling, isFrameStop, isWallSlide, isRoll;
    private float dashEnergy, coyoteCounter, jumpBufferCounter, bounceBufferCounter, dashBufferCounter, dashGroundCounter, superJumpCounter;
    private Vector2 lastDashDir; // 上一次冲刺的方向
    public float gravityScale = 6;
    private AnimationCurve curRotationCurve;
    private Coroutine rotateCor;
    private Coroutine dashCor;

    public bool DetectInput { get => detectInput; set => detectInput = value; }
    public bool HiddenDirHint { get => hiddenDirHint; set => hiddenDirHint = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool CanJump { get => canJump; set => canJump = value; }
    public bool CanDash { get => canDash; set => canDash = value; }

    #endregion

    #region Event
    public static Action onJump;
    public static Action<Vector2> onDash;
    public static Action<float> increaseEnergy;
    #endregion

    #region WairFor
    private WaitForSeconds w_ignoreGround= new WaitForSeconds(0.1f);
    #endregion

    #region Input Variables
    private bool jump;
    private bool hintOpen;
    private Vector2 hint_last_dir, dirInput;

    public Vector2 DirInput
    {
        get => dirInput;
        set
        {
            dirInput = value;
            UpdateDirectionHint();
        }
    }
    #endregion



    #region 核心
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<DreamCho.Collision>();
        GameObject obj = GameObject.Find("Dash Ghost Parent");
        if (obj != null) ghost_parent = obj.transform;
        else Debug.LogError("Dash Ghost Parent not found!");

        // 注册输入事件
        if (Key.Instance == null)
            Debug.LogWarning("Key 不存在，无法获取输入，请确保场景有一个Key对象!");

        DreamCho.KeyEvent.onJump_Down += GetJumpDown;
        DreamCho.KeyEvent.onJump += GetJump;
        DreamCho.KeyEvent.onDash_Down += GetDashDown;
        onDash += OnDash;
        increaseEnergy += UpdateEnergy;

        // 为部分渲染添加通用材质
        Material mat = Resources.Load<Material>("Dynamic Color");
        if (mat != null)
        {
            sr.material = mat;
            foreach (var sr in dirHint)
                sr.material = mat;
            trail.material = mat;
            par_dash.GetComponent<ParticleSystemRenderer>().material = mat;
            par_walk.GetComponent<ParticleSystemRenderer>().material = mat;
            par_slide.GetComponent<ParticleSystemRenderer>().material = mat;
        }
        else Debug.LogError("材质不存在!");

        // 初始化部分属性
        P.maxDashEnergy = P.defMaxDash;
        dashEnergy = P.maxDashEnergy;
        UpdateEnergyCount((int)P.maxDashEnergy);
        defaultParams = InitializeDefaultParams();
    }
    private void OnDestroy()
    {
        DreamCho.KeyEvent.onJump_Down -= GetJumpDown;
        DreamCho.KeyEvent.onJump -= GetJump;
        DreamCho.KeyEvent.onDash_Down -= GetDashDown;
        onDash -= OnDash;
        increaseEnergy -= UpdateEnergy;
        RestoreAllControlParams(); // 恢复默认参数，避免下次游戏时参数错误
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            UpdateEnergyCount((int)P.maxDashEnergy + 1);
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            UpdateEnergyCount((int)P.maxDashEnergy - 1);
        }

        if (detectInput && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            DirInput = new Vector2(Key.rawAxisX, Key.rawAxisY);
            InputHandle();
        }
        else
        {
            if (dashCor != null)
            {
                StopCoroutine(dashCor);
            }
        }
    }
    private void FixedUpdate()
    {
        // 处理物理移动相关
        if (isDashing == false && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            if (canMove)
            {
                Walk();
                WalkParticle();
            }
            if (!collision.OnGround) BetterJump();
        }

        
    }
    #endregion



    #region 输入检测相关
    private void GetJump(bool value) => jump = value;
    private void GetJumpDown()
    {
        jumpBufferCounter = P.jumpBufferTime;
        bounceBufferCounter = P.bounceBufferTime;
    }
    private void GetDashDown()
    {
        dashBufferCounter = P.dashBufferTime;
    }

    /// <summary>
    /// 检测PC输入和检测附近地形，实现多种操作
    /// </summary>
    void InputHandle()
    {
        if (!collision.OnGround) // 不在地面时
        {
            coyoteCounter -= Time.deltaTime; //减少土狼时间

            if (isRoll == false && !isDashFalling && canRoll)
            {
                if (rotateCor == null)
                {
                    Rotate(DirInput, .3f, P.dashRotateCurve);
                }
                isRoll = true;
            }
        }
        else
        {
            if(isFalling) isFalling = false;
            if (isDashFalling) isDashFalling = false;
            if (isRoll) isRoll = false;
            coyoteCounter = P.coyoteTime;

            // 在地面，恢复冲刺耐力
            if (dashEnergy < P.maxDashEnergy && superJumpCounter <= 0)
            {
                dashEnergy = P.maxDashEnergy;
            }
            else if(dashEnergy > P.maxDashEnergy) dashEnergy = P.maxDashEnergy;
        }

        if (collision.OnWall)
        {
            //抓墙/没有上升下降时停止WallParticle
            WallParticle(rb.linearVelocity.y == 0 ? 0 : collision.OnRightWall ? 1 : -1);
        }
        else WallParticle(0);

        //预输入
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;
        if (dashBufferCounter > 0) dashBufferCounter -= Time.deltaTime;
        if (superJumpCounter > 0) superJumpCounter -= Time.deltaTime;
        if (dashGroundCounter > 0) dashGroundCounter -= Time.deltaTime;
        if (bounceBufferCounter > 0) bounceBufferCounter -= Time.deltaTime;

        //当处于土狼时间且预输入成立则跳跃
        if (canJump && coyoteCounter > 0 && jumpBufferCounter > 0 && !isFrameStop)
        {
            DashEnd();
            Jump();
            
            coyoteCounter = 0;
            jumpBufferCounter = 0;
        }

        // 冲弹机制
        // 当按下跳跃，
        if (bounceBufferCounter > 0 && isDashing)
        {
            // 接触地面，且向下冲刺时，触发上弹
            if (lastDashDir == Vector2.down && collision.OnGround)
                Debug.Log("上弹");
            // 接触天花板，且向上冲刺时，触发下弹
            else if (lastDashDir == Vector2.up && collision.OnHead)
                Debug.Log("下弹");
            // 接触左墙，且向右冲刺时，触发左弹
            else if (lastDashDir == Vector2.right && collision.OnLeftWall)
                Debug.Log("左弹");
            // 接触右墙，且向左冲刺时，触发右弹
            else if (lastDashDir == Vector2.left && collision.OnRightWall)
                Debug.Log("右弹");
        }

        if (canDash && DirInput != Vector2.zero && dashEnergy >= 1 && dashBufferCounter > 0)
        {
            // 只有在地面时才检查 dashGroundCounter
            if (!collision.OnGround || dashGroundCounter <= 0)
            {
                DashEnd();
                dashCor = StartCoroutine(Dash());
                dashBufferCounter = 0;
            }
        }
    }
    #endregion

    #region 移动相关(Walk, Jump, BetterJump)
    void Walk()
    {
        // 平滑过渡 X 轴速度
        float velocityX = Mathf.MoveTowards(rb.linearVelocity.x, DirInput.x * P.moveSpeed, P.move_deceleration * Time.fixedDeltaTime);

        // Y 轴速度仅在低于最大下落速度时做限制
        float velocityY = rb.linearVelocity.y < P.maxFallSpeed ?
                          Mathf.MoveTowards(rb.linearVelocity.y, P.maxFallSpeed, P.fallAcceleration * Time.fixedDeltaTime) :
                          rb.linearVelocity.y;

        rb.linearVelocity = new Vector2(velocityX, velocityY);
    }
    /// 区别长按和短按的跳跃高度
    /// </summary>
    void BetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            if (isFalling == false) isFalling = true; // 进入落下状态
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (P.fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0) //向上的期间松开会立刻向下，
        {
            if (!jump && !isDashFalling && isFalling == false) //不按住跳跃且未开始下落前，则加速下降
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (P.lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }
    }
    void Jump()
    {
        float jumpForce = P.jumpForce;
        bool normalJump = true;

        // 设定起跳速度，比如大跳，凌波微步等
        if (superJumpCounter > 0)
        {
            if (lastDashDir.x != 0 && lastDashDir.y == -1) // 凌波微步(斜下冲刺+跳跃)
            {
                if (lastDashDir.x == DirInput.x) // 确保方向一致
                {
                    dashEnergy++; // 凌波微步不消耗冲刺耐力
                    rb.linearVelocity = new Vector2(DirInput.x * P.hyperJump_moveForce, 0);
                    jumpForce = P.hyperJumpForce;
                    normalJump = false;
                    Debug.Log("凌波微步");
                }
            }
            else if (lastDashDir.x != 0 && lastDashDir.y == 0) // 大跳(冲刺+跳跃)
            {
                Debug.Log("大跳");
                rb.linearVelocity = new Vector2(rb.linearVelocityX + (DirInput.x * P.moveSpeed), 0); // 运用惯性
                normalJump = false;
            }
        }
        if (normalJump) 
            rb.linearVelocity = new Vector2(DirInput.x * P.moveSpeed, 0);

        // 跳跃
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Rotate(DirInput, .3f, P.dashRotateCurve);
        StartCoroutine(IgnoreGround());
    }
    IEnumerator IgnoreGround()
    {
        DreamCho.Collision.ignoreDetect = "Ground";
        yield return w_ignoreGround;
        DreamCho.Collision.ignoreDetect = "";
    }
    #endregion

    #region 冲刺相关(Dash, OnDash)
    void DashEnd()
    {
        if (dashCor != null)
        {
            StopCoroutine(dashCor);

            rb.gravityScale = gravityScale;
            isDashing = false;
            canDash = true;
            isFrameStop = false;
            isDashFalling = false;
            trail.emitting = true;
        }
    }
    IEnumerator Dash()
    {
        if (dirInput == Vector2.zero) yield break;

        lastDashDir = XMath.V_Round(dirInput.normalized);
        Vector2 dashDir = dirInput.normalized * P.dashForce;
        float dashTime = P.dashTime;

        isDashing = true;
        rb.gravityScale = 0;
        canDash = false; // 冷却
        jumpBufferCounter = 0;
        // 在地面且横向冲刺时有冷却
        if (collision.OnGround && dashDir.x != 0 && dashDir.y == 0) dashGroundCounter = P.dashGroundCold;

        superJumpCounter = P.superJumpTimeForDash;

        UpdateEnergy(-1);
        trail.emitting = false;

        

        rb.linearVelocity = Vector2.zero; // 进入停顿

        isFrameStop  = true;
        yield return new WaitForSeconds(P.dashFrameStop);
        isFrameStop = false;

        StartCoroutine(DashGhost());
        onDash?.Invoke(dashDir);

        float elapsedTime = 0f;  // 记录冲刺经过的时间
        float dashCold = P.dashCold;

        if (dashDir.x != 0 && dashDir.y == 0) dashTime *= 1.5f; // 横向冲刺距离稍微长一些

        while (elapsedTime < dashTime)
        {
            if (rb.bodyType == RigidbodyType2D.Static)
                yield break;

            float t = elapsedTime / dashTime;  // 归一化时间 0 ~ 1
            float speedMultiplier = P.dashForceCurve.Evaluate(t); // 计算速度系数（0~1）
            rb.linearVelocity = dashDir * speedMultiplier * Time.fixedDeltaTime; // 控制速度
            elapsedTime += Time.fixedDeltaTime;

            dashCold -= Time.fixedDeltaTime;
            if(dashCold <= 0 && canDash == false) // 冲刺冷却
                canDash = true;

            yield return new WaitForFixedUpdate();
        }

        while (dashCold > 0)
        {
            dashCold -= Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = gravityScale;
        isDashFalling = true; // 确保冲刺后直到落地前都不会被betterjump影响
        isDashing = false;

        yield return new WaitForSeconds(trail.time - P.dashCold - dashTime);

        //trail.Clear();
        trail.emitting = true;
    }
    IEnumerator DashGhost()
    {
        for (int i = 0; i < P.ghostCount; i++)
        {
            SpriteRenderer ghost = ObjectPool.Instance.GetObjectFromPool(sr, ghost_parent);
            ghost.transform.position = sr.transform.position;
            ghost.transform.rotation = sr.transform.rotation;
            ghost.color = sr.color;
            SpriteRenderer childSr = ghost.transform.GetChild(0).GetComponent<SpriteRenderer>();
            childSr.ChangeAlpha(1);
            childSr.DOFade(0, P.ghostFadeOut);
            ghost.DOFade(0, P.ghostFadeOut).OnComplete(() => ObjectPool.Instance.ReturnObjectToPool(ghost, 0));
            yield return new WaitForSeconds(P.ghostInterval);
        }
    }
    void OnDash(Vector2 dashDir)
    {
        par_dash.Play();
        SoundShake.Instance.Shake((Vector2)sr.transform.position - dashDir.normalized, P.dashShakeStrength, P.dashShakeFrequency, P.dashShakeDuration);
        AudioManager.PlayFx("Dash");
        Rotate(dashDir.normalized, .3f, P.dashRotateCurve);
    }
    public float GetDashEnergy()
    {
        return dashEnergy;
    }
    void UpdateEnergyCount(int newValue)
    {
        if (newValue < 0) return;

        dashEnergy = Mathf.Max(0, newValue - (P.maxDashEnergy - dashEnergy));
        P.maxDashEnergy = newValue;
        Event.onEnergyCountUpdate?.Invoke((int)P.maxDashEnergy);
    }
    public void UpdateEnergy(float increase)
    {
        dashEnergy = Mathf.Clamp(dashEnergy + increase, 0, P.maxDashEnergy);
        //Event.onDashEnergyEmpty?.Invoke(dashEnergy >= 1);

    }
    #endregion

    #region 粒子效果(WalkParticle, WallParticle, GrabParticle)
    void WalkParticle()
    {
        if (!isWalking && collision.OnGround && rb.linearVelocity != Vector2.zero)
        {
            main = par_walk.main; main.startColor = Color.white;
            isWalking = true;
        }
        else if (isWalking)
        {
            if (rb.linearVelocity == Vector2.zero || !collision.OnGround)
            {
                main = par_walk.main; main.startColor = Color.clear;
                isWalking = false;
            }
        }
        if (trail.emitting == false) // 传送后会将trail关闭，因此这里需要打开
        {
            trail.Clear();
            trail.emitting = true;
        }
    }
    void WallParticle(int dir)
    {
        if (dir == 0)
        {
            main = par_slide.main; main.startColor = Color.clear;
            isWallSlide = false;

            return;
        }
        if (!isWallSlide && rb.linearVelocity.y != 0)
        {
            main = par_slide.main; main.startColor = Color.white;
            par_slide.transform.parent.localScale = new Vector3(dir, 1, 1);
            isWallSlide = true;
        }
    }
    #endregion

    #region Direction Hint
    private void UpdateDirectionHint()
    {
        if (dirHint.Length == 0) return;

        if (dashEnergy >= 1 && canDash)
        {
            if (dirInput == Vector2.zero)
            {
                if (hintOpen) CloseHint();
            }
            else
            {
                if (hint_last_dir != dirInput) dirHint[0].transform.parent.DORotate(new Vector3(0, 0, -90 + XMath.VecToA(dirInput)), 0.3f, RotateMode.Fast).SetEase(Ease.OutQuart);
                hint_last_dir = dirInput;
                if (!hintOpen) ShowHint();
            }
        }
        else if (hintOpen)
        {
            CloseHint(!canDash && dashEnergy >= 1 ? new Color(0.68f, 0.85f, 0.9f) : Color.red);
        }
    }

    private void ShowHint()
    {
        if (hiddenDirHint) return;

        hintOpen = true;
        for (int i = 0; i < dirHint.Length; i++)
        {
            DOTween.Kill(dirHint[i]);
            /*if(Player.CurrentCharacter != null)
                dirHint[i].DOColor(Player.CurrentCharacter._themeColor, .3f);
            else*/ dirHint[i].DOColor(Color.white, .3f);
        }
    }

    public void CloseHint(Color color = default)
    {
        HintColor(false, color, 0, 1);
    }

    private void HintColor(bool open, Color color, float fade, float duration)
    {
        hintOpen = open;
        for (int i = 0; i < dirHint.Length; i++)
        {
            DOTween.Kill(dirHint[i]);
            if (color != default) dirHint[i].color = color;
            dirHint[i].DOFade(fade, duration);
        }
    }
    #endregion

    #region 旋转相关
    private void Rotate(Vector2 dir, float duration, AnimationCurve rotateCurve) //跳跃旋转角度调整
    {
        Vector2 direction = XMath.V_Round(dir);

        float rotateValue = -90;
        if (direction.x == 0)
            rotateValue = 0;

        rotateValue *= direction.x;

        curRotationCurve = rotateCurve;

        StartRotation(sr.transform.rotation.eulerAngles.z + rotateValue, duration);
    }
    private void StartRotation(float targetAngle, float duration)
    {
        Quaternion initialRotation = sr.transform.rotation;

        // 角度取整
        float dif = initialRotation.eulerAngles.z % 90;

        if (dif < 45 && dif > 0)
        {
            targetAngle -= initialRotation.eulerAngles.z % 90;
        }
        else if (dif < 90 && dif > 45)
        {
            targetAngle += 90 - dif;
        }

        // 确保目标角度是 0, 90, 180 或 270
        targetAngle = Mathf.Round(targetAngle / 90) * 90;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        if (rotateCor != null)
            StopCoroutine(rotateCor);
        rotateCor = StartCoroutine(RotateOverTime(initialRotation, targetRotation, duration));
    }
    private IEnumerator RotateOverTime(Quaternion startRotation, Quaternion endRotation, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            float curveValue = curRotationCurve.Evaluate(progress);
            sr.transform.rotation = Quaternion.Lerp(startRotation, endRotation, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sr.transform.rotation = endRotation; // 确保最终旋转角度达到目标角度
        rotateCor = null;
    }
    #endregion

    #region 状态
    private DefaultParams InitializeDefaultParams()
    {
        DefaultParams defaultParams = new DefaultParams();

        defaultParams.moveSpeed = P.moveSpeed;
        defaultParams.move_deceleration = P.move_deceleration;
        defaultParams.fallAcceleration = P.fallAcceleration;
        defaultParams.jumpForce = P.jumpForce;
        defaultParams.bounceJumpForce = P.bounceJumpForce;
        defaultParams.jumpBufferTime = P.jumpBufferTime;
        defaultParams.bounceBufferTime = P.bounceBufferTime;
        defaultParams.fallMultiplier = P.fallMultiplier;
        defaultParams.lowJumpMultiplier = P.lowJumpMultiplier;
        defaultParams.maxFallSpeed = P.maxFallSpeed;
        defaultParams.coyoteTime = P.coyoteTime;
        defaultParams.dashForce = P.dashForce;
        defaultParams.dashTime = P.dashTime;
        defaultParams.dashCold = P.dashCold; 
        defaultParams.dashBufferTime = P.dashBufferTime;
        defaultParams.dashFrameStop = P.dashFrameStop;
        defaultParams.dashGroundCold = P.dashGroundCold;
        defaultParams.dashShakeStrength = P.dashShakeStrength;
        defaultParams.dashShakeFrequency = P.dashShakeFrequency;
        defaultParams.dashShakeDuration = P.dashShakeDuration;
        defaultParams.hyperJumpForce = P.hyperJumpForce;
        defaultParams.hyperJump_moveForce = P.hyperJump_moveForce;
        defaultParams.superJumpTimeForDash = P.superJumpTimeForDash;
        defaultParams.ghostInterval = P.ghostInterval;
        defaultParams.ghostFadeOut = P.ghostFadeOut;
        defaultParams.ghostCount = P.ghostCount;
        defaultParams.dashRotateCurve = P.dashRotateCurve;
        defaultParams.dashForceCurve = P.dashForceCurve;
        defaultParams.maxDashEnergy = P.maxDashEnergy;
        defaultParams.defMaxDash = P.defMaxDash;
        defaultParams.dashEnergyRestoreSpeed = P.dashEnergyRestoreSpeed; 

        return defaultParams;
    }
    private void RestoreAllControlParams()
    {
        P.moveSpeed = defaultParams.moveSpeed;
        P.move_deceleration = defaultParams.move_deceleration;
        P.fallAcceleration = defaultParams.fallAcceleration;
        P.jumpForce = defaultParams.jumpForce;
        P.bounceJumpForce = defaultParams.bounceJumpForce;
        P.jumpBufferTime = defaultParams.jumpBufferTime;
        P.bounceBufferTime = defaultParams.bounceBufferTime;
        P.fallMultiplier = defaultParams.fallMultiplier;
        P.lowJumpMultiplier = defaultParams.lowJumpMultiplier;
        P.maxFallSpeed = defaultParams.maxFallSpeed;
        P.coyoteTime = defaultParams.coyoteTime;
        P.dashForce = defaultParams.dashForce;
        P.dashTime = defaultParams.dashTime;
        P.dashCold = defaultParams.dashCold;
        P.dashBufferTime = defaultParams.dashBufferTime;
        P.dashFrameStop = defaultParams.dashFrameStop;
        P.dashGroundCold = defaultParams.dashGroundCold;
        P.dashShakeStrength = defaultParams.dashShakeStrength;
        P.dashShakeFrequency = defaultParams.dashShakeFrequency;
        P.dashShakeDuration = defaultParams.dashShakeDuration;
        P.hyperJumpForce = defaultParams.hyperJumpForce;
        P.hyperJump_moveForce = defaultParams.hyperJump_moveForce;
        P.superJumpTimeForDash = defaultParams.superJumpTimeForDash;
        P.ghostInterval = defaultParams.ghostInterval;
        P.ghostFadeOut = defaultParams.ghostFadeOut;
        P.ghostCount = defaultParams.ghostCount;
        P.dashRotateCurve = defaultParams.dashRotateCurve;
        P.dashForceCurve = defaultParams.dashForceCurve;
        P.maxDashEnergy = defaultParams.maxDashEnergy;
        P.defMaxDash = defaultParams.defMaxDash;
        P.dashEnergyRestoreSpeed = defaultParams.dashEnergyRestoreSpeed;
    }

    public void ResetState()
    {
        // 重置各种属性，确保PlayerController能正常运行
        rb.gravityScale = gravityScale;
        detectInput = true;
        canMove = true;
        canJump = true;
        canDash = true;
        isWalking = false;
        isRoll = false;
        isDashing = false;
        isFalling = false;
        isWallSlide = false;
        coyoteCounter = 0;
        jumpBufferCounter = 0;
        dashBufferCounter = 0;
    }
    public void SetMove(float moveSpeed, float moveDeceleration) // 设置新移动速度
    {
        P.moveSpeed = moveSpeed;
        P.move_deceleration = moveDeceleration;
    }
    public void SetMove() // 恢复默认移动速度
    {
        P.moveSpeed = defaultParams.moveSpeed;
        P.move_deceleration = defaultParams.move_deceleration;
    }
    #endregion



    #region External Function
    public void OnEnterCatapult()
    {
        isDashing = false;
        isFalling = false;
        isDashFalling = false;
        isRoll = false;
        canRoll = false;
        trail.emitting = false;
        main = par_walk.main; main.startColor = Color.clear;
    }
    public void OnCatapultShoot_Start(Vector2 direction) // 当被弹射炮发射
    {
        StartCoroutine(CanDashCor()); // 等待一帧后才可以冲刺（ 这样就不会导致和弹射炮的冲刺键冲突 ）
        IEnumerator CanDashCor()
        {
            canDash = false;

            yield return new WaitForEndOfFrame();

            canDash = true;
        }

        isDashing = true;
        canJump = false;
        jumpBufferCounter = 0;
        dashBufferCounter = 0;
        UpdateEnergy(P.maxDashEnergy - GetDashEnergy()); // 补满冲刺机会

        trail.emitting = true;

        Rotate(direction, 0.3f, P.dashRotateCurve);

    }
    public void OnCatapultShoot_End(Vector2 direction) // 当弹射炮发射结束
    {
        //CanDash = true;
        isDashing = false;
        canJump = true;
        rb.gravityScale = gravityScale;
        isDashFalling = true; // 确保冲刺后直到落地前都不会被betterjump影响
        canRoll = true;
    }

    #endregion
}