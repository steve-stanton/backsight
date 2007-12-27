namespace Backsight.Editor.Forms
{
    partial class IntersectTwoDistancesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.distanceOnePage = new Gui.Wizard.WizardPage();
            this.getDistance1 = new Backsight.Editor.Forms.GetDistanceControl();
            this.distanceTwoPage = new Gui.Wizard.WizardPage();
            this.getDistance2 = new Backsight.Editor.Forms.GetDistanceControl();
            this.finishPage = new Gui.Wizard.WizardPage();
            this.intersectInfo = new Backsight.Editor.Forms.IntersectInfoControl();
            this.wizard.SuspendLayout();
            this.distanceOnePage.SuspendLayout();
            this.distanceTwoPage.SuspendLayout();
            this.finishPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.Controls.Add(this.finishPage);
            this.wizard.Controls.Add(this.distanceTwoPage);
            this.wizard.Controls.Add(this.distanceOnePage);
            this.wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.distanceOnePage,
            this.distanceTwoPage,
            this.finishPage});
            this.wizard.Size = new System.Drawing.Size(759, 238);
            // 
            // distanceOnePage
            // 
            this.distanceOnePage.Controls.Add(this.getDistance1);
            this.distanceOnePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.distanceOnePage.IsFinishPage = false;
            this.distanceOnePage.Location = new System.Drawing.Point(0, 0);
            this.distanceOnePage.Name = "distanceOnePage";
            this.distanceOnePage.Size = new System.Drawing.Size(759, 190);
            this.distanceOnePage.TabIndex = 1;
            // 
            // getDistance1
            // 
            this.getDistance1.DistanceTitle = "First Distance";
            this.getDistance1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDistance1.Location = new System.Drawing.Point(0, 0);
            this.getDistance1.Name = "getDistance1";
            this.getDistance1.Size = new System.Drawing.Size(759, 190);
            this.getDistance1.TabIndex = 0;
            // 
            // distanceTwoPage
            // 
            this.distanceTwoPage.Controls.Add(this.getDistance2);
            this.distanceTwoPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.distanceTwoPage.IsFinishPage = false;
            this.distanceTwoPage.Location = new System.Drawing.Point(0, 0);
            this.distanceTwoPage.Name = "distanceTwoPage";
            this.distanceTwoPage.Size = new System.Drawing.Size(759, 190);
            this.distanceTwoPage.TabIndex = 2;
            // 
            // getDistance2
            // 
            this.getDistance2.DistanceTitle = "Second Distance";
            this.getDistance2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDistance2.Location = new System.Drawing.Point(0, 0);
            this.getDistance2.Name = "getDistance2";
            this.getDistance2.Size = new System.Drawing.Size(759, 190);
            this.getDistance2.TabIndex = 0;
            // 
            // finishPage
            // 
            this.finishPage.Controls.Add(this.intersectInfo);
            this.finishPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finishPage.IsFinishPage = true;
            this.finishPage.Location = new System.Drawing.Point(0, 0);
            this.finishPage.Name = "finishPage";
            this.finishPage.Size = new System.Drawing.Size(759, 190);
            this.finishPage.TabIndex = 3;
            this.finishPage.CloseFromBack += new Gui.Wizard.PageEventHandler(this.finishPage_CloseFromBack);
            this.finishPage.ShowFromNext += new System.EventHandler(this.finishPage_ShowFromNext);
            // 
            // intersectInfo
            // 
            this.intersectInfo.CanHaveTwoIntersections = true;
            this.intersectInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.intersectInfo.Location = new System.Drawing.Point(0, 0);
            this.intersectInfo.Name = "intersectInfo";
            this.intersectInfo.Size = new System.Drawing.Size(759, 190);
            this.intersectInfo.TabIndex = 0;
            // 
            // IntersectTwoDistancesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 238);
            this.Name = "IntersectTwoDistancesForm";
            this.Text = "Intersect Two Distances";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.IntersectTwoDistancesForm_Shown);
            this.wizard.ResumeLayout(false);
            this.distanceOnePage.ResumeLayout(false);
            this.distanceTwoPage.ResumeLayout(false);
            this.finishPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gui.Wizard.WizardPage finishPage;
        private IntersectInfoControl intersectInfo;
        private Gui.Wizard.WizardPage distanceTwoPage;
        private GetDistanceControl getDistance2;
        private Gui.Wizard.WizardPage distanceOnePage;
        private GetDistanceControl getDistance1;

        //private GetDistanceControl getDistanceTwoControl;
        //private GetDistanceControl getDistanceOneControl;
    }
}