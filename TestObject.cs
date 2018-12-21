using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(PhotonView))]
public class TestObject : Photon.MonoBehaviour, IPunObservable
{
    internal struct State
    {
        internal double timestamp;
        internal Vector3 pos;
    }

    // We store twenty states with "playback" information
    private State[] m_BufferedState = new State[20];

    // Keep track of what slots are used
    private int m_TimestampCount;

    public double InterpolationDelay = 0.15;

    private bool connected = false;
    private Vector3 correctPlayerPos = Vector3.zero;
    
    private void OnDestroy()
    {
        Debug.Log(string.Format("{0} die", photonView.viewID));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        connected = true;

        if (stream.isWriting)
        {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
            stream.SendNext(Environment.TickCount);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);

            for (int i = this.m_BufferedState.Length - 1; i >= 1; i--)
            {
                this.m_BufferedState[i] = this.m_BufferedState[i - 1];
            }

            State state;
            state.timestamp = info.timestamp;
            state.pos = pos;
            this.m_BufferedState[0] = state;

            // Increment state count but never exceed buffer size
            this.m_TimestampCount = Mathf.Min(this.m_TimestampCount + 1, this.m_BufferedState.Length);

            // Check integrity, lowest numbered state in the buffer is newest and so on
            for (int i = 0; i < this.m_TimestampCount - 1; i++)
            {
                if (this.m_BufferedState[i].timestamp < this.m_BufferedState[i + 1].timestamp)
                {
                    Debug.Log("State inconsistent");
                }
            }
        }
    }

    private void Update()
    {
        if (this.photonView.isMine || !PhotonNetwork.inRoom)
        {
            return;     // if this object is under our control, we don't need to apply received position-updates 
        }

        double currentTime = PhotonNetwork.time;
            double interpolationTime = currentTime - this.InterpolationDelay;

            // We have a window of InterpolationDelay where we basically play back old updates.
            // By having InterpolationDelay the average ping, you will usually use interpolation.
            // And only if no more data arrives we will use the latest known position.

            // Use interpolation, if the interpolated time is still "behind" the update timestamp time:
            if (this.m_BufferedState[0].timestamp > interpolationTime)
            {
                for (int i = 0; i < this.m_TimestampCount; i++)
                {
                    // Find the state which matches the interpolation time (time+0.1) or use last state
                    if (this.m_BufferedState[i].timestamp <= interpolationTime || i == this.m_TimestampCount - 1)
                    {
                        // The state one slot newer (<100ms) than the best playback state
                        State rhs = this.m_BufferedState[Mathf.Max(i - 1, 0)];
                        // The best playback state (closest to 100 ms old (default time))
                        State lhs = this.m_BufferedState[i];

                        // Use the time between the two slots to determine if interpolation is necessary
                        double diffBetweenUpdates = rhs.timestamp - lhs.timestamp;
                        float t = 0.0F;
                        // As the time difference gets closer to 100 ms t gets closer to 1 in 
                        // which case rhs is only used
                        if (diffBetweenUpdates > 0.0001)
                        {
                            t = (float)((interpolationTime - lhs.timestamp) / diffBetweenUpdates);
                        }

                        // if t=0 => lhs is used directly
                        transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                        return;
                    }
                }
            }
            else
            {
                // If our interpolation time catched up with the time of the latest update:
                // Simply move to the latest known position.

                State latest = this.m_BufferedState[0];

                transform.localPosition = Vector3.Lerp(transform.localPosition, latest.pos, Time.deltaTime * 20);
            }
    }
}
