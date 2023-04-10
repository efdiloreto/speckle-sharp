#if ADVANCESTEEL2023
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Objects.BuiltElements;
using ASBeam = Autodesk.AdvanceSteel.Modelling.Beam;
using ASPoint3d = Autodesk.AdvanceSteel.Geometry.Point3d;

using static Autodesk.AdvanceSteel.DotNetRoots.Units.Unit;

namespace Objects.Converter.AutocadCivil
{
  public class BeamProperties : ASBaseProperties<ASBeam>, IASProperties
  {
    public override Dictionary<string, ASProperty> BuildedPropertyList()
    {
      Dictionary<string, ASProperty> dictionary = new Dictionary<string, ASProperty>();

      InsertProperty(dictionary, "profile section name", nameof(ASBeam.ProfSectionName));
      InsertProperty(dictionary, "profile section type", nameof(ASBeam.ProfSectionType));
      InsertProperty(dictionary, "systemline length", nameof(ASBeam.SysLength));
      InsertProperty(dictionary, "deviation", nameof(ASBeam.Deviation));
      InsertProperty(dictionary, "shrink value", nameof(ASBeam.ShrinkValue));
      InsertProperty(dictionary, "angle (radians)", nameof(ASBeam.Angle), eUnitType.kAngle);
      InsertProperty(dictionary, "profile name", nameof(ASBeam.ProfName));
      InsertProperty(dictionary, "run name", nameof(ASBeam.Runname));
      InsertProperty(dictionary, "Offsets", nameof(ASBeam.Offsets));
      InsertProperty(dictionary, "length", nameof(ASBeam.GetLength), eUnitType.kDistance);
      InsertProperty(dictionary, "weight (per meter)", nameof(ASBeam.GetWeightPerMeter), eUnitType.kWeightPerDistance);
      InsertProperty(dictionary, "paint area", nameof(ASBeam.GetPaintArea), eUnitType.kArea);

      InsertCustomProperty(dictionary, "start point", nameof(BeamProperties.GetPointAtStart), null);
      InsertCustomProperty(dictionary, "end point", nameof(BeamProperties.GetPointAtEnd), null);
      InsertCustomProperty(dictionary, "is cross section mirrored", nameof(BeamProperties.IsCrossSectionMirrored), null);
      InsertCustomProperty(dictionary, "reference axis description", nameof(BeamProperties.GetReferenceAxisDescription), null);
      InsertCustomProperty(dictionary, "reference axis", nameof(BeamProperties.GetReferenceAxis), null);
      InsertCustomProperty(dictionary, "weight", nameof(BeamProperties.GetWeight), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "weight (exact)", nameof(BeamProperties.GetWeightExact), null, eUnitType.kWeight);
      InsertCustomProperty(dictionary, "weight (fast)", nameof(BeamProperties.GetWeightFast), null, eUnitType.kWeight);
      //InsertCustomProperty(dictionary, "beam points", nameof(BeamProperties.GetListPoints), null);
      //InsertCustomProperty(dictionary, "beam line", nameof(BeamProperties.GetLine), null);
      InsertCustomProperty(dictionary, "profile type code", nameof(BeamProperties.GetProfileTypeCode), null);
      InsertCustomProperty(dictionary, "profile type", nameof(BeamProperties.GetProfileType), null);
      InsertCustomProperty(dictionary, "saw length", nameof(BeamProperties.GetSawLength), null, eUnitType.kAreaPerDistance);
      InsertCustomProperty(dictionary, "flange angle at start", nameof(BeamProperties.GetFlangeAngleAtStart), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "flange angle at end", nameof(BeamProperties.GetFlangeAngleAtEnd), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "web angle at start", nameof(BeamProperties.GetWebAngleAtStart), null, eUnitType.kAngle);
      InsertCustomProperty(dictionary, "web angle at end", nameof(BeamProperties.GetWebAngleAtEnd), null, eUnitType.kAngle);
      //InsertCustomProperty(dictionary, "saw information", nameof(BeamProperties.GetSawInformationComplete), null);

      return dictionary;
    }

    private static ASPoint3d GetPointAtStart(ASBeam beam)
    {
      return beam.GetPointAtStart();
    }

    private static ASPoint3d GetPointAtEnd(ASBeam beam)
    {
      return beam.GetPointAtEnd();
    }

    private static bool IsCrossSectionMirrored(ASBeam beam)
    {
      return beam.IsCrossSectionMirrored;
    }

    private static string GetReferenceAxisDescription(ASBeam beam)
    {
      return beam.RefAxis.ToString();
    }

    private static int GetReferenceAxis(ASBeam beam)
    {
      return (int)beam.RefAxis;
    }

