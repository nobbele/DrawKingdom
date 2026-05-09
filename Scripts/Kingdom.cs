using Godot;
using System;
using System.Collections.Generic;

public partial class Kingdom : Area2D, IBuilding, IRoadConnectionPoint
{
    public List<IRoadConnectionPoint> Connections { get; } = [];
    public IReadOnlyDictionary<ResourceKind, float> Consumption => new Dictionary<ResourceKind, float>();
    
    public void OnMissingProductionResult(Dictionary<ResourceKind, float> result)
    {
        if (result.Count > 0)
        {
            GD.Print("Sad kingdom");
        }
    }
}
