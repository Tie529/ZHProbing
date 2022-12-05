using NXOpen;
using System;

public class CustomOperate
{
    public static Point3d AddPoint3d(Point3d a, Point3d b)
    {
        return new Point3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
}

public static class Extend
{
    public static string LastGeomtryName = "";

    public static double[] ToDouble(this Vector3d v3d)
    {
        return new double[3] { v3d.X, v3d.Y, v3d.Z };
    }

   public static Point3d ToMCSPoint(this Point3d p3d)
    {

        var workPart = ZHProbing.theSession.Parts.Work;

        NXOpen.CAM.OrientGeometry mcs = (NXOpen.CAM.OrientGeometry)workPart.CAMSetup.CAMGroupCollection.FindObject(LastGeomtryName);

        NXOpen.CAM.MillOrientGeomBuilder millOrientGeomBuilder1;
        millOrientGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillOrientGeomBuilder(mcs);

        millOrientGeomBuilder1.Commit();

        double[] from_origin = { 0, 0, 0 };
        double[] mx = new double[16];

        double[] to_origin = new double[3] { millOrientGeomBuilder1.Mcs.Origin.X, millOrientGeomBuilder1.Mcs.Origin.Y, millOrientGeomBuilder1.Mcs.Origin.Z };

        Vector3d v321;
        Vector3d v322;
        millOrientGeomBuilder1.Mcs.GetDirections(out v321, out v322);

        // NXOpen.UF.UFSession.GetUFSession().Mtx4.CsysToCsys(from_origin, v31.ToDouble(), v32.ToDouble(), to_origin, v321.ToDouble(), v322.ToDouble(), mx);


        NXOpen.UF.UFSession.GetUFSession().Mtx4.CsysToCsys( to_origin, v321.ToDouble(), v322.ToDouble(), from_origin, new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, mx);

        double[] inTarget = { p3d.X, p3d.Y, p3d.Z };
        double[] outTarget = new double[3];

        NXOpen.UF.UFSession.GetUFSession().Mtx4.Vec3Multiply(inTarget, mx, outTarget);

      
        // NXOpen.Utilities.NXObjectManager.Get)

        millOrientGeomBuilder1.Destroy();

        return new Point3d(outTarget[0], outTarget[1], outTarget[2]);
    }

    public static Point3d WordDirToMCSDir(this Point3d p3d)
    {

        var workPart = ZHProbing.theSession.Parts.Work;

        INXObject __MCS = workPart.CAMSetup.CAMGroupCollection.FindObject(LastGeomtryName);

        NXOpen.CAM.OrientGeometry mcs = __MCS as NXOpen.CAM.OrientGeometry;

        NXOpen.CAM.MillOrientGeomBuilder millOrientGeomBuilder1;
        millOrientGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillOrientGeomBuilder(mcs);

        millOrientGeomBuilder1.Commit();

        double[] from_origin = { 0, 0, 0 };
        double[] mx = new double[16];

        double[] to_origin = new double[3] { millOrientGeomBuilder1.Mcs.Origin.X, millOrientGeomBuilder1.Mcs.Origin.Y, millOrientGeomBuilder1.Mcs.Origin.Z };

        Vector3d v321;
        Vector3d v322;
        millOrientGeomBuilder1.Mcs.GetDirections(out v321, out v322);

        NXOpen.UF.UFSession.GetUFSession().Mtx4.CsysToCsys( from_origin, new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, to_origin, v321.ToDouble(), v322.ToDouble(), mx);

    
        double[] inTarget = { p3d.X, p3d.Y, p3d.Z, 0};
        double[] outTarget = new double[4];

        NXOpen.UF.UFSession.GetUFSession().Mtx4.VecMultiply(inTarget, mx, outTarget);


        millOrientGeomBuilder1.Destroy();

        return new Point3d(outTarget[0], outTarget[1], outTarget[2]);
    }

    public static Point3d MCSDirToWordDir(this Point3d p3d)
    {

        var workPart = ZHProbing.theSession.Parts.Work;

        INXObject __MCS = workPart.CAMSetup.CAMGroupCollection.FindObject(LastGeomtryName);

        NXOpen.CAM.OrientGeometry mcs = __MCS as NXOpen.CAM.OrientGeometry;

        NXOpen.CAM.MillOrientGeomBuilder millOrientGeomBuilder1;
        millOrientGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillOrientGeomBuilder(mcs);

        millOrientGeomBuilder1.Commit();

        double[] from_origin = { 0, 0, 0 };
        double[] mx = new double[16];

        double[] to_origin = new double[3] { millOrientGeomBuilder1.Mcs.Origin.X, millOrientGeomBuilder1.Mcs.Origin.Y, millOrientGeomBuilder1.Mcs.Origin.Z };

        Vector3d v321;
        Vector3d v322;
        millOrientGeomBuilder1.Mcs.GetDirections(out v321, out v322);

        NXOpen.UF.UFSession.GetUFSession().Mtx4.CsysToCsys(to_origin, v321.ToDouble(), v322.ToDouble(), from_origin, new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, mx);


        double[] inTarget = { p3d.X, p3d.Y, p3d.Z, 0 };
        double[] outTarget = new double[4];

        NXOpen.UF.UFSession.GetUFSession().Mtx4.VecMultiply(inTarget, mx, outTarget);


        millOrientGeomBuilder1.Destroy();

        return new Point3d(outTarget[0], outTarget[1], outTarget[2]);
    }

    public static Point3d ToWCSPoint(this Point3d p3d)
    {

        var workPart = ZHProbing.theSession.Parts.Work;

        INXObject __MCS = workPart.CAMSetup.CAMGroupCollection.FindObject(LastGeomtryName);

        NXOpen.CAM.OrientGeometry mcs = __MCS as NXOpen.CAM.OrientGeometry;

        NXOpen.CAM.MillOrientGeomBuilder millOrientGeomBuilder1;
        millOrientGeomBuilder1 = workPart.CAMSetup.CAMGroupCollection.CreateMillOrientGeomBuilder(mcs);

        millOrientGeomBuilder1.Commit();


        double[] from_origin = { 0, 0, 0 };
        double[] mx = new double[16];

        Vector3d v31;
        Vector3d v32;
        workPart.WCS.CoordinateSystem.GetDirections(out v31, out v32);

        double[] to_origin = new double[3] { millOrientGeomBuilder1.Mcs.Origin.X, millOrientGeomBuilder1.Mcs.Origin.Y, millOrientGeomBuilder1.Mcs.Origin.Z };

        Vector3d v321;
        Vector3d v322;
        millOrientGeomBuilder1.Mcs.GetDirections(out v321, out v322);

        // NXOpen.UF.UFSession.GetUFSession().Mtx4.CsysToCsys(from_origin, v31.ToDouble(), v32.ToDouble(), to_origin, v321.ToDouble(), v322.ToDouble(), mx);


        NXOpen.UF.UFSession.GetUFSession().Mtx4.CsysToCsys(from_origin, new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 }, to_origin, v321.ToDouble(), v322.ToDouble(), mx);

        double[] inTarget = { p3d.X, p3d.Y, p3d.Z };
        double[] outTarget = new double[3];

        NXOpen.UF.UFSession.GetUFSession().Mtx4.Vec3Multiply(inTarget, mx, outTarget);


        // NXOpen.Utilities.NXObjectManager.Get)

        millOrientGeomBuilder1.Destroy();

        return new Point3d(outTarget[0], outTarget[1], outTarget[2]);
    }

}
