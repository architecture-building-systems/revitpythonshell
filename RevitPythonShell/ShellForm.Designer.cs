using IronTextBox;

namespace RevitPythonShell
{
    partial class ShellForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShellForm));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.ironTextBoxControl = new IronTextBoxControl();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(633, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // ironTextBoxControl
            // 
            this.ironTextBoxControl.ConsoleTextBackColor = System.Drawing.Color.White;
            this.ironTextBoxControl.ConsoleTextFont = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ironTextBoxControl.ConsoleTextForeColor = System.Drawing.SystemColors.WindowText;
            this.ironTextBoxControl.DefBuilder = ((System.Text.StringBuilder)(resources.GetObject("ironTextBoxControl.DefBuilder")));
            this.ironTextBoxControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ironTextBoxControl.Location = new System.Drawing.Point(0, 25);
            this.ironTextBoxControl.Name = "ironTextBoxControl";
            this.ironTextBoxControl.Prompt = ">>>";
            this.ironTextBoxControl.Size = new System.Drawing.Size(633, 395);
            this.ironTextBoxControl.TabIndex = 1;
            // 
            // ShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 420);
            this.Controls.Add(this.ironTextBoxControl);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShellForm";
            this.Text = "RevitPythonShell";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private IronTextBoxControl ironTextBoxControl;
    }
}