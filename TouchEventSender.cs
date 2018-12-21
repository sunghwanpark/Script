using UnityEngine;

namespace SHProject.Ingame
{
    public class TouchEventSender : MonoBehaviour
    {
        void Update()
        {
            if(Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                }
                if(touch.phase == TouchPhase.Moved)
                {

                }
                if(touch.phase == TouchPhase.Canceled)
                {

                }
                if(touch.phase == TouchPhase.Ended)
                {
                }
            }
        }
    }
}