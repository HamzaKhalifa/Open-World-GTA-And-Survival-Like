using UnityEngine;

public class PlayerObstacleMount : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _obstacleMountChecker = null;
    [SerializeField] private float _checkDistance = 1;

    [Header("Tiny Obstacles")]
    [SerializeField] private float _tinyObstacleThreshold = .3f;
    [SerializeField] private float _tinyObstacleMinimumThreshold = .5f;

    private Player _player = null;

    private Vector3 _obstacleMountCheckerInitialPosition = Vector3.zero;
    private RaycastHit _hitInfo;

    public float ObstacleHeight => _obstacleMountChecker.transform.localPosition.y - _obstacleMountCheckerInitialPosition.y;

    public bool CanMountTinyObstacle => ObstacleHeight <= _tinyObstacleThreshold
        && ObstacleHeight > 0
        && ObstacleHeight >= _tinyObstacleMinimumThreshold
        && IsFacingObstacle;

    public bool IsFacingObstacle => Vector3.Angle(-_player.transform.forward, _hitInfo.normal) <= 20f;

    private void Awake()
    {
        _player = GetComponent<Player>();

        _obstacleMountCheckerInitialPosition = _obstacleMountChecker.localPosition;
    }

    private void Update()
    {
        //Debug.Log(ObstacleHeight);
        CheckForEdge();
    }

    private void CheckForEdge()
    {
        bool reachedEdge = false;
        Vector3 currentPositionToCheck = _obstacleMountCheckerInitialPosition;
        Vector3 lastValidPosition = _obstacleMountCheckerInitialPosition;

        while (!reachedEdge)
        {
            if (Physics.Raycast(currentPositionToCheck + transform.position, _obstacleMountChecker.transform.forward, out _hitInfo, _checkDistance, _layerMask))
            {
                lastValidPosition = currentPositionToCheck;
                currentPositionToCheck = new Vector3(currentPositionToCheck.x, currentPositionToCheck.y + .05f, currentPositionToCheck.z);
            }
            else
            {
                reachedEdge = true;
            }
        }

        _obstacleMountChecker.transform.localPosition = lastValidPosition;
    }
}
