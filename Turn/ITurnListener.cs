using System;

namespace SHProject.Ingame
{
    public interface ITurnListener
    {
        Action<float>       Event_TurnTimer { get; set; }
    }
}