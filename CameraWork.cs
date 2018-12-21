using System;
using System.Collections;
using UnityEngine;

namespace SHProject.Ingame
{
    [AutoRegisterEvent]
    public class CameraWork : MonoBehaviourBase
    {
        public Transform target;
        public Vector3 offset = new Vector3(1f, 10f, -4f);
        private bool followToggle = false;

        private IEnumerator Follow()
        {
            while(followToggle)
            {
                transform.position = target.position + offset;
                yield return new WaitForEndOfFrame();
            }
        }

        [EventMethod(EventEnum.CharacterJoin)]
        public void OnJoinTarget(object sender, EventArgs args)
        {
            TValueEventArgs<Transform> eventArgs = args as TValueEventArgs<Transform>;
            target = eventArgs.arg;

            transform.position = target.position + offset;
        }

        [EventMethod(EventEnum.CharacterMove)]
        public void OnFollow(object sender, EventArgs args)
        {
            followToggle = true;
            StartCoroutine(Follow());
        }

        [EventMethod(EventEnum.CharacterStop)]
        public void OnFollowStop(object sender, EventArgs args)
        {
            followToggle = false;
        }
    }
}