using Speckle.Core.Models;

namespace Speckle.Converters.ArcGIS3.Utils;

public struct ObjectConversionTracker
{
  public Base Base { get; set; }
  public string NestedLayerName { get; set; }
  public ACG.Geometry? HostAppGeom { get; set; }
  public string? DatasetId { get; set; }
  public int? DatasetRow { get; set; }
  public string? MappedLayerURI { get; set; }
  public Exception? Exception { get; set; }

  public void AddException(Exception ex)
  {
    Exception = ex;
    HostAppGeom = null;
    DatasetId = null;
    DatasetRow = null;
    MappedLayerURI = null;
  }

  public ObjectConversionTracker(Base baseObj, string nestedLayerName)
  {
    Base = baseObj;
    NestedLayerName = nestedLayerName;
  }

  public ObjectConversionTracker(Base baseObj, string nestedLayerName, ACG.Geometry hostAppGeom)
  {
    Base = baseObj;
    NestedLayerName = nestedLayerName;
    HostAppGeom = hostAppGeom;
  }

  public ObjectConversionTracker(Base baseObj, string nestedLayerName, string datasetId)
  {
    Base = baseObj;
    NestedLayerName = nestedLayerName;
    DatasetId = datasetId;
  }
}
