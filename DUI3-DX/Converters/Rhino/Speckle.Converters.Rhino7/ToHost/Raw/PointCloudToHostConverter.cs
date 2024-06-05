﻿using System.Drawing;
using Rhino.Collections;
using Speckle.Converters.Common.Objects;

namespace Speckle.Converters.Rhino7.ToHost.Raw;

public class PointCloudToHostConverter : ITypedConverter<SOG.Pointcloud, RG.PointCloud>
{
  private readonly ITypedConverter<IReadOnlyList<double>, Point3dList> _pointListConverter;

  public PointCloudToHostConverter(ITypedConverter<IReadOnlyList<double>, Point3dList> pointListConverter)
  {
    _pointListConverter = pointListConverter;
  }

  /// <summary>
  /// Converts raw Speckle point cloud data to Rhino PointCloud object.
  /// </summary>
  /// <param name="target">The raw Speckle Pointcloud object to convert.</param>
  /// <returns>The converted Rhino PointCloud object.</returns>
  /// <remarks>⚠️ This conversion does NOT perform scaling.</remarks>
  public RG.PointCloud Convert(SOG.Pointcloud target)
  {
    var rhinoPoints = _pointListConverter.Convert(target.points);
    var rhinoPointCloud = new RG.PointCloud(rhinoPoints);

    if (target.colors.Count == rhinoPoints.Count)
    {
      for (int i = 0; i < rhinoPoints.Count; i++)
      {
        rhinoPointCloud[i].Color = Color.FromArgb(target.colors[i]);
      }
    }

    return rhinoPointCloud;
  }
}