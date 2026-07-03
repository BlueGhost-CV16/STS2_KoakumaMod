using Godot;
using Godot.Bridge;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace BG_Koakuma.Vfx;

[ScriptPath("res://BG_KoakumaCode/Vfx/NJackBombThrowVfx.cs")]
public partial class NJackBombThrowVfx : Node2D
{
    private const float ImpactSettleSeconds = 0.42f;

    private Node2D? _projectile;
    private Sprite2D? _bombSprite;
    private GpuParticles2D? _trailSpecks;
    private GpuParticles2D? _trailSmoke;
    private GpuParticles2D[] _throwParticles = [];
    private GpuParticles2D[] _impactParticles = [];
    private CanvasItem[] _modulateParticles = [];

    private Vector2 _start;
    private Vector2 _control;
    private Vector2 _end;
    private float _elapsed;
    private float _duration;
    private float _spinDirection;
    private bool _boundNodes;
    private bool _playing;

    public override void _Ready()
    {
        BindNodes();
    }

    private void BindNodes()
    {
        if (_boundNodes)
        {
            return;
        }

        _projectile = GetNodeOrNull<Node2D>("throw_container/projectile");
        _bombSprite = GetNodeOrNull<Sprite2D>("throw_container/projectile/bomb_sprite");
        _trailSpecks = GetNodeOrNull<GpuParticles2D>("throw_container/projectile/trail_specks");
        _trailSmoke = GetNodeOrNull<GpuParticles2D>("throw_container/projectile/trail_smoke");

        _throwParticles = new GpuParticles2D?[]
        {
            _trailSpecks,
            _trailSmoke,
            GetNodeOrNull<GpuParticles2D>("throw_container/launch_specks")
        }.OfType<GpuParticles2D>().ToArray();

        _impactParticles = new GpuParticles2D?[]
        {
            GetNodeOrNull<GpuParticles2D>("impact_container/impact_core"),
            GetNodeOrNull<GpuParticles2D>("impact_container/impact_flare"),
            GetNodeOrNull<GpuParticles2D>("impact_container/impact_specks"),
            GetNodeOrNull<GpuParticles2D>("impact_container/impact_smoke"),
            GetNodeOrNull<GpuParticles2D>("impact_container/explosion_ring")
        }.OfType<GpuParticles2D>().ToArray();

        _modulateParticles = new CanvasItem?[]
        {
            _trailSpecks,
            GetNodeOrNull<CanvasItem>("impact_container/impact_core"),
            GetNodeOrNull<CanvasItem>("impact_container/impact_flare"),
            GetNodeOrNull<CanvasItem>("impact_container/impact_specks")
        }.OfType<CanvasItem>().ToArray();

        DuplicateProcessMaterials(_throwParticles);
        DuplicateProcessMaterials(_impactParticles);
        SetProcess(false);
        _boundNodes = true;
    }

    public async Task PlayUntilImpactAsync(Vector2 startGlobalPosition, Vector2 targetGlobalPosition, Color? tint = null)
    {
        BindNodes();

        if (_projectile == null)
        {
            this.QueueFreeSafely();
            return;
        }

        _start = ToLocal(startGlobalPosition);
        _end = ToLocal(targetGlobalPosition);

        var horizontalDistance = Mathf.Abs(_end.X - _start.X);
        var arcHeight = Mathf.Clamp(horizontalDistance * 0.35f, 190.0f, 300.0f);
        _control = _start.Lerp(_end, 0.58f) + Vector2.Up * arcHeight;

        var distance = _start.DistanceTo(_end);
        _duration = Mathf.Clamp(distance / 2775.0f, 0.20f, 0.28f);
        _spinDirection = _end.X >= _start.X ? 1.0f : -1.0f;

        if (_bombSprite != null)
        {
            _bombSprite.FlipH = _spinDirection < 0.0f;
        }

        _projectile.Position = _start;
        _projectile.Rotation = 0.0f;
        _projectile.Scale = Vector2.One * 0.68f;
        _projectile.Visible = true;

        if (tint.HasValue)
        {
            foreach (var item in _modulateParticles)
            {
                item.SelfModulate = tint.Value;
            }
        }

        EmitThrowParticles(true);

        _elapsed = 0.0f;
        _playing = true;
        SetProcess(true);

        await ToSignal(GetTree().CreateTimer(_duration), SceneTreeTimer.SignalName.Timeout);

        _playing = false;
        SetProcess(false);

        _projectile.Position = _end;
        _projectile.Visible = false;
        EmitThrowParticles(false);
        EmitImpactParticles();

        NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);
        _ = CleanupAfterImpactAsync();
    }

    private async Task CleanupAfterImpactAsync()
    {
        await ToSignal(GetTree().CreateTimer(ImpactSettleSeconds), SceneTreeTimer.SignalName.Timeout);
        this.QueueFreeSafely();
    }

    public override void _Process(double delta)
    {
        if (!_playing || _projectile == null)
        {
            return;
        }

        _elapsed += (float)delta;

        var rawT = Mathf.Clamp(_elapsed / _duration, 0.0f, 1.0f);
        var t = Mathf.Pow(rawT, 0.88f);

        _projectile.Position = QuadraticBezier(_start, _control, _end, t);

        var tangent = QuadraticBezierDerivative(_start, _control, _end, t);
        if (!tangent.IsZeroApprox())
        {
            var normalizedTangent = tangent.Normalized();
            var pathTilt = normalizedTangent.Angle() * 0.18f;
            var spin = Mathf.Lerp(0.0f, _spinDirection * 1.45f, t);
            _projectile.Rotation = pathTilt + spin;
            UpdateTrailDirection(-normalizedTangent);
        }

        var pulse = Mathf.Sin(t * Mathf.Pi);
        _projectile.Scale = Vector2.One * Mathf.Lerp(0.68f, 0.78f, pulse);
    }

    private void EmitThrowParticles(bool emitting)
    {
        foreach (var particles in _throwParticles)
        {
            particles.Emitting = emitting;
            if (emitting)
            {
                particles.Restart();
            }
        }
    }

    private void EmitImpactParticles()
    {
        foreach (var particles in _impactParticles)
        {
            particles.GlobalPosition = ToGlobal(_end);
            particles.Restart();
            particles.Emitting = true;
        }
    }

    private void UpdateTrailDirection(Vector2 backward)
    {
        UpdateParticleDirection(_trailSpecks, backward);
        UpdateParticleDirection(_trailSmoke, backward);
    }

    private static void UpdateParticleDirection(GpuParticles2D? particles, Vector2 direction)
    {
        if (particles?.ProcessMaterial is ParticleProcessMaterial material)
        {
            material.Direction = new Vector3(direction.X, direction.Y, 0.0f);
        }
    }

    private static void DuplicateProcessMaterials(IEnumerable<GpuParticles2D> particles)
    {
        foreach (var particle in particles)
        {
            if (particle.ProcessMaterial != null)
            {
                particle.ProcessMaterial = (Material)particle.ProcessMaterial.Duplicate();
            }
        }
    }

    private static Vector2 QuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        var inv = 1.0f - t;
        return inv * inv * start + 2.0f * inv * t * control + t * t * end;
    }

    private static Vector2 QuadraticBezierDerivative(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        return 2.0f * (1.0f - t) * (control - start) + 2.0f * t * (end - control);
    }
}
