using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class UserAnswersViewModel
    {
        public virtual AmsaReportStudentData student { get; set; }
        public virtual AmsaReportsItems items { get; set; }
        public Random rnd { get; set; }

        public UserAnswersViewModel(AmsaReportStudentData student,AmsaReportsItems items)
        {
            this.rnd = new Random();
            this.student = student;
            this.items = items;
        }

        //Get path to img to be set on template
        public string getScaleByInt(int i)
        {
            string ret = "../../Reports/Artwork/Scale_Score_Images/scale_score_";
            ret += i.ToString();
            ret += ".png";
            return ret;
        }
        
        public string getAverageTaskStyle()
        {
            double ret = ((double)(this.student.Stanine_Drive + this.student.Stanine_Structure)) / (double)2;
            return ret.ToString("0.0");
        }
        //Conceptual, Open-mindedness, Mastery
        public string getAdaptationStyle()
        {
            double ret = ((double)(this.student.Stanine_Conceptual + this.student.Stanine_Flexibility + this.student.Stanine_Mastery)) / (double)3;
            return ret.ToString("0.0");
        }
        
        //Ambition, Control, 
        public string getAchivementStyle()
        {
            double ret = ((double)(this.student.Stanine_Ambition + this.student.Stanine_Power)) / (double)2;
            return ret.ToString("0.0");
        }

        //Assertiveness, Approachability
        public string getInteractionStyle()
        {
            double ret = ((double)(this.student.Stanine_Assertiveness + this.student.Stanine_Liveliness)) / (double)2;
            return ret.ToString("0.0");
        }

        //Self-control, Positivity, Self-Awareness
        public string getEmotionalStyle()
        {
            double ret = ((double)(this.student.Stanine_Composure + this.student.Stanine_Positivity + this.student.Stanine_Awareness)) / (double)3;
            return ret.ToString("0.0");
        }
        //Group focus, Compassion, Humility
        public string getTeamworkStyle()
        {
            double ret = ((double)(this.student.Stanine_Cooperativeness + this.student.Stanine_Sensitivity + this.student.Stanine_Humility)) / (double)3;
            return ret.ToString("0.0");
        }
        //Get stings strings for responses
        //In this method we get strings to show scale to the user, using a b we build the list form 2 lists alternating using the random number funcion
        public List<string> getScaleItems(string scale, int stanine_number)
         {
            string gateField = getGateField(stanine_number);
            List<string> scaleFields = new List<string>();
            if (scale.Equals("Motivation")){
                scaleFields = getStanineListItemsForDisplay(this.items.InternalDrive, gateField);
            }
            if (scale.Equals("Organization")){
                scaleFields = getStanineListItemsForDisplay(this.items.Organization, gateField);
            }
            if (scale.Equals("Conceptual")){
                scaleFields = getStanineListItemsForDisplay(this.items.Conceptual, gateField);
            }
            if (scale.Equals("Open-mindedness")){
                scaleFields = getStanineListItemsForDisplay(this.items.OpenMindedness, gateField);
            }
            if (scale.Equals("Mastery")){
                scaleFields = getStanineListItemsForDisplay(this.items.Mastery, gateField);
            }
            if (scale.Equals("Ambition")){
                scaleFields = getStanineListItemsForDisplay(this.items.Ambition, gateField);
            }
            if (scale.Equals("Control")){
                scaleFields = getStanineListItemsForDisplay(this.items.Control, gateField);
            }
            if (scale.Equals("Assertiveness")){
                scaleFields = getStanineListItemsForDisplay(this.items.Assertiveness, gateField);
            }
            if (scale.Equals("Approachability")){
                scaleFields = getStanineListItemsForDisplay(this.items.Approachability, gateField);
            }
            if (scale.Equals("Self-Control")){
                scaleFields = getStanineListItemsForDisplay(this.items.Composure, gateField);
            }
            if (scale.Equals("Positivity")){
                scaleFields = getStanineListItemsForDisplay(this.items.Positivity, gateField);
            }
            if (scale.Equals("Self-Awareness")){
                scaleFields = getStanineListItemsForDisplay(this.items.SelfAwareness, gateField);
            }
            if (scale.Equals("Group Focus")){
                scaleFields = getStanineListItemsForDisplay(this.items.TeamOrientation, gateField);
            }
            if (scale.Equals("Compassion")){
                scaleFields = getStanineListItemsForDisplay(this.items.Compassion, gateField);
            }
            if (scale.Equals("Humility")){
                scaleFields = getStanineListItemsForDisplay(this.items.Humility, gateField);
            }
            return scaleFields;
        }
         
        //Get stanine list items (calls other methods but centralizes call to reduce repeat)
        public List<string> getStanineListItemsForDisplay(Report r, string gateField)
        {
            List<string> a = getListA(r, gateField);
            List<string> b = getListB(r, gateField);
            return getStanineItems(a, b);
        }

        //Get Paragraph
        public string getParagraph(string scale, int stanineDominant, int stanineSecondary)
        {
            //Get gate field values to pass over to reporutilities so it returns the paragraph
            string gateFieldDominant = this.getGateField(stanineDominant);
            string gateFieldSecondary = this.getGateField(stanineSecondary);
            //Get paragraph
            return ReportUtilities.GetReportUtilities.getParagraph(scale, gateFieldDominant, gateFieldSecondary);
        }
        
        public string getGateField(int stanine_number) {
            string gateField = "";
            //Get gate field
            if (stanine_number == 0 || stanine_number == 1 || stanine_number == 2 || stanine_number == 3)
                gateField = "Low";
            if (stanine_number == 4 || stanine_number == 5 || stanine_number == 6)
                gateField = "Med";
            if (stanine_number == 7 || stanine_number == 8 || stanine_number == 9)
                gateField = "High";
            return gateField;
        }


        //Get report and gate field (Low, Med, High) and return A List
        public List<string> getListA(Report r, string gateField) {
            List<string> ret = new List<string>();
            if (gateField.Equals("Low"))
            {
                ret = r.lstLowA;
            }
            if (gateField.Equals("Med"))
            {
                ret = r.lstMedA;
            }
            if (gateField.Equals("High"))
            {
                ret = r.lstHighA;
            }
            return ret;
        }

        //Get report and gate field (Low, Med, High) and return B List
        public List<string> getListB(Report r, string gateField)
        {
            List<string> ret = new List<string>();
            if (gateField.Equals("Low"))
            {
                ret = r.lstLowB;
            }
            if (gateField.Equals("Med"))
            {
                ret = r.lstMedB;
            }
            if (gateField.Equals("High"))
            {
                ret = r.lstHighB;
            }
            return ret;
        }

        public List<string> getStanineItems(List<string> a, List<string> b) {
            int i = 0;
            List<string> ret = new List<string>();
            foreach (string ans in a)
            {
                //Since both of the lists are the same one we will get through a and then switch to be depending on the value of the random int
                int rand = this.rnd.Next(1, 3);
                if (rand == 1)
                    ret.Add(a[i]);
                if (rand == 2)
                    ret.Add(b[i]);
                i++;
            }
            return ret;
        }



    }
}