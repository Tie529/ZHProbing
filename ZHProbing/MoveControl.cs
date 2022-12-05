using NXOpen;
using NXOpen.CAM;

public class MoveControl
{

    /// <summary>
    /// 线性坐标移动
    /// </summary>
    /// <param name="genericMotionControl"></param>
    /// <param name="p3d"></param>
    /// <returns></returns>
    public static NXOpen.CAM.Move CreateLineMoveToPoint(NXOpen.CAM.GenericMotionControl genericMotionControl, Point3d p3d, MoveBuilder.Motion mo = MoveBuilder.Motion.Traversal )
    {
        var workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.Move nullNXOpen_CAM_Move = null;

        //NXOpen.CAM.MoveToPointBuilder moveToPointBuilder1;
        //moveToPointBuilder1 = genericMotionControl.CAMMoveCollection.CreateMoveToPointBuilder(nullNXOpen_CAM_Move);

        //NXOpen.Point3d origin2 = new NXOpen.Point3d(0.0, 0.0, 0.0);
        //NXOpen.Vector3d vector1 = new NXOpen.Vector3d(0.0, 0.0, 1.0);
        //NXOpen.Direction direction1;
        //direction1 = workPart.Directions.CreateDirection(origin2, vector1, NXOpen.SmartObject.UpdateOption.AfterModeling);

        //moveToPointBuilder1.ProtectedMove = true;

        //moveToPointBuilder1.OffsetData.OffsetVector = direction1;

        //moveToPointBuilder1.RoundPoint.Rounding = false;

        //moveToPointBuilder1.RoundPoint.Decimals = NXOpen.CAM.RoundPointBuilder.Output.NoRounding;

        //moveToPointBuilder1.Point = workPart.Points.CreatePoint(p3d);


        ////移动类型
        //moveToPointBuilder1.MotionType = mo;

        //moveToPointBuilder1.FeedType = NXOpen.CAM.MoveBuilder.Feed.Motion;

        //nullNXOpen_CAM_Move = (NXOpen.CAM.Move)moveToPointBuilder1.Commit();

        //moveToPointBuilder1.Destroy();

        //return nullNXOpen_CAM_Move;

        NXOpen.CAM.AlongMcsAxisMoveBuilder alongMcsAxisMoveBuilder1;
        alongMcsAxisMoveBuilder1 = genericMotionControl.CAMMoveCollection.CreateAlongMcsAxisMoveBuilder(nullNXOpen_CAM_Move);

        alongMcsAxisMoveBuilder1.XAxis = p3d.X;

        alongMcsAxisMoveBuilder1.YAxis = p3d.Y;

        alongMcsAxisMoveBuilder1.ZAxis = p3d.Z;

        alongMcsAxisMoveBuilder1.ProtectedMove = true;

        alongMcsAxisMoveBuilder1.MotionType = mo;

        nullNXOpen_CAM_Move = (NXOpen.CAM.Move)alongMcsAxisMoveBuilder1.Commit();

        return nullNXOpen_CAM_Move;
    }

    /// <summary>
    /// 探测点
    /// </summary>
    /// <param name="genericMotionControl"></param>
    /// <param name="p3d"></param>
    /// <returns></returns>
    public static NXOpen.CAM.Move CreateProbeInspectPoint(ZHProbing ui, NXOpen.CAM.GenericMotionControl genericMotionControl, Point3d p3d, double safeDistance,
        NXOpen.CAM.ProbeInspectPointMoveBuilder.Direction dir, 
        MoveBuilder.Motion mo = MoveBuilder.Motion.Traversal)
    {
        var workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.Move nullNXOpen_CAM_Move = null;
        NXOpen.CAM.ProbeInspectPointMoveBuilder probeInspectPointMoveBuilder1;
        probeInspectPointMoveBuilder1 = genericMotionControl.CAMMoveCollection.CreateProbeInspectPointMoveBuilder(nullNXOpen_CAM_Move);


        probeInspectPointMoveBuilder1.DirectionType = dir;

        probeInspectPointMoveBuilder1.Point = workPart.Points.CreatePoint(p3d);

        probeInspectPointMoveBuilder1.ProbeProtectedParameters.StandoffDistance = safeDistance;


        probeInspectPointMoveBuilder1.MotionType = mo;

        probeInspectPointMoveBuilder1.ProtectedMove = true;
        NXOpen.CAM.Move obj = (NXOpen.CAM.Move)probeInspectPointMoveBuilder1.Commit();


        probeInspectPointMoveBuilder1.Destroy();

        return obj;
    }

