using Godot;
using Vector2 = Godot.Vector2;
using Color = Godot.Color;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public float Acceleration = 800f;
    [Export] public float Friction = 1000f;

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputVector = GetInputVector();

        if (inputVector != Vector2.Zero)
        {
            Velocity = Velocity.MoveToward(inputVector * Speed, Acceleration * (float)delta);
        }

        else
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * (float)delta);
        }

        MoveAndSlide();
    }

    private Vector2 GetInputVector()
    {
        Vector2 inputVector = Vector2.Zero;

        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
        {
            inputVector.X += 1;
        }

        if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
        {
            inputVector.X -= 1;
        }

        if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed(Key.S))
        {
            inputVector.Y += 1;
        }

        if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed(Key.W))
        {
            inputVector.Y -= 1;
        }

        return inputVector.Normalized();
    }
}
