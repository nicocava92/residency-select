using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    //Singleton class for report utilities
    public class ReportUtilities
    {
        //Matrix used to get data for Scale paragraphs
        public static string[,] gateFieldMatrix { get; set; }
        private static ReportUtilities getReportUtilities;

        public static ReportUtilities GetReportUtilities
        {
            get {
                if(getReportUtilities == null)
                    getReportUtilities = new ReportUtilities();
                return getReportUtilities; }
            set { getReportUtilities = value; }
        }
        


        //Set values in the matrix to get scale values
        private ReportUtilities()
        {
            gateFieldMatrix = new string[4,4];
            gateFieldMatrix[1, 1] = "LM";
            gateFieldMatrix[1, 2] = "LM";
            gateFieldMatrix[1, 3] = "LM";
            gateFieldMatrix[2, 1] = "ML";
            gateFieldMatrix[2, 2] = "MM";
            gateFieldMatrix[2, 3] = "MH";
            gateFieldMatrix[3, 1] = "HM";
            gateFieldMatrix[3, 2] = "HM";
            gateFieldMatrix[3, 3] = "HM";
        }
        

        //Receives values for low, med, high and scale. Returns paragraph for summary
        public string getParagraph(string scale, string gateFieldDominant, string gateFieldSecondary)
        {
            //Get int values for the gateFields
            int dominant = this.intForGateField(gateFieldDominant);
            int secondary = this.intForGateField(gateFieldSecondary);
            //Get gateField value for the paragaraph related to the scale and dominante and secondary scales
            string gateFieldP = gateFieldMatrix[dominant,secondary];
            //Search for values in string loaded from database
            ApplicationDbContext dbo = new ApplicationDbContext();
            AonParagraphs a = dbo.lstAonParagraphs.Where(m => m.GATEField.Equals(gateFieldP) && m.scale.Equals(scale)).FirstOrDefault();
            return a.paragraph;
        }

        public int intForGateField(string s)
        {
            int ret = 1;
            if (s.Equals("Low"))
                ret = 1;
            if (s.Equals("Med"))
                ret = 2;
            if (s.Equals("High"))
                ret = 3;
            return ret;
        }
        
    }
}