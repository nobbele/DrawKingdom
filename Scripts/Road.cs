using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class Road : Area2D, IRoadConnectionPoint
{
    [Export] private Array<Texture2D> _segmentTextures;
    
    private float _length;
    [Export]
    public float Length
    {
        get => _length;
        set
        {
            _length = value;
            SetLength();
        }
    }

    public List<IRoadConnectionPoint> Connections { get; } = [];

    public override void _Ready()
    {
        Recompute();
    }

    private Texture2D GetRandomSegmentTexture()
        => _segmentTextures[Random.Shared.Next(_segmentTextures.Count)];

    private void Recompute()
    {
        var spriteContainer = GetNodeOrNull<Node2D>("SpriteContainer");
        if (spriteContainer == null)
        {
            spriteContainer = new Node2D { Name = "SpriteContainer" };
            AddChild(spriteContainer);
        }

        foreach (var child in spriteContainer.GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        var scale = 0.4f;

        var segmentLength = 112 * scale;
        var segmentCount = Mathf.FloorToInt(_length / segmentLength);
        for (var i = 0; i < segmentCount; i++)
        {
            var sprite = new Sprite2D();
            sprite.Scale = Vector2.One * scale;
            sprite.Texture = GetRandomSegmentTexture();
            sprite.Position = Vector2.Down * (i * segmentLength + sprite.Texture.GetHeight() * scale / 2f);
            AddChild(sprite);
        }  
        
        // var partialSegmentLength = _length - segmentCount * segmentLength;
        // var partialSegmentRatio = partialSegmentLength / segmentLength;
        // if (partialSegmentLength > 0)
        // {
        //     var sprite = new Sprite2D();
        //     sprite.Texture = GetRandomSegmentTexture();
        //
        //     var regionHeight = sprite.Texture.GetHeight() * partialSegmentRatio;
        //     
        //     sprite.Position = Vector2.Down * (segmentCount * segmentLength + sprite.Texture.GetHeight() / 2f);
        //     sprite.RegionEnabled = true;
        //     sprite.RegionRect = new Rect2(Vector2.Zero, new Vector2(sprite.Texture.GetWidth(), regionHeight));
        //     AddChild(sprite);
        // }
    }

    private void SetLength()
    {
        if (IsInsideTree())
        {
            Recompute();
        }
    }
}
