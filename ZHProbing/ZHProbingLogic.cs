using NXOpen.CAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class ZHProbing
{

    void SelectProbingType()
    {
        SelectPointIndex = 0;

        this.Vector1.Show = false;

        this.ProbingSelectType.Show = true;

        this.ConeFace.Show = false;
        ToggleFaceAndPoint.Show = false;

        this.AngleSet.Show = false;

        IsChangeCsys.Value = false;
        this.ChangeCsysType.Show = IsChangeCsys.Value;

        //this.ToggleMultiAXIS.Value = false;
        //this.ToggleMultilinesString.Value = false;

        this.multiline_string0.Show = this.ToggleMultilinesString.Value;
        this.coord_system0.Show = this.ToggleMultiAXIS.Value;
        this.SafeClearancePlane.Show = false;

        pointStrList.Clear();
        this.pointList.SetListItems(new string[] { });

        if (this.ProbingType.Value == "内圆" || this.ProbingType.Value == "外圆")
        {
            this.group7.Show = true;
            this.ProbingPointCount.Show = true;

          

            if (this.ProbingPointCount.Value == 4)
            {
                this.Angle1.Show = false;
                this.Angle2.Show = false;
                this.Angle3.Show = false;
            }
            else
            {
                this.Angle1.Show = true;
                this.Angle2.Show = true;
                this.Angle3.Show = true;
            }

            this.RadialClearance.Show = true;
        }
        else
        {
            this.ProbingPointCount.Show = false;
            this.Angle1.Show = false;
            this.Angle2.Show = false;
            this.Angle3.Show = false;

            this.group7.Show = false;

            this.RadialClearance.Show = false;
        }
        

        if (this.ProbingType.Value == "内圆")
        {
            this.InnerCirclePoint.LabelString = "选择中心点";
            this.InnerCircleDiameter.Label = "内圆直径";
            this.ProbingPointCount.Show = true;
     
            this.InCircleDepth.Show = true;
            this.InnerCircleDiameter.Show = true;

            this.pointList.Show = false;
            this.SafeClearance.Show = true;
            this.ProbingDirection.Show = false;
        
            this.face_select1.Show = true;
        
            this.face_select1.LabelString = "内圆面";
            this.SafeClearance.Label = "避开距离";

            this.InnerCirclePoint.Show = true;

            this.TextInfo.Show = false;

            this.pointList.Show = true;

            ToggleFaceAndPoint.Show = true;
            ToggleFaceAndPoint.Value = false;

        }

        if (this.ProbingType.Value == "外圆")
        {

            this.InnerCirclePoint.LabelString = "选择中心点";
            this.InnerCircleDiameter.Label = "外圆直径";

            this.ProbingPointCount.Show = true;
         
            this.InCircleDepth.Show = true;
            this.InnerCircleDiameter.Show = true;

            this.pointList.Show = false;
            this.SafeClearance.Show = true;
            this.ProbingDirection.Show = false;
            this.SafeClearancePlane.Show = false;
            this.face_select1.Show = true;


            this.face_select1.LabelString = "外圆面";
            this.SafeClearance.Label = "避开距离";

            this.TextInfo.Show = false;

            this.pointList.Show = true;

            this.InnerCirclePoint.Show = true;

            this.SafeClearancePlane.Show = true;

            ToggleFaceAndPoint.Show = true;
            ToggleFaceAndPoint.Value = false;
        }


        if (this.ProbingType.Value == "台阶面")
        {
            this.ProbingPointCount.Show = false;
          
            this.InnerCircleDiameter.Show = false;
            this.InCircleDepth.Show = false;
            this.InnerCirclePoint.LabelString = "第一点";

            this.SafeClearance.Show = true;
            this.InnerCirclePoint.Show = true;
            this.face_select1.Show = false;

            this.SafeClearancePlane.Show = true;

            this.pointList.Show = true;

            this.TextInfo.Show = false;
        }

        if(this.ProbingType.Value == "平面")
        {
            this.ProbingPointCount.Show = false;
          
            this.InnerCircleDiameter.Show = false;
            this.InCircleDepth.Show = false;
    
            this.InnerCirclePoint.LabelString = "选择点";

            this.SafeClearance.Show = true;
            this.InnerCirclePoint.Show = true;
   
            this.pointList.Show = true;

            this.SafeClearancePlane.Show = true;

            this.TextInfo.Show = false;

            this.face_select1.Show = false;

            this.AngularX.Show = true;
            this.AngularY.Show = true;
        }
        else
        {
            this.AngularX.Show = false;
            this.AngularY.Show = false;
        }

        if (this.ProbingType.Value == "单向")
        {
            this.SafeClearancePlane.Show = false;
            this.ProbingPointCount.Show = false;
          
            this.InnerCircleDiameter.Show = false;
            this.InCircleDepth.Show = false;
  
            this.InnerCirclePoint.LabelString = "选择点";

            this.InnerCirclePoint.Show = true;
  
            this.pointList.Show = false;

            this.face_select1.Show = false;
   
            this.ProbingDirection.Show = true;

            this.pointList.Show = true;

            this.RadialClearance.Show = false;
            this.TextInfo.Show = false;
        }
     

        if (this.ProbingType.Value != "单向")
        {
            this.ProbingDirection.Show = false;
           
        }

        if (this.ProbingType.Value == "凹槽")
        {
            this.ProbingPointCount.Show = false;
            this.InnerCircleDiameter.Show = false;
            this.InCircleDepth.Show = true;
            this.InnerCirclePoint.Show = false;

            this.InnerCirclePoint.LabelString = "选择点";

            this.SafeClearance.Show = true;

            this.face_select1.Show = true;

            this.face_select1.LabelString = "面选择";
 
            this.SafeClearancePlane.Show = false;

      
            this.RadialClearance.Show = true;

            this.pointList.Show = true;
            this.plane0.Show = true;

            this.TextInfo.Show = true;

            this.AngleSet.Show = true;
        }
        else if (this.ProbingType.Value == "凸台")
        {
            this.plane0.Show = true;
            this.ProbingPointCount.Show = false;
            this.InnerCircleDiameter.Show = false;
            this.InCircleDepth.Show = true;
            this.InnerCirclePoint.Show = false;
       
            this.InnerCirclePoint.LabelString = "选择点";

            this.SafeClearance.Show = true;
        
            this.face_select1.Show = true;

            this.face_select1.LabelString = "面选择";
   
            this.SafeClearancePlane.Show = true;
        
            this.pointList.Show = true;

            this.RadialClearance.Show = false;

            this.TextInfo.Show = true;

            this.AngleSet.Show = true;

        }
        else
        {

            this.plane0.Show = false;
        }

        if (this.ProbingType.Value == "两孔角向")
        {
            this.ProbingPointCount.Show = false;
            
            this.InnerCircleDiameter.Show = false;
 
            this.InnerCirclePoint.Show = true;
        
            this.InnerCirclePoint.LabelString = "选择点";
    
            this.face_select1.Show = true;
     
            this.SafeClearancePlane.Show = true;

            this.face_select1.LabelString = "选择面";

            this.pointList.Show = true;

            this.InCircleDepth.Show = true;
            this.RadialClearance.Show = true;
            this.TextInfo.Show = true;

        }
        else if (this.ProbingType.Value == "两点角向")
        {
            this.ProbingPointCount.Show = false;
        
            this.InnerCircleDiameter.Show = false;
            this.InCircleDepth.Show = false;
            this.InnerCirclePoint.Show = true;
    
            this.InnerCirclePoint.LabelString = "选择点";

            this.ProbingDirection.Show = true;
           
            this.SafeClearancePlane.Show = false;
            this.face_select1.Show = false;
   
            this.InCircleDepth.Show = true;
           //this.RadialClearance.Show = true;
            this.pointList.Show = true;
            this.TextInfo.Show = true;

            this.AngleSet.Show = true;

        }

    }

    /// <summary>
    /// 补偿 == 工具设置  找正 ==切换测量坐标系
    /// </summary>
    void GroupChange()
    {
        if (ProbingSelectType.Value == "找正")
        {
            this.group6.Show = true;
            this.group18.Show = false;
        }
        else if (ProbingSelectType.Value == "测量")
        {
            this.group6.Show = false;
            this.group18.Show = false;
        }
        else if (ProbingSelectType.Value == "补偿")
        {
            this.group6.Show = false;
            this.group18.Show = true;
        }
    }

    void InitNCGroups()
    {
        var workPart = theSession.Parts.Work;

        NXOpen.CAM.NCGroup ncRoot = ((NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("NC_PROGRAM"));

        GetNCGroups(ncRoot);

        this.ProgramGroup.SetListItems(_NCGroupDic.Keys.ToArray());

        if (_NCGroupDic.Keys.Count > 0)
            this.ProgramGroup.Value = _NCGroupDic.Keys.ToArray()[0];
        else
            MessageBox("先建立程序文件夹");
    }

    void InitNCTools()
    {
        var workPart = theSession.Parts.Work;

        NXOpen.CAM.NCGroup ncRoot = ((NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("GENERIC_MACHINE"));

        CAMObject[] toos = ncRoot.GetMembers();

        foreach (CAMObject tool in toos)
        {
            if (tool.GetType() == typeof(Tool))
            {
                Tool.Types ty1;
                Tool.Subtypes ty2;

                Tool t1 = tool as Tool;

                t1.GetTypeAndSubtype(out ty1, out ty2);

                if (ty1 == Tool.Types.Solid && ty2 == Tool.Subtypes.Probe)
                {
                    if (!_NCToolDic.ContainsKey(tool.Name))
                        _NCToolDic.Add(tool.Name, t1);
                }

            }
        }



        if (_NCToolDic.Count > 0)
        {
            this.NCTool.SetListItems(_NCToolDic.Keys.ToArray());
            this.NCTool.Value = _NCToolDic.Keys.ToArray()[0];

            NXOpen.CAM.Tool tool1 = (NXOpen.CAM.Tool)DicTool[NCTool.Value];

            this.OffsetToolName.SetListItems(_NCToolDic.Keys.ToArray());
            this.OffsetToolName.Value = this.NCTool.Value;

            SetToolRadius(tool1);
        }
        else
        {
            MessageBox("先建立刀具");
        }

    }

    void InitNCOffsetTools()
    {

        if (ProbingSelectType.Value == "找正")
        {
            Probing.momProbeTybpe = "MCS";
        }
        else if (ProbingSelectType.Value == "测量")
        {
            Probing.momProbeTybpe = "MEASURE";
        }
        else if (ProbingSelectType.Value == "补偿")
        {
            Probing.momProbeTybpe = "TOOL";
        }

        var workPart = theSession.Parts.Work;

        NXOpen.CAM.NCGroup ncRoot = ((NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("GENERIC_MACHINE"));

        CAMObject[] toos = ncRoot.GetMembers();

        foreach (CAMObject tool in toos)
        {
            if (tool.GetType() == typeof(Tool))
            {
                Tool.Types ty1;
                Tool.Subtypes ty2;

                Tool t1 = tool as Tool;

                t1.GetTypeAndSubtype(out ty1, out ty2);

               // if (ty1 == Tool.Types.Solid || ty2 == Tool.Subtypes.Probe || ty1 == Tool.Types.Mill)
                {
                    if (!_NCOffsetToolDic.ContainsKey(tool.Name))
                        _NCOffsetToolDic.Add(tool.Name, t1);
                }

            }
        }


        if (_NCOffsetToolDic.Count > 0)
        {
            this.OffsetToolName.SetListItems(_NCOffsetToolDic.Keys.ToArray());
            this.OffsetToolName.Value = this._NCOffsetToolDic.Keys.ToList()[0];
        }
   
    }


    void InitMillGeoms()
    {
        var workPart = theSession.Parts.Work;

        NXOpen.CAM.NCGroup ncRoot = ((NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("GEOMETRY"));

        GetGeomtrys(ncRoot);

        if (_GeomtryDic.ContainsKey("GEOMETRY"))
            _GeomtryDic.Remove("_GeomtryDic");

        this.Geomtry.SetListItems(_GeomtryDic.Keys.ToArray());

        foreach(var v in _GeomtryDic)
        {
            if (v.Value.GetType() == typeof(NXOpen.CAM.OrientGeometry) && v.Key != "GEOMETRY")
            {
                this.Geomtry.Value = v.Key;
                Extend.LastGeomtryName = this.Geomtry.Value;

                MCSRoot = this.Geomtry.Value;
                break;
            }
            
        }
    }

    void InitMethods()
    {
        var workPart = theSession.Parts.Work;

        NXOpen.CAM.NCGroup ncRoot = ((NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("METHOD"));

        CAMObject[] methods = ncRoot.GetMembers();

        foreach (CAMObject obj in methods)
        {
            if (obj.GetType() == typeof(Method))
            {
                if (!_MethodDic.ContainsKey(obj.Name))
                    _MethodDic.Add(obj.Name, (Method)obj);

            
            }
        }

        this.Method.SetListItems(_MethodDic.Keys.ToArray());
        this.Method.Value = _MethodDic.Keys.ToArray()[0];

    }


    /// <summary>
    /// 递归获取所有程序文件夹
    /// </summary>
    /// <param name="root"></param>
    void GetNCGroups(NCGroup root)
    {
        if (!_NCGroupDic.ContainsKey(root.Name))
        {
            _NCGroupDic.Add(root.Name, root);
        }


        CAMObject[] camObjects = root.GetMembers();

        foreach (CAMObject obj in camObjects)
        {
            if (obj.GetType() == typeof(NCGroup))
            {
                if (!_NCGroupDic.ContainsKey(obj.Name))
                    _NCGroupDic.Add(obj.Name, (NCGroup)obj);

                GetNCGroups((NCGroup)obj);
            }
        }
    }

    /// <summary>
    /// 递归找出 几何体
    /// </summary>
    void GetGeomtrys(NCGroup root)
    {
        if (!_GeomtryDic.ContainsKey(root.Name) && root.Name != "GEOMETRY")
        {
            _GeomtryDic.Add(root.Name, root);
        }


        CAMObject[] camObjects = root.GetMembers();

        foreach (CAMObject obj in camObjects)
        {
            if ((obj.GetType() == typeof(OrientGeometry) || obj.GetType() == typeof(FeatureGeometry) )&& obj.Name != "GEOMETRY")
            {
                if (!_GeomtryDic.ContainsKey(obj.Name))
                    _GeomtryDic.Add(obj.Name, (NCGroup)obj);

                // MessageBox(obj.GetType().ToString());

                GetGeomtrys((NCGroup)obj);

            }
        }
    }

    /// <summary>
    /// 程序名检测
    /// </summary>
    bool CheckOperationName()
    {

        var workPart = theSession.Parts.Work;

        NXOpen.CAM.NCGroup ncRoot = ((NXOpen.CAM.NCGroup)workPart.CAMSetup.CAMGroupCollection.FindObject("NC_PROGRAM"));

        _OperaNameList.Clear();

        GetOperaNames(ncRoot);

        bool check = false;
        for(int i = 0; i < _OperaNameList.Count; i++)
        {
            if (_OperaNameList.Contains(this.ProgramName.Value.ToUpper()))
            {
                check = true;

                this.ProgramName.Value += new System.Random().Next(1,9);

                break;
            }
        }

        return check;
    }

    void GetOperaNames(NCGroup root)
    {
        CAMObject[] camObjects = root.GetMembers();

        foreach (CAMObject obj in camObjects)
        {
            if (!_OperaNameList.Contains(obj.Name))
                _OperaNameList.Add(obj.Name);

            if (obj.GetType() == typeof(NCGroup))
                GetOperaNames((NCGroup)obj);
        }
    }

    void SetToolRadius(Tool tool)
    {
        var workPart = theSession.Parts.Work;

        string info = "";
        NXOpen.UF.UFSession.GetUFSession().Param.AskStrValue(tool.Tag, NXOpen.UF.UFConstants.UF_PARAM_TL_DESCRIPTION, out info);
        System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[1-9]+");
        System.Text.RegularExpressions.MatchCollection amtch = reg.Matches(info);

        float val12 = 0;
        if (!float.TryParse(amtch[0].Value, out val12))
        {
            MessageBox("测量头描述不正确");
        }
        Probing.ToolDiameter = val12;

       // this.InCircleDepth.Value = Probing.ToolDiameter * 0.5f + 1;
    }

    NXOpen.CAM.OrientGeometry CreateCsys(NXOpen.CartesianCoordinateSystem sys)
    {
        var workPart = theSession.Parts.Work;

        NXOpen.CAM.NCGroup nCGroup1;
        nCGroup1 = workPart.CAMSetup.CAMGroupCollection.CreateGeometry((NXOpen.CAM.OrientGeometry)DicGeom[MCSRoot], "probing", "MCS_MILL", NXOpen.CAM.NCGroupCollection.UseDefaultName.True, "MCS_MILL_5");

        NXOpen.CAM.OrientGeometry orientGeometry1 = (NXOpen.CAM.OrientGeometry)nCGroup1;
        NXOpen.CAM.MillOrientGeomBuilder millOrientGeomBuilder1;
        millOrientGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillOrientGeomBuilder(orientGeometry1); 

  

        millOrientGeomBuilder1.SetCsysPurposeMode(NXOpen.CAM.OrientGeomBuilder.CsysPurposeModes.Local);

        millOrientGeomBuilder1.SetSpecialOutputMode(NXOpen.CAM.OrientGeomBuilder.SpecialOutputModes.CsysRotation);

        millOrientGeomBuilder1.Mcs = sys;

        orientGeometry1 = (NXOpen.CAM.OrientGeometry)millOrientGeomBuilder1.Commit();

        millOrientGeomBuilder1.Destroy();

        NXOpen.UF.UFSession.GetUFSession().UiOnt.Refresh();

        return orientGeometry1;
    }
}

