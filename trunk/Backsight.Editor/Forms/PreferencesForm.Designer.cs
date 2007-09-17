/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

namespace Backsight.Editor.Forms
{
    partial class PreferencesForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.PointsTabPage = new System.Windows.Forms.TabPage();
            this.showIntersectionsCheckBox = new System.Windows.Forms.CheckBox();
            this.pointSizeTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pointScaleTextBox = new System.Windows.Forms.TextBox();
            this.LabelsTabPage = new System.Windows.Forms.TabPage();
            this.textRotationAngleLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.changeFontButton = new System.Windows.Forms.Button();
            this.fontNameTextBox = new System.Windows.Forms.TextBox();
            this.nominalScaleTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelScaleTextBox = new System.Windows.Forms.TextBox();
            this.UnitsTabPage = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.displayAsEnteredRadioButton = new System.Windows.Forms.RadioButton();
            this.displayChainsRadioButton = new System.Windows.Forms.RadioButton();
            this.displayFeetRadioButton = new System.Windows.Forms.RadioButton();
            this.displayMetersRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.enterChainsRadioButton = new System.Windows.Forms.RadioButton();
            this.enterFeetRadioButton = new System.Windows.Forms.RadioButton();
            this.enterMetersRadioButton = new System.Windows.Forms.RadioButton();
            this.LineAnnotationTabPage = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.annoHeightTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.annoScaleTextBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.observedAnglesCheckBox = new System.Windows.Forms.CheckBox();
            this.annoNothingRadioButton = new System.Windows.Forms.RadioButton();
            this.annoAdjustedLengthsRadioButton = new System.Windows.Forms.RadioButton();
            this.annoObservedLengthsRadioButton = new System.Windows.Forms.RadioButton();
            this.SymbologyTabPage = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.PointsTabPage.SuspendLayout();
            this.LabelsTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.UnitsTabPage.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.LineAnnotationTabPage.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SymbologyTabPage.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(645, 35);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.PointsTabPage);
            this.tabControl.Controls.Add(this.LabelsTabPage);
            this.tabControl.Controls.Add(this.UnitsTabPage);
            this.tabControl.Controls.Add(this.LineAnnotationTabPage);
            this.tabControl.Controls.Add(this.SymbologyTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(763, 488);
            this.tabControl.TabIndex = 2;
            // 
            // PointsTabPage
            // 
            this.PointsTabPage.Controls.Add(this.showIntersectionsCheckBox);
            this.PointsTabPage.Controls.Add(this.pointSizeTextBox);
            this.PointsTabPage.Controls.Add(this.label2);
            this.PointsTabPage.Controls.Add(this.label1);
            this.PointsTabPage.Controls.Add(this.pointScaleTextBox);
            this.PointsTabPage.Location = new System.Drawing.Point(4, 25);
            this.PointsTabPage.Name = "PointsTabPage";
            this.PointsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.PointsTabPage.Size = new System.Drawing.Size(755, 459);
            this.PointsTabPage.TabIndex = 0;
            this.PointsTabPage.Text = "Points";
            this.PointsTabPage.UseVisualStyleBackColor = true;
            // 
            // showIntersectionsCheckBox
            // 
            this.showIntersectionsCheckBox.AutoSize = true;
            this.showIntersectionsCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showIntersectionsCheckBox.Location = new System.Drawing.Point(61, 167);
            this.showIntersectionsCheckBox.Name = "showIntersectionsCheckBox";
            this.showIntersectionsCheckBox.Size = new System.Drawing.Size(192, 20);
            this.showIntersectionsCheckBox.TabIndex = 4;
            this.showIntersectionsCheckBox.Text = "Show intersection points too";
            this.showIntersectionsCheckBox.UseVisualStyleBackColor = true;
            // 
            // pointSizeTextBox
            // 
            this.pointSizeTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointSizeTextBox.Location = new System.Drawing.Point(248, 120);
            this.pointSizeTextBox.Name = "pointSizeTextBox";
            this.pointSizeTextBox.Size = new System.Drawing.Size(69, 22);
            this.pointSizeTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(58, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Size (in meters on the ground)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(60, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hide at scales smaller than 1:";
            // 
            // pointScaleTextBox
            // 
            this.pointScaleTextBox.AcceptsReturn = true;
            this.pointScaleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pointScaleTextBox.Location = new System.Drawing.Point(248, 76);
            this.pointScaleTextBox.Name = "pointScaleTextBox";
            this.pointScaleTextBox.Size = new System.Drawing.Size(69, 22);
            this.pointScaleTextBox.TabIndex = 0;
            // 
            // LabelsTabPage
            // 
            this.LabelsTabPage.Controls.Add(this.textRotationAngleLabel);
            this.LabelsTabPage.Controls.Add(this.groupBox1);
            this.LabelsTabPage.Controls.Add(this.label3);
            this.LabelsTabPage.Controls.Add(this.labelScaleTextBox);
            this.LabelsTabPage.Location = new System.Drawing.Point(4, 25);
            this.LabelsTabPage.Name = "LabelsTabPage";
            this.LabelsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.LabelsTabPage.Size = new System.Drawing.Size(755, 459);
            this.LabelsTabPage.TabIndex = 1;
            this.LabelsTabPage.Text = "Labels";
            this.LabelsTabPage.UseVisualStyleBackColor = true;
            // 
            // textRotationAngleLabel
            // 
            this.textRotationAngleLabel.AutoSize = true;
            this.textRotationAngleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textRotationAngleLabel.Location = new System.Drawing.Point(60, 349);
            this.textRotationAngleLabel.Name = "textRotationAngleLabel";
            this.textRotationAngleLabel.Size = new System.Drawing.Size(156, 16);
            this.textRotationAngleLabel.TabIndex = 5;
            this.textRotationAngleLabel.Text = "Text orientation angle = 0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.changeFontButton);
            this.groupBox1.Controls.Add(this.fontNameTextBox);
            this.groupBox1.Controls.Add(this.nominalScaleTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(59, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(516, 157);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default font";
            // 
            // changeFontButton
            // 
            this.changeFontButton.Location = new System.Drawing.Point(401, 86);
            this.changeFontButton.Name = "changeFontButton";
            this.changeFontButton.Size = new System.Drawing.Size(92, 23);
            this.changeFontButton.TabIndex = 7;
            this.changeFontButton.Text = "&Change...";
            this.changeFontButton.UseVisualStyleBackColor = true;
            // 
            // fontNameTextBox
            // 
            this.fontNameTextBox.Location = new System.Drawing.Point(31, 87);
            this.fontNameTextBox.Name = "fontNameTextBox";
            this.fontNameTextBox.Size = new System.Drawing.Size(351, 22);
            this.fontNameTextBox.TabIndex = 6;
            // 
            // nominalScaleTextBox
            // 
            this.nominalScaleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nominalScaleTextBox.Location = new System.Drawing.Point(189, 34);
            this.nominalScaleTextBox.Name = "nominalScaleTextBox";
            this.nominalScaleTextBox.Size = new System.Drawing.Size(69, 22);
            this.nominalScaleTextBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(76, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Nominal scale 1:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(60, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Hide at scales smaller than 1:";
            // 
            // labelScaleTextBox
            // 
            this.labelScaleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelScaleTextBox.Location = new System.Drawing.Point(248, 79);
            this.labelScaleTextBox.Name = "labelScaleTextBox";
            this.labelScaleTextBox.Size = new System.Drawing.Size(69, 22);
            this.labelScaleTextBox.TabIndex = 2;
            // 
            // UnitsTabPage
            // 
            this.UnitsTabPage.Controls.Add(this.groupBox3);
            this.UnitsTabPage.Controls.Add(this.groupBox2);
            this.UnitsTabPage.Location = new System.Drawing.Point(4, 25);
            this.UnitsTabPage.Name = "UnitsTabPage";
            this.UnitsTabPage.Size = new System.Drawing.Size(755, 459);
            this.UnitsTabPage.TabIndex = 2;
            this.UnitsTabPage.Text = "Units";
            this.UnitsTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.displayAsEnteredRadioButton);
            this.groupBox3.Controls.Add(this.displayChainsRadioButton);
            this.groupBox3.Controls.Add(this.displayFeetRadioButton);
            this.groupBox3.Controls.Add(this.displayMetersRadioButton);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(281, 57);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(187, 196);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Display";
            // 
            // displayAsEnteredRadioButton
            // 
            this.displayAsEnteredRadioButton.AutoSize = true;
            this.displayAsEnteredRadioButton.Location = new System.Drawing.Point(28, 151);
            this.displayAsEnteredRadioButton.Name = "displayAsEnteredRadioButton";
            this.displayAsEnteredRadioButton.Size = new System.Drawing.Size(91, 20);
            this.displayAsEnteredRadioButton.TabIndex = 3;
            this.displayAsEnteredRadioButton.TabStop = true;
            this.displayAsEnteredRadioButton.Text = "As entered";
            this.displayAsEnteredRadioButton.UseVisualStyleBackColor = true;
            // 
            // displayChainsRadioButton
            // 
            this.displayChainsRadioButton.AutoSize = true;
            this.displayChainsRadioButton.Location = new System.Drawing.Point(28, 100);
            this.displayChainsRadioButton.Name = "displayChainsRadioButton";
            this.displayChainsRadioButton.Size = new System.Drawing.Size(67, 20);
            this.displayChainsRadioButton.TabIndex = 2;
            this.displayChainsRadioButton.TabStop = true;
            this.displayChainsRadioButton.Text = "Chains";
            this.displayChainsRadioButton.UseVisualStyleBackColor = true;
            // 
            // displayFeetRadioButton
            // 
            this.displayFeetRadioButton.AutoSize = true;
            this.displayFeetRadioButton.Location = new System.Drawing.Point(28, 74);
            this.displayFeetRadioButton.Name = "displayFeetRadioButton";
            this.displayFeetRadioButton.Size = new System.Drawing.Size(53, 20);
            this.displayFeetRadioButton.TabIndex = 1;
            this.displayFeetRadioButton.TabStop = true;
            this.displayFeetRadioButton.Text = "Feet";
            this.displayFeetRadioButton.UseVisualStyleBackColor = true;
            // 
            // displayMetersRadioButton
            // 
            this.displayMetersRadioButton.AutoSize = true;
            this.displayMetersRadioButton.Location = new System.Drawing.Point(28, 48);
            this.displayMetersRadioButton.Name = "displayMetersRadioButton";
            this.displayMetersRadioButton.Size = new System.Drawing.Size(67, 20);
            this.displayMetersRadioButton.TabIndex = 0;
            this.displayMetersRadioButton.TabStop = true;
            this.displayMetersRadioButton.Text = "Meters";
            this.displayMetersRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.enterChainsRadioButton);
            this.groupBox2.Controls.Add(this.enterFeetRadioButton);
            this.groupBox2.Controls.Add(this.enterMetersRadioButton);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(40, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(187, 196);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Entry";
            // 
            // enterChainsRadioButton
            // 
            this.enterChainsRadioButton.AutoSize = true;
            this.enterChainsRadioButton.Location = new System.Drawing.Point(28, 100);
            this.enterChainsRadioButton.Name = "enterChainsRadioButton";
            this.enterChainsRadioButton.Size = new System.Drawing.Size(67, 20);
            this.enterChainsRadioButton.TabIndex = 2;
            this.enterChainsRadioButton.TabStop = true;
            this.enterChainsRadioButton.Text = "Chains";
            this.enterChainsRadioButton.UseVisualStyleBackColor = true;
            // 
            // enterFeetRadioButton
            // 
            this.enterFeetRadioButton.AutoSize = true;
            this.enterFeetRadioButton.Location = new System.Drawing.Point(28, 74);
            this.enterFeetRadioButton.Name = "enterFeetRadioButton";
            this.enterFeetRadioButton.Size = new System.Drawing.Size(53, 20);
            this.enterFeetRadioButton.TabIndex = 1;
            this.enterFeetRadioButton.TabStop = true;
            this.enterFeetRadioButton.Text = "Feet";
            this.enterFeetRadioButton.UseVisualStyleBackColor = true;
            // 
            // enterMetersRadioButton
            // 
            this.enterMetersRadioButton.AutoSize = true;
            this.enterMetersRadioButton.Location = new System.Drawing.Point(28, 48);
            this.enterMetersRadioButton.Name = "enterMetersRadioButton";
            this.enterMetersRadioButton.Size = new System.Drawing.Size(67, 20);
            this.enterMetersRadioButton.TabIndex = 0;
            this.enterMetersRadioButton.TabStop = true;
            this.enterMetersRadioButton.Text = "Meters";
            this.enterMetersRadioButton.UseVisualStyleBackColor = true;
            // 
            // LineAnnotationTabPage
            // 
            this.LineAnnotationTabPage.Controls.Add(this.label10);
            this.LineAnnotationTabPage.Controls.Add(this.annoHeightTextBox);
            this.LineAnnotationTabPage.Controls.Add(this.label9);
            this.LineAnnotationTabPage.Controls.Add(this.annoScaleTextBox);
            this.LineAnnotationTabPage.Controls.Add(this.groupBox4);
            this.LineAnnotationTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineAnnotationTabPage.Location = new System.Drawing.Point(4, 25);
            this.LineAnnotationTabPage.Name = "LineAnnotationTabPage";
            this.LineAnnotationTabPage.Size = new System.Drawing.Size(755, 459);
            this.LineAnnotationTabPage.TabIndex = 3;
            this.LineAnnotationTabPage.Text = "Line Annotation";
            this.LineAnnotationTabPage.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(100, 358);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(222, 16);
            this.label10.TabIndex = 5;
            this.label10.Text = "Text height (in meters on the ground)";
            // 
            // annoHeightTextBox
            // 
            this.annoHeightTextBox.AcceptsReturn = true;
            this.annoHeightTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.annoHeightTextBox.Location = new System.Drawing.Point(328, 355);
            this.annoHeightTextBox.Name = "annoHeightTextBox";
            this.annoHeightTextBox.Size = new System.Drawing.Size(69, 22);
            this.annoHeightTextBox.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(140, 311);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(182, 16);
            this.label9.TabIndex = 3;
            this.label9.Text = "Hide at scales smaller than 1:";
            // 
            // annoScaleTextBox
            // 
            this.annoScaleTextBox.AcceptsReturn = true;
            this.annoScaleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.annoScaleTextBox.Location = new System.Drawing.Point(328, 308);
            this.annoScaleTextBox.Name = "annoScaleTextBox";
            this.annoScaleTextBox.Size = new System.Drawing.Size(69, 22);
            this.annoScaleTextBox.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.observedAnglesCheckBox);
            this.groupBox4.Controls.Add(this.annoNothingRadioButton);
            this.groupBox4.Controls.Add(this.annoAdjustedLengthsRadioButton);
            this.groupBox4.Controls.Add(this.annoObservedLengthsRadioButton);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(62, 49);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(528, 214);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Show What";
            // 
            // observedAnglesCheckBox
            // 
            this.observedAnglesCheckBox.AutoSize = true;
            this.observedAnglesCheckBox.Location = new System.Drawing.Point(285, 50);
            this.observedAnglesCheckBox.Name = "observedAnglesCheckBox";
            this.observedAnglesCheckBox.Size = new System.Drawing.Size(132, 20);
            this.observedAnglesCheckBox.TabIndex = 3;
            this.observedAnglesCheckBox.Text = "Observed Angles";
            this.observedAnglesCheckBox.UseVisualStyleBackColor = true;
            // 
            // annoNothingRadioButton
            // 
            this.annoNothingRadioButton.AutoSize = true;
            this.annoNothingRadioButton.Location = new System.Drawing.Point(41, 141);
            this.annoNothingRadioButton.Name = "annoNothingRadioButton";
            this.annoNothingRadioButton.Size = new System.Drawing.Size(137, 20);
            this.annoNothingRadioButton.TabIndex = 2;
            this.annoNothingRadioButton.TabStop = true;
            this.annoNothingRadioButton.Text = "Don\'t show lengths";
            this.annoNothingRadioButton.UseVisualStyleBackColor = true;
            // 
            // annoAdjustedLengthsRadioButton
            // 
            this.annoAdjustedLengthsRadioButton.AutoSize = true;
            this.annoAdjustedLengthsRadioButton.Location = new System.Drawing.Point(41, 95);
            this.annoAdjustedLengthsRadioButton.Name = "annoAdjustedLengthsRadioButton";
            this.annoAdjustedLengthsRadioButton.Size = new System.Drawing.Size(244, 20);
            this.annoAdjustedLengthsRadioButton.TabIndex = 1;
            this.annoAdjustedLengthsRadioButton.TabStop = true;
            this.annoAdjustedLengthsRadioButton.Text = "Adjusted lengths (on mapping plane)";
            this.annoAdjustedLengthsRadioButton.UseVisualStyleBackColor = true;
            // 
            // annoObservedLengthsRadioButton
            // 
            this.annoObservedLengthsRadioButton.AutoSize = true;
            this.annoObservedLengthsRadioButton.Location = new System.Drawing.Point(41, 49);
            this.annoObservedLengthsRadioButton.Name = "annoObservedLengthsRadioButton";
            this.annoObservedLengthsRadioButton.Size = new System.Drawing.Size(132, 20);
            this.annoObservedLengthsRadioButton.TabIndex = 0;
            this.annoObservedLengthsRadioButton.TabStop = true;
            this.annoObservedLengthsRadioButton.Text = "Observed lengths";
            this.annoObservedLengthsRadioButton.UseVisualStyleBackColor = true;
            // 
            // SymbologyTabPage
            // 
            this.SymbologyTabPage.Controls.Add(this.label8);
            this.SymbologyTabPage.Controls.Add(this.textBox1);
            this.SymbologyTabPage.Controls.Add(this.label7);
            this.SymbologyTabPage.Controls.Add(this.label6);
            this.SymbologyTabPage.Location = new System.Drawing.Point(4, 25);
            this.SymbologyTabPage.Name = "SymbologyTabPage";
            this.SymbologyTabPage.Size = new System.Drawing.Size(755, 459);
            this.SymbologyTabPage.TabIndex = 4;
            this.SymbologyTabPage.Text = "Symbology";
            this.SymbologyTabPage.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(42, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 3;
            this.label8.Text = "Scale 1:";
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(104, 66);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(69, 22);
            this.textBox1.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(42, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(317, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "The value you enter applies to every map you open.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(42, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(565, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "Specify the limiting scale at which any colours and line patterning should be use" +
    "d during draws.";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tabControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.cancelButton);
            this.splitContainer.Panel2.Controls.Add(this.okButton);
            this.splitContainer.Size = new System.Drawing.Size(763, 587);
            this.splitContainer.SplitterDistance = 488;
            this.splitContainer.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(550, 35);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // PreferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 587);
            this.Controls.Add(this.splitContainer);
            this.Name = "PreferencesForm";
            this.Text = "Preferences";
            this.tabControl.ResumeLayout(false);
            this.PointsTabPage.ResumeLayout(false);
            this.PointsTabPage.PerformLayout();
            this.LabelsTabPage.ResumeLayout(false);
            this.LabelsTabPage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.UnitsTabPage.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.LineAnnotationTabPage.ResumeLayout(false);
            this.LineAnnotationTabPage.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.SymbologyTabPage.ResumeLayout(false);
            this.SymbologyTabPage.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage PointsTabPage;
        private System.Windows.Forms.TabPage LabelsTabPage;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TabPage UnitsTabPage;
        private System.Windows.Forms.TabPage LineAnnotationTabPage;
        private System.Windows.Forms.TabPage SymbologyTabPage;
        private System.Windows.Forms.CheckBox showIntersectionsCheckBox;
        private System.Windows.Forms.TextBox pointSizeTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pointScaleTextBox;
        private System.Windows.Forms.Label textRotationAngleLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox fontNameTextBox;
        private System.Windows.Forms.TextBox nominalScaleTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox labelScaleTextBox;
        private System.Windows.Forms.Button changeFontButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton enterChainsRadioButton;
        private System.Windows.Forms.RadioButton enterFeetRadioButton;
        private System.Windows.Forms.RadioButton enterMetersRadioButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton displayAsEnteredRadioButton;
        private System.Windows.Forms.RadioButton displayChainsRadioButton;
        private System.Windows.Forms.RadioButton displayFeetRadioButton;
        private System.Windows.Forms.RadioButton displayMetersRadioButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton annoNothingRadioButton;
        private System.Windows.Forms.RadioButton annoAdjustedLengthsRadioButton;
        private System.Windows.Forms.RadioButton annoObservedLengthsRadioButton;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox annoHeightTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox annoScaleTextBox;
        private System.Windows.Forms.CheckBox observedAnglesCheckBox;
        private System.Windows.Forms.Button cancelButton;
    }
}
