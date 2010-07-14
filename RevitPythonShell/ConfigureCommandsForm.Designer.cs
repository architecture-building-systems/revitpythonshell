namespace RevitPythonShell
{
    partial class ConfigureCommandsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureCommandsForm));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnCommandAdd = new System.Windows.Forms.Button();
            this.btnCommandRemove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCommandBrowse = new System.Windows.Forms.Button();
            this.txtCommandPath = new System.Windows.Forms.TextBox();
            this.txtCommandName = new System.Windows.Forms.TextBox();
            this.btnCommandMoveDown = new System.Windows.Forms.Button();
            this.btnCommandMoveUp = new System.Windows.Forms.Button();
            this.lstCommands = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtSearchPath = new System.Windows.Forms.TextBox();
            this.btnSearchPathBrowse = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSearchPathAdd = new System.Windows.Forms.Button();
            this.btnSearchPathRemove = new System.Windows.Forms.Button();
            this.lstSearchPaths = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtVariableValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtVariableName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnVariableAdd = new System.Windows.Forms.Button();
            this.btnVariableRemove = new System.Windows.Forms.Button();
            this.lstVariables = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(452, 235);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(99, 24);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnCommandSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(12, 235);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 24);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(558, 217);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnCommandAdd);
            this.tabPage1.Controls.Add(this.btnCommandRemove);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btnCommandBrowse);
            this.tabPage1.Controls.Add(this.txtCommandPath);
            this.tabPage1.Controls.Add(this.txtCommandName);
            this.tabPage1.Controls.Add(this.btnCommandMoveDown);
            this.tabPage1.Controls.Add(this.btnCommandMoveUp);
            this.tabPage1.Controls.Add(this.lstCommands);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(550, 191);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Commands";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnCommandAdd
            // 
            this.btnCommandAdd.Location = new System.Drawing.Point(436, 6);
            this.btnCommandAdd.Name = "btnCommandAdd";
            this.btnCommandAdd.Size = new System.Drawing.Size(99, 24);
            this.btnCommandAdd.TabIndex = 23;
            this.btnCommandAdd.Text = "Add";
            this.btnCommandAdd.UseVisualStyleBackColor = true;
            this.btnCommandAdd.Click += new System.EventHandler(this.btnCommandAdd_Click);
            // 
            // btnCommandRemove
            // 
            this.btnCommandRemove.Location = new System.Drawing.Point(436, 36);
            this.btnCommandRemove.Name = "btnCommandRemove";
            this.btnCommandRemove.Size = new System.Drawing.Size(99, 24);
            this.btnCommandRemove.TabIndex = 22;
            this.btnCommandRemove.Text = "Remove";
            this.btnCommandRemove.UseVisualStyleBackColor = true;
            this.btnCommandRemove.Click += new System.EventHandler(this.btnCommandRemove_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Path";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Name";
            // 
            // btnCommandBrowse
            // 
            this.btnCommandBrowse.Location = new System.Drawing.Point(436, 148);
            this.btnCommandBrowse.Name = "btnCommandBrowse";
            this.btnCommandBrowse.Size = new System.Drawing.Size(99, 24);
            this.btnCommandBrowse.TabIndex = 19;
            this.btnCommandBrowse.Text = "Browse...";
            this.btnCommandBrowse.UseVisualStyleBackColor = true;
            this.btnCommandBrowse.Click += new System.EventHandler(this.btnCommandBrowse_Click);
            // 
            // txtCommandPath
            // 
            this.txtCommandPath.Location = new System.Drawing.Point(32, 151);
            this.txtCommandPath.Name = "txtCommandPath";
            this.txtCommandPath.Size = new System.Drawing.Size(368, 20);
            this.txtCommandPath.TabIndex = 18;
            this.txtCommandPath.TextChanged += new System.EventHandler(this.txtCommandPath_TextChanged);
            // 
            // txtCommandName
            // 
            this.txtCommandName.Location = new System.Drawing.Point(32, 125);
            this.txtCommandName.Name = "txtCommandName";
            this.txtCommandName.Size = new System.Drawing.Size(368, 20);
            this.txtCommandName.TabIndex = 17;
            this.txtCommandName.TextChanged += new System.EventHandler(this.txtCommandName_TextChanged);
            // 
            // btnCommandMoveDown
            // 
            this.btnCommandMoveDown.Location = new System.Drawing.Point(436, 96);
            this.btnCommandMoveDown.Name = "btnCommandMoveDown";
            this.btnCommandMoveDown.Size = new System.Drawing.Size(99, 24);
            this.btnCommandMoveDown.TabIndex = 16;
            this.btnCommandMoveDown.Text = "Move down";
            this.btnCommandMoveDown.UseVisualStyleBackColor = true;
            this.btnCommandMoveDown.Click += new System.EventHandler(this.btnCommandMoveDown_Click);
            // 
            // btnCommandMoveUp
            // 
            this.btnCommandMoveUp.Location = new System.Drawing.Point(436, 66);
            this.btnCommandMoveUp.Name = "btnCommandMoveUp";
            this.btnCommandMoveUp.Size = new System.Drawing.Size(99, 24);
            this.btnCommandMoveUp.TabIndex = 15;
            this.btnCommandMoveUp.Text = "Move up";
            this.btnCommandMoveUp.UseVisualStyleBackColor = true;
            this.btnCommandMoveUp.Click += new System.EventHandler(this.btnCommandMoveUp_Click);
            // 
            // lstCommands
            // 
            this.lstCommands.DisplayMember = "Name";
            this.lstCommands.FormattingEnabled = true;
            this.lstCommands.Location = new System.Drawing.Point(0, 6);
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(400, 108);
            this.lstCommands.TabIndex = 12;
            this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtSearchPath);
            this.tabPage2.Controls.Add(this.btnSearchPathBrowse);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.btnSearchPathAdd);
            this.tabPage2.Controls.Add(this.btnSearchPathRemove);
            this.tabPage2.Controls.Add(this.lstSearchPaths);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(550, 191);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Search Paths";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtSearchPath
            // 
            this.txtSearchPath.Location = new System.Drawing.Point(72, 144);
            this.txtSearchPath.Name = "txtSearchPath";
            this.txtSearchPath.Size = new System.Drawing.Size(328, 20);
            this.txtSearchPath.TabIndex = 28;
            this.txtSearchPath.TextChanged += new System.EventHandler(this.txtSearchPath_TextChanged);
            // 
            // btnSearchPathBrowse
            // 
            this.btnSearchPathBrowse.Location = new System.Drawing.Point(436, 141);
            this.btnSearchPathBrowse.Name = "btnSearchPathBrowse";
            this.btnSearchPathBrowse.Size = new System.Drawing.Size(99, 24);
            this.btnSearchPathBrowse.TabIndex = 27;
            this.btnSearchPathBrowse.Text = "Browse...";
            this.btnSearchPathBrowse.UseVisualStyleBackColor = true;
            this.btnSearchPathBrowse.Click += new System.EventHandler(this.btnSearchPathBrowse_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-3, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Search Path";
            // 
            // btnSearchPathAdd
            // 
            this.btnSearchPathAdd.Location = new System.Drawing.Point(436, 6);
            this.btnSearchPathAdd.Name = "btnSearchPathAdd";
            this.btnSearchPathAdd.Size = new System.Drawing.Size(99, 24);
            this.btnSearchPathAdd.TabIndex = 25;
            this.btnSearchPathAdd.Text = "Add";
            this.btnSearchPathAdd.UseVisualStyleBackColor = true;
            this.btnSearchPathAdd.Click += new System.EventHandler(this.btnSearchPathAdd_Click);
            // 
            // btnSearchPathRemove
            // 
            this.btnSearchPathRemove.Location = new System.Drawing.Point(436, 36);
            this.btnSearchPathRemove.Name = "btnSearchPathRemove";
            this.btnSearchPathRemove.Size = new System.Drawing.Size(99, 24);
            this.btnSearchPathRemove.TabIndex = 24;
            this.btnSearchPathRemove.Text = "Remove";
            this.btnSearchPathRemove.UseVisualStyleBackColor = true;
            this.btnSearchPathRemove.Click += new System.EventHandler(this.btnSearchPathRemove_Click);
            // 
            // lstSearchPaths
            // 
            this.lstSearchPaths.DisplayMember = "Name";
            this.lstSearchPaths.FormattingEnabled = true;
            this.lstSearchPaths.Location = new System.Drawing.Point(0, 6);
            this.lstSearchPaths.Name = "lstSearchPaths";
            this.lstSearchPaths.Size = new System.Drawing.Size(400, 134);
            this.lstSearchPaths.TabIndex = 13;
            this.lstSearchPaths.SelectedIndexChanged += new System.EventHandler(this.lstSearchPaths_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.txtVariableValue);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.txtVariableName);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.btnVariableAdd);
            this.tabPage3.Controls.Add(this.btnVariableRemove);
            this.tabPage3.Controls.Add(this.lstVariables);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(550, 191);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Variables";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtVariableValue
            // 
            this.txtVariableValue.Location = new System.Drawing.Point(73, 159);
            this.txtVariableValue.Name = "txtVariableValue";
            this.txtVariableValue.Size = new System.Drawing.Size(328, 20);
            this.txtVariableValue.TabIndex = 35;
            this.txtVariableValue.TextChanged += new System.EventHandler(this.txtVariableValue_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-2, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Value";
            // 
            // txtVariableName
            // 
            this.txtVariableName.Location = new System.Drawing.Point(73, 134);
            this.txtVariableName.Name = "txtVariableName";
            this.txtVariableName.Size = new System.Drawing.Size(328, 20);
            this.txtVariableName.TabIndex = 33;
            this.txtVariableName.TextChanged += new System.EventHandler(this.txtVariableName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-2, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Name";
            // 
            // btnVariableAdd
            // 
            this.btnVariableAdd.Location = new System.Drawing.Point(436, 6);
            this.btnVariableAdd.Name = "btnVariableAdd";
            this.btnVariableAdd.Size = new System.Drawing.Size(99, 24);
            this.btnVariableAdd.TabIndex = 31;
            this.btnVariableAdd.Text = "Add";
            this.btnVariableAdd.UseVisualStyleBackColor = true;
            this.btnVariableAdd.Click += new System.EventHandler(this.btnVariableAdd_Click);
            // 
            // btnVariableRemove
            // 
            this.btnVariableRemove.Location = new System.Drawing.Point(436, 36);
            this.btnVariableRemove.Name = "btnVariableRemove";
            this.btnVariableRemove.Size = new System.Drawing.Size(99, 24);
            this.btnVariableRemove.TabIndex = 30;
            this.btnVariableRemove.Text = "Remove";
            this.btnVariableRemove.UseVisualStyleBackColor = true;
            this.btnVariableRemove.Click += new System.EventHandler(this.btnVariableRemove_Click);
            // 
            // lstVariables
            // 
            this.lstVariables.DisplayMember = "Name";
            this.lstVariables.FormattingEnabled = true;
            this.lstVariables.Location = new System.Drawing.Point(1, 6);
            this.lstVariables.Name = "lstVariables";
            this.lstVariables.Size = new System.Drawing.Size(400, 121);
            this.lstVariables.TabIndex = 29;
            this.lstVariables.SelectedIndexChanged += new System.EventHandler(this.lstVariables_SelectedIndexChanged);
            // 
            // ConfigureCommandsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 267);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigureCommandsForm";
            this.Text = "Configure Commands";
            this.Load += new System.EventHandler(this.ConfigureCommandsForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnCommandBrowse;
        private System.Windows.Forms.TextBox txtCommandPath;
        private System.Windows.Forms.TextBox txtCommandName;
        private System.Windows.Forms.Button btnCommandMoveDown;
        private System.Windows.Forms.Button btnCommandMoveUp;
        private System.Windows.Forms.ListBox lstCommands;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnCommandAdd;
        private System.Windows.Forms.Button btnCommandRemove;
        private System.Windows.Forms.ListBox lstSearchPaths;
        private System.Windows.Forms.TextBox txtSearchPath;
        private System.Windows.Forms.Button btnSearchPathBrowse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSearchPathAdd;
        private System.Windows.Forms.Button btnSearchPathRemove;
        private System.Windows.Forms.TextBox txtVariableName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnVariableAdd;
        private System.Windows.Forms.Button btnVariableRemove;
        private System.Windows.Forms.ListBox lstVariables;
        private System.Windows.Forms.TextBox txtVariableValue;
        private System.Windows.Forms.Label label5;
    }
}