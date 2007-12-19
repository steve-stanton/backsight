namespace Backsight.Editor.Forms
{
    partial class IntersectTwoDirectionsForm
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
            this.directionOnePage = new Gui.Wizard.WizardPage();
            this.getDirection1 = new Backsight.Editor.Forms.GetDirectionControl();
            this.directionTwoPage = new Gui.Wizard.WizardPage();
            this.getDirection2 = new Backsight.Editor.Forms.GetDirectionControl();
            this.finishPage = new Gui.Wizard.WizardPage();
            this.intersectInfo = new Backsight.Editor.Forms.IntersectInfoControl();
            this.wizard.SuspendLayout();
            this.directionOnePage.SuspendLayout();
            this.directionTwoPage.SuspendLayout();
            this.finishPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.Controls.Add(this.directionTwoPage);
            this.wizard.Controls.Add(this.finishPage);
            this.wizard.Controls.Add(this.directionOnePage);
            this.wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.directionOnePage,
            this.directionTwoPage,
            this.finishPage});
            this.wizard.Size = new System.Drawing.Size(759, 238);
            // 
            // directionOnePage
            // 
            this.directionOnePage.Controls.Add(this.getDirection1);
            this.directionOnePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directionOnePage.IsFinishPage = false;
            this.directionOnePage.Location = new System.Drawing.Point(0, 0);
            this.directionOnePage.Name = "directionOnePage";
            this.directionOnePage.Size = new System.Drawing.Size(759, 190);
            this.directionOnePage.TabIndex = 1;
            // 
            // getDirection1
            // 
            this.getDirection1.DirectionTitle = "First Direction";
            this.getDirection1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDirection1.Location = new System.Drawing.Point(0, 0);
            this.getDirection1.Name = "getDirection1";
            this.getDirection1.Size = new System.Drawing.Size(759, 190);
            this.getDirection1.TabIndex = 0;
            // 
            // directionTwoPage
            // 
            this.directionTwoPage.Controls.Add(this.getDirection2);
            this.directionTwoPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directionTwoPage.IsFinishPage = false;
            this.directionTwoPage.Location = new System.Drawing.Point(0, 0);
            this.directionTwoPage.Name = "directionTwoPage";
            this.directionTwoPage.Size = new System.Drawing.Size(759, 190);
            this.directionTwoPage.TabIndex = 2;
            this.directionTwoPage.ShowFromNext += new System.EventHandler(this.directionTwoPage_ShowFromNext);
            // 
            // getDirection2
            // 
            this.getDirection2.DirectionTitle = "Second Direction";
            this.getDirection2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDirection2.Location = new System.Drawing.Point(0, 0);
            this.getDirection2.Name = "getDirection2";
            this.getDirection2.Size = new System.Drawing.Size(759, 190);
            this.getDirection2.TabIndex = 0;
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
            this.finishPage.ShowFromNext += new System.EventHandler(this.finishPage_ShowFromNext);
            // 
            // intersectInfo
            // 
            this.intersectInfo.CanHaveTwoIntersections = false;
            this.intersectInfo.Location = new System.Drawing.Point(12, 12);
            this.intersectInfo.Name = "intersectInfo";
            this.intersectInfo.Size = new System.Drawing.Size(604, 136);
            this.intersectInfo.TabIndex = 0;
            // 
            // IntersectTwoDirectionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(759, 238);
            this.Name = "IntersectTwoDirectionsForm";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.IntersectTwoDirectionsForm_Shown);
            this.wizard.ResumeLayout(false);
            this.directionOnePage.ResumeLayout(false);
            this.directionTwoPage.ResumeLayout(false);
            this.finishPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gui.Wizard.WizardPage finishPage;
        private Gui.Wizard.WizardPage directionTwoPage;
        private GetDirectionControl getDirection2;
        private Gui.Wizard.WizardPage directionOnePage;
        private GetDirectionControl getDirection1;
        private IntersectInfoControl intersectInfo;

    }
}
