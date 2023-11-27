using DungeonGeneration;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;                    // A static reference to the CameraController instance, making it accessible globally

    [SerializeField] private float _timeToTransition = 5f;      // The time it takes for the camera to transition between rooms
    private float _timeElapsed;                                 // The elapsed time during camera transition
    private bool _switchingCams;                                // A flag indicating if the camera is currently transitioning between rooms
    private Vector3 _targetPosition = new Vector3(0, 0, -10);   // The target position for the camera

    private void Awake()
    {
        // Singleton for the camera
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_switchingCams)
        {
            UpdateCameraPosition();
        }
    }

    private void UpdateCameraPosition()
    {
        if (_timeElapsed < _timeToTransition)
        {
            // Interpolate the camera's position towards the target position using Lerp
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _timeElapsed / _timeToTransition);
            _timeElapsed += Time.deltaTime; // Update the elapsed time
        }
        else
        {
            transform.position = _targetPosition; // Snap the camera to the target position when the transition is complete
        }
    }

    // Moves the camera to a certain room based on direction
    public void MoveCamera(Direction direction)
    {
        // Set the current camera position to the target position
        // Used just in case the camera hasn't reached the room by time the room has to transition again
        transform.position = _targetPosition;

        // Calculate the new target position by adding the direction movement vector
        _targetPosition = transform.position + (Vector3)DungeonCrawlerController.directionMovementMap[direction];

        _switchingCams = true; // Set the switchingCams flag to initiate the camera transition
        _timeElapsed = 0; // Reset the elapsed time for the new transition
    }
}
