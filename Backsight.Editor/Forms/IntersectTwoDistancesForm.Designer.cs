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
            this.wizard = new Gui.Wizard.Wizard();
            this.intersectionPage = new Gui.Wizard.WizardPage();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pointIdComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pointTypeComboBox = new System.Windows.Forms.ComboBox();
            this.otherButton = new System.Windows.Forms.Button();
            this.eastingLabel = new System.Windows.Forms.Label();
            this.northingLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.getDistanceTwoPage = new Gui.Wizard.WizardPage();
            this.getDistanceTwoControl = new Backsight.Editor.Forms.GetDistanceControl();
            this.getDistanceOnePage = new Gui.Wizard.WizardPage();
            this.getDistanceOneControl = new Backsight.Editor.Forms.GetDistanceControl();
            this.wizard.SuspendLayout();
            this.intersectionPage.SuspendLayout();
            this.getDistanceTwoPage.SuspendLayout();
            this.getDistanceOnePage.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.Controls.Add(this.getDistanceOnePage);
            this.wizard.Controls.Add(this.intersectionPage);
            this.wizard.Controls.Add(this.getDistanceTwoPage);
            this.wizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizard.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wizard.Location = new System.Drawing.Point(0, 0);
            this.wizard.Name = "wizard";
            this.wizard.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.getDistanceOnePage,
            this.getDistanceTwoPage,
            this.intersectionPage});
            this.wizard.Size = new System.Drawing.Size(622, 224);
            this.wizard.TabIndex = 0;
            // 
            // intersectionPage
            // 
            this.intersectionPage.Controls.Add(this.label5);
            this.intersectionPage.Controls.Add(this.label4);
            this.intersectionPage.Controls.Add(this.pointIdComboBox);
            this.intersectionPage.Controls.Add(this.label3);
            this.intersectionPage.Controls.Add(this.pointTypeComboBox);
            this.intersectionPage.Controls.Add(this.otherButton);
            this.intersectionPage.Controls.Add(this.eastingLabel);
            this.intersectionPage.Controls.Add(this.northingLabel);
            this.intersectionPage.Controls.Add(this.label2);
            this.intersectionPage.Controls.Add(this.label1);
            this.intersectionPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.intersectionPage.IsFinishPage = false;
            this.intersectionPage.Location = new System.Drawing.Point(0, 0);
            this.intersectionPage.Name = "intersectionPage";
            this.intersectionPage.Size = new System.Drawing.Size(622, 176);
            this.intersectionPage.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(433, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "at the intersection";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(433, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Specify the type of entity";
            // 
            // pointIdComboBox
            // 
            this.pointIdComboBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointIdComboBox.FormattingEnabled = true;
            this.pointIdComboBox.Location = new System.Drawing.Point(272, 69);
            this.pointIdComboBox.Name = "pointIdComboBox";
            this.pointIdComboBox.Size = new System.Drawing.Size(133, 24);
            this.pointIdComboBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(246, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "ID";
            // 
            // pointTypeComboBox
            // 
            this.pointTypeComboBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointTypeComboBox.FormattingEnabled = true;
            this.pointTypeComboBox.Location = new System.Drawing.Point(239, 36);
            this.pointTypeComboBox.Name = "pointTypeComboBox";
            this.pointTypeComboBox.Size = new System.Drawing.Size(188, 24);
            this.pointTypeComboBox.TabIndex = 5;
            // 
            // otherButton
            // 
            this.otherButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.otherButton.Location = new System.Drawing.Point(86, 107);
            this.otherButton.Name = "otherButton";
            this.otherButton.Size = new System.Drawing.Size(137, 23);
            this.otherButton.TabIndex = 4;
            this.otherButton.Text = "&Other Intersection";
            this.otherButton.UseVisualStyleBackColor = true;
            // 
            // eastingLabel
            // 
            this.eastingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.eastingLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eastingLabel.Location = new System.Drawing.Point(86, 70);
            this.eastingLabel.Name = "eastingLabel";
            this.eastingLabel.Size = new System.Drawing.Size(137, 23);
            this.eastingLabel.TabIndex = 3;
            this.eastingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // northingLabel
            // 
            this.northingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.northingLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.northingLabel.Location = new System.Drawing.Point(86, 37);
            this.northingLabel.Name = "northingLabel";
            this.northingLabel.Size = new System.Drawing.Size(137, 23);
            this.northingLabel.TabIndex = 2;
            this.northingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Easting";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Northing";
            // 
            // getDistanceTwoPage
            // 
            this.getDistanceTwoPage.Controls.Add(this.getDistanceTwoControl);
            this.getDistanceTwoPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDistanceTwoPage.IsFinishPage = false;
            this.getDistanceTwoPage.Location = new System.Drawing.Point(0, 0);
            this.getDistanceTwoPage.Name = "getDistanceTwoPage";
            this.getDistanceTwoPage.Size = new System.Drawing.Size(622, 176);
            this.getDistanceTwoPage.TabIndex = 3;
            // 
            // getDistanceTwoControl
            // 
            this.getDistanceTwoControl.DistanceTitle = "Second Distance";
            this.getDistanceTwoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDistanceTwoControl.Location = new System.Drawing.Point(0, 0);
            this.getDistanceTwoControl.Name = "getDistanceTwoControl";
            this.getDistanceTwoControl.Size = new System.Drawing.Size(622, 176);
            this.getDistanceTwoControl.TabIndex = 0;
            // 
            // getDistanceOnePage
            // 
            this.getDistanceOnePage.Controls.Add(this.getDistanceOneControl);
            this.getDistanceOnePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDistanceOnePage.IsFinishPage = false;
            this.getDistanceOnePage.Location = new System.Drawing.Point(0, 0);
            this.getDistanceOnePage.Name = "getDistanceOnePage";
            this.getDistanceOnePage.Size = new System.Drawing.Size(622, 176);
            this.getDistanceOnePage.TabIndex = 2;
            // 
            // getDistanceOneControl
            // 
            this.getDistanceOneControl.DistanceTitle = "First Distance";
            this.getDistanceOneControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.getDistanceOneControl.Location = new System.Drawing.Point(0, 0);
            this.getDistanceOneControl.Name = "getDistanceOneControl";
            this.getDistanceOneControl.Size = new System.Drawing.Size(622, 176);
            this.getDistanceOneControl.TabIndex = 0;
            // 
            // IntersectTwoDistancesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 224);
            this.Controls.Add(this.wizard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "IntersectTwoDistancesForm";
            this.Text = "Intersect Two Distances";
            this.TopMost = true;
            this.wizard.ResumeLayout(false);
            this.intersectionPage.ResumeLayout(false);
            this.intersectionPage.PerformLayout();
            this.getDistanceTwoPage.ResumeLayout(false);
            this.getDistanceOnePage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Gui.Wizard.Wizard wizard;
        private Gui.Wizard.WizardPage getDistanceTwoPage;
        private Gui.Wizard.WizardPage getDistanceOnePage;
        private Gui.Wizard.WizardPage intersectionPage;
        private System.Windows.Forms.Label northingLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        //private GetDistanceControl getDistanceTwoControl;
        //private GetDistanceControl getDistanceOneControl;
        private System.Windows.Forms.ComboBox pointIdComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox pointTypeComboBox;
        private System.Windows.Forms.Button otherButton;
        private System.Windows.Forms.Label eastingLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private GetDistanceControl getDistanceTwoControl;
        private GetDistanceControl getDistanceOneControl;
    }
}