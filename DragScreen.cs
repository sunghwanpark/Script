using UnityEngine;

namespace SHProject.Ingame
{
    public class DragScreen : MonoBehaviour
    {
        private const float dragSpeed = 1;
        private Vector3 dragOrigin;
        private Transform cachedTransform;

        private void Awake()
        {
            cachedTransform = GetComponent<Transform>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(0)) return;

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

            if (cachedTransform.localPosition.x + move.x < 0 || cachedTransform.localPosition.z + move.z < 0)
                return;

            transform.Translate(move, Space.World);
        }
    }
}