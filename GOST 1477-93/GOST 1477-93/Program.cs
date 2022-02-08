using System;
using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;
using NXOpenUI;
using ScrewNX;
using System.Windows.Forms;
using GOST_1477_93;

public class Program
{
    // class members
    private static Session theSession;
    private static UI theUI;
    private static UFSession theUfSession;
    public static Program theProgram;
    public static bool isDisposeCalled;

    //------------------------------------------------------------------------------
    // Constructor
    //------------------------------------------------------------------------------
    public Program()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theUfSession = UFSession.GetUFSession();
            isDisposeCalled = false;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            // UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, ex.Message);
        }
    }

    //------------------------------------------------------------------------------
    //  Explicit Activation
    //      This entry point is used to activate the application explicitly
    //------------------------------------------------------------------------------
    public static int Main(string[] args)
    {
        int retValue = 0;
        try
        {
            theProgram = new Program();
            Form f = new ScrewOptionsForm(theProgram);
            f.ShowDialog();
            theProgram.Dispose();
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Исключение", NXMessageBox.DialogType.Warning, ex.Message);
        }
        return retValue;
    }

    //------------------------------------------------------------------------------
    // Following method disposes all the class members
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        try
        {
            if (isDisposeCalled == false)
            {
                //TODO: Add your application code here 
            }
            isDisposeCalled = true;
        }
        catch (NXOpen.NXException ex)
        {
            // ---- Enter your exception handling code here -----

        }
    }

    public static int GetUnloadOption(string arg)
    {
        //Unloads the image explicitly, via an unload dialog
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);

        //Unloads the image immediately after execution within NX
        return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

        //Unloads the image when the NX session terminates
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }
    public void CreateScrew(Screw screw, double length, string name)
    {
        Tag part;
        int units = 1;
        theUfSession.Part.New(name, units, out part);

        double faskSize = screw.FaskSize;
        double diameter = screw.OuterDiameter;
        double innerDiameter = screw.InnerDiameter;
        double slotwidth = screw.SlotWidth;
        double threadPitch = screw.ThreadPitch;

        double[] rpt1 = { 0, 0, 0 };
        double[] rpt2 = { 0, 0, diameter / 2 };
        double[] rpt3 = { length, 0, diameter / 2 };
        double[] rpt4 = { length, 0, 0 };

        UFCurve.Line rline1 = new UFCurve.Line();
        UFCurve.Line rline2 = new UFCurve.Line();
        UFCurve.Line rline3 = new UFCurve.Line();
        UFCurve.Line rline4 = new UFCurve.Line();

        rline1.start_point = new double[3];
        rpt1.CopyTo(rline1.start_point, 0);
        rline1.end_point = new double[3];
        rpt2.CopyTo(rline1.end_point, 0);

        rline2.start_point = new double[3];
        rpt2.CopyTo(rline2.start_point, 0);
        rline2.end_point = new double[3];
        rpt3.CopyTo(rline2.end_point, 0);
        
        rline3.start_point = new double[3];
        rpt3.CopyTo(rline3.start_point, 0);
        rline3.end_point = new double[3];
        rpt4.CopyTo(rline3.end_point, 0);

        rline4.start_point = new double[3];
        rpt4.CopyTo(rline4.start_point, 0);
        rline4.end_point = new double[3];
        rpt1.CopyTo(rline4.end_point, 0);

        Tag[] srtags = new Tag[4];
        theUfSession.Curve.CreateLine(ref rline1, out srtags[0]);
        theUfSession.Curve.CreateLine(ref rline2, out srtags[1]);
        theUfSession.Curve.CreateLine(ref rline3, out srtags[2]);
        theUfSession.Curve.CreateLine(ref rline4, out srtags[3]);

        double[] ref_pt = { 0, 0, 0 };
        double[] direction1 = { 1, 0, 0 };
        string[] limits1 = { "0", "360" };
        Tag[] features1;
        theUfSession.Modl.CreateRevolved(srtags, limits1, ref_pt, direction1, FeatureSigns.Nullsign, out features1);
        
        
        Tag threadfeat;
        UFModl.SymbThreadData threadData = new UFModl.SymbThreadData();

        Tag body_obj_id;
        Tag[] faces;
        theUfSession.Modl.AskFeatBody(features1[0], out body_obj_id);
        theUfSession.Modl.AskBodyFaces(body_obj_id, out faces);
        
        foreach (var tag in faces)
        {
            TaggedObject obj = NXObjectManager.Get(tag);
            Face face = (Face)obj;
            if (face.SolidFaceType == Face.FaceType.Cylindrical)
                threadData.cyl_face = tag;
        }

        threadData.start_face = faces[2];
        threadData.pitch = threadPitch.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        threadData.rotation = 1;
        threadData.length_flag = 2;
        threadData.length = length.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        threadData.axis_direction = new double[] { 1, 0, 0 };
        threadData.angle = "60";
        threadData.minor_dia = innerDiameter.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        threadData.major_dia = diameter.ToString("G", System.Globalization.CultureInfo.InvariantCulture);

        threadData.callout = "M" +
            diameter.ToString("G", System.Globalization.CultureInfo.InvariantCulture) +
            "_X_" + 
            threadPitch.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        threadData.form = "Metric";
        threadData.num_starts = 1;
        threadData.include_instances = 2;

        theUfSession.Modl.CreateSymbThread(ref threadData, out threadfeat);


        double[] ept1 = { 0, diameter / 2 * (-1), slotwidth / 2 * (-1) };
        double[] ept2 = { 0, diameter / 2 * (-1), slotwidth / 2 };
        double[] ept3 = { 0, diameter / 2, slotwidth / 2 };
        double[] ept4 = { 0, diameter / 2, slotwidth / 2 * (-1) };

        UFCurve.Line eline1 = new UFCurve.Line();
        UFCurve.Line eline2 = new UFCurve.Line();
        UFCurve.Line eline3 = new UFCurve.Line();
        UFCurve.Line eline4 = new UFCurve.Line();

        eline1.start_point = new double[3];
        ept1.CopyTo(eline1.start_point, 0);
        eline1.end_point = new double[3];
        ept2.CopyTo(eline1.end_point, 0);

        eline2.start_point = new double[3];
        ept2.CopyTo(eline2.start_point, 0);
        eline2.end_point = new double[3];
        ept3.CopyTo(eline2.end_point, 0);

        eline3.start_point = new double[3];
        ept3.CopyTo(eline3.start_point, 0);
        eline3.end_point = new double[3];
        ept4.CopyTo(eline3.end_point, 0);

        eline4.start_point = new double[3];
        ept4.CopyTo(eline4.start_point, 0);
        eline4.end_point = new double[3];
        ept1.CopyTo(eline4.end_point, 0);

        Tag[] setags = new Tag[4];
        theUfSession.Curve.CreateLine(ref eline1, out setags[0]);
        theUfSession.Curve.CreateLine(ref eline2, out setags[1]);
        theUfSession.Curve.CreateLine(ref eline3, out setags[2]);
        theUfSession.Curve.CreateLine(ref eline4, out setags[3]);

        double[] direction2 = { 1, 0, 0 };
        string[] limits2 = { "0", ((diameter - innerDiameter) * 2).ToString("G", System.Globalization.CultureInfo.InvariantCulture) };
        string taper_angle1 = "0.0";
        Tag[] features2;
        theUfSession.Modl.CreateExtruded2(setags, taper_angle1, limits2, ref_pt, direction2, FeatureSigns.Negative, out features2);

        double[] rpt11 = { 0, 0, diameter / 2 - (diameter - innerDiameter) / 2 };
        double[] rpt12 = { 0, 0, diameter / 2 };
        double[] rpt13 = { (diameter - innerDiameter) / 2, 0, diameter / 2 };

        if (length < screw.MinStandartLength)  // для коротких винтов
            rpt13[0] = Math.Tan(30*180/Math.PI) * (diameter - innerDiameter) / 2;

        double[] rpt21 = { length, 0, diameter / 2 - faskSize };
        double[] rpt22 = { length, 0, diameter / 2 };
        double[] rpt23 = { length - faskSize, 0, diameter / 2 };

        UFCurve.Line rline11 = new UFCurve.Line();
        UFCurve.Line rline12 = new UFCurve.Line();
        UFCurve.Line rline13 = new UFCurve.Line();
        UFCurve.Line rline21 = new UFCurve.Line();
        UFCurve.Line rline22 = new UFCurve.Line();
        UFCurve.Line rline23 = new UFCurve.Line();

        rline11.start_point = new double[3];
        rpt11.CopyTo(rline11.start_point, 0);
        rline11.end_point = new double[3];
        rpt12.CopyTo(rline11.end_point, 0);

        rline12.start_point = new double[3];
        rpt12.CopyTo(rline12.start_point, 0);
        rline12.end_point = new double[3];
        rpt13.CopyTo(rline12.end_point, 0);

        rline13.start_point = new double[3];
        rpt13.CopyTo(rline13.start_point, 0);
        rline13.end_point = new double[3];
        rpt11.CopyTo(rline13.end_point, 0);

        rline21.start_point = new double[3];
        rpt21.CopyTo(rline21.start_point, 0);
        rline21.end_point = new double[3];
        rpt22.CopyTo(rline21.end_point, 0);

        rline22.start_point = new double[3];
        rpt22.CopyTo(rline22.start_point, 0);
        rline22.end_point = new double[3];
        rpt23.CopyTo(rline22.end_point, 0);

        rline23.start_point = new double[3];
        rpt23.CopyTo(rline23.start_point, 0);
        rline23.end_point = new double[3];
        rpt21.CopyTo(rline23.end_point, 0);

        Tag[] srtags1 = new Tag[3];
        theUfSession.Curve.CreateLine(ref rline11, out srtags1[0]);
        theUfSession.Curve.CreateLine(ref rline12, out srtags1[1]);
        theUfSession.Curve.CreateLine(ref rline13, out srtags1[2]);
        Tag[] srtags2 = new Tag[3];
        theUfSession.Curve.CreateLine(ref rline21, out srtags2[0]);
        theUfSession.Curve.CreateLine(ref rline22, out srtags2[1]);
        theUfSession.Curve.CreateLine(ref rline23, out srtags2[2]);

        Tag[] features3;
        theUfSession.Modl.CreateRevolved(srtags1, limits1, ref_pt, direction1, FeatureSigns.Negative, out features3);
        Tag[] features4;
        theUfSession.Modl.CreateRevolved(srtags2, limits1, ref_pt, direction1, FeatureSigns.Negative, out features4);
        //theUfSession.Part.Save();
    }
}

