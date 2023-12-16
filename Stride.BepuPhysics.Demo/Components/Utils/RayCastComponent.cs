﻿using System.Runtime;
using Stride.BepuPhysics.Configurations;
using Stride.BepuPhysics.Definitions.Raycast;
using Stride.BepuPhysics.Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;

#warning This should not be part of the base API, move it to demo/sample

namespace Stride.BepuPhysics.Demo.Components.Utils
{
    //[DataContract("SpawnerComponent", Inherited = true)]
    [ComponentCategory("BepuDemo - Utils")]
    public class RayCastComponent : SyncScript
    {
        private BepuConfiguration? _bepuConfig;
        public int SimulationIndex { get; set; } = 0;

        public Vector3 Offset { get; set; } = Vector3.Zero;
        public Vector3 Dir { get; set; } = Vector3.UnitZ;
        public float MaxT { get; set; } = 100;


        public override void Start()
        {
            _bepuConfig = Services.GetService<BepuConfiguration>();
        }

        public override void Update() //maybe it would be a better idea to do that in SimulationUpdate since the result will not change without simupdate.
        {
            if (_bepuConfig == null)
                return;

            Entity.Transform.GetWorldTransformation(out var position, out var rotation, out var scale);
            var worldDir = Dir;
            rotation.Rotate(ref worldDir);
            var buffer = System.Buffers.ArrayPool<HitInfo>.Shared.Rent(16);
            _bepuConfig.BepuSimulations[SimulationIndex].RaycastPenetrating(Entity.Transform.GetWorldPos() + Offset, worldDir, MaxT, buffer, out var hits);
            if (hits.Length > 0)
            {
                for (int j = 0; j < hits.Length; j++)
                {
                    var hitInfo = hits[j];
                    DebugText.Print($"T : {hitInfo.Distance}  |  normal : {hitInfo.Normal}  |  Entity : {hitInfo.Container.Entity} (worldDir : {worldDir})", new((int)(Game.Window.PreferredWindowedSize.X - 500 / 1.3f), 830 + 25 * j));
                }
            }
            else
            {
                DebugText.Print($"no raycast hit", new((int)(Game.Window.PreferredWindowedSize.X - 500 / 1.3f), 830));
            }
            System.Buffers.ArrayPool<HitInfo>.Shared.Return(buffer);
        }
    }

}
