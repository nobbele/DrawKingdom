using Godot;
using System;

[GlobalClass]
public partial class GameResource : Resource
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public string Name { get; set; }
    [Export] public ResourceKind Kind { get; set; }
    
    [Export] public int SellingPrice { get; set; }
    [Export] public float Importance { get; set; }
    
    public GameResource() {}
}