    /// <summary>
    /// 探测点
    /// </summary>
    /// <param name="genericMotionControl"></param>
    /// <param name="p3d"></param>
    /// <returns></returns>
    public static NXOpen.CAM.Move CreateProbeInspectPoint2(ZHProbing ui, NXOpen.CAM.GenericMotionControl genericMotionControl, Point3d p3d, double safeDistance, NXOpen.CAM.ProbeInspectPointMoveBuilder.Direction dir)
    {
        var workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.Move nullNXOpen_CAM_Move = null;
        NXOpen.CAM.ProbeInspectPointMoveBuilder probeInspectPointMoveBuilder1;
        probeInspectPointMoveBuilder1 = genericMotionControl.CAMMoveCollection.CreateProbeInspectPointMoveBuilder(nullNXOpen_CAM_Move);


        probeInspectPointMoveBuilder1.DirectionType = dir;

        probeInspectPointMoveBuilder1.Point = workPart.Points.CreatePoint(p3d);

        probeInspectPointMoveBuilder1.ProbeProtectedParameters.StandoffDistance = safeDistance;

        probeInspectPointMoveBuilder1.ProtectedMove = true;
        NXOpen.CAM.Move obj = (NXOpen.CAM.Move)probeInspectPointMoveBuilder1.Commit();


        probeInspectPointMoveBuilder1.Destroy();

        return obj;
    }

    /// <summary>
    /// 曲面圆形
    /// </summary>
    /// <param name="genericMotionControl"></param>
    /// <param name="p3d"></param>
    /// <returns></returns>
    public static NXOpen.CAM.Move CreateProbeBoreboss(ZHProbing ui, NXOpen.CAM.GenericMotionControl genericMotionControl,
     TaggedObject face, ProbeInspectBorebossMoveBuilder.Cycle cycle)
    {

        var workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.Move nullNXOpen_CAM_Move = null;
        NXOpen.CAM.ProbeInspectBorebossMoveBuilder probeInspectPointMoveBuilder1;
        probeInspectPointMoveBuilder1 = genericMotionControl.CAMMoveCollection.CreateProbeInspectBorebossMoveBuilder(nullNXOpen_CAM_Move);
        //nullNXOpen_CAM_Move.SetName("Probe Point22");

        probeInspectPointMoveBuilder1.Cylinder = (NXOpen.Face)face;

        probeInspectPointMoveBuilder1.CycleType = cycle;

        probeInspectPointMoveBuilder1.BossDimension = ui.RadialClearance.Value;

        if (ui.InCircleDepth.Value < 0.01)
        {
            probeInspectPointMoveBuilder1.DepthType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Depth.Midpoint;
            probeInspectPointMoveBuilder1.MeasurementDepth = 0;
        }
        else
        {
            probeInspectPointMoveBuilder1.DepthType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Depth.Specify;
            probeInspectPointMoveBuilder1.MeasurementDepth = ui.InCircleDepth.Value;
        }


        probeInspectPointMoveBuilder1.ProbeProtectedParameters.StandoffDistance = ui.SafeClearance.Value;

        probeInspectPointMoveBuilder1.ProtectedMove = false;
        NXOpen.CAM.Move obj = (NXOpen.CAM.Move)probeInspectPointMoveBuilder1.Commit();

        probeInspectPointMoveBuilder1.Destroy();

        return obj;

    }

