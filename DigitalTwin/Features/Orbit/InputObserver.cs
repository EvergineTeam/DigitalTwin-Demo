using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Input.Keyboard;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Common.Input.Pointer;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace DigitalTwin.Features.Orbit
{
    public abstract class InputObserver : Behavior
    {
        protected Camera3D camera;

        protected KeyboardDispatcher keyboardDispatcher;

        protected MouseDispatcher mouseDispatcher;

        protected PointerDispatcher touchDispatcher;

        protected override void OnActivated()
        {
            var display = this.Managers.RenderManager.ActiveCamera3D?.Display;

            if (display != null)
            {
                this.keyboardDispatcher = display.KeyboardDispatcher;
                this.mouseDispatcher = display.MouseDispatcher;
                this.touchDispatcher = display.TouchDispatcher;
            }

            base.OnActivated();
        }

        protected bool GetPointerPosition(out Vector2 position)
        {
            if (this.mouseDispatcher.Points.Count > 0)
            {
                position = this.mouseDispatcher.Points[0].Position.ToVector2();
                return true;
            }

            if (this.touchDispatcher.Points.Count > 0)
            {
                position = this.touchDispatcher.Points[0].Position.ToVector2();
                return true;
            }

            position = this.mouseDispatcher.Position.ToVector2();
            return position != Vector2.Zero;
        }
    }
}
