namespace Backsight.Editor.Forms
{
    partial class IntersectDirectionAndDistanceForm
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
            this.directionPage = new Gui.Wizard.WizardPage();
            this.getDirection = new Backsight.Editor.Forms.GetDirectionControl();
            this.distancePage = new Gui.Wizard.WizardPage();
            this.getDistance = new Backsight.Editor.Forms.GetDistanceControl();
            this.finishPage = new Gui.Wizard.WizardPage();
            this.intersectInfo = new Backsight.Editor.Forms.IntersectInfoControl();
            this.wizard.SuspendLayout();
            this.directionPage.SuspendLayout();
            this.distancePage.SuspendLayout();
            this.finishPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.Controls.Add(this.finishPage);
            this.wizard.Controls.Add(this.distancePage);
            this.wizard.Controls.Add(this.directionPage);
            this.wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.directionPage,
            this.distancePage,
            this.finishPage});
            this.wizard.Size = new System.Drawing.Size(759, 238);
            // 
            // directionPage
            // 
            this.directionPage.Controls.Add(this.getDirection);
            this.directionPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directionPage.IsFinishPage = false;
            this.directionPage.Location = new System.Drawing.Point(0, 0);
            this.directionPage.Name = "directionPage";
            this.directionPage.Size = new System.Drawing.Size(759, 190);
            this.directionPage.TabIndex = 1;
            // 
            // getDirection
            // 
            this.getDirection.DirectionTitle = "Direction";
            this.getDirection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDirection.Location = new System.Drawing.Point(0, 0);
            this.getDirection.Name = "getDirection";
            this.getDirection.Size = new System.Drawing.Size(759, 190);
            this.getDirection.TabIndex = 0;
            // 
            // distancePage
            // 
            this.distancePage.Controls.Add(this.getDistance);
            this.distancePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.distancePage.IsFinishPage = false;
            this.distancePage.Location = new System.Drawing.Point(0, 0);
            this.distancePage.Name = "distancePage";
            this.distancePage.Size = new System.Drawing.Size(759, 190);
            this.distancePage.TabIndex = 2;
            // 
            // getDistance
            // 
            this.getDistance.DistanceTitle = "Distance";
            this.getDistance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDistance.Location = new System.Drawing.Point(0, 0);
            this.getDistance.Name = "getDistance";
            this.getDistance.Size = new System.Drawing.Size(759, 190);
            this.getDistance.TabIndex = 0;
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
            // IntersectDirectionAndDistanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 238);
            this.Name = "IntersectDirectionAndDistanceForm";
            this.Text = "Intersect Direction and Distance";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.IntersectDirectionAndDistanceForm_Shown);
            this.wizard.ResumeLayout(false);
            this.directionPage.ResumeLayout(false);
            this.distancePage.ResumeLayout(false);
            this.finishPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gui.Wizard.WizardPage finishPage;
        private IntersectInfoControl intersectInfo;
        private Gui.Wizard.WizardPage distancePage;
        private GetDistanceControl getDistance;
        private Gui.Wizard.WizardPage directionPage;
        private GetDirectionControl getDirection;

    }
}