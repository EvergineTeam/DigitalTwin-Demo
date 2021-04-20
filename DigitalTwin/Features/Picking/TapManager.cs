using DigitalTwin.Features.Colliders;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Mathematics;

namespace DigitalTwin.Features.Picking
{
    public class TapManager : Component
    {
        [BindComponent(source: BindComponentSource.Scene)]
        private CollisionManager collisionManager;

        [BindComponent(source: BindComponentSource.Scene)]
        private PointerManager pointerManager = null;

        private bool checkTap = false;

        protected override bool OnAttached()
        {
            this.pointerManager.PointerUp += this.PointerManager_PointerUp;
            this.pointerManager.PointerMove += this.PointerManager_PointerMove;
            this.pointerManager.PointerDown += this.PointerManager_PointerDown;
            return base.OnAttached();
        }

        protected override void OnDetach()
        {
            this.pointerManager.PointerUp -= this.PointerManager_PointerUp;
            this.pointerManager.PointerMove -= this.PointerManager_PointerMove;
            this.pointerManager.PointerDown -= this.PointerManager_PointerDown;

            base.OnDeactivated();
        }

        private void PointerManager_PointerDown(object sender, PointerArgs e)
        {
            checkTap = true;
        }

        private void PointerManager_PointerMove(object sender, PointerArgs e)
        {
            checkTap = false;
        }

        private void PointerManager_PointerUp(object sender, PointerArgs e)
        {
            if (this.checkTap)
            {
                this.Test(e.ScreenPosition);
            }
        }

        private void Test(Vector2 screenPos)
        {
            var camera3D = this.Managers.RenderManager.ActiveCamera3D;
            camera3D.CalculateRay(ref screenPos, out var ray);

            var result = this.collisionManager.Test(ref ray);

            if (result != null)
            {
                MyApplication.FireEntitySelected(result.Name);
            }
        }
    }
}
