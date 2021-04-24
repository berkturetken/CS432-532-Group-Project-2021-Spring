namespace Secure_Server
{
    partial class Form1
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
            this.label_port = new System.Windows.Forms.Label();
            this.textBox_port_input = new System.Windows.Forms.TextBox();
            this.button_listen = new System.Windows.Forms.Button();
            this.richTextBox_ConsoleOut = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(12, 311);
            this.label_port.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(38, 17);
            this.label_port.TabIndex = 0;
            this.label_port.Text = "Port:";
            // 
            // textBox_port_input
            // 
            this.textBox_port_input.Location = new System.Drawing.Point(59, 308);
            this.textBox_port_input.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_port_input.Name = "textBox_port_input";
            this.textBox_port_input.Size = new System.Drawing.Size(132, 22);
            this.textBox_port_input.TabIndex = 1;
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(59, 341);
            this.button_listen.Margin = new System.Windows.Forms.Padding(4);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(133, 28);
            this.button_listen.TabIndex = 2;
            this.button_listen.Text = "Listen";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click);
            // 
            // richTextBox_ConsoleOut
            // 
            this.richTextBox_ConsoleOut.Location = new System.Drawing.Point(263, 16);
            this.richTextBox_ConsoleOut.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox_ConsoleOut.Name = "richTextBox_ConsoleOut";
            this.richTextBox_ConsoleOut.ReadOnly = true;
            this.richTextBox_ConsoleOut.Size = new System.Drawing.Size(193, 352);
            this.richTextBox_ConsoleOut.TabIndex = 3;
            this.richTextBox_ConsoleOut.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 384);
            this.Controls.Add(this.richTextBox_ConsoleOut);
            this.Controls.Add(this.button_listen);
            this.Controls.Add(this.textBox_port_input);
            this.Controls.Add(this.label_port);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_port;
        private System.Windows.Forms.TextBox textBox_port_input;
        private System.Windows.Forms.Button button_listen;
        private System.Windows.Forms.RichTextBox richTextBox_ConsoleOut;
    }
}

