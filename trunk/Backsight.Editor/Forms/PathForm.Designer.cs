namespace Backsight.Editor.Forms
{
    partial class PathForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.endCurveButton = new System.Windows.Forms.Button();
            this.culDeSacButton = new System.Windows.Forms.Button();
            this.angleButton = new System.Windows.Forms.Button();
            this.curveButton = new System.Windows.Forms.Button();
            this.distanceButton = new System.Windows.Forms.Button();
            this.toTextBox = new System.Windows.Forms.TextBox();
            this.fromTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.defaultUnitsLabel = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.previewButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.endCurveButton);
            this.splitContainer1.Panel1.Controls.Add(this.culDeSacButton);
            this.splitContainer1.Panel1.Controls.Add(this.angleButton);
            this.splitContainer1.Panel1.Controls.Add(this.curveButton);
            this.splitContainer1.Panel1.Controls.Add(this.distanceButton);
            this.splitContainer1.Panel1.Controls.Add(this.toTextBox);
            this.splitContainer1.Panel1.Controls.Add(this.fromTextBox);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(700, 277);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.TabIndex = 0;
            // 
            // endCurveButton
            // 
            this.endCurveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endCurveButton.Location = new System.Drawing.Point(139, 224);
            this.endCurveButton.Name = "endCurveButton";
            this.endCurveButton.Size = new System.Drawing.Size(100, 30);
            this.endCurveButton.TabIndex = 9;
            this.endCurveButton.Text = "&End Curve";
            this.endCurveButton.UseVisualStyleBackColor = true;
            this.endCurveButton.Click += new System.EventHandler(this.endCurveButton_Click);
            // 
            // culDeSacButton
            // 
            this.culDeSacButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.culDeSacButton.Location = new System.Drawing.Point(139, 179);
            this.culDeSacButton.Name = "culDeSacButton";
            this.culDeSacButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.culDeSacButton.Size = new System.Drawing.Size(100, 30);
            this.culDeSacButton.TabIndex = 8;
            this.culDeSacButton.Text = "Cul de &sac...";
            this.culDeSacButton.UseVisualStyleBackColor = true;
            this.culDeSacButton.Click += new System.EventHandler(this.culDeSacButton_Click);
            // 
            // angleButton
            // 
            this.angleButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.angleButton.Location = new System.Drawing.Point(15, 179);
            this.angleButton.Name = "angleButton";
            this.angleButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.angleButton.Size = new System.Drawing.Size(100, 30);
            this.angleButton.TabIndex = 7;
            this.angleButton.Text = "&Angle...";
            this.angleButton.UseVisualStyleBackColor = true;
            this.angleButton.Click += new System.EventHandler(this.angleButton_Click);
            // 
            // curveButton
            // 
            this.curveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.curveButton.Location = new System.Drawing.Point(139, 132);
            this.curveButton.Name = "curveButton";
            this.curveButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.curveButton.Size = new System.Drawing.Size(100, 30);
            this.curveButton.TabIndex = 6;
            this.curveButton.Text = "&Curve...";
            this.curveButton.UseVisualStyleBackColor = true;
            this.curveButton.Click += new System.EventHandler(this.curveButton_Click);
            // 
            // distanceButton
            // 
            this.distanceButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.distanceButton.Location = new System.Drawing.Point(15, 132);
            this.distanceButton.Name = "distanceButton";
            this.distanceButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.distanceButton.Size = new System.Drawing.Size(100, 30);
            this.distanceButton.TabIndex = 5;
            this.distanceButton.Text = "&Distance...";
            this.distanceButton.UseVisualStyleBackColor = true;
            this.distanceButton.Click += new System.EventHandler(this.distanceButton_Click);
            // 
            // toTextBox
            // 
            this.toTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toTextBox.Location = new System.Drawing.Point(139, 80);
            this.toTextBox.Name = "toTextBox";
            this.toTextBox.Size = new System.Drawing.Size(101, 22);
            this.toTextBox.TabIndex = 4;
            // 
            // fromTextBox
            // 
            this.fromTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fromTextBox.Location = new System.Drawing.Point(139, 49);
            this.fromTextBox.Name = "fromTextBox";
            this.fromTextBox.Size = new System.Drawing.Size(100, 22);
            this.fromTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(44, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "To point";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(44, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "From point";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.cancelButton);
            this.splitContainer2.Panel2.Controls.Add(this.previewButton);
            this.splitContainer2.Size = new System.Drawing.Size(436, 277);
            this.splitContainer2.SplitterDistance = 306;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.defaultUnitsLabel);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pathTextBox);
            this.splitContainer3.Size = new System.Drawing.Size(306, 277);
            this.splitContainer3.SplitterDistance = 36;
            this.splitContainer3.TabIndex = 0;
            // 
            // defaultUnitsLabel
            // 
            this.defaultUnitsLabel.AutoSize = true;
            this.defaultUnitsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.defaultUnitsLabel.Location = new System.Drawing.Point(3, 9);
            this.defaultUnitsLabel.Name = "defaultUnitsLabel";
            this.defaultUnitsLabel.Size = new System.Drawing.Size(80, 16);
            this.defaultUnitsLabel.TabIndex = 0;
            this.defaultUnitsLabel.Text = "Default units";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathTextBox.Location = new System.Drawing.Point(0, 0);
            this.pathTextBox.Multiline = true;
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(306, 237);
            this.pathTextBox.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(14, 224);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cancelButton.Size = new System.Drawing.Size(100, 30);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // previewButton
            // 
            this.previewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewButton.Location = new System.Drawing.Point(14, 132);
            this.previewButton.Name = "previewButton";
            this.previewButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.previewButton.Size = new System.Drawing.Size(100, 30);
            this.previewButton.TabIndex = 0;
            this.previewButton.Text = "&Preview";
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // PathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 277);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "PathForm";
            this.Text = "Connection Path";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.PathForm_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label defaultUnitsLabel;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button previewButton;
        private System.Windows.Forms.Button endCurveButton;
        private System.Windows.Forms.Button culDeSacButton;
        private System.Windows.Forms.Button angleButton;
        private System.Windows.Forms.Button curveButton;
        private System.Windows.Forms.Button distanceButton;
        private System.Windows.Forms.TextBox toTextBox;
        private System.Windows.Forms.TextBox fromTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}