    /// <summary>
    /// 曲面圆形
    /// </summary>
    /// <param name="genericMotionControl"></param>
    /// <param name="p3d"></param>
    /// <returns></returns>
    public static NXOpen.CAM.Move CreateProbeBoreboss圆柱面(ZHProbing ui,
        NXOpen.CAM.GenericMotionControl genericMotionControl,
        Point3d p3d, 
        TaggedObject face, ProbeInspectBorebossMoveBuilder.Cycle cycle )
    {

        var workPart = ZHProbing.theSession.Parts.Work;
        NXOpen.CAM.Move nullNXOpen_CAM_Move = null;
        NXOpen.CAM.ProbeInspectBorebossMoveBuilder probeInspectPointMoveBuilder1;
        probeInspectPointMoveBuilder1 = genericMotionControl.CAMMoveCollection.CreateProbeInspectBorebossMoveBuilder(nullNXOpen_CAM_Move);
        probeInspectPointMoveBuilder1.Cylinder = (NXOpen.Face)face;
        probeInspectPointMoveBuilder1.CycleType = cycle;
        probeInspectPointMoveBuilder1.ProbeProtectedParameters.StandoffDistance = ui.SafeClearance.Value;
        probeInspectPointMoveBuilder1.ProtectedMove = false;
        probeInspectPointMoveBuilder1.BossDimension = ui.RadialClearance.Value;

        if (ui.ProbingPointCount.Value == 4)
        {
            probeInspectPointMoveBuilder1.AngleType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Angle.Predefined;
        }
        else if(ui.ProbingPointCount.Value == 3)
        {
            probeInspectPointMoveBuilder1.AngleType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Angle.Custom;

            //double angle = (ui.EndAngle.Value - ui.StartAngle.Value) / 3;

            //probeInspectPointMoveBuilder1.Angle1 = angle * 0 + ui.StartAngle.Value;
            //probeInspectPointMoveBuilder1.Angle2 = angle * 1 + ui.StartAngle.Value;
            //probeInspectPointMoveBuilder1.Angle3 = angle * 2 + ui.StartAngle.Value;

            probeInspectPointMoveBuilder1.Angle1 = ui.Angle1.Value;
            probeInspectPointMoveBuilder1.Angle2 = ui.Angle2.Value;
            probeInspectPointMoveBuilder1.Angle3 = ui.Angle3.Value;
        }

        if(ui.InCircleDepth.Value < 0.01)
        {
            probeInspectPointMoveBuilder1.DepthType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Depth.Midpoint;
            probeInspectPointMoveBuilder1.MeasurementDepth = 0;
        }
        else
        {
            probeInspectPointMoveBuilder1.DepthType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Depth.Specify;
            probeInspectPointMoveBuilder1.MeasurementDepth = ui.InCircleDepth.Value;
        }
      

        NXOpen.CAM.Move obj = (NXOpen.CAM.Move)probeInspectPointMoveBuilder1.Commit();

        probeInspectPointMoveBuilder1.Destroy();

        return obj;

    }


