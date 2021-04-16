namespace Client
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
            this.button_send = new System.Windows.Forms.Button();
            this.button_connect = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Port_input = new System.Windows.Forms.TextBox();
            this.textBox_IP_input = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(78, 237);
            this.button_send.Margin = new System.Windows.Forms.Padding(2);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(90, 25);
            this.button_send.TabIndex = 17;
            this.button_send.Text = "send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(78, 113);
            this.button_connect.Margin = new System.Windows.Forms.Padding(2);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(90, 25);
            this.button_connect.TabIndex = 16;
            this.button_connect.Text = "Connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(206, 36);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(137, 194);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            // 
            // textBox_message
            // 
            this.textBox_message.Location = new System.Drawing.Point(78, 211);
            this.textBox_message.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_message.Name = "textBox_message";
            this.textBox_message.Size = new System.Drawing.Size(92, 20);
            this.textBox_message.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 211);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Message:";
            // 
            // textBox_Port_input
            // 
            this.textBox_Port_input.Location = new System.Drawing.Point(78, 84);
            this.textBox_Port_input.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Port_input.Name = "textBox_Port_input";
            this.textBox_Port_input.Size = new System.Drawing.Size(92, 20);
            this.textBox_Port_input.TabIndex = 12;
            // 
            // textBox_IP_input
            // 
            this.textBox_IP_input.Location = new System.Drawing.Point(78, 54);
            this.textBox_IP_input.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_IP_input.Name = "textBox_IP_input";
            this.textBox_IP_input.Size = new System.Drawing.Size(92, 20);
            this.textBox_IP_input.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "IP:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 277);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox_message);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Port_input);
            this.Controls.Add(this.textBox_IP_input);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox_message;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Port_input;
        private System.Windows.Forms.TextBox textBox_IP_input;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

