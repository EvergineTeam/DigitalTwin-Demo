using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Batchers;
using Evergine.Framework.Services;
using Evergine.Mathematics;

namespace DigitalTwin
{
    public class MyScene : Scene
    {
        public override void RegisterManagers()
        {
            base.RegisterManagers();
            this.Managers.AddManager(new global::Evergine.Framework.Physics.PhysicsManager() { Gravity = new Vector3(0f, -9.8f, 0f), CollisionMatrix = new uint[] { 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0xFFFFu, 0x10000u, 0x20000u, 0x40000u, 0x80000u, 0x100000u, 0x200000u, 0x400000u, 0x800000u, 0x1000000u, 0x2000000u, 0x4000000u, 0x8000000u, 0x10000000u, 0x20000000u, 0x40000000u, 0x80000000u } });
        }

        protected override void CreateScene()
        {
        }
    }
}