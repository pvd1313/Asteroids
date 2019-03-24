using System;

namespace Asteroids.Gravity.Mono
{
    partial class KeplerBody
    {
        partial class EventComputer
        {
            public struct EventData
            {
                public KeplerBody Body { get; set; }
                public double ExecutionTime { get; set; }
                public Orbit.Motion NewMotion { get; set; }
                public GravityField NewParent { get; set; }
            }

            public class Event
            {
                public double ExecutionTime { get { return _data.ExecutionTime; } }

                private EventData _data;

                public Event(EventData data) { _data = data; }

                public void Execute(double currentTime)
                {
                    if (_data.Body == null) throw new Exception("Can execute gravity event, current body is destroyed.");
                    if (_data.NewParent == null) throw new Exception("Can execute gravity event, new parent is destroyed.");

                    var motion = new Orbit.Motion(_data.NewMotion, currentTime - ExecutionTime);
                    _data.Body.ChangeParent(_data.NewParent, motion);
                }
            }
        }
    }
}