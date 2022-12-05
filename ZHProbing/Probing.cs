using NXOpen;
using NXOpen.Annotations;
using NXOpen.CAM;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Probing
{
    static public string momProbeTybpe = "";
    static public float ToolDiameter = 0;
    static public Vector3 ProbingDirection = new Vector3(1,0,0);
    static public int WSC { get; set; } = 1;

    public static void CreateProbing单向(ZHProbing ui)
    {

        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        
        NXOpen.CAM.NCGroup orientGeometry1 = ui.IsMultiAsix ? ui.MutltiAXISCsys : (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", 
            NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);

        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        NXOpen.CAM.ProbeInspectPointMoveBuilder.Direction dir = ProbeInspectPointMoveBuilder.Direction.Inferred;
        Point3d PointOffset = new Point3d(0, 0, 0);
        Point3d mcsVector = new Point3d(0, 0, 0);

  

        { // offset
            if (ui.ProbingDirection.Value == "-Z")
            {
                dir = ProbeInspectPointMoveBuilder.Direction.Negzm;
                mcsVector.Z = -1;
            }
            else if (ui.ProbingDirection.Value == "+X")
            {
                dir = ProbeInspectPointMoveBuilder.Direction.Xm;
                mcsVector.X = 1;

            }
            else if (ui.ProbingDirection.Value == "-X")
            {
                dir = ProbeInspectPointMoveBuilder.Direction.Negxm;

                mcsVector.X = -1;
             
            }
            else if (ui.ProbingDirection.Value == "+Y")
            {

                dir = ProbeInspectPointMoveBuilder.Direction.Ym;

                mcsVector.Y = 1;
            }
            else if (ui.ProbingDirection.Value == "-Y")
            {

                dir = ProbeInspectPointMoveBuilder.Direction.Negym;

                mcsVector.Y = -1;
            }
        }

        {
            if (mcsVector.Z == -1)
            {
                PointOffset.Z = ui.SafeClearance.Value + ToolDiameter * 0.5;
            }
            else if (mcsVector.X == 1)
            {
                PointOffset.X = -(ui.SafeClearance.Value + ToolDiameter * 0.5);
            }
            else if (mcsVector.X == -1)
            {
                PointOffset.X = ui.SafeClearance.Value + ToolDiameter * 0.5;
            }
            else if (mcsVector.Y == 1)
            {
                PointOffset.Y = -(ui.SafeClearance.Value + ToolDiameter * 0.5);
            }
            else if (mcsVector.Y == -1)
            {
                PointOffset.Y = ui.SafeClearance.Value + ToolDiameter * 0.5;

            }
        }

        Point3d p3d1 = ui.SelectPointList[0].WordPoint;
        Point3d p3d2 = ui.SelectPointList[0].MCSPoint;

        {//线性操作  X Y定位

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d pp1 = p3d2;
            Point3d pp2 = plane.Origin.ToMCSPoint();


            Point3d p1 = new Point3d(pp1.X, pp1.Y, pp2.Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1,CustomOperate.AddPoint3d(  p1 ,PointOffset));

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {

            Point3d pp1 = p3d2;

            Point3d p1 = CustomOperate.AddPoint3d(pp1, PointOffset);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        { //探测

            Point3d p1 = p3d1;

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint(ui,genericMotionControl1, p1,ui.SafeClearance.Value, dir);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        //回退
        {

            Point3d pp1 = p3d2;

            Point3d p1 = CustomOperate.AddPoint3d(pp1, PointOffset);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        {//回到安全高度

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d pp1 = p3d2;
            Point3d pp2 = plane.Origin.ToMCSPoint();


            Point3d p1 = new Point3d(pp1.X, pp1.Y, pp2.Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, CustomOperate.AddPoint3d(p1, PointOffset));

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //添加后处理 变量

            Point3d p1 = ui.SelectPointList[0].MCSPoint;

            if (ui.IsChangeCsys.Value)
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "1-POINT");
   
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量方向, dirStr);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[0].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[0].Point.Y);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[0].Point.Z);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);

            //ProbeAttribute.CreateAttribute(genericMotionControl1, "nxt_pos(0)", 0);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, "nxt_pos(1)", 0);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, "nxt_pos(2)", 0);


            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.探测触碰点0, p1.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.探测触碰点1, p1.Y);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.探测触碰点2, p1.Z);

            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
            //预留变量
            if (ui.ToggleMultilinesString.Value)
            {
                string[] values = ui.multiline_string0.GetValue();
                if (values.Length > 0)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        string[] momValue = values[i].Split('=');
                        ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                    }

                }
            }


        }

       
        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }


        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();


        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);


        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();


    }
    public static void CreateProbing单孔(ZHProbing ui)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);


        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }

        {//线性操作  X Y定位

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p1 = new Point3d(ui.InnerCirclePoint.Point.X, ui.InnerCirclePoint.Point.Y, plane.Origin.Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //探测点

            Point3d p1 = new Point3d(ui.InnerCirclePoint.Point.X, ui.InnerCirclePoint.Point.Y, ui.InnerCirclePoint.Point.Z);

            TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeBoreboss(ui,genericMotionControl1, selectObj[0], ProbeInspectBorebossMoveBuilder.Cycle.Bore);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);


        }


        {//回到安全高度

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p1 = new Point3d(ui.InnerCirclePoint.Point.X, ui.InnerCirclePoint.Point.Y, plane.Origin.Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //添加后处理 变量

            TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();
            Unit _uint = workPart.UnitCollection.FindObject("MilliMeter");
            NXOpen.MeasureDistance dis = workPart.MeasureManager.NewDistance(_uint, (Face)selectObj[0], NXOpen.MeasureManager.RadialMeasureType.Diameter);

            float minDistance = (float)dis.Value;
            dis.Dispose();


            if (ui.IsChangeCsys.Value)
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "HOLE");
          
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, minDistance);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, 1.01f);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.Angle1.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.InnerCirclePoint.Point.X);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.InnerCirclePoint.Point.Y);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.InnerCirclePoint.Point.Z);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标X, ui.OffsetX.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Y, ui.OffsetY.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Z, ui.OffsetZ.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点数, 1);
        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();


        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }


        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();

    }
    public static void CreateProbing外圆(ZHProbing ui)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);

        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }

        {//线性操作  X Y定位

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {
            Plane pl = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];

            NXOpen.CAM.Move nullNXOpen_CAM_MoveCl = null;
            NXOpen.CAM.ProbeClearanceMoveBuilder probeClearanceMoveBuilder1;
            probeClearanceMoveBuilder1 = genericMotionControl1.CAMMoveCollection.CreateProbeClearanceMoveBuilder(nullNXOpen_CAM_MoveCl);

            NXOpen.CAM.NcmClearanceBuilder ncmClearanceBuilder2;
            ncmClearanceBuilder2 = probeClearanceMoveBuilder1.ClearanceBuilder;
            ncmClearanceBuilder2.ClearanceType = NXOpen.CAM.NcmClearanceBuilder.ClearanceTypes.Plane;

            pl.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

            ncmClearanceBuilder2.PlaneXform = pl;

            ncmClearanceBuilder2.SafeDistance = pl.Expression.Value;

            nullNXOpen_CAM_MoveCl = (NXOpen.CAM.Move)probeClearanceMoveBuilder1.Commit();

            genericMotionControl1.AppendMove(nullNXOpen_CAM_MoveCl);

            probeClearanceMoveBuilder1.Destroy();
        }
      



        { //搜索行程

            if (ui.ToggleFaceAndPoint.Value)
            {

                Point3d p = ui.SelectPointList[0].WordPoint;

                p.Z = ui.ConeFace.Point.Z;

                Point3d p1 = p;

                NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeBoreboss圆柱点(ui, genericMotionControl1, p1, ProbeInspectBorebossMoveBuilder.Cycle.Boss);

                genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
            }
            else
            {

                Plane pl = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];

                Point3d p = pl.Origin.ToMCSPoint();

                p.Z = p.Z - ui.InCircleDepth.Value;

                Point3d p1 = p.ToWCSPoint();

                TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();

                NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeBoreboss圆柱面(ui, genericMotionControl1, p1, selectObj[0], ProbeInspectBorebossMoveBuilder.Cycle.Boss);

                genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
            }  
        }


        {//回到安全高度

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }



        if (ui.IsChangeCsys.Value)
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);

        { //添加后处理 变量

            //TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();
            //Unit _uint = workPart.UnitCollection.FindObject("MilliMeter");
            //NXOpen.MeasureDistance dis = workPart.MeasureManager.NewDistance(_uint, (Face)selectObj[0], NXOpen.MeasureManager.RadialMeasureType.Diameter);

            //float minDistance = (float)dis.Value;
            //dis.Dispose();
            if (ui.ProbingPointCount.Value == 4)
            {
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "1-PIN");
            
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, ui.InnerCircleDiameter.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.Angle1.Value);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[0].Point.X);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[0].Point.Y);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[0].Point.Z);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标X, ui.OffsetX.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Y, ui.OffsetY.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Z, ui.OffsetZ.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");
            }

            if (ui.ProbingPointCount.Value == 3)
            {
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "1-PIN");
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, ui.Geomtry.Value);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
               // ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, ui.InnerCircleDiameter.Value);
     
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.中心横坐标, ui.SelectPointList[0].MCSPoint.X);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.中心纵坐标, ui.SelectPointList[0].MCSPoint.Y);
     
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.RadialClearance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.Angle1.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.增量角, ui.Angle2.Value);

                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[0].Point.X);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[0].Point.Y);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[0].Point.Z);
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Name);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标X, ui.OffsetX.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Y, ui.OffsetY.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Z, ui.OffsetZ.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");

            }
        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();


        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();

    }
    public static void CreateProbing内圆(ZHProbing ui)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", 
            NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value
            );


        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }

        {//线性操作  X Y定位

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //搜索行程
         

            if(ui.ToggleFaceAndPoint.Value)
            {
                Point3d p = ui.SelectPointList[0].MCSPoint.ToWCSPoint();

                p.Z = ui.ConeFace.Point.Z;

                Point3d p1 = p;

                TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();

                NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeBoreboss圆柱点(ui, genericMotionControl1, p1, ProbeInspectBorebossMoveBuilder.Cycle.Bore);

                genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
            }
            else
            {
                Point3d p = ui.SelectPointList[0].MCSPoint.ToWCSPoint();

                Point3d p1 = p;

                TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();

                NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeBoreboss圆柱面(ui, genericMotionControl1, p1, selectObj[0], ProbeInspectBorebossMoveBuilder.Cycle.Bore);

                genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
            }
           
        }


        {//回到安全高度

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        if (ui.IsChangeCsys.Value)
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);

        { //添加后处理 变量
            if (ui.ProbingPointCount.Value == 4)
            {
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "1-HOLE");
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, Extend.LastGeomtryName);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, ui.InnerCircleDiameter.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, 1.01);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, 0);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[0].MCSPoint.X);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[0].MCSPoint.Y);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[0].MCSPoint.Z);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Name);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标X, ui.OffsetX.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Y, ui.OffsetY.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Z, ui.OffsetZ.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点数, ui.ProbingPointCount.Value);
            }

            if (ui.ProbingPointCount.Value == 3)
            {
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "1-HOLE");
                //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, Extend.LastGeomtryName);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, ui.InnerCircleDiameter.Value);

                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.RadialClearance.Value);

                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.Angle1.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.增量角, ui.Angle2.Value);
          

                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.中心横坐标, 0);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.中心纵坐标, 0);
      
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[0].MCSPoint.X);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[0].MCSPoint.Y);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[0].MCSPoint.Z);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Name);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标X, ui.OffsetX.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Y, ui.OffsetY.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Z, ui.OffsetZ.Value);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");

            }
        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();

        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();

    }
    public static void CreateProbing台阶(ZHProbing ui)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);

        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }


        {//线性操作  X Y定位

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//安全平面
           
            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeClearanceMove(genericMotionControl1, (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0], (Probing.ToolDiameter).ToString());;

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        { //Point1

            Point3d p1 = ui.SelectPointList[0].MCSPoint.ToWCSPoint();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint(ui,genericMotionControl1, p1, ui.SafeClearance.Value, ProbeInspectPointMoveBuilder.Direction.Negzm);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        { //Point2
           
            Point3d p1 = ui.SelectPointList[1].MCSPoint.ToWCSPoint();

            TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint(ui,genericMotionControl1, p1, ui.SafeClearance.Value, ProbeInspectPointMoveBuilder.Direction.Negzm);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//回到安全高度

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            var p = ui.SelectPointList[1].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //添加后处理 变量

   

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "2-POINT-HEIGHT");
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");

            if (ui.IsChangeCsys.Value)
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);


            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[0].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[0].Point.Y);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[0].Point.Z);

            //ProbeAttribute.CreateAttribute(genericMotionControl1, "nxt_pos(0)", ui.SelectPointList[0].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, "nxt_pos(1)", ui.SelectPointList[0].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, "nxt_pos(2)", ui.SelectPointList[0].Point.X);


            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.探测触碰点0, ui.SelectPointList[0].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.探测触碰点1, ui.SelectPointList[0].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.探测触碰点2, ui.SelectPointList[0].Point.X);

            double z = System.Math.Abs(System.Math.Abs(ui.SelectPointList[0].MCSPoint.Z) - System.Math.Abs(ui.SelectPointList[1].MCSPoint.Z));

            ProbeAttribute.CreateAttribute(genericMotionControl1, "probe_height", (z + 0.001).ToString("F3")); //台阶面高度名义值

            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);

     
        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();

        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();

    }
    public static void CreateProbing平面(ZHProbing ui)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);


        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }


        double safePointZ = ((Plane)ui.SafeClearancePlane.GetSelectedObjects()[0]).Origin.ToMCSPoint().Z;


        {//线性操作  X Y定位

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//下降到安全间隙面

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, safePointZ);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        {//探测点1

            Point3d p1 = ui.SelectPointList[0].MCSPoint.ToWCSPoint();
            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint(ui, genericMotionControl1, p1, ui.SafeClearance.Value, ProbeInspectPointMoveBuilder.Direction.Negzm);
            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//探测点1抬起

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, safePointZ);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1, MoveBuilder.Motion.Rapid );

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }



        {//探测点2定位

            Point3d p = ui.SelectPointList[1].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, safePointZ);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        { //探测点2
            Point3d p1 = ui.SelectPointList[1].MCSPoint.ToWCSPoint();
            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint(ui, genericMotionControl1, p1, 0, ProbeInspectPointMoveBuilder.Direction.Negzm);
            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//探测点2抬起

            Point3d p = ui.SelectPointList[1].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, safePointZ);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//探测点3定位

            Point3d p = ui.SelectPointList[2].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, safePointZ);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//探测点3

            Point3d p1 = ui.SelectPointList[2].MCSPoint.ToWCSPoint();
            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint(ui, genericMotionControl1, p1, ui.SafeClearance.Value, ProbeInspectPointMoveBuilder.Direction.Negzm);
            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//探测点3抬起

            Point3d p = ui.SelectPointList[2].MCSPoint;

            Point3d p1 = new Point3d(p.X, p.Y, safePointZ);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        {//回到安全高度
            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p1 = ui.SelectPointList[ui.SelectPointList.Count - 1].MCSPoint;

            Point3d p2 = new Point3d(p1.X, p1.Y, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p2);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //添加后处理 变量

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "3-POINT-PLANE");


            if (ui.IsChangeCsys.Value)
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);



            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.AngularX.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.增量角, ui.AngularY.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);


            if (ui.SelectPointList.Count == 3)
            {
                string disx1 = System.Math.Abs(ui.SelectPointList[0].MCSPoint.X - ui.SelectPointList[1].MCSPoint.X).ToString("F3");
                string disx2 = System.Math.Abs(ui.SelectPointList[0].MCSPoint.X - ui.SelectPointList[2].MCSPoint.X).ToString("F3");
                string disy2 = System.Math.Abs(ui.SelectPointList[0].MCSPoint.Y - ui.SelectPointList[2].MCSPoint.Y).ToString("F3");

                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.在平面第1轴上, disx1);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.在平面第2轴上, disx2);
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.在平面第3轴上, disy2);
            }

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Name);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);


            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");
        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();

        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();

    }
    public static void  CreateProbiung凸台(ZHProbing ui)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        Unit _uint = workPart.UnitCollection.FindObject("MilliMeter");

        NXOpen.MeasureDistance dis = workPart.MeasureManager.NewDistance(_uint, MeasureManager.MeasureType.Minimum,
            (Face)ui.face_select1.GetSelectedObjects()[0],
            (Face)ui.face_select1.GetSelectedObjects()[1]
        );

        //  dis.Information();

        float minDistance = (float)dis.Value;
        dis.Dispose();
        dis = null;

        Line[] lines = GetLines(ui);

        Point3d firstPoint = new Point3d();
        Point3d secondPoint = new Point3d();

        if (lines[0].StartPoint.ToMCSPoint().Z > lines[0].EndPoint.ToMCSPoint().Z)
        {
            Point3d p01 = lines[0].StartPoint.ToMCSPoint();
            Point3d p02 = lines[1].StartPoint.ToMCSPoint();

            firstPoint = p01.ToWCSPoint();
            secondPoint = (new Point3d(p02.X, p02.Y, p01.Z)).ToWCSPoint(); 
        }
        else
        {
            Point3d p01 = lines[0].EndPoint.ToMCSPoint();
            Point3d p02 = lines[1].EndPoint.ToMCSPoint();

            firstPoint = p01.ToWCSPoint();
            secondPoint = (new Point3d(p02.X, p02.Y, p01.Z)).ToWCSPoint();
        }

        double _z1 = firstPoint.ToMCSPoint().Z;
        double _z2 = secondPoint.ToMCSPoint().Z;

        Point3d pp1 = new Point3d(firstPoint.ToMCSPoint().X, firstPoint.ToMCSPoint().Y, _z1 - ui.InCircleDepth.Value);
        Point3d pp2 = new Point3d(secondPoint.ToMCSPoint().X, secondPoint.ToMCSPoint().Y, _z2 - ui.InCircleDepth.Value);


        firstPoint = pp1.ToWCSPoint();

        secondPoint = pp2.ToWCSPoint();

        //检测点1
        Vector3 point1 = pp1.ToVector3();

        //检测点2  
        Vector3 point2 = pp2.ToVector3();


        //目标检测2点方向
        Vector3 brobePointDir1 = (point1 - point2).normalized;
        Vector3 brobePointDir2 = brobePointDir1 * -1;
        //缓冲点1
        Vector3 safePoint1 = point1 + brobePointDir1 * ( 0 + (float)ui.SafeClearance.Value + ToolDiameter * 0.5f);

        //缓冲点2
        Vector3 safePoint2 = point2 + brobePointDir2 * ( 0 + (float)ui.SafeClearance.Value + ToolDiameter * 0.5f);


        //中心点
        Vector3 centerPoint = point1 + (point2 - point1).normalized * minDistance * 0.5f;


        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];
        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];
        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];
        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];
        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);

        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;


        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }


        {//线性操作  X Y定位 1

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p3d = new Point3d(centerPoint.x, centerPoint.z, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Traversal);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//Z 下降 

            Plane safeFace = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];

            Point3d p3d = new Point3d(centerPoint.x, centerPoint.z, safeFace.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Traversal);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        { //安全间隙高度

            Plane safeFace = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];
          
            Point3d p3d = new Point3d(safePoint1.x, safePoint1.z, safeFace.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //缓冲点
            Point3d p3d = new Point3d(safePoint1.x, safePoint1.z, safePoint1.y);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }



        { //搜索行程1

            NXOpen.CAM.Move nullNXOpen_CAM_Move = null;

            Point3d p1 = firstPoint.ToMCSPoint();
            Point3d p2 = safePoint1.ToNXPoint3d();

            if (System.Math.Abs(p1.Y - p2.Y) < 0.001) //如果是Y 方向
            {
                if(p1.X > p2.X)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Xm);
                }
               else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negxm);
                }
            }
            else
            {
                if (p1.Y > p2.Y)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Ym);
                }
                else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negym);
                }
            }

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //回退行程1
            Point3d p3d = safePoint1.ToNXPoint3d();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        { //安全间隙高度

            Plane safeFace = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];

            Point3d p3d = new Point3d(safePoint1.x, safePoint1.z, safeFace.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//线性操作  X Y定位 2
            Plane safeFace = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];
           
            Point3d p3d = new Point3d(safePoint2.x, safePoint2.z, safeFace.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//Z 下降  2
            Point3d p3d = safePoint2.ToNXPoint3d();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        { //搜索行程2

            Point3d p1 = secondPoint.ToMCSPoint();
            Point3d p2 = safePoint2.ToNXPoint3d();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = null;

            if (System.Math.Abs(p1.Y - p2.Y) < 0.001) //如果是X 方向
            {
                if (p1.X > p2.X)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Xm);
                }
                else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negxm);
                }
            }

          else
            {
                if (p1.Y > p2.Y)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Ym);
                }
                else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negym);
                }
            }


            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {// 回退
            Point3d p3d = safePoint2.ToNXPoint3d();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//线性操作  X Y定位 2
            Plane safeFace = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];
            Point3d p1 = safeFace.Origin.ToMCSPoint();

            Point3d p3d = new Point3d(safePoint2.x, safePoint2.z, p1.Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Rapid);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//回到XY 定位

            Plane safeFace = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];
            Point3d p1 = safeFace.Origin.ToMCSPoint();

            Point3d temp = new Point3d(safePoint1.x, safePoint1.z, p1.Z);

            Point3d p3d = new Point3d(centerPoint.x, centerPoint.z, temp.Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Traversal);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//回到安全高度

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p3d = new Point3d(centerPoint.x, centerPoint.z, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d, MoveBuilder.Motion.Traversal);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        if (ui.IsChangeCsys.Value)
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);


        { //添加后处理 变量
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "1-RIB");
           // ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, Extend.LastGeomtryName);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, minDistance);

            Plane safePlane = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];

            ProbeAttribute.CreateAttribute(genericMotionControl1, " probe_S_ID", safePlane.Expression.Value + ui.InCircleDepth.Value);


            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.AngleSet.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, point1.ToNXPoint3d().X);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, point1.ToNXPoint3d().Y);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, point1.ToNXPoint3d().Z);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标X, ui.OffsetX.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Y, ui.OffsetY.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Z, ui.OffsetZ.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");

        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();

        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh(); 
    }
    public static void CreateProbiung凹槽(ZHProbing ui)
    {
        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        Unit _uint = workPart.UnitCollection.FindObject("MilliMeter");

        NXOpen.MeasureDistance dis = workPart.MeasureManager.NewDistance(_uint, MeasureManager.MeasureType.Minimum,
            (Face)ui.face_select1.GetSelectedObjects()[0],
            (Face)ui.face_select1.GetSelectedObjects()[1]
        );

        //  dis.Information();

        float minDistance = (float)dis.Value;
        dis.Dispose();
        dis = null;

        Line[] lines = GetLines(ui);

        Point3d firstPoint = new Point3d();
        Point3d secondPoint = new Point3d();

        if (lines[0].StartPoint.ToMCSPoint().Z > lines[0].EndPoint.ToMCSPoint().Z)
        {
            Point3d p01 = lines[0].StartPoint.ToMCSPoint();
            Point3d p02 = lines[1].StartPoint.ToMCSPoint();

            firstPoint = p01.ToWCSPoint();
            secondPoint = (new Point3d(p02.X, p02.Y, p01.Z)).ToWCSPoint();
        }
        else
        {
            Point3d p01 = lines[0].EndPoint.ToMCSPoint();
            Point3d p02 = lines[1].EndPoint.ToMCSPoint();

            firstPoint = p01.ToWCSPoint();
            secondPoint = (new Point3d(p02.X, p02.Y, p01.Z)).ToWCSPoint();
        }

        double _z1 = firstPoint.ToMCSPoint().Z;
        double _z2 = secondPoint.ToMCSPoint().Z;


        Point3d pp1 = new Point3d(firstPoint.ToMCSPoint().X, firstPoint.ToMCSPoint().Y, _z1 - ui.InCircleDepth.Value);
        Point3d pp2 = new Point3d(secondPoint.ToMCSPoint().X, secondPoint.ToMCSPoint().Y, _z2 - ui.InCircleDepth.Value);


        firstPoint = pp1.ToWCSPoint();
        secondPoint = pp2.ToWCSPoint();


        //检测点1
        Vector3 point1 = pp1.ToVector3();

        //检测点2
        Vector3 point2 = pp2.ToVector3();


        //目标检测2天方向
        Vector3 brobePointDir1 = (point1 - point2).normalized;
        Vector3 brobePointDir2 = brobePointDir1 * -1;

        //缓冲点1
        Vector3 safePoint1 = point1 - brobePointDir1 * (0 + (float)ui.SafeClearance.Value + ToolDiameter * 0.5f);

        //缓冲点2
        Vector3 safePoint2 = point2 - brobePointDir2 * (0 + (float)ui.SafeClearance.Value + ToolDiameter * 0.5f);


        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];
        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];
        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];
        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];
        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);

        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }

        {//线性操作  X Y定位 1

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Vector3 p = point1 + (point2 - point1).normalized * (minDistance * 0.5f);

            Point3d p3d = new Point3d(p.x, p.z, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//Z 下降 

            Vector3 p = point1 + (point2 - point1).normalized * (minDistance * 0.5f);

            Point3d p3d = new Point3d(p.x, p.z, safePoint1.y);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        //{//靠近1

        //    Point3d p3d = safePoint1.ToNXPoint3d();

        //    NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d);

        //    genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        //}

        { //搜索行程1

            NXOpen.CAM.Move nullNXOpen_CAM_Move = null;

            Point3d p1 = firstPoint.ToMCSPoint();
            Point3d p2 = safePoint1.ToNXPoint3d();

            if (System.Math.Abs(p1.Y - p2.Y) < 0.001) //如果是Y 方向
            {
                if (p1.X > p2.X)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Xm);
                }
                else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negxm);
                }
            }
            else
            {
                if (p1.Y > p2.Y)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Ym);
                }
                else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, firstPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negym);
                }
            }

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        //{//靠近2

        //    Point3d p3d = safePoint2.ToNXPoint3d();

        //    NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d);

        //    genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        //}



        { //搜索行程2

            Point3d p1 = secondPoint.ToMCSPoint();
            Point3d p2 = safePoint2.ToNXPoint3d();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = null;

            if (System.Math.Abs(p1.Y - p2.Y) < 0.001) //如果是X 方向
            {
                if (p1.X > p2.X)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Xm);
                }
                else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negxm);
                }
            }

            else
            {
                if (p1.Y > p2.Y)
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Ym);
                }
                else
                {
                    nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui, genericMotionControl1, secondPoint, 0, ProbeInspectPointMoveBuilder.Direction.Negym);
                }
            }


            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {// 回退
            Vector3 p = point1 + (point2 - point1).normalized * (minDistance * 0.5f);

            Point3d p3d = new Point3d(p.x, p.z, safePoint2.y);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//回到安全高度

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Vector3 p = point1 + (point2 - point1).normalized * (minDistance * 0.5f);

            Point3d p3d = new Point3d(p.x, p.z, plane.Origin.ToMCSPoint().Z);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p3d);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //添加后处理 变量

            if (ui.IsChangeCsys.Value)
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);


            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "1-SLOT");
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, Extend.LastGeomtryName);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, minDistance);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.AngleSet.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, point1.ToNXPoint3d().X);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, point1.ToNXPoint3d().Y);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, point1.ToNXPoint3d().Z);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标X, ui.OffsetX.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Y, ui.OffsetY.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.设置坐标系偏置坐标Z, ui.OffsetZ.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");


        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();

        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();
    }
    public static void CreateProbiung2孔角向(ZHProbing ui)
    {

    

        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);


        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }

        {//线性操作  X Y定位
            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p1 = ui.SelectPointList[0].MCSPoint;
            p1.Z = plane.Origin.ToMCSPoint().Z;

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//安全平面

            Plane safeFace = (Plane)ui.SafeClearancePlane.GetSelectedObjects()[0];

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeClearanceMove(genericMotionControl1, safeFace, (Probing.ToolDiameter).ToString());

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        for(int i = 0; i< ui.SelectPointList.Count; i++)
        {
            { //探测点1


                TaggedObject[] selectObj = ui.face_select1.GetSelectedObjects();

                NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeBoreboss(ui, genericMotionControl1, selectObj[i], ProbeInspectBorebossMoveBuilder.Cycle.Bore);

                genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
            }

        }


        {//回到安全高度
            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p1 = ui.SelectPointList[ui.SelectPointList.Count - 1].MCSPoint;
            p1.Z = plane.Origin.ToMCSPoint().Z;

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        if (ui.IsChangeCsys.Value)
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);


        { //添加后处理 变量
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, "MCS");
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "2-HOLE-ANGLE");
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, ui.Geomtry.Value);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.外圆直径, ui.InnerCircleDiameter.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, 0);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[0].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[0].Point.Y);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[0].Point.Z);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点X, ui.SelectPointList[1].Point.X);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Y, ui.SelectPointList[1].Point.Y);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量点Z, ui.SelectPointList[1].Point.Z);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.刀具名称, ui.NCTool.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);
      
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");

        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();


        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();
    }
    public static void CreateProbiung2点角向(ZHProbing ui)
    {

        NXOpen.Part workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1 = (NXOpen.CAM.NCGroup)ui.DicNCGroup[ui.ProgramGroup.Value];

        NXOpen.CAM.Method method1 = (NXOpen.CAM.Method)ui.DicMethod[ui.Method.Value];

        NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)ui.DicTool[ui.NCTool.Value];

        NXOpen.CAM.NCGroup orientGeometry1 = (NXOpen.CAM.NCGroup)ui.DicGeom[Extend.LastGeomtryName];

        NXOpen.CAM.Operation operation1;
        operation1 = workPart.CAMSetup.CAMOperationCollection.Create(nCGroup1, method1, tool1, orientGeometry1, "probing", "PROBING", NXOpen.CAM.OperationCollection.UseDefaultName.True, ui.ProgramName.Value);


        NXOpen.CAM.GenericMotionControl genericMotionControl1 = ((NXOpen.CAM.GenericMotionControl)operation1);
        NXOpen.CAM.GmcOpBuilder gmcOpBuilder1;
        gmcOpBuilder1 = workPart.CAMSetup.CAMOperationCollection.CreateGmcopBuilder(genericMotionControl1);
        gmcOpBuilder1.FeedsBuilder.FeedCutBuilder.Value = ui.CutSpeed.Value;

        {
            if (ui.FastMoveType.Value == "G0-快速模式")
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G0;
            }
            else
            {
                gmcOpBuilder1.FeedsBuilder.FeedRapidOutput.Value = FeedRapidOutputMode.G1;
                gmcOpBuilder1.FeedsBuilder.FeedRapidBuilder.Value = ui.G01FastCutSpeed.Value;
            }
        }

        NXOpen.CAM.ProbeInspectPointMoveBuilder.Direction dir = ProbeInspectPointMoveBuilder.Direction.Inferred;
        Point3d PointOffset = new Point3d(0, 0, 0);
        Point3d mcsVector = new Point3d(0,0,0);

        { // offset
            if (ui.ProbingDirection.Value == "-Z")
            {
                dir = ProbeInspectPointMoveBuilder.Direction.Negzm;
                mcsVector.Z = -1;
            }
            else if (ui.ProbingDirection.Value == "+X")
            {
                dir = ProbeInspectPointMoveBuilder.Direction.Xm;
                mcsVector.X = 1;
            }
            else if (ui.ProbingDirection.Value == "-X")
            {
                dir = ProbeInspectPointMoveBuilder.Direction.Negxm;

                mcsVector.X = -1;
            }
            else if (ui.ProbingDirection.Value == "+Y")
            {
              
                dir = ProbeInspectPointMoveBuilder.Direction.Ym;

                mcsVector.Y = 1;
            }
            else if (ui.ProbingDirection.Value == "-Y")
            {
               
                dir = ProbeInspectPointMoveBuilder.Direction.Negym;

                mcsVector.Y = -1;
            }
        }

   
        {
            if (mcsVector.Z == -1)
            {
                PointOffset.Z = ui.SafeClearance.Value + ToolDiameter * 0.5;
            }
            else if (mcsVector.X == 1)
            {
                PointOffset.X = -(ui.SafeClearance.Value + ToolDiameter * 0.5);
            }
            else if (mcsVector.X == -1)
            {
                PointOffset.X = ui.SafeClearance.Value + ToolDiameter * 0.5;
            }
            else if (mcsVector.Y == 1)
            {
                PointOffset.Y = -(ui.SafeClearance.Value + +ToolDiameter * 0.5);
            }
            else if (mcsVector.Y == -1)
            {
                PointOffset.Y = ui.SafeClearance.Value + +ToolDiameter * 0.5;

            }
        }


        {//线性操作  X Y定位

            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = CustomOperate.AddPoint3d(PointOffset, new Point3d(p.X, p.Y, plane.Origin.ToMCSPoint().Z));

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        {//Z

            Point3d p = ui.SelectPointList[0].MCSPoint;

            Point3d p1 = CustomOperate.AddPoint3d(PointOffset, p);

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //探测点1

            Point3d p1 = ui.SelectPointList[0].MCSPoint.ToWCSPoint();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, p1, ui.SafeClearance.Value, dir);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

    

        { //探测点2

            Point3d p1 = ui.SelectPointList[1].MCSPoint.ToWCSPoint();

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateProbeInspectPoint2(ui,genericMotionControl1, p1, ui.SafeClearance.Value, dir);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }

        {//回退

            Point3d p01 = ui.SelectPointList[0].MCSPoint;
            Point3d p02 = ui.SelectPointList[1].MCSPoint;

            Point3d p1 = CustomOperate.AddPoint3d(PointOffset, new Point3d(p02.X, p02.Y, p01.Z));

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        {//回到安全高度
            Plane plane = (Plane)ui.SafeHeight.GetSelectedObjects()[0];

            Point3d p02 = ui.SelectPointList[1].MCSPoint;

            Point3d p1 = CustomOperate.AddPoint3d(PointOffset,new Point3d(p02.X, p02.Y, plane.Origin.ToMCSPoint().Z));

            NXOpen.CAM.Move nullNXOpen_CAM_Move = MoveControl.CreateLineMoveToPoint(genericMotionControl1, p1);

            genericMotionControl1.AppendMove(nullNXOpen_CAM_Move);
        }


        { //添加后处理 变量

            if (ui.IsChangeCsys.Value)
                ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, WSC);


            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量类型, momProbeTybpe);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.类型选择, "2-POINT-ANGLE");
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.指定坐标系, ui.Geomtry.Value);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.补偿刀具编号, ui.AddToolNumber.Value);
   
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量行程, ui.SafeClearance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果的置信区域, ui.ProbingResultRang.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.初始角度, ui.AngleSet.Value);
            //ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量方向, dir1);
            ProbeAttribute.CreateAttribute(genericMotionControl1, "probing_axis", ui.TextInfo.Value);

            {
                var point1 = ui.SelectPointList[0].MCSPoint;
                var point2 = ui.SelectPointList[1].MCSPoint;

                double disX = System.Math.Abs(point1.X - point2.X);
                double disY = System.Math.Abs(point1.Y - point2.Y);

               // pointList.Add($"点距 : X={ disX.ToString("F3")}," + $" Y={disY.ToString("F3")}");

                if(ui.TextInfo.Value == "XAXIS")
                    ProbeAttribute.CreateAttribute(genericMotionControl1, " probe_S_ID", disX.ToString("F3"));
                else
                    ProbeAttribute.CreateAttribute(genericMotionControl1, " probe_S_ID", disY.ToString("F3"));

            }

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测头长度, ui.ProbingHeadLenght.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件侧面余量, ui.BlankStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.部件底面余量, ui.WallStock.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.尺寸差异检查, ui.DimensRangCheck.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.上公差, ui.UpperTolerance.Value);
            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.下公差, ui.LowerTolerance.Value);

            ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.测量结果输出, ui.IsProbingResult.Value ? "NO" : "OFF");

        }



        gmcOpBuilder1.Commit();

        gmcOpBuilder1.Destroy();

        workPart.CAMSetup.GenerateToolPath(new NXOpen.CAM.CAMObject[] { genericMotionControl1 });

        NXOpen.CAM.GenericMotionControl gener = (NXOpen.CAM.GenericMotionControl)workPart.CAMSetup.CAMOperationCollection.FindObject(genericMotionControl1.JournalIdentifier);
        gener.SetName(ui.ProgramName.Value);

        ProbeAttribute.CreateAttribute(genericMotionControl1, MomDefine.ToolOffsetName, ui.OffsetToolName.Value);
        //预留变量
        if (ui.ToggleMultilinesString.Value)
        {
            string[] values = ui.multiline_string0.GetValue();
            if (values.Length > 0)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] momValue = values[i].Split('=');
                    ProbeAttribute.CreateAttribute(genericMotionControl1, momValue[0], momValue[1]);
                }

            }
        }

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();

    }

    public static Line[] GetLines(ZHProbing ui)
    {

        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.Part workPart = theSession.Parts.Work;
        NXOpen.Part displayPart = theSession.Parts.Display;
 
        NXOpen.Features.Feature nullNXOpen_Features_Feature = null;


        NXOpen.Features.IntersectionCurveBuilder intersectionCurveBuilder1;
        intersectionCurveBuilder1 = workPart.Features.CreateIntersectionCurveBuilder(nullNXOpen_Features_Feature);

  
        NXOpen.Plane plane2 = (NXOpen.Plane)ui.plane0.GetSelectedObjects()[0];
 
        NXOpen.Face[] face = new Face[ui.face_select1.GetSelectedObjects().Length];

        for (int i = 0; i < ui.face_select1.GetSelectedObjects().Length; i++)
        {
            face[i] = (NXOpen.Face)ui.face_select1.GetSelectedObjects()[i];
        }

        intersectionCurveBuilder1.CurveFitData.Tolerance = 0.001;

        intersectionCurveBuilder1.CurveFitData.AngleTolerance = 0.05;

        intersectionCurveBuilder1.FirstSet.Clear();


        NXOpen.FaceDumbRule faceDumbRule2;
        faceDumbRule2 = workPart.ScRuleFactory.CreateRuleFaceDumb(face);

        NXOpen.SelectionIntentRule[] rules3 = new NXOpen.SelectionIntentRule[1];
        rules3[0] = faceDumbRule2;
        intersectionCurveBuilder1.FirstFace.ReplaceRules(rules3, false);

        intersectionCurveBuilder1.FirstSet.Add(face);


        intersectionCurveBuilder1.SecondPlane = plane2;

        NXOpen.NXObject nXObject1;
        nXObject1 = intersectionCurveBuilder1.Commit();

        intersectionCurveBuilder1.Destroy();

        NXOpen.Features.IntersectionCurve curve = nXObject1 as NXOpen.Features.IntersectionCurve;

        Line[] lines = new Line[2];

        NXObject[] entities = curve.GetEntities();

        lines[0] = (Line)entities[0];

        lines[1] = (Line)entities[1];

        NXOpen.Features.RemoveParametersBuilder removeParametersBuilder1;
        removeParametersBuilder1 = workPart.Features.CreateRemoveParametersBuilder();

        removeParametersBuilder1.Objects.Add(new NXObject[] { nXObject1 });

        removeParametersBuilder1.Commit();

        removeParametersBuilder1.Destroy();

        theSession.DisplayManager.BlankObjects(lines);

        workPart.ModelingViews.WorkView.FitAfterShowOrHide(NXOpen.View.ShowOrHideType.HideOnly);

        return lines;
    }

    static void MessageBox(string msg)
    {
        ZHProbing.theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, msg);
    }

    static NXOpen.Features.Sphere CreateProbePreviewSphere(Point3d p)
    {
        NXOpen.Session theSession = NXOpen.Session.GetSession();

        var workPart = theSession.Parts.Work;

        NXOpen.Features.Sphere nullNXOpen_Features_Sphere = null;
        NXOpen.Features.SphereBuilder sphereBuilder1;
        sphereBuilder1 = workPart.Features.CreateSphereBuilder(nullNXOpen_Features_Sphere);
        sphereBuilder1.CenterPoint = workPart.Points.CreatePoint(p);


        sphereBuilder1.Diameter.RightHandSide = "1.5";
        sphereBuilder1.BooleanOption.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;
        nullNXOpen_Features_Sphere = (NXOpen.Features.Sphere)sphereBuilder1.Commit();

        {//点颜色设置

            NXOpen.DisplayModification displayModification1;
            displayModification1 = theSession.DisplayManager.NewDisplayModification();

            displayModification1.ApplyToAllFaces = true;

            displayModification1.ApplyToOwningParts = false;

            displayModification1.NewColor = 120;

            displayModification1.NewWidth = NXOpen.DisplayableObject.ObjectWidth.One;


            NXOpen.DisplayableObject[] objects1 = new NXOpen.DisplayableObject[1];
            NXOpen.Body body1 = (NXOpen.Body)workPart.Bodies.FindObject(nullNXOpen_Features_Sphere.JournalIdentifier);
            objects1[0] = body1;

            displayModification1.Apply(objects1);

            displayModification1.Dispose();

        }

        sphereBuilder1.Destroy();

        return nullNXOpen_Features_Sphere;
    }

    public static List<BalloonNote> BalloonList = new List<BalloonNote>();

    public static void CreateBollon(Point3d p,string indexStr)
    {
        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.Part workPart = theSession.Parts.Work;
        NXOpen.Part displayPart = theSession.Parts.Display;

        NXOpen.Annotations.BalloonNote nullAnnotations_BalloonNote = null;
        NXOpen.Annotations.BalloonNoteBuilder ballBuilder1;
        ballBuilder1 = workPart.PmiManager.PmiAttributes.CreateBalloonNoteBuilder(nullAnnotations_BalloonNote);
        ballBuilder1.Origin.SetInferRelativeToGeometry(true);
        ballBuilder1.Origin.Anchor = NXOpen.Annotations.OriginBuilder.AlignmentPosition.MidCenter;
        ballBuilder1.BalloonText = indexStr;
        ballBuilder1.Origin.Plane.PlaneMethod = NXOpen.Annotations.PlaneBuilder.PlaneMethodType.UserDefined;
        NXOpen.Annotations.LeaderData leaderData1;
        leaderData1 = workPart.Annotations.CreateLeaderData();


        ballBuilder1.Leader.Leaders.Append(leaderData1);

        leaderData1.StubSide = NXOpen.Annotations.LeaderSide.Inferred;


        Point3d coordinates1 = new Point3d(p.X, p.Y, p.Z);
        Point point1;
        point1 = workPart.Points.CreatePoint(coordinates1);

        Point3d point2 = new Point3d(p.X, p.Y, p.Z);


         leaderData1.Leader.SetValue(point1, workPart.ModelingViews.WorkView, point2);

        NXOpen.Annotations.LeaderData leaderData2;
        leaderData2 = workPart.Annotations.CreateLeaderData();

        leaderData2.Arrowhead = NXOpen.Annotations.LeaderData.ArrowheadType.Origin;

        ballBuilder1.Leader.Leaders.Append(leaderData2);

        leaderData2.StubSide = NXOpen.Annotations.LeaderSide.Inferred;

   

        Point3d point3 = new Point3d(p.X + 10, p.Y + 10, p.Z);
        ballBuilder1.Origin.Origin.SetValue(workPart.Points.CreatePoint(point3), null, point3); ;
        ballBuilder1.Origin.Anchor = OriginBuilder.AlignmentPosition.MidCenter;
        nullAnnotations_BalloonNote = (BalloonNote)ballBuilder1.Commit();
        ballBuilder1.Destroy();

        BalloonList.Add(nullAnnotations_BalloonNote);
    }

}

