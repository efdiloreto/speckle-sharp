using Speckle.Converters.Common.Objects;
using Speckle.Converters.Common;
using ArcGIS.Desktop.Mapping;

namespace Speckle.Converters.ArcGIS3.Geometry;

public class EllipticArcToSpeckleConverter : ITypedConverter<ACG.EllipticArcSegment, SOG.Polyline>
{
  private readonly IConversionContextStack<Map, ACG.Unit> _contextStack;
  private readonly ITypedConverter<ACG.MapPoint, SOG.Point> _pointConverter;

  public EllipticArcToSpeckleConverter(
    IConversionContextStack<Map, ACG.Unit> contextStack,
    ITypedConverter<ACG.MapPoint, SOG.Point> pointConverter
  )
  {
    _contextStack = contextStack;
    _pointConverter = pointConverter;
  }

  public SOG.Polyline Convert(ACG.EllipticArcSegment target)
  {
    // Determine the number of vertices to create along the arc
    int numVertices = Math.Max((int)target.Length, 3); // Determine based on desired segment length or other criteria
    List<SOG.Point> points = new();

    // get correct direction
    int coeff = 1;
    double fullAngle = target.EndAngle - target.StartAngle;
    double angleStart = target.StartAngle;

    // define the direction
    if (
      !((target.IsCounterClockwise is false || fullAngle >= 0) && (target.IsCounterClockwise is true || fullAngle < 0))
    )
    {
      fullAngle = Math.PI * 2 - Math.Abs(fullAngle);
      if (target.IsCounterClockwise is false)
      {
        coeff = -1;
      }
    }

    // Calculate the vertices along the arc
    for (int i = 0; i <= numVertices; i++)
    {
      // Calculate the point along the arc
      double angle = angleStart + coeff * fullAngle * (i / (double)numVertices);
      ACG.MapPoint pointOnArc = ACG.MapPointBuilderEx.CreateMapPoint(
        target.CenterPoint.X + target.SemiMajorAxis * Math.Cos(angle),
        target.CenterPoint.Y + target.SemiMinorAxis * Math.Sin(angle),
        target.SpatialReference
      );

      points.Add(_pointConverter.Convert(pointOnArc));
    }

    // create Speckle Polyline
    SOG.Polyline polyline =
      new(points.SelectMany(pt => new[] { pt.x, pt.y, pt.z }).ToList(), _contextStack.Current.SpeckleUnits)
      {
        // bbox = box,
        length = target.Length
      };
    return polyline;
  }
}
