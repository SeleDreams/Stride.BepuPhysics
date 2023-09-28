﻿using BepuPhysicIntegrationTest.Integration.Components.Simulations;
using BepuPhysicIntegrationTest.Integration.Processors;
using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace BepuPhysicIntegrationTest.Integration.Components.Containers
{
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(ContainerProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [ComponentCategory("Bepu - Containers")]

    public abstract class ContainerComponent : EntityComponent
    {
        private SimulationComponent _bepuSimulation = null;

        /// <summary>
        /// Get or set the SimulationComponent. If set null, it will try to find it in this or parent entities
        /// </summary>
        public SimulationComponent BepuSimulation
        {
            get => _bepuSimulation ?? Entity.GetInMeOrParents<SimulationComponent>();
            set
            {
                ContainerData?.DestroyShape();
                _bepuSimulation = value;
                ContainerData?.BuildShape();
            }
        }

        /// <summary>
        /// ContainerData is the bridge to Bepu.
        /// Automatically set by processor.
        /// </summary>
        internal ContainerData ContainerData { get; set; }
    }
}
