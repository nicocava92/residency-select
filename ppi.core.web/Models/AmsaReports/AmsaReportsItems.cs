using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{

    public class Report
    {
        public List<string> lstLowA { get; set; }
        public List<string> lstLowB { get; set; }
        public List<string> lstMedA { get; set; }
        public List<string> lstMedB { get; set; }
        public List<string> lstHighA { get; set; }
        public List<string> lstHighB { get; set; }

        public Report()
        {
            lstLowA = new List<string>();
            lstLowB = new List<string>();
            lstMedA = new List<string>();
            lstMedB = new List<string>();
            lstHighA = new List<string>();
            lstHighB = new List<string>();
        }

    }

    public class InternalDrive : Report
    {
        public InternalDrive() : base() { }
    }
    public class Organization : Report
    {
        public Organization() : base() { }
    }
    public class Conceptual : Report
    {
        public Conceptual() : base() { }
    }
    public class OpenMindedness : Report
    {
        public OpenMindedness() : base() { }
    }

    public class Mastery : Report
    {
        public Mastery() : base() { }
    }
    public class Ambition : Report
    {
        public Ambition() : base() { }
    }
    public class Control : Report
    {
        public Control() : base() { }
    }
    public class Assertiveness : Report
    {
        public Assertiveness() : base() { }
    }
    public class Approachability : Report
    {
        public Approachability() : base() { }
    }
    public class Composure : Report
    {
        public Composure() : base() { }
    }
    public class Positivity : Report
    {
        public Positivity() : base() { }
    }
    public class SelfAwareness : Report
    {
        public SelfAwareness() : base() { }
    }

    public class TeamOrientation : Report
    {
        public TeamOrientation() : base() { }
    }
    public class Compassion : Report
    {
        public Compassion() : base() { }
    }
    public class Humility : Report
    {
        public Humility() : base() { }
    }

    public class AmsaReportsItems
    {
        public InternalDrive InternalDrive { get; set; }
        public Organization Organization { get; set; }
        public Conceptual Conceptual { get; set; }
        public OpenMindedness OpenMindedness { get; set; }
        public Mastery Mastery { get; set; }
        public Ambition Ambition { get; set; }
        public Control Control { get; set; }
        public Assertiveness Assertiveness { get; set; }
        public Approachability Approachability { get; set; }
        public Composure Composure { get; set; }
        public Positivity Positivity { get; set; }
        public SelfAwareness SelfAwareness { get; set; }
        public TeamOrientation TeamOrientation { get; set; }
        public Compassion Compassion { get; set; }
        public Humility Humility { get; set; }
        public AonItemsAF716AMSAVersionAB lstAon { get; set; }
        
        public AmsaReportsItems() {
            lstAon = new AonItemsAF716AMSAVersionAB();
            this.InternalDrive = new InternalDrive();
            this.Organization = new Organization();
            this.Conceptual = new Conceptual();
            this.OpenMindedness = new OpenMindedness();
            this.Mastery = new Mastery();
            this.Ambition = new Ambition();
            this.Control = new Control();
            this.Assertiveness = new Assertiveness();
            this.Approachability = new Approachability();
            this.Composure = new Composure();
            this.Positivity = new Positivity();
            this.SelfAwareness = new SelfAwareness();
            this.TeamOrientation = new TeamOrientation();
            this.Compassion = new Compassion();
            this.Humility = new Humility();
            //Now load Version a and Version b onto Low, Med, High
            ApplicationDbContext db = new ApplicationDbContext();
            List<AonItemsAF716AMSAVersionAB> lstReportItems = new List<AonItemsAF716AMSAVersionAB>();
            lstReportItems = db.lstReportItems.ToList();
            Console.WriteLine("Hello");
            //Cargar datos a las distintas listas
            foreach(var a in lstReportItems)
            {
                AonItemsAF716AMSAVersionAB r = (AonItemsAF716AMSAVersionAB)a;
                if (r.Report.Equals("Drive")){
                    this.AddField(this.InternalDrive, r);
                }
                if (r.Report.Equals("Structure"))
                {
                    this.AddField(this.Organization, r);
                }
                if (r.Report.Equals("Conceptual"))
                {
                    this.AddField(this.Conceptual, r);
                }
                if (r.Report.Equals("Flexibility"))
                {
                    this.AddField(this.OpenMindedness, r);
                }
                if (r.Report.Equals("Mastery"))
                {
                    this.AddField(this.Mastery, r);
                }
                if (r.Report.Equals("Ambition"))
                {
                    this.AddField(this.Ambition, r);
                }
                if (r.Report.Equals("Power"))
                {
                    this.AddField(this.Control, r);
                }
                if (r.Report.Equals("Assertiveness"))
                {
                    this.AddField(this.Assertiveness, r);
                }
                if (r.Report.Equals("Liveliness"))
                {
                    this.AddField(this.Approachability, r);
                }
                if (r.Report.Equals("Composure"))
                {
                    this.AddField(this.Composure,r);
                }
                if (r.Report.Equals("Positivity")) 
                {
                    this.AddField(this.Positivity, r);
                }
                if (r.Report.Equals("Awareness"))
                {
                    this.AddField(this.SelfAwareness, r);
                }
                if (r.Report.Equals("Cooperativeness"))
                {
                    this.AddField(this.TeamOrientation, r);
                }
                if (r.Report.Equals("Sensitivity"))
                {
                    this.AddField(this.Compassion, r);
                }
                if (r.Report.Equals("Humility"))
                {
                    this.AddField(this.Humility, r);
                }
            }
            
        }

        public void AddField(Report report, AonItemsAF716AMSAVersionAB r)
        {
            Console.WriteLine(r);
            if (r.GATEFields.Equals("Low"))
            {
                report.lstLowA.Add(r.VersionA);
                report.lstLowB.Add(r.VersionB);
            }
            if (r.GATEFields.Equals("Med"))
            {
                report.lstMedA.Add(r.VersionA);
                report.lstMedB.Add(r.VersionB);
            }
            if (r.GATEFields.Equals("High"))
            {
                report.lstHighA.Add(r.VersionA);
                report.lstHighB.Add(r.VersionB);
            }
        }
    }


}