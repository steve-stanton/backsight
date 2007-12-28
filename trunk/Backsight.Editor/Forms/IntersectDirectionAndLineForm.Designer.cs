namespace Backsight.Editor.Forms
{
    partial class IntersectDirectionAndLineForm
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
            this.linePage = new Gui.Wizard.WizardPage();
            this.getLine = new Backsight.Editor.Forms.GetLineControl();
            this.finishPage = new Gui.Wizard.WizardPage();
            this.intersectInfo = new Backsight.Editor.Forms.IntersectInfoControl();
            this.wizard.SuspendLayout();
            this.directionPage.SuspendLayout();
            this.linePage.SuspendLayout();
            this.finishPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.Controls.Add(this.finishPage);
            this.wizard.Controls.Add(this.linePage);
            this.wizard.Controls.Add(this.directionPage);
            this.wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.directionPage,
            this.linePage,
            this.finishPage});
            this.wizard.Size = new System.Drawing.Size(753, 247);
            // 
            // directionPage
            // 
            this.directionPage.Controls.Add(this.getDirection);
            this.directionPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directionPage.IsFinishPage = false;
            this.directionPage.Location = new System.Drawing.Point(0, 0);
            this.directionPage.Name = "directionPage";
            this.directionPage.Size = new System.Drawing.Size(753, 199);
            this.directionPage.TabIndex = 1;
            // 
            // getDirection
            // 
            this.getDirection.DirectionTitle = "Direction";
            this.getDirection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDirection.Location = new System.Drawing.Point(0, 0);
            this.getDirection.Name = "getDirection";
            this.getDirection.Size = new System.Drawing.Size(753, 199);
            this.getDirection.TabIndex = 0;
            // 
            // linePage
            // 
            this.linePage.Controls.Add(this.getLine);
            this.linePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linePage.IsFinishPage = false;
            this.linePage.Location = new System.Drawing.Point(0, 0);
            this.linePage.Name = "linePage";
            this.linePage.Size = new System.Drawing.Size(753, 199);
            this.linePage.TabIndex = 2;
            // 
            // getLine
            // 
            this.getLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getLine.Location = new System.Drawing.Point(0, 0);
            this.getLine.Name = "getLine";
            this.getLine.Size = new System.Drawing.Size(753, 199);
            this.getLine.TabIndex = 0;
            // 
            // finishPage
            // 
            this.finishPage.Controls.Add(this.intersectInfo);
            this.finishPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finishPage.IsFinishPage = true;
            this.finishPage.Location = new System.Drawing.Point(0, 0);
            this.finishPage.Name = "finishPage";
            this.finishPage.Size = new System.Drawing.Size(753, 199);
            this.finishPage.TabIndex = 3;
            // 
            // intersectInfo
            // 
            this.intersectInfo.CanHaveClosestPoint = true;
            this.intersectInfo.CanHaveTwoIntersections = false;
            this.intersectInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.intersectInfo.Location = new System.Drawing.Point(0, 0);
            this.intersectInfo.Name = "intersectInfo";
            this.intersectInfo.Size = new System.Drawing.Size(753, 199);
            this.intersectInfo.TabIndex = 0;
            // 
            // IntersectDirectionAndLineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(753, 247);
            this.Name = "IntersectDirectionAndLineForm";
            this.Text = "Intersect Direction and Line";
            this.wizard.ResumeLayout(false);
            this.directionPage.ResumeLayout(false);
            this.linePage.ResumeLayout(false);
            this.finishPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gui.Wizard.WizardPage linePage;
        private Gui.Wizard.WizardPage finishPage;
        private IntersectInfoControl intersectInfo;
        private Gui.Wizard.WizardPage directionPage;
        private GetDirectionControl getDirection;
        private GetLineControl getLine;

    }
}
