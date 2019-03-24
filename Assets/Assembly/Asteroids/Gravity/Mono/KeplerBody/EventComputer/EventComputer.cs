using UnityEngine;
using System;
using System.Collections.Generic;

namespace Asteroids.Gravity.Mono
{
    partial class KeplerBody
    {
        private static partial class EventComputer
        {
            private static readonly List<GravityField> fieldsBuffer = new List<GravityField>();

            public static Event ComputeNext(KeplerBody body)
            {
                var nextEvent = ComputeNextEscape(body);
                nextEvent = ChooseNearestEvent(nextEvent, ComputeNextHit(body));
                return ChooseNearestEvent(nextEvent, ComputeNextJoin(body));
            }

            private static Event ComputeNextEscape(KeplerBody body)
            {
                var parent = body.Parent;
                if (!parent) return null;

                var parentBody = parent.GetComponent<KeplerBody>();
                if (!parentBody) return null;

                var grandParent = parentBody.Parent;
                if (!grandParent) return null;

                var bodyMotion = body.Motion;
                var escapeDeltaTime = bodyMotion.PredictTimeWhenAbove(parent.InfluenceRadius);
                if (escapeDeltaTime == null) return null;

                var eventDeltaTime = escapeDeltaTime.Value;
                var parentMotionWhenEscape = new Orbit.Motion(parentBody.Motion, eventDeltaTime);
                var bodyPositionWhenEscape = body.Motion.ComputePosition(eventDeltaTime);
                var eventData = new EventData
                {
                    Body = body,
                    ExecutionTime = Time.time + eventDeltaTime,
                    NewMotion = parentMotionWhenEscape.ComputeRelativeMotion(bodyPositionWhenEscape),
                    NewParent = grandParent
                };

                return new Event(eventData);
            }

            private static Event ComputeNextHit(KeplerBody body)
            {
                return null;
            }

            private static Event ComputeNextJoin(KeplerBody body)
            {
                Event nextEvent = null;
                var bodyParent = body.Parent;
                if (bodyParent == null) return null;
                bodyParent.GetComponentsInFirstChildren(true, fieldsBuffer);
                for(var i = 0; i < fieldsBuffer.Count; i++)
                {
                    var join = ComputeNextJoin(body, fieldsBuffer[i]);
                    nextEvent = ChooseNearestEvent(join, nextEvent);
                }
                return nextEvent;
            }

            private static Event ComputeNextJoin(KeplerBody body, GravityField neighbour)
            {
                var neighbourBody = neighbour.GetComponent<KeplerBody>();
                if (neighbourBody == null || neighbourBody == body) return null;

                var bodyMotion = body.Motion;
                var neighbourMotion = neighbourBody.Motion;
                var joinDeltaTime = neighbourMotion.TryPredictTimeWhenApproach(bodyMotion, neighbour.InfluenceRadius);
                if (joinDeltaTime == null) return null;

                var eventDeltaTime = joinDeltaTime.Value;
                var deltaPosition = bodyMotion.ComputePosition(eventDeltaTime) - neighbourMotion.ComputePosition(eventDeltaTime);
                var stableMotion = neighbour.CreateStableMotion(deltaPosition);
                var eventData = new EventData()
                {
                    Body = body,
                    ExecutionTime = Time.time + eventDeltaTime,
                    NewParent = neighbour,
                    NewMotion = stableMotion,
                };

                return new Event(eventData);
            }

            private static Event ChooseNearestEvent(Event eventA, Event eventB)
            {
                if (eventA == null) return eventB;
                if (eventB == null) return eventA;
                return eventA.ExecutionTime < eventB.ExecutionTime ? eventA : eventB;
            }
        }
    }
}