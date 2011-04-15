namespace IronPythonConsole
{
    partial class IronPythonConsole
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
            this.console = new IronPythonConsoleControl.IronPythonConsoleControl();
            this.SuspendLayout();
            // 
            // console
            // 
            this.console.ErrorOutput = null;
            this.console.Location = new System.Drawing.Point(12, 12);
            this.console.Name = "console";
            this.console.Output = null;
            this.console.Size = new System.Drawing.Size(423, 274);
            this.console.TabIndex = 0;
            this.console.Text = "ironPythonConsoleControl1";
            // 
            // IronPythonConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 298);
            this.Controls.Add(this.console);
            this.Name = "IronPythonConsole";
            this.Text = "IronPythonConsole";
            this.Load += new System.EventHandler(this.IronPythonConsole_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private IronPythonConsoleControl.IronPythonConsoleControl console;
    }
}