using Godot;
using Vector2 = Godot.Vector2;
using Color = Godot.Color;

public partial class ParticleController : GpuParticles2D
{
    private ShaderMaterial _shaderMaterial;
    private float _time = 0f;

    public override void _Ready()
    {
        Shader customShader = GD.Load<Shader>("res://scripts/custom_particle.gdshader");

        _shaderMaterial = new ShaderMaterial();
        _shaderMaterial.Shader = customShader;

        _shaderMaterial.SetShaderParameter("wave_intensity", 0.1f);
        _shaderMaterial.SetShaderParameter("wave_frequency", 10.0f);
        _shaderMaterial.SetShaderParameter("color_start", new Color(1.0f, 0.5f, 0.0f, 1.0f));
        _shaderMaterial.SetShaderParameter("color_end", new Color(1.0f, 0.0f, 0.5f, 1.0f));

        Material = _shaderMaterial;

        Amount = 100;
        Lifetime = 2.0f;
        Explosiveness = 0.0f;
        Randomness = 0.5f;

        var processMaterial = new ParticleProcessMaterial();
        processMaterial.Direction = new Vector3(0, -1, 0);
        processMaterial.Spread = 45f;
        processMaterial.InitialVelocityMin = 50f;
        processMaterial.InitialVelocityMax = 100f;
        processMaterial.Gravity = new Vector3(0, 98, 0);
        processMaterial.ScaleMin = 2.0f;
        processMaterial.ScaleMax = 2.0f;

        ProcessMaterial = processMaterial;

        var texture = new PlaceholderTexture2D();
        texture.Size = new Vector2I(32, 32);
        Texture = texture;

        Emitting = true;

        GD.Print("Particle system ready at: ", GlobalPosition);
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;

        if (_shaderMaterial != null)
        {
            float waveIntensity = 0.1f + Mathf.Sin(_time * 2f) * 0.05f;
            _shaderMaterial.SetShaderParameter("wave_intensity", waveIntensity);

            float colorShift = (Mathf.Sin(_time * 0.5f) + 1f) * 0.5f;
            Color colorStart = new Color(1.0f, 0.5f * colorShift, 0.0f, 1.0f);
            _shaderMaterial.SetShaderParameter("color_start", colorStart);
        }
    }
}