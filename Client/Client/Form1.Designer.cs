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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Username = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_connect = new System.Windows.Forms.Button();
            this.textBox_Port_input = new System.Windows.Forms.TextBox();
            this.textBox_IP_input = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.button_disconnect = new System.Windows.Forms.Button();
            this.button_Login = new System.Windows.Forms.Button();
            this.serverPubKey = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.clientPublicKey = new System.Windows.Forms.Button();
            this.clientPrivateKey = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.serverPubText = new System.Windows.Forms.TextBox();
            this.clientPubText = new System.Windows.Forms.TextBox();
            this.clientPrivText = new System.Windows.Forms.TextBox();
            this.button_Upload = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.keyLocation_text = new System.Windows.Forms.TextBox();
            this.browseKeylocation = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonDownloadLocation = new System.Windows.Forms.Button();
            this.textBoxDownloadLocation = new System.Windows.Forms.TextBox();
            this.labelDownloadLocation = new System.Windows.Forms.Label();
            this.buttonRequest = new System.Windows.Forms.Button();
            this.textBoxRequestFileName = new System.Windows.Forms.TextBox();
            this.labelRequestFileName = new System.Windows.Forms.Label();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonReject = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(49, 485);
            this.button_send.Margin = new System.Windows.Forms.Padding(2);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(90, 25);
            this.button_send.TabIndex = 17;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.richTextBox1.Location = new System.Drawing.Point(9, 9);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(606, 167);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            // 
            // textBox_message
            // 
            this.textBox_message.Location = new System.Drawing.Point(49, 448);
            this.textBox_message.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_message.Multiline = true;
            this.textBox_message.Name = "textBox_message";
            this.textBox_message.ReadOnly = true;
            this.textBox_message.Size = new System.Drawing.Size(165, 20);
            this.textBox_message.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 448);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "File:";
            // 
            // textBox_Username
            // 
            this.textBox_Username.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Username.Location = new System.Drawing.Point(104, 60);
            this.textBox_Username.Name = "textBox_Username";
            this.textBox_Username.Size = new System.Drawing.Size(96, 20);
            this.textBox_Username.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Password:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Username:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Port:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "IP:";
            // 
            // button_connect
            // 
            this.button_connect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_connect.AutoSize = true;
            this.button_connect.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button_connect.Location = new System.Drawing.Point(2, 86);
            this.button_connect.Margin = new System.Windows.Forms.Padding(2);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(97, 24);
            this.button_connect.TabIndex = 16;
            this.button_connect.Text = "Connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // textBox_Port_input
            // 
            this.textBox_Port_input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Port_input.Location = new System.Drawing.Point(103, 32);
            this.textBox_Port_input.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_Port_input.Name = "textBox_Port_input";
            this.textBox_Port_input.Size = new System.Drawing.Size(98, 20);
            this.textBox_Port_input.TabIndex = 12;
            // 
            // textBox_IP_input
            // 
            this.textBox_IP_input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_IP_input.Location = new System.Drawing.Point(103, 4);
            this.textBox_IP_input.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_IP_input.Name = "textBox_IP_input";
            this.textBox_IP_input.Size = new System.Drawing.Size(98, 20);
            this.textBox_IP_input.TabIndex = 11;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_IP_input, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Password, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Port_input, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Username, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.button_connect, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.button_disconnect, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.button_Login, 0, 5);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 188);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(203, 169);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // textBox_Password
            // 
            this.textBox_Password.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Password.Location = new System.Drawing.Point(104, 116);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(96, 20);
            this.textBox_Password.TabIndex = 21;
            this.textBox_Password.UseSystemPasswordChar = true;
            // 
            // button_disconnect
            // 
            this.button_disconnect.Location = new System.Drawing.Point(103, 86);
            this.button_disconnect.Margin = new System.Windows.Forms.Padding(2);
            this.button_disconnect.Name = "button_disconnect";
            this.button_disconnect.Size = new System.Drawing.Size(79, 22);
            this.button_disconnect.TabIndex = 23;
            this.button_disconnect.Text = "Disconnect";
            this.button_disconnect.UseVisualStyleBackColor = true;
            this.button_disconnect.Click += new System.EventHandler(this.button_disconnect_Click);
            // 
            // button_Login
            // 
            this.button_Login.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Login.AutoSize = true;
            this.button_Login.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.button_Login, 2);
            this.button_Login.Location = new System.Drawing.Point(3, 143);
            this.button_Login.Name = "button_Login";
            this.button_Login.Size = new System.Drawing.Size(197, 23);
            this.button_Login.TabIndex = 22;
            this.button_Login.Text = "Login";
            this.button_Login.UseVisualStyleBackColor = true;
            this.button_Login.Click += new System.EventHandler(this.button_Login_Click);
            // 
            // serverPubKey
            // 
            this.serverPubKey.Location = new System.Drawing.Point(525, 205);
            this.serverPubKey.Margin = new System.Windows.Forms.Padding(2);
            this.serverPubKey.Name = "serverPubKey";
            this.serverPubKey.Size = new System.Drawing.Size(90, 25);
            this.serverPubKey.TabIndex = 19;
            this.serverPubKey.Text = "Browse";
            this.serverPubKey.UseVisualStyleBackColor = true;
            this.serverPubKey.Click += new System.EventHandler(this.serverPubKey_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(246, 205);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Server Public Key:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(246, 241);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Client Public Key:";
            // 
            // clientPublicKey
            // 
            this.clientPublicKey.Location = new System.Drawing.Point(525, 236);
            this.clientPublicKey.Margin = new System.Windows.Forms.Padding(2);
            this.clientPublicKey.Name = "clientPublicKey";
            this.clientPublicKey.Size = new System.Drawing.Size(90, 25);
            this.clientPublicKey.TabIndex = 22;
            this.clientPublicKey.Text = "Browse";
            this.clientPublicKey.UseVisualStyleBackColor = true;
            this.clientPublicKey.Click += new System.EventHandler(this.clientPublicKey_Click);
            // 
            // clientPrivateKey
            // 
            this.clientPrivateKey.Location = new System.Drawing.Point(525, 271);
            this.clientPrivateKey.Margin = new System.Windows.Forms.Padding(2);
            this.clientPrivateKey.Name = "clientPrivateKey";
            this.clientPrivateKey.Size = new System.Drawing.Size(90, 25);
            this.clientPrivateKey.TabIndex = 23;
            this.clientPrivateKey.Text = "Browse";
            this.clientPrivateKey.UseVisualStyleBackColor = true;
            this.clientPrivateKey.Click += new System.EventHandler(this.clientPrivateKey_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(246, 277);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Client Private Key:";
            // 
            // serverPubText
            // 
            this.serverPubText.Location = new System.Drawing.Point(344, 205);
            this.serverPubText.Margin = new System.Windows.Forms.Padding(2);
            this.serverPubText.Name = "serverPubText";
            this.serverPubText.ReadOnly = true;
            this.serverPubText.Size = new System.Drawing.Size(165, 20);
            this.serverPubText.TabIndex = 25;
            // 
            // clientPubText
            // 
            this.clientPubText.Location = new System.Drawing.Point(344, 239);
            this.clientPubText.Margin = new System.Windows.Forms.Padding(2);
            this.clientPubText.Name = "clientPubText";
            this.clientPubText.ReadOnly = true;
            this.clientPubText.Size = new System.Drawing.Size(165, 20);
            this.clientPubText.TabIndex = 26;
            // 
            // clientPrivText
            // 
            this.clientPrivText.Location = new System.Drawing.Point(343, 275);
            this.clientPrivText.Margin = new System.Windows.Forms.Padding(2);
            this.clientPrivText.Name = "clientPrivText";
            this.clientPrivText.ReadOnly = true;
            this.clientPrivText.Size = new System.Drawing.Size(165, 20);
            this.clientPrivText.TabIndex = 27;
            // 
            // button_Upload
            // 
            this.button_Upload.Location = new System.Drawing.Point(225, 441);
            this.button_Upload.Margin = new System.Windows.Forms.Padding(2);
            this.button_Upload.Name = "button_Upload";
            this.button_Upload.Size = new System.Drawing.Size(90, 28);
            this.button_Upload.TabIndex = 28;
            this.button_Upload.Text = "Browse";
            this.button_Upload.UseVisualStyleBackColor = true;
            this.button_Upload.Click += new System.EventHandler(this.button_Upload_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 132);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "Key Location:";
            // 
            // keyLocation_text
            // 
            this.keyLocation_text.Location = new System.Drawing.Point(104, 130);
            this.keyLocation_text.Margin = new System.Windows.Forms.Padding(2);
            this.keyLocation_text.Name = "keyLocation_text";
            this.keyLocation_text.ReadOnly = true;
            this.keyLocation_text.Size = new System.Drawing.Size(165, 20);
            this.keyLocation_text.TabIndex = 30;
            // 
            // browseKeylocation
            // 
            this.browseKeylocation.Location = new System.Drawing.Point(284, 130);
            this.browseKeylocation.Margin = new System.Windows.Forms.Padding(2);
            this.browseKeylocation.Name = "browseKeylocation";
            this.browseKeylocation.Size = new System.Drawing.Size(90, 25);
            this.browseKeylocation.TabIndex = 31;
            this.browseKeylocation.Text = "Browse";
            this.browseKeylocation.UseVisualStyleBackColor = true;
            this.browseKeylocation.Click += new System.EventHandler(this.browseKeylocation_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.browseKeylocation);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.keyLocation_text);
            this.groupBox1.Location = new System.Drawing.Point(240, 180);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(389, 192);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Selection";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(9, 388);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(316, 145);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Upload File";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonReject);
            this.groupBox3.Controls.Add(this.buttonAccept);
            this.groupBox3.Controls.Add(this.buttonDownloadLocation);
            this.groupBox3.Controls.Add(this.textBoxDownloadLocation);
            this.groupBox3.Controls.Add(this.labelDownloadLocation);
            this.groupBox3.Controls.Add(this.buttonRequest);
            this.groupBox3.Controls.Add(this.textBoxRequestFileName);
            this.groupBox3.Controls.Add(this.labelRequestFileName);
            this.groupBox3.Location = new System.Drawing.Point(329, 388);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(300, 164);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Request File";
            // 
            // buttonDownloadLocation
            // 
            this.buttonDownloadLocation.Location = new System.Drawing.Point(219, 99);
            this.buttonDownloadLocation.Name = "buttonDownloadLocation";
            this.buttonDownloadLocation.Size = new System.Drawing.Size(75, 23);
            this.buttonDownloadLocation.TabIndex = 5;
            this.buttonDownloadLocation.Text = "Browse";
            this.buttonDownloadLocation.UseVisualStyleBackColor = true;
            this.buttonDownloadLocation.Click += new System.EventHandler(this.buttonDownloadLocation_Click);
            // 
            // textBoxDownloadLocation
            // 
            this.textBoxDownloadLocation.Location = new System.Drawing.Point(113, 100);
            this.textBoxDownloadLocation.Name = "textBoxDownloadLocation";
            this.textBoxDownloadLocation.Size = new System.Drawing.Size(100, 20);
            this.textBoxDownloadLocation.TabIndex = 4;
            // 
            // labelDownloadLocation
            // 
            this.labelDownloadLocation.AutoSize = true;
            this.labelDownloadLocation.Location = new System.Drawing.Point(5, 103);
            this.labelDownloadLocation.Name = "labelDownloadLocation";
            this.labelDownloadLocation.Size = new System.Drawing.Size(102, 13);
            this.labelDownloadLocation.TabIndex = 3;
            this.labelDownloadLocation.Text = "Download Location:";
            // 
            // buttonRequest
            // 
            this.buttonRequest.Location = new System.Drawing.Point(89, 60);
            this.buttonRequest.Name = "buttonRequest";
            this.buttonRequest.Size = new System.Drawing.Size(100, 23);
            this.buttonRequest.TabIndex = 2;
            this.buttonRequest.Text = "Request";
            this.buttonRequest.UseVisualStyleBackColor = true;
            this.buttonRequest.Click += new System.EventHandler(this.buttonRequest_Click);
            // 
            // textBoxRequestFileName
            // 
            this.textBoxRequestFileName.Location = new System.Drawing.Point(89, 27);
            this.textBoxRequestFileName.Name = "textBoxRequestFileName";
            this.textBoxRequestFileName.Size = new System.Drawing.Size(179, 20);
            this.textBoxRequestFileName.TabIndex = 1;
            // 
            // labelRequestFileName
            // 
            this.labelRequestFileName.AutoSize = true;
            this.labelRequestFileName.Location = new System.Drawing.Point(26, 30);
            this.labelRequestFileName.Name = "labelRequestFileName";
            this.labelRequestFileName.Size = new System.Drawing.Size(57, 13);
            this.labelRequestFileName.TabIndex = 0;
            this.labelRequestFileName.Text = "File Name:";
            this.labelRequestFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(73, 128);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 23);
            this.buttonAccept.TabIndex = 6;
            this.buttonAccept.Text = "Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonReject
            // 
            this.buttonReject.Location = new System.Drawing.Point(172, 128);
            this.buttonReject.Name = "buttonReject";
            this.buttonReject.Size = new System.Drawing.Size(75, 23);
            this.buttonReject.TabIndex = 7;
            this.buttonReject.Text = "Reject";
            this.buttonReject.UseVisualStyleBackColor = true;
            this.buttonReject.Click += new System.EventHandler(this.buttonReject_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 559);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_Upload);
            this.Controls.Add(this.clientPrivText);
            this.Controls.Add(this.clientPubText);
            this.Controls.Add(this.serverPubText);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.clientPrivateKey);
            this.Controls.Add(this.clientPublicKey);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.serverPubKey);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox_message);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox_message;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Username;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_IP_input;
        private System.Windows.Forms.TextBox textBox_Port_input;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Button button_Login;
        private System.Windows.Forms.Button button_disconnect;
        private System.Windows.Forms.Button serverPubKey;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button clientPublicKey;
        private System.Windows.Forms.Button clientPrivateKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox serverPubText;
        private System.Windows.Forms.TextBox clientPubText;
        private System.Windows.Forms.TextBox clientPrivText;
        private System.Windows.Forms.Button button_Upload;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox keyLocation_text;
        private System.Windows.Forms.Button browseKeylocation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonRequest;
        private System.Windows.Forms.TextBox textBoxRequestFileName;
        private System.Windows.Forms.Label labelRequestFileName;
        private System.Windows.Forms.Button buttonDownloadLocation;
        private System.Windows.Forms.TextBox textBoxDownloadLocation;
        private System.Windows.Forms.Label labelDownloadLocation;
        private System.Windows.Forms.Button buttonReject;
        private System.Windows.Forms.Button buttonAccept;
    }
}

