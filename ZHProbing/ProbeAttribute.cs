using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.NXObject;

public class ProbeAttribute
{
    public static void MessageBox(string str)
    {
        UI theUI = UI.GetUI();

        theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, str);
    }

    public static void CreateAttribute(NXObject obj,string title, string value)
    {
        try
        {
            if (obj.HasUserAttribute(title, AttributeType.String, 0))
            {
                obj.DeleteUserAttribute(AttributeType.String, title, true, Update.Option.Now);
            }

            NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1] { obj };
            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = NXOpen.Session.GetSession().AttributeManager.CreateAttributePropertiesBuilder(ZHProbing.theSession.Parts.Work,
                objects1, NXOpen.AttributePropertiesBuilder.OperationType.None);


            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.String;

            attributePropertiesBuilder1.Category = "CustomEvent";

            attributePropertiesBuilder1.Title = title;

            attributePropertiesBuilder1.StringValue = value;

            attributePropertiesBuilder1.Commit();

            attributePropertiesBuilder1.Destroy();
        }
        catch(System.Exception ex)
        {
            MessageBox($"属性{title} = {value} 错误") ;
        }

    }

    public static void CreateAttribute(NXObject obj, string title, int value)
    {
        try
        {

            if (obj.HasUserAttribute(title, AttributeType.Integer, 0))
            {
                obj.DeleteUserAttribute(AttributeType.Integer, title, true, Update.Option.Now);
            }

            NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1] { obj };
            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = NXOpen.Session.GetSession().AttributeManager.CreateAttributePropertiesBuilder(ZHProbing.theSession.Parts.Work,
                objects1, NXOpen.AttributePropertiesBuilder.OperationType.None);


            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Integer;


            attributePropertiesBuilder1.Category = "CustomEvent";

            attributePropertiesBuilder1.Title = title;

            attributePropertiesBuilder1.IntegerValue = value;

            attributePropertiesBuilder1.Commit();

            attributePropertiesBuilder1.Destroy();

        }
        catch (System.Exception ex)
        {
            MessageBox($"属性{title} = {value}, {ex.Message}");
          
        }
    }

    public static void CreateAttribute(NXObject obj, string title, double value)
    {
        if (obj.HasUserAttribute(title, AttributeType.Real, 0))
        {
            obj.DeleteUserAttribute(AttributeType.Real, title, true, Update.Option.Now);
        }

        try
        {
            NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1] { obj };
            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = NXOpen.Session.GetSession().AttributeManager.CreateAttributePropertiesBuilder(ZHProbing.theSession.Parts.Work,
                objects1, NXOpen.AttributePropertiesBuilder.OperationType.None);


            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Number;

            attributePropertiesBuilder1.Category = "CustomEvent";

            attributePropertiesBuilder1.Title = title;

            attributePropertiesBuilder1.NumberValue = value;

            attributePropertiesBuilder1.Commit();

            attributePropertiesBuilder1.Destroy();
        }
        catch (System.Exception ex)
        {
            MessageBox($"属性{title} = {value} 错误");
        }
    }

    public static void SetUIAttribute(NXObject obj, string title,string value)
    {
        try
        {
            if (obj.HasUserAttribute(title, AttributeType.String, 0))
            {
                obj.DeleteUserAttribute(AttributeType.String, title, true, Update.Option.Now);
            }

            NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1] { obj };
            NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
            attributePropertiesBuilder1 = NXOpen.Session.GetSession().AttributeManager.CreateAttributePropertiesBuilder(ZHProbing.theSession.Parts.Work,
                objects1, NXOpen.AttributePropertiesBuilder.OperationType.None);


            attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.String;

            attributePropertiesBuilder1.Category = "UISetting";

            attributePropertiesBuilder1.Title = title;

            attributePropertiesBuilder1.StringValue = value;

            attributePropertiesBuilder1.Commit();

            attributePropertiesBuilder1.Destroy();

        }
        catch (System.Exception ex)
        {
            MessageBox($"属性{title} = {value} 错误");
        }
    }

    public static void SetUIAttribute(NXObject obj, string title, double value)
    {
        if (obj.HasUserAttribute(title, AttributeType.Real, 0))
        {
            obj.DeleteUserAttribute(AttributeType.Real, title, true, Update.Option.Now);
        }

        NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1] { obj };
        NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
        attributePropertiesBuilder1 = NXOpen.Session.GetSession().AttributeManager.CreateAttributePropertiesBuilder(ZHProbing.theSession.Parts.Work,
            objects1, NXOpen.AttributePropertiesBuilder.OperationType.None);


        attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Number;

        attributePropertiesBuilder1.Category = "UISetting";

        attributePropertiesBuilder1.Title = title;

        attributePropertiesBuilder1.NumberValue = value;

        attributePropertiesBuilder1.Commit();

        attributePropertiesBuilder1.Destroy();
    }

    public static void SetUIAttribute(NXObject obj, string title, bool value)
    {
        if (obj.HasUserAttribute(title, AttributeType.Boolean, 0))
        {
            obj.DeleteUserAttribute(AttributeType.Boolean, title, true, Update.Option.Now);
        }

        NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1] { obj };
        NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
        attributePropertiesBuilder1 = NXOpen.Session.GetSession().AttributeManager.CreateAttributePropertiesBuilder(ZHProbing.theSession.Parts.Work,
            objects1, NXOpen.AttributePropertiesBuilder.OperationType.None);


        attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Boolean;

        attributePropertiesBuilder1.Category = "UISetting";

        attributePropertiesBuilder1.Title = title;

        attributePropertiesBuilder1.BooleanValue = value ? NXOpen.AttributePropertiesBaseBuilder.BooleanValueOptions.True : AttributePropertiesBaseBuilder.BooleanValueOptions.False;

        attributePropertiesBuilder1.Commit();

        attributePropertiesBuilder1.Destroy();
    }

    public static void SetUIAttribute(NXObject obj, string title, int value)
    {
        if (obj.HasUserAttribute(title, AttributeType.Integer, 0))
        {
            obj.DeleteUserAttribute(AttributeType.Integer, title, true, Update.Option.Now);
        }

        NXOpen.NXObject[] objects1 = new NXOpen.NXObject[1] { obj };
        NXOpen.AttributePropertiesBuilder attributePropertiesBuilder1;
        attributePropertiesBuilder1 = NXOpen.Session.GetSession().AttributeManager.CreateAttributePropertiesBuilder(ZHProbing.theSession.Parts.Work,
            objects1, NXOpen.AttributePropertiesBuilder.OperationType.None);


        attributePropertiesBuilder1.DataType = NXOpen.AttributePropertiesBaseBuilder.DataTypeOptions.Integer;

        attributePropertiesBuilder1.Category = "UISetting";

        attributePropertiesBuilder1.Title = title;

        attributePropertiesBuilder1.IntegerValue = value;

        attributePropertiesBuilder1.Commit();

        attributePropertiesBuilder1.Destroy();
    }

    public static (bool, string) GetUIAttributeString(NXObject obj,string title)
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;
        if (workPart.HasUserAttribute(title, AttributeType.String, 0))
        {
            return (true, workPart.GetStringUserAttribute(title, 0));
        }

        return (false,"");
    }

    public static (bool, double) GetUIAttributeDouble (NXObject obj, string title)
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;
        if (workPart.HasUserAttribute(title, AttributeType.Real, 0))
        {
            return (true, workPart.GetRealUserAttribute(title, 0));
        }

        return (false, 0);
    }

    public static (bool, int) GetUIAttributeInt(NXObject obj, string title)
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;
        if (workPart.HasUserAttribute(title, AttributeType.Integer, 0))
        {
            return (true, workPart.GetIntegerUserAttribute(title, 0));
        }

        return (false, 0);
    }
}

