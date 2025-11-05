using Godot;
using Vector2 = Godot.Vector2;
using Color = Godot.Color;

public partial class LaserDetector : Node2D
{
    [Export] public float LaserLength = 500f;
    [Export] public Color LaserColorNormal = Colors.Green;
    [Export] public Color LaserColorAlert = Colors.Red;
    [Export] public NodePath PlayerPath;

    private RayCast2D _rayCast;
    private Line2D _laserBeam;
    private Node2D _player;
    private bool _isAlarmActive = false;
    private Timer _alarmTimer;
    private ColorRect _alarmFlash;
    private float _flashTime = 0f;

    public override void _Ready()
    {
        SetupRaycast();
        SetupVisuals();
        SetupAlarm();

        if (PlayerPath != null && !PlayerPath.IsEmpty)
        {
            _player = GetNode<Node2D>(PlayerPath);
        }
    }

    private void SetupRaycast()
    {
        _rayCast = new RayCast2D();
        _rayCast.Enabled = true;
        _rayCast.TargetPosition = new Vector2(LaserLength, 0);

        _rayCast.CollisionMask = 1;

        AddChild(_rayCast);
    }

    private void SetupVisuals()
    {
        _laserBeam = new Line2D();
        _laserBeam.Width = 3.0f;
        _laserBeam.DefaultColor = LaserColorNormal;
        _laserBeam.AddPoint(Vector2.Zero);
        _laserBeam.AddPoint(new Vector2(LaserLength, 0));

        AddChild(_laserBeam);
    }

    private void SetupAlarm()
    {
        _alarmTimer = new Timer();
        _alarmTimer.WaitTime = 0.5f;
        _alarmTimer.OneShot = false;
        _alarmTimer.Timeout += OnAlarmTimerTimeout;
        AddChild(_alarmTimer);

        _alarmFlash = new ColorRect();
        _alarmFlash.Size = new Vector2(100, 100);
        _alarmFlash.Position = new Vector2(-50, -50);
        _alarmFlash.Color = new Color(1, 0, 0, 0);
        AddChild(_alarmFlash);
    }

    public override void _PhysicsProcess(double delta)
    {
        _rayCast.ForceRaycastUpdate();

        bool isColliding = _rayCast.IsColliding();
        Vector2 endPoint;

        if (isColliding)
        {
            endPoint = _rayCast.GetCollisionPoint() - GlobalPosition;

            var collider = _rayCast.GetCollider();

            if (collider is Node node && IsPlayerOrChild(node))
            {
                if (!_isAlarmActive)
                {
                    TriggerAlarm();
                }
            }

            else
            {
                if (_isAlarmActive)
                {
                    ResetAlarm();
                }
            }
        }

        else
        {
            endPoint = new Vector2(LaserLength, 0);

            if (_isAlarmActive)
            {
                ResetAlarm();
            }
        }

        UpdateLaserBeam(endPoint);

        if (_isAlarmActive)
        {
            _flashTime += (float)delta;
            float alpha = (Mathf.Sin(_flashTime * 10f) + 1f) * 0.5f *0.3f;
            _alarmFlash.Color = new Color(1, 0, 0, alpha);
        }
    }

    private bool IsPlayerOrChild(Node node)
    {
        if (_player == null)
        {
            return false;
        }

        Node current = node;

        while (current != null)
        {
            if (current == _player)
            {
                return true;
            }

            current = current.GetParent();
        }

        return false;
    }

    private void UpdateLaserBeam(Vector2 endPoint)
    {
        if (_laserBeam.GetPointCount() >= 2)
        {
            _laserBeam.SetPointPosition(0, Vector2.Zero);
            _laserBeam.SetPointPosition(1, endPoint);
        }
    }

    private void TriggerAlarm()
    {
        _isAlarmActive = true;

        _laserBeam.DefaultColor = LaserColorAlert;

        _alarmTimer.Start();

        GD.Print("ALARM! Player detected!");

        _flashTime = 0f;
    }

    private void ResetAlarm()
    {
        _isAlarmActive = false;

        _laserBeam.DefaultColor = LaserColorNormal;

        _alarmTimer.Stop();

        _alarmFlash.Color = new Color(1, 0, 0, 0);

        GD.Print("Alarm reset - no threat detected");
    }

    private void OnAlarmTimerTimeout()
    {
        GD.Print("!! ALARM ACTIVE !!");
    }
}
