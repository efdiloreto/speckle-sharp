﻿using Rhino;
using Speckle.Converters.Common;
using Speckle.Converters.Common.Objects;

namespace Speckle.Converters.Rhino7.ToHost.TopLevel;

[NameAndRankValue(nameof(SOG.Line), NameAndRankValueAttribute.SPECKLE_DEFAULT_RANK)]
public class SpeckleLineToHostLineCurveConversion : SpeckleToHostGeometryBaseConversion<SOG.Line, RG.LineCurve>
{
  public SpeckleLineToHostLineCurveConversion(
    IConversionContextStack<RhinoDoc, UnitSystem> contextStack,
    IRawConversion<SOG.Line, RG.LineCurve> geometryBaseConverter
  )
    : base(contextStack, geometryBaseConverter) { }
}