    /// <summary>
    /// 曲面圆形
    /// </summary>
    /// <param name="genericMotionControl"></param>
    /// <param name="p3d"></param>
    /// <returns></returns>
    public static NXOpen.CAM.Move CreateProbeBoreboss圆柱点(ZHProbing ui,
        NXOpen.CAM.GenericMotionControl genericMotionControl,
        Point3d p3d,
        ProbeInspectBorebossMoveBuilder.Cycle cycle)
    {

        var workPart = ZHProbing.theSession.Parts.Work;
        NXOpen.CAM.Move nullNXOpen_CAM_Move = null;
        NXOpen.CAM.ProbeInspectBorebossMoveBuilder probeInspectPointMoveBuilder1;
        probeInspectPointMoveBuilder1 = genericMotionControl.CAMMoveCollection.CreateProbeInspectBorebossMoveBuilder(nullNXOpen_CAM_Move);
        probeInspectPointMoveBuilder1.GeometryType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Geometry.Point;

        probeInspectPointMoveBuilder1.Point = workPart.Points.CreatePoint(p3d);
        {
   
            NXOpen.CAM.OrientGeometry mcs = (NXOpen.CAM.OrientGeometry)workPart.CAMSetup.CAMGroupCollection.FindObject(Extend.LastGeomtryName);

            NXOpen.CAM.MillOrientGeomBuilder millOrientGeomBuilder1;
            millOrientGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillOrientGeomBuilder(mcs);

            millOrientGeomBuilder1.Commit();

            NXOpen.Vector3d vector4 = new NXOpen.Vector3d(0.0, 0.0, 1.0);
            NXOpen.Direction direction4;
            direction4 = workPart.Directions.CreateDirection(millOrientGeomBuilder1.Mcs.Origin, vector4, NXOpen.SmartObject.UpdateOption.AfterModeling);
            probeInspectPointMoveBuilder1.ArcVector = direction4;


            millOrientGeomBuilder1.Destroy();

            probeInspectPointMoveBuilder1.Diameter = ui.InnerCircleDiameter.Value * 2;
        }

        probeInspectPointMoveBuilder1.ArcVector = (NXObject)ui.Vector1.GetSelectedObjects()[0];

        probeInspectPointMoveBuilder1.CycleType = cycle;
        probeInspectPointMoveBuilder1.ProbeProtectedParameters.StandoffDistance = ui.SafeClearance.Value;
        probeInspectPointMoveBuilder1.ProtectedMove = false;
        probeInspectPointMoveBuilder1.BossDimension = ui.RadialClearance.Value;

        if (ui.ProbingPointCount.Value == 4)
        {
            probeInspectPointMoveBuilder1.AngleType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Angle.Predefined;
        }
        else if (ui.ProbingPointCount.Value == 3)
        {
            probeInspectPointMoveBuilder1.AngleType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Angle.Custom;

            probeInspectPointMoveBuilder1.Angle1 = ui.Angle1.Value;
            probeInspectPointMoveBuilder1.Angle2 = ui.Angle2.Value;
            probeInspectPointMoveBuilder1.Angle3 = ui.Angle3.Value;
        }

        if (ui.InCircleDepth.Value < 0.01)
        {
            probeInspectPointMoveBuilder1.DepthType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Depth.Midpoint;
            probeInspectPointMoveBuilder1.MeasurementDepth = 0;
        }
        else
        {
            probeInspectPointMoveBuilder1.DepthType = NXOpen.CAM.ProbeInspectBorebossMoveBuilder.Depth.Specify;
            probeInspectPointMoveBuilder1.MeasurementDepth = ui.InCircleDepth.Value;
        }

        NXOpen.CAM.Move obj = (NXOpen.CAM.Move)probeInspectPointMoveBuilder1.Commit();

        probeInspectPointMoveBuilder1.Destroy();

        return obj;

    }


    /// <summary>
    /// 安全平面
    /// </summary>
    /// <param name="genericMotionControl"></param>
    /// <returns></returns>
    public static NXOpen.CAM.Move CreateProbeClearanceMove(NXOpen.CAM.GenericMotionControl genericMotionControl, Plane plane, string safeHeight)
    {
        var workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.Move nullNXOpen_CAM_Move = null;
        NXOpen.CAM.ProbeClearanceMoveBuilder probeClearanceMoveBuilder1;
        probeClearanceMoveBuilder1 = genericMotionControl.CAMMoveCollection.CreateProbeClearanceMoveBuilder(nullNXOpen_CAM_Move);


        NXOpen.CAM.NcmClearanceBuilder ncmClearanceBuilder1;
        ncmClearanceBuilder1 = probeClearanceMoveBuilder1.ClearanceBuilder;

        ncmClearanceBuilder1.ClearanceType = NXOpen.CAM.NcmClearanceBuilder.ClearanceTypes.Plane;

        ncmClearanceBuilder1.SafeDistance = plane.Origin.Z;


        ncmClearanceBuilder1.PlaneXform = plane;

        NXOpen.NXObject nXObject1;
        nXObject1 = probeClearanceMoveBuilder1.Commit();

        return (NXOpen.CAM.Move)nXObject1;
    }
}

