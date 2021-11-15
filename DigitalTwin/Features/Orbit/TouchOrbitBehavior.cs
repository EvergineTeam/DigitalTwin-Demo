using System;
using System.Diagnostics;
using System.Linq;
using Evergine.Common.Input;
using Evergine.Common.Input.Pointer;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace DigitalTwin.Features.Orbit
{
    public class TouchOrbitBehavior : OrbitBehavior
    {
        private static readonly TimeSpan maxTimeBetweenResetPoints = TimeSpan.FromMilliseconds(150);
        
        private Point firstPointToResetPosition;
        
        private DateTime firstPointToResetTime;

        protected override bool TryGetPointerPosition(out Vector2 position)
        {
            position = default;

            if (this.touchDispatcher == null)
            {
                return false;
            }

            if (this.touchDispatcher.Points.Count == 1)
            {
                position = this.touchDispatcher.Points[0].Position.ToVector2();
            }
            else if (this.touchDispatcher.Points.Count == 2 && 
                this.touchDispatcher.Points.Any(item => item.State == ButtonState.Releasing) &&
                this.touchDispatcher.Points.Any(item => item.State == ButtonState.Pressed))
            {
                position = this.touchDispatcher.Points
                    .First(item => item.State == ButtonState.Pressed)
                    .Position.ToVector2();
            }
            else
            {
                position = this.lastPointerPosition;
            }

            return true;
        }

        protected override bool IsOrbitRequested()
        {
            var isRequested = this.touchDispatcher.Points.Count == 1;

            if (isRequested && this.touchDispatcher.Points[0].State == ButtonState.Pressing)
            {
                this.lastPointerPosition = this.currentPointerPosition;
            }

            return isRequested;
        }
    }
}