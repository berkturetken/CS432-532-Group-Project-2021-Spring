using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Secure_Server
{
    
    public partial class Form1 : Form
    {

        bool terminating = false;
        bool listening = false;
        bool remoteConnected = false;

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> socketList = new List<Socket>();
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }

        private void Receive(Socket s)
        {
            bool connected = true;

            while (connected && !terminating)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    s.Receive(buffer);

                    string incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    richTextBox_ConsoleOut.AppendText(incomingMessage + "\n");
                }
                catch
                {
                    if (!terminating)
                    {
                        richTextBox_ConsoleOut.AppendText("A client has disconnected\n");
                    }

                    s.Close();
                    socketList.Remove(s);
                    connected = false;
                }
            }
        }

        private void Accept()
        {
            while (listening)
            {
                try
                {

                    Socket newClient = serverSocket.Accept();
                    socketList.Add(newClient);
                    richTextBox_ConsoleOut.AppendText("A client is connected \n");

                    Thread receiveThread = new Thread(() => Receive(newClient));
                    receiveThread.Start();
                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        richTextBox_ConsoleOut.AppendText("The socket stopped working \n");
                    }
                }
            }
        }



        private void button_listen_Click(object sender, EventArgs e)
        {
            int serverPort;

            if (Int32.TryParse(textBox_port_input.Text, out serverPort))
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
                serverSocket.Listen(3);

                listening = true;
                button_listen.Enabled = false;
                Thread acceptThread = new Thread(Accept);
                acceptThread.Start();

                richTextBox_ConsoleOut.AppendText("Started listening on port: " + serverPort + "\n");
            }
            else
            {
                richTextBox_ConsoleOut.AppendText("Please check port number \n");
            }
        }
    }
}
