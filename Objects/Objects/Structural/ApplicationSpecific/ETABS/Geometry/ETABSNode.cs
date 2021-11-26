﻿using System;
using System.Collections.Generic;
using System.Text;
using Objects.Structural.Geometry;
using Objects.Structural.ETABS.Properties;
using Speckle.Core.Kits;
using Objects.Structural.Properties;
using Objects.Geometry;

namespace Objects.Structural.ETABS.Geometry
{
  public class ETABSNode : Node
  {
    public ETABSSpringProperty ETABSSpringProperty { get; set; }

    [SchemaInfo("Node with properties", "Creates a Speckle ETABS node with spring, mass and/or damper properties", "Structural", "Geometry")]
    public ETABSNode(Point basePoint,
    string name = null,
    [SchemaParamInfo("If null, restraint condition defaults to free/fully released")] Restraint restraint = null,
    [SchemaParamInfo("If null, axis defaults to world xy (z axis defines the vertical direction, positive direction is up)")] Axis constraintAxis = null,
    ETABSSpringProperty springProperty = null, PropertyMass massProperty = null, PropertyDamper damperProperty = null)
    {
      this.basePoint = basePoint;
      this.name = name;
      this.restraint = restraint == null ? new Restraint("RRRRRR") : restraint;
      this.constraintAxis = constraintAxis == null ? new Axis("Global", AxisType.Cartesian, new Plane(new Point(0, 0, 0), new Vector(0, 0, 1), new Vector(1, 0, 0), new Vector(0, 1, 0))) : constraintAxis;
      this.ETABSSpringProperty = springProperty;
      this.massProperty = massProperty;
      this.damperProperty = damperProperty;
    }

    public ETABSNode()
    {
    }
  }
}