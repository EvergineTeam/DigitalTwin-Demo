using Evergine.Bullet;
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
            this.Managers.AddManager(new BulletPhysicManager3D());
        }

        protected override void CreateScene()
        {
        }
    }
}