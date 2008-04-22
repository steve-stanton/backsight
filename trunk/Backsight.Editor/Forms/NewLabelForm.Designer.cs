namespace Backsight.Editor.Forms
{
    partial class NewLabelForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewLabelForm));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.entityTypeComboBox = new Backsight.Editor.Forms.EntityTypeComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.schemaComboBox = new Backsight.Editor.Forms.SchemaComboBox();
            this.allSchemasCheckBox = new System.Windows.Forms.CheckBox();
            this.noAttributesCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.defaultAnnotationCheckBox = new System.Windows.Forms.CheckBox();
            this.annotationTemplateComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(279, 227);
            this.okButton.Margin = new System.Windows.Forms.Padding(4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 30);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(157, 227);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 30);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.TabStop = false;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Type of polygon";
            // 
            // entityTypeComboBox
            // 
            this.entityTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.entityTypeComboBox.FormattingEnabled = true;
            this.entityTypeComboBox.Location = new System.Drawing.Point(121, 12);
            this.entityTypeComboBox.Name = "entityTypeComboBox";
            this.entityTypeComboBox.Size = new System.Drawing.Size(226, 24);
            this.entityTypeComboBox.TabIndex = 0;
            this.entityTypeComboBox.SelectedValueChanged += new System.EventHandler(this.entityTypeComboBox_SelectedValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.schemaComboBox);
            this.groupBox1.Controls.Add(this.allSchemasCheckBox);
            this.groupBox1.Controls.Add(this.noAttributesCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(561, 78);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What sort of Attributes?";
            // 
            // schemaComboBox
            // 
            this.schemaComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.schemaComboBox.FormattingEnabled = true;
            this.schemaComboBox.Location = new System.Drawing.Point(13, 27);
            this.schemaComboBox.Name = "schemaComboBox";
            this.schemaComboBox.Size = new System.Drawing.Size(363, 24);
            this.schemaComboBox.TabIndex = 3;
            this.schemaComboBox.SelectedValueChanged += new System.EventHandler(this.schemaComboBox_SelectedValueChanged);
            // 
            // allSchemasCheckBox
            // 
            this.allSchemasCheckBox.AutoSize = true;
            this.allSchemasCheckBox.Location = new System.Drawing.Point(394, 43);
            this.allSchemasCheckBox.Name = "allSchemasCheckBox";
            this.allSchemasCheckBox.Size = new System.Drawing.Size(114, 20);
            this.allSchemasCheckBox.TabIndex = 2;
            this.allSchemasCheckBox.TabStop = false;
            this.allSchemasCheckBox.Text = "List &all choices";
            this.allSchemasCheckBox.UseVisualStyleBackColor = true;
            this.allSchemasCheckBox.CheckedChanged += new System.EventHandler(this.allSchemasCheckBox_CheckedChanged);
            // 
            // noAttributesCheckBox
            // 
            this.noAttributesCheckBox.AutoSize = true;
            this.noAttributesCheckBox.Location = new System.Drawing.Point(394, 17);
            this.noAttributesCheckBox.Name = "noAttributesCheckBox";
            this.noAttributesCheckBox.Size = new System.Drawing.Size(148, 20);
            this.noAttributesCheckBox.TabIndex = 1;
            this.noAttributesCheckBox.TabStop = false;
            this.noAttributesCheckBox.Text = "&Don\'t have attributes";
            this.noAttributesCheckBox.UseVisualStyleBackColor = true;
            this.noAttributesCheckBox.CheckedChanged += new System.EventHandler(this.noAttributesCheckBox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.defaultAnnotationCheckBox);
            this.groupBox2.Controls.Add(this.annotationTemplateComboBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(558, 73);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "What sort of Annotation?";
            // 
            // defaultAnnotationCheckBox
            // 
            this.defaultAnnotationCheckBox.AutoSize = true;
            this.defaultAnnotationCheckBox.Location = new System.Drawing.Point(394, 30);
            this.defaultAnnotationCheckBox.Name = "defaultAnnotationCheckBox";
            this.defaultAnnotationCheckBox.Size = new System.Drawing.Size(68, 20);
            this.defaultAnnotationCheckBox.TabIndex = 1;
            this.defaultAnnotationCheckBox.TabStop = false;
            this.defaultAnnotationCheckBox.Text = "Use &ID";
            this.defaultAnnotationCheckBox.UseVisualStyleBackColor = true;
            this.defaultAnnotationCheckBox.CheckedChanged += new System.EventHandler(this.defaultAnnotationCheckBox_CheckedChanged);
            // 
            // annotationTemplateComboBox
            // 
            this.annotationTemplateComboBox.FormattingEnabled = true;
            this.annotationTemplateComboBox.Location = new System.Drawing.Point(13, 28);
            this.annotationTemplateComboBox.Name = "annotationTemplateComboBox";
            this.annotationTemplateComboBox.Size = new System.Drawing.Size(363, 24);
            this.annotationTemplateComboBox.TabIndex = 2;
            this.annotationTemplateComboBox.SelectedValueChanged += new System.EventHandler(this.annotationTemplateComboBox_SelectedValueChanged);
            // 
            // NewLabelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 282);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.entityTypeComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "NewLabelForm";
            this.Text = "Add polygon labels";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.NewLabelForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private EntityTypeComboBox entityTypeComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox allSchemasCheckBox;
        private System.Windows.Forms.CheckBox noAttributesCheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox defaultAnnotationCheckBox;
        private System.Windows.Forms.ComboBox annotationTemplateComboBox;
        private SchemaComboBox schemaComboBox;
    }
}