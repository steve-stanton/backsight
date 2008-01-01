namespace Backsight.Editor.Forms
{
    partial class IntersectTwoLinesForm
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
            this.line1Page = new Gui.Wizard.WizardPage();
            this.getLine1 = new Backsight.Editor.Forms.GetLineControl();
            this.line2Page = new Gui.Wizard.WizardPage();
            this.getLine2 = new Backsight.Editor.Forms.GetLineControl();
            this.finishPage = new Gui.Wizard.WizardPage();
            this.intersectInfo = new Backsight.Editor.Forms.IntersectInfoControl();
            this.wizard.SuspendLayout();
            this.line1Page.SuspendLayout();
            this.line2Page.SuspendLayout();
            this.finishPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.Controls.Add(this.finishPage);
            this.wizard.Controls.Add(this.line2Page);
            this.wizard.Controls.Add(this.line1Page);
            this.wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.line1Page,
            this.line2Page,
            this.finishPage});
            this.wizard.Size = new System.Drawing.Size(673, 241);
            // 
            // line1Page
            // 
            this.line1Page.Controls.Add(this.getLine1);
            this.line1Page.Dock = System.Windows.Forms.DockStyle.Fill;
            this.line1Page.IsFinishPage = false;
            this.line1Page.Location = new System.Drawing.Point(0, 0);
            this.line1Page.Name = "line1Page";
            this.line1Page.Size = new System.Drawing.Size(620, 188);
            this.line1Page.TabIndex = 1;
            // 
            // getLine1
            // 
            this.getLine1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getLine1.Location = new System.Drawing.Point(0, 0);
            this.getLine1.Name = "getLine1";
            this.getLine1.Size = new System.Drawing.Size(620, 188);
            this.getLine1.TabIndex = 0;
            // 
            // line2Page
            // 
            this.line2Page.Controls.Add(this.getLine2);
            this.line2Page.Dock = System.Windows.Forms.DockStyle.Fill;
            this.line2Page.IsFinishPage = false;
            this.line2Page.Location = new System.Drawing.Point(0, 0);
            this.line2Page.Name = "line2Page";
            this.line2Page.Size = new System.Drawing.Size(620, 188);
            this.line2Page.TabIndex = 2;
            // 
            // getLine2
            // 
            this.getLine2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getLine2.Location = new System.Drawing.Point(0, 0);
            this.getLine2.Name = "getLine2";
            this.getLine2.Size = new System.Drawing.Size(620, 188);
            this.getLine2.TabIndex = 0;
            // 
            // finishPage
            // 
            this.finishPage.Controls.Add(this.intersectInfo);
            this.finishPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finishPage.IsFinishPage = true;
            this.finishPage.Location = new System.Drawing.Point(0, 0);
            this.finishPage.Name = "finishPage";
            this.finishPage.Size = new System.Drawing.Size(673, 193);
            this.finishPage.TabIndex = 3;
            this.finishPage.CloseFromBack += new Gui.Wizard.PageEventHandler(this.finishPage_CloseFromBack);
            this.finishPage.ShowFromNext += new System.EventHandler(this.finishPage_ShowFromNext);
            // 
            // intersectInfo
            // 
            this.intersectInfo.CanHaveClosestPoint = true;
            this.intersectInfo.CanHaveTwoIntersections = false;
            this.intersectInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.intersectInfo.Location = new System.Drawing.Point(0, 0);
            this.intersectInfo.Name = "intersectInfo";
            this.intersectInfo.Size = new System.Drawing.Size(673, 193);
            this.intersectInfo.TabIndex = 0;
            // 
            // IntersectTwoLinesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 241);
            this.Name = "IntersectTwoLinesForm";
            this.Text = "Intersect Two Lines";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.IntersectTwoLinesForm_Shown);
            this.wizard.ResumeLayout(false);
            this.line1Page.ResumeLayout(false);
            this.line2Page.ResumeLayout(false);
            this.finishPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gui.Wizard.WizardPage finishPage;
        private IntersectInfoControl intersectInfo;
        private Gui.Wizard.WizardPage line2Page;
        private GetLineControl getLine2;
        private Gui.Wizard.WizardPage line1Page;
        private GetLineControl getLine1;
    }
}