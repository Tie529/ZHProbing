using NXOpen;


public class SelectPoint
{
    public Tag Tag;

    public string Name;

    public Point3d MCSPoint;

    public Point3d WordPoint;
    public string PointName(int index)
    {
        int y_espaceCount = 7 -MCSPoint.Y.ToString("F2").Length;

        string y = "";
        for(int i = 0; i < y_espaceCount; i++)
        {
            y += " ";
        }
        y += MCSPoint.Y.ToString("F2");


        int z_espaceCount = 7 - MCSPoint.Z.ToString("F2").Length;

        string z = "";
        for (int i = 0; i < z_espaceCount; i++)
        {
            z += " ";
        }
        z += MCSPoint.Z.ToString("F2");

        return $"{index},  ({MCSPoint.X.ToString("F2")}; {y}; {z})";
    }

    public SelectPoint(Point3d mp,Point3d wp)
    {
        MCSPoint = mp;

        WordPoint = wp;
    }
}