    private static double GetWeight(ASBeam beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(beam.GetWeight(1));
    }

    private static double GetWeightExact(ASBeam beam)
    {
      //1 yields the weight, 2 the exact weight
      return RoundWeight(beam.GetWeight(2));
    }

    private static double GetWeightFast(ASBeam beam)
    {
      //3 the fast weight
      return RoundWeight(beam.GetWeight(3));
    }

    private static double RoundWeight(double value)
    {
      return Math.Round(value, 5, MidpointRounding.AwayFromZero);
    }

    //private static List<DSPoint> GetListPoints(ASBeam beam)
    //{
    //  List<DSPoint> pointList = new List<DSPoint>();

    //  if (beam is ASPolyBeam)
    //  {
    //    ASPolyBeam polyBeam = beam as ASPolyBeam;

    //    var polyLine = polyBeam.GetPolyline(true);
    //    foreach (var item in polyLine.Vertices)
    //      pointList.Add(item.ToDynPoint());
    //  }
    //  else
    //  {
    //    pointList.Add(beam.GetPointAtStart().ToDynPoint());
    //    pointList.Add(beam.GetPointAtEnd().ToDynPoint());
    //  }

    //  return pointList;
    //}

    //private static Autodesk.DesignScript.Geometry.Line GetLine(ASBeam beam)
    //{
    //  return Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(beam.GetPointAtStart().ToDynPoint(), beam.GetPointAtEnd().ToDynPoint());
    //}

    private static string GetProfileTypeCode(ASBeam beam)
    {
      return beam.GetProfType().GetDSTVValues().GetProfileTypeString();
    }

    private static int GetProfileType(ASBeam beam)
    {
      return (int)beam.GetProfType().GetDSTVValues().DSTVType;
    }

    private static double GetSawLength(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return sawLength;
    }

    private static double GetFlangeAngleAtStart(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return DegreeToRadian(flangeAngleAtStart);
    }

    private static double GetWebAngleAtStart(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return DegreeToRadian(webAngleAtStart);
    }

    private static double GetFlangeAngleAtEnd(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return DegreeToRadian(flangeAngleAtEnd);
    }

    private static double GetWebAngleAtEnd(ASBeam beam)
    {
      GetSawInformation(beam, out var sawLength, out var flangeAngleAtStart, out var webAngleAtStart, out var flangeAngleAtEnd, out var webAngleAtEnd);
      return DegreeToRadian(webAngleAtEnd);
    }

    //private static Dictionary<string, double> GetSawInformationComplete(ASBeam beam)
    //{
    //  Dictionary<string, double> ret = new Dictionary<string, double>();

    //  double sawLength = 0;
    //  double flangeAngleAtStart = 0;
    //  double webAngleAtStart = 0;
    //  double flangeAngleAtEnd = 0;
    //  double webAngleAtEnd = 0;
    //  ret.Add("SawLength", Utils.FromInternalDistanceUnits(sawLength, true));
    //  ret.Add("FlangeAngleAtStart", flangeAngleAtStart);
    //  ret.Add("WebAngleAtStart", webAngleAtStart);
    //  ret.Add("FlangeAngleAtEnd", flangeAngleAtEnd);
    //  ret.Add("WebAngleAtEnd", webAngleAtEnd);

    //  GetSawInformation(beam, out sawLength, out flangeAngleAtStart, out webAngleAtStart, out flangeAngleAtEnd, out webAngleAtEnd);

    //  ret["SawLength"] = Utils.FromInternalDistanceUnits(sawLength, true);
    //  ret["FlangeAngleAtStart"] = Utils.FromInternalAngleUnits(DegreeToRadian(flangeAngleAtStart), true);
    //  ret["WebAngleAtStart"] = Utils.FromInternalAngleUnits(DegreeToRadian(webAngleAtStart), true);
    //  ret["FlangeAngleAtEnd"] = Utils.FromInternalAngleUnits(DegreeToRadian(flangeAngleAtEnd), true);
    //  ret["WebAngleAtEnd"] = Utils.FromInternalAngleUnits(DegreeToRadian(webAngleAtEnd), true);

    //  return ret;
    //}

    private static void GetSawInformation(ASBeam beam, out double sawLength, out double flangeAngleAtStart, out double webAngleAtStart, out double flangeAngleAtEnd, out double webAngleAtEnd)
    {
      int executed = beam.GetSawInformation(out sawLength, out flangeAngleAtStart, out webAngleAtStart, out flangeAngleAtEnd, out webAngleAtEnd);
      //if (executed <= 0)
      //{
      //  throw new System.Exception("No values were found for this steel Beam from Function");
      //}
    }

  }
}
#endif
