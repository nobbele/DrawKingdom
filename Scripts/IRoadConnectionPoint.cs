using System.Collections.Generic;
using System.Linq;

public interface IRoadConnectionPoint
{
    List<IRoadConnectionPoint> Connections { get; }
}

public static class RoadConnectionPointExtensions
{
    public static IEnumerable<IRoadConnectionPoint> GetAllConnectionPoints(this IRoadConnectionPoint startPoint)
    {
        HashSet<IRoadConnectionPoint> visited = [];
        Queue<IRoadConnectionPoint> toVisit = [];
        toVisit.Enqueue(startPoint);

        while (toVisit.TryDequeue(out var point))
        {
            yield return point;
            visited.Add(point);
            
            foreach (var conn in point.Connections.Where(conn => !visited.Contains(conn)))
            {
                toVisit.Enqueue(conn);
            }
        }
    }

    public static IEnumerable<IBuilding> GetAllConnectedBuilding(this IRoadConnectionPoint startPoint)
        => startPoint.GetAllConnectionPoints().OfType<IBuilding>();
    
    public static IEnumerable<Town> GetAllConnectedTown(this IRoadConnectionPoint startPoint)
        => startPoint.GetAllConnectionPoints().OfType<Town>();
}