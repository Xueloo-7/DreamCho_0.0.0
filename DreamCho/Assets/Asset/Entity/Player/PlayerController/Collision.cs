using UnityEngine;

namespace DreamCho {
    public class Collision : MonoBehaviour
    {
        #region Reference
        [Header("Collision")]
        [SerializeField] Vector2 bottomSize = new Vector2(.5f, .1f);
        [SerializeField] Vector2 rightSize = new Vector2(.1f, .95f);
        [SerializeField] Vector2 leftSize = new Vector2(.1f, .95f);

        [SerializeField]
        float bottomOffset = .5f,
                rightOffset = .5f,
                leftOffset = .5f;
        public GameObject TouchObj { get; private set; }
        public int DetectLayer { get; private set; }
        #endregion



        #region State
        public static string ignoreDetect;
        public bool OnGround { get; private set; }
        public bool OnHead { get; private set; }
        public bool OnWall { get; private set; }
        public bool OnRightWall { get; private set; }
        public bool OnLeftWall { get; private set; }
        public int WallSide { get; private set; }
        #endregion



        private void Start()
        {
            DetectLayer = LayerMask.GetMask("Ground", "Obstacle");
            if (DetectLayer == 0) Debug.LogError("Ground layer is not exist!");
        }
        void Update()
        {
            RaycastHit2D groundHit = Physics2D.BoxCast(transform.position, bottomSize, 0, -Vector2.up, bottomOffset, DetectLayer);

            if (ignoreDetect != "Ground") OnGround = groundHit.collider != null;
            else OnGround = false;

            if (OnGround) TouchObj = groundHit.collider.gameObject;

            OnHead = Physics2D.BoxCast(transform.position, bottomSize, 0, Vector2.up, bottomOffset, DetectLayer);

            OnWall = Physics2D.BoxCast(transform.position, rightSize, 0, Vector2.right, rightOffset, DetectLayer)
                || Physics2D.BoxCast(transform.position, leftSize, 0, Vector2.left, leftOffset, DetectLayer);

            OnRightWall = Physics2D.BoxCast(transform.position, rightSize, 0, Vector2.right, rightOffset, DetectLayer);
            OnLeftWall = Physics2D.BoxCast(transform.position, leftSize, 0, Vector2.left, leftOffset, DetectLayer);

            if (OnRightWall) WallSide = 1;
            else if (OnLeftWall) WallSide = -1;
            else WallSide = 0;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position - Vector3.up * bottomOffset, bottomSize); // Ground BoxCast
            Gizmos.DrawWireCube(transform.position + Vector3.up * bottomOffset, bottomSize); // Head BoxCast
            Gizmos.DrawWireCube(transform.position + Vector3.right * rightOffset, rightSize);
            Gizmos.DrawWireCube(transform.position + Vector3.left * leftOffset, leftSize);
        }
    }
}

