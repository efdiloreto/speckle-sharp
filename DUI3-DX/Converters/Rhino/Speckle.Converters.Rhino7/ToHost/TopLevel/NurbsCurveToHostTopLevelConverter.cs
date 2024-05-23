﻿using Rhino;
using Speckle.Converters.Common;
using Speckle.Converters.Common.Objects;

namespace Speckle.Converters.Rhino7.ToHost.TopLevel;

[NameAndRankValue(nameof(SOG.Curve), NameAndRankValueAttribute.SPECKLE_DEFAULT_RANK)]
public class NurbsCurveToHostTopLevelConverter : SpeckleToHostGeometryBaseTopLevelConverter<SOG.Curve, RG.NurbsCurve>
{
  public NurbsCurveToHostTopLevelConverter(
    IConversionContextStack<RhinoDoc, UnitSystem> contextStack,
    ITypedConverter<SOG.Curve, RG.NurbsCurve> geometryBaseConverter
  )
    : base(contextStack, geometryBaseConverter) { }
}