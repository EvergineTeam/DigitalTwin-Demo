using System;
using Evergine.Common.Input.Mouse;
using Evergine.Common.Input.Pointer;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace DigitalTwin.Features.Picking
{
    public struct PointerArgs
    {
        public long Id;
        public Vector2 ScreenPosition;
        public Vector3 Position;

        public PointerArgs(long id, Vector2 screenPosition, Vector3 position)
        {
            this.Id = id;
            this.ScreenPosition = screenPosition;
            this.Position = position;
        }
    }

    public class PointerManager : Component
    {
        protected MouseDispatcher mouseDispatcher;
        protected PointerDispatcher touchDispatcher;
        public event EventHandler<PointerArgs> PointerDown;
        public event EventHandler<PointerArgs> PointerUp;
        public event EventHandler<PointerArgs> PointerMove;

        private bool initialized;
        private Camera3D camera;

        public int PointerCount { get; private set; }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (this.initialized)
            {
                this.RegisterEvents();
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.UnregisterEvents();
        }

        protected override void Start()
        {
            base.Start();
            this.RegisterEvents();
            this.initialized = true;
        }

        private void UnregisterEvents()
        {
            if (this.touchDispatcher != null)
            {
                this.touchDispatcher.PointerUp -= this.TouchDispatcher_PointerUp;
                this.touchDispatcher.PointerDown -= this.TouchDispatcher_PointerDown;
                this.touchDispatcher.PointerMove -= this.TouchDispatcher_PointerMove;
            }

            if (this.mouseDispatcher != null)
            {
                this.mouseDispatcher.PointerUp -= this.TouchDispatcher_PointerUp;
                this.mouseDispatcher.PointerDown -= this.TouchDispatcher_PointerDown;
                this.mouseDispatcher.PointerMove -= this.TouchDispatcher_PointerMove;
            }
        }

        private void RegisterEvents()
        {
            // For simplification, this code only works when the camera and/or display does not change...
            this.camera = this.Managers.RenderManager.ActiveCamera3D;
            this.touchDispatcher = this.Managers.RenderManager.ActiveCamera3D?.Display?.TouchDispatcher;
            if (this.touchDispatcher != null)
            {
                this.touchDispatcher.PointerUp += this.TouchDispatcher_PointerUp;
                this.touchDispatcher.PointerDown += this.TouchDispatcher_PointerDown;
                this.touchDispatcher.PointerMove += this.TouchDispatcher_PointerMove;
            }

            this.mouseDispatcher = this.Managers.RenderManager.ActiveCamera3D?.Display?.MouseDispatcher;
            if (this.mouseDispatcher != null)
            {
                this.mouseDispatcher.PointerUp += this.TouchDispatcher_PointerUp;
                this.mouseDispatcher.PointerDown += this.TouchDispatcher_PointerDown;
                this.mouseDispatcher.PointerMove += this.TouchDispatcher_PointerMove;
            }
        }

        private void TouchDispatcher_PointerMove(object sender, PointerEventArgs e)
        {
            if (this.PointerMove != null)
            {
                var position = ProjectPoint(e.Position);
                this.PointerMove.Invoke(sender, new PointerArgs(e.Id, e.Position.ToVector2(), position));
            }
        }

        private void TouchDispatcher_PointerUp(object sender, PointerEventArgs e)
        {
            this.PointerCount++;
            if (this.PointerUp != null)
            {
                var position = ProjectPoint(e.Position);
                this.PointerUp.Invoke(sender, new PointerArgs(e.Id, e.Position.ToVector2(), position));
            }
        }

        private void TouchDispatcher_PointerDown(object sender, Evergine.Common.Input.Pointer.PointerEventArgs e)
        {
            if (this.PointerDown != null)
            {
                var position = ProjectPoint(e.Position);
                this.PointerDown.Invoke(sender, new PointerArgs(e.Id, e.Position.ToVector2(), position));
            }

            this.PointerCount--;
        }

        private Vector3 ProjectPoint(Point point)
        {
            var screenCoords = point.ToVector2();
            this.camera.CalculateRay(ref screenCoords, out var ray);

            return ray.IntersectionYPlane(0);
        }
    }
}
