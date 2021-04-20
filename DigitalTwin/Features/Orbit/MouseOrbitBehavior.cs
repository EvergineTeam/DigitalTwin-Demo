using System;
using System.Diagnostics;
using System.Linq;
using WaveEngine.Common.Input.Keyboard;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace DigitalTwin.Features.Orbit
{
    public class MouseOrbitBehavior : OrbitBehavior
    {
        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override bool TryGetPointerPosition(out Vector2 position)
        {
            position = default;

            if (this.mouseDispatcher == null)
            {
                return false;
            }

            position = this.mouseDispatcher.Position.ToVector2();
            
            return true;
        }

        protected override bool IsOrbitRequested()
        {
            return this.mouseDispatcher.IsButtonDown(MouseButtons.Left) || this.mouseDispatcher.IsButtonDown(MouseButtons.Right);
        }
    }
}