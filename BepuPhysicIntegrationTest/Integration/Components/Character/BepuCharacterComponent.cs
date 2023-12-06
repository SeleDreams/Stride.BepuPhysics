﻿using BepuPhysicIntegrationTest.Integration.Components.Containers;
using BepuPhysicIntegrationTest.Integration.Configurations;
using BepuPhysicIntegrationTest.Integration.Extensions;
using BepuPhysics;
using Stride.Core;
using Stride.Core.Mathematics;

namespace BepuPhysicIntegrationTest.Integration.Components.Utils;
public class BepuCharacterComponent : SimulationUpdateComponent
{
	public float Speed { get; set; } = 10f;
	public float JumpSpeed { get; set; } = 1f;

	[DataMemberIgnore]
	public Quaternion Orientation { get; set; }
	[DataMemberIgnore]
	public Vector3 Velocity { get; set; }
	[DataMemberIgnore]
	public bool TryJump { get; set; }

	public BodyContainerComponent? CharacterBody { get; set; }

	private BodyReference? _bodyReference;

	public override void Start()
	{
		_bodyReference = CharacterBody?.GetPhysicBody().Value;
		// prevent tipping of character while moving
		_bodyReference.Value.LocalInertia = new BodyInertia { InverseMass = 1f };

		base.Start();
	}

	public override void Update()
	{
		DebugText.Print(Input.MouseDelta.ToString(), new Int2(50, 50));
		DebugText.Print(Velocity.ToString(), new Int2(50, 75));
		DebugText.Print(Orientation.ToString(), new Int2(50, 100));
	}

	public void Move(Vector3 direction)
	{
		Velocity = direction * Speed;
	}

	public void Rotate(Quaternion rotation)
	{
		Orientation = rotation;
	}

	public void Jump()
	{
		TryJump = true;
	}

	public override void SimulationUpdate(float simTimeStep)
	{
		// probably inneficient but I needed a way to wake up the body
		// or else it sleeps after a second.
		BepuSimulation.Simulation.Awakener.AwakenBody(_bodyReference.Value.Handle);

		_bodyReference.Value.Pose.Orientation = Orientation.ToNumericQuaternion();

		_bodyReference.Value.Velocity.Linear = new System.Numerics.Vector3
			(Velocity.ToNumericVector().X, _bodyReference.Value.Velocity.Linear.Y, Velocity.ToNumericVector().Z);

		// prevent character from sliding
		if(Velocity.Length() < 0.01f)
			_bodyReference.Value.Velocity.Linear = new System.Numerics.Vector3(0, _bodyReference.Value.Velocity.Linear.Y, 0);

        if (TryJump)
        {
			_bodyReference.Value.ApplyLinearImpulse(System.Numerics.Vector3.UnitY * JumpSpeed * 10);
			TryJump = false;
        }
    }
}
