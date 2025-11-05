using Godot;
using System.Collections.Generic;
using Vector2 = Godot.Vector2;
using Color = Godot.Color;

public partial class PhysicsChain : Node2D
{
	[Export] public int ChainSegments = 5;
	[Export] public float SegmentDistance = 30f;
	[Export] public float SegmentWidth = 20f;
	[Export] public float SegmentHeight = 40f;

	private List<Node2D> _segments = new List<Node2D>();
	private List<Joint2D> _joints = new List<Joint2D>();

	public override void _Ready()
	{
		CreateChain();
	}

	private void CreateChain()
	{
		Node2D previousSegment = null;

		for (int i = 0; i < ChainSegments; i++)
		{
			Node2D segment;

			if (i == 0)
			{
				var staticBody = new StaticBody2D();

				staticBody.CollisionLayer = 2;
				staticBody.CollisionMask = 1;

				segment = staticBody;
			}

			else
			{
				var rigidBody = new RigidBody2D();
				rigidBody.Mass = 1.0f;
				rigidBody.GravityScale = 1.0f;

				rigidBody.CollisionLayer = 2;
				rigidBody.CollisionMask = 3;

				rigidBody.ContactMonitor = true;
				rigidBody.MaxContactsReported = 4;

				rigidBody.LinearDamp = 0.5f;
				rigidBody.AngularDamp = 0.5f;

				segment = rigidBody;
			}

			segment.Position = new Vector2(0, i * SegmentDistance);

			var collisionShape = new CollisionShape2D();
			var rectShape = new RectangleShape2D();
			rectShape.Size = new Vector2(SegmentWidth, SegmentHeight);
			collisionShape.Shape = rectShape;
			segment.AddChild(collisionShape);

			var colorRect = new ColorRect();
			colorRect.Size = new Vector2(SegmentWidth, SegmentHeight);
			colorRect.Position = new Vector2(-SegmentWidth / 2, -SegmentHeight / 2);

			colorRect.Color = i % 2 == 0 ? new Color(0.2f, 0.6f, 0.8f) : new Color(0.8f, 0.4f, 0.2f);
			segment.AddChild(colorRect);

			AddChild(segment);
			_segments.Add(segment);

			if (previousSegment != null)
			{
				var joint = new PinJoint2D();
				joint.NodeA = previousSegment.GetPath();
				joint.NodeB = segment.GetPath();

				joint.Position = previousSegment.Position + new Vector2(0, SegmentDistance / 2);

				joint.Softness = 0.1f;
				joint.Bias = 0.3f;

				AddChild(joint);
				_joints.Add(joint);
			}

			previousSegment = segment;
		}
	}

	public void ApplyForceToSegment(int segmentIndex, Vector2 force)
	{
		if (segmentIndex >= 0 && segmentIndex < _segments.Count)
		{
			var segment = _segments[segmentIndex];

			if (segment is RigidBody2D rigidBody)
			{
				rigidBody.ApplyImpulse(force);
			}
		}
	}

	public void ApplyForceToEnd(Vector2 force)
	{
		ApplyForceToSegment(_segments.Count - 1, force);
	}

	public Node2D GetSegment(int index)
	{
		if (index >= 0 && index < _segments.Count)
		{
			return _segments[index];
		}

		return null;
	}
}
