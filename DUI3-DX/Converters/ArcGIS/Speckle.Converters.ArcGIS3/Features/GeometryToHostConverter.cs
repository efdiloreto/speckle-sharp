using Speckle.Converters.Common;
using Speckle.Converters.Common.Objects;
using Speckle.Core.Models;

namespace Speckle.Converters.ArcGIS3.Features;

public class GeometryToHostConverter : IRawConversion<IReadOnlyList<Base>, ACG.Geometry>
{
  private readonly IRawConversion<List<SOG.Polyline>, ACG.Polyline> _polylineConverter;
  private readonly IRawConversion<List<SOG.Point>, ACG.Multipoint> _multipointConverter;
  private readonly IRawConversion<List<SGIS.PolygonGeometry3d>, ACG.Multipatch> _polygon3dConverter;
  private readonly IRawConversion<List<SGIS.PolygonGeometry>, ACG.Polygon> _polygonConverter;
  private readonly IRawConversion<List<SGIS.GisMultipatchGeometry>, ACG.Multipatch> _multipatchConverter;

  public GeometryToHostConverter(
    IRawConversion<List<SOG.Polyline>, ACG.Polyline> polylineConverter,
    IRawConversion<List<SOG.Point>, ACG.Multipoint> multipointConverter,
    IRawConversion<List<SGIS.PolygonGeometry3d>, ACG.Multipatch> polygon3dConverter,
    IRawConversion<List<SGIS.PolygonGeometry>, ACG.Polygon> polygonConverter,
    IRawConversion<List<SGIS.GisMultipatchGeometry>, ACG.Multipatch> multipatchConverter
  )
  {
    _polylineConverter = polylineConverter;
    _multipointConverter = multipointConverter;
    _polygon3dConverter = polygon3dConverter;
    _polygonConverter = polygonConverter;
    _multipatchConverter = multipatchConverter;
  }

  public ACG.Geometry RawConvert(IReadOnlyList<Base> target)
  {
    try
    {
      if (target.Count > 0)
      {
        switch (target[0])
        {
          case SOG.Point point:
            return _multipointConverter.RawConvert(target.Cast<SOG.Point>().ToList());
          case SOG.Polyline polyline:
            return _polylineConverter.RawConvert(target.Cast<SOG.Polyline>().ToList());
          case SGIS.PolygonGeometry3d geometry3d:
            return _polygon3dConverter.RawConvert(target.Cast<SGIS.PolygonGeometry3d>().ToList());
          case SGIS.PolygonGeometry geometry:
            return _polygonConverter.RawConvert(target.Cast<SGIS.PolygonGeometry>().ToList());
          case SGIS.GisMultipatchGeometry mesh:
            return _multipatchConverter.RawConvert(target.Cast<SGIS.GisMultipatchGeometry>().ToList());
          default:
            throw new NotSupportedException($"No conversion found for type {target[0]}");
        }
      }
      throw new NotSupportedException($"Feature contains no geometry");
    }
    catch (SpeckleConversionException e)
    {
      Console.WriteLine(e);
      throw; // log errors
    }
  }
}
