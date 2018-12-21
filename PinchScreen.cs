using UnityEngine;

namespace SHProject.Ingame
{ 
    public class PinchScreen : MonoBehaviour
    {
        private const float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
        private const float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.
        private Camera _camera = null;
    
        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }
    
        void Update()
        {
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
    
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
    
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
    
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
    
                if (_camera.orthographic)
                {
                    _camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
    
                    _camera.orthographicSize = Mathf.Max(_camera.orthographicSize, 0.1f);
                }
                else
                {
                    _camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                    _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, 0.1f, 179.9f);
                }
            }
        }
    }
}