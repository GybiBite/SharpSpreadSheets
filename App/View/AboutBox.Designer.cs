namespace SharpSpreadSheets.View
{
    partial class AboutBox
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
            label1 = new Label();
            panel1 = new Panel();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(262, 40);
            label1.TabIndex = 0;
            label1.Text = "SharpSpreadSheets";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.MenuText;
            panel1.Location = new Point(12, 52);
            panel1.Name = "panel1";
            panel1.Size = new Size(262, 1);
            panel1.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.FlatStyle = FlatStyle.Flat;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 107);
            label2.Name = "label2";
            label2.Size = new Size(61, 17);
            label2.TabIndex = 2;
            label2.Text = "Authors:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(25, 154);
            label3.Name = "label3";
            label3.Size = new Size(89, 15);
            label3.TabIndex = 3;
            label3.Text = "Zachary Adams";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(25, 139);
            label4.Name = "label4";
            label4.Size = new Size(91, 15);
            label4.TabIndex = 4;
            label4.Text = "Daniel DeMarco";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(25, 124);
            label5.Name = "label5";
            label5.Size = new Size(92, 15);
            label5.TabIndex = 5;
            label5.Text = "Adrian Alemany";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 56);
            label6.Name = "label6";
            label6.Size = new Size(190, 30);
            label6.TabIndex = 6;
            label6.Text = "Simple spreadsheet app written for\r\nTCSS 342, using .NET and C#";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // AboutBox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(291, 188);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutBox";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Panel panel1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}