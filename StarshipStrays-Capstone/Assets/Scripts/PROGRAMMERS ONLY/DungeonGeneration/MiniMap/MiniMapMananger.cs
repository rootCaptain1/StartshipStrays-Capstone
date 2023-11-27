using UnityEngine;

namespace MiniMap
{
    public class MiniMapMananger : MonoBehaviour
    {
        [SerializeField] private Camera _miniMapCamera;
        [SerializeField] private int _mapFullscreenScaler;
        [SerializeField] private float _mapCamZoomOutOrthoSize;

        private Transform _parentTransform;
        private RectTransform _mapRectTransform;
        private Vector2 _mapFullscreenScaleSize;
        private Vector2 _initialMapPosition;
        private Vector2 _initialMapScale;
        private Vector3 _localCameraPosition;
        private Vector3 _cameraCenteredPosition = new Vector3(0, 0, -10);
        private float _miniMapCameraOrthoSize;
        private bool _isFullscreen;

        private void Start()
        {
            _mapRectTransform = GetComponent<RectTransform>();
            _parentTransform = _miniMapCamera.transform.parent;

            _initialMapPosition = _mapRectTransform.anchoredPosition;
            _initialMapScale = _mapRectTransform.localScale;
            _mapFullscreenScaleSize = _initialMapScale * _mapFullscreenScaler;
            _miniMapCameraOrthoSize = _miniMapCamera.orthographicSize;

            // Record initial local position of the camera relative to its parent
            _localCameraPosition = _miniMapCamera.transform.localPosition;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (!_isFullscreen)
                {
                    // Detach camera, set its position, and adjust RectTransform
                    _miniMapCamera.transform.SetParent(null);
                    _miniMapCamera.transform.localPosition = _cameraCenteredPosition;
                    _mapRectTransform.anchoredPosition = Vector2.zero;
                    _mapRectTransform.localScale = _mapFullscreenScaleSize;
                    _miniMapCamera.orthographicSize = _mapCamZoomOutOrthoSize;
                }
                else
                {
                    // Reattach camera to its original parent, reset position, and adjust RectTransform
                    _miniMapCamera.transform.SetParent(_parentTransform);
                    _miniMapCamera.transform.localPosition = _localCameraPosition;
                    _mapRectTransform.anchoredPosition = _initialMapPosition;
                    _mapRectTransform.localScale = _initialMapScale;
                    _miniMapCamera.orthographicSize = _miniMapCameraOrthoSize;
                }

                _isFullscreen = !_isFullscreen; // Toggle the fullscreen state
            }
        }
    }
}