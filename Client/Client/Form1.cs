using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;
using Client.Models;
using System.Net;


namespace Client
{
    public partial class Form1 : Form
    {
        string name;
        bool terminating = false;
        bool connected = false;
        bool authenticated = false;
        bool canContinue = true;
        Socket serverSocket;

        private string ServerKey = "";
        private string UserPublicKey = "";
        private string UserEncryptedPrivateKey = "";
        private string UserPrivateKey = "";
        private string SessionKey = "";
        private string randomNumber = "";
        private byte[] AES256Key = new byte[32];
        private byte[] AES256IV = new byte[16];
        private string uploadPath = "";
        private string tempHexaAES256Key = "";
        private string tempHexaAES256IV = "";
        private string keyLocationPath = "";
        private string downloadPath = "";
        private string tempFileName = "";

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            //Initial button and textbox situations
            button_send.Enabled = false;
            textBox_message.Enabled = false;
            button_Login.Enabled = false;
            textBox_Password.Enabled = false;
            button_disconnect.Enabled = false;
            browseKeylocation.Enabled = false;
            button_Upload.Enabled = false;
        }

        private void Receive()
        {
            while (!authenticated && connected)
            {
                try
                {
                    CommunicationMessage msg = receiveMessage(2176);
                    if (msg.msgCode == MessageCodes.DisconnectResponse)
                    {
                        serverSocket.Close();
                        richTextBox1.AppendText("The server has disconnected.\n");
                        connectionClosedButtons();
                    }
                    else if (msg.msgCode == MessageCodes.Request)
                    {
                        try
                        {
                            //Receive Verification and Session Key from server
                            string verificationString = msg.message;
                            //Split the sent message into signature and message                     
                            string hmacMessage = verificationString.Substring(0, verificationString.Length - 1024);
                            string signatureHexa = verificationString.Substring(verificationString.Length - 1024);
                            byte[] signatureBytes = hexStringToByteArray(signatureHexa);
                            string signature = Encoding.Default.GetString(signatureBytes);

                            CommunicationMessage hmacCommMessage = JsonConvert.DeserializeObject<CommunicationMessage>(hmacMessage);
                            MessageCodes verificationResult = hmacCommMessage.msgCode;

                            //If it is a positive acknowledgement
                            if (verificationResult == MessageCodes.SuccessfulResponse)
                            {
                                bool isSignatureVerified = verifyWithRSA(hmacMessage, 4096, ServerKey, signatureBytes);
                                if (isSignatureVerified) // If signature is verified
                                {
                                    byte[] encryptedHmacBytes = hexStringToByteArray(hmacCommMessage.message);
                                    string encryptedHmac = Encoding.Default.GetString(encryptedHmacBytes);
                                    byte[] decryptedHmacBytes = decryptWithRSA(encryptedHmac, 4096, UserPrivateKey);
                                    SessionKey = Encoding.Default.GetString(decryptedHmacBytes);
                                    richTextBox1.AppendText("Session key: " + generateHexStringFromByteArray(decryptedHmacBytes) + "\n");

                                    authenticated = true;
                                    richTextBox1.AppendText("You are authenticated.\n");
                                    // Manage GUI elements
                                    button_Login.Enabled = false;
                                    button_send.Enabled = true;
                                    textBox_message.Enabled = true;
                                    browseKeylocation.Enabled = true;
                                    button_Upload.Enabled = true;
                                }
                                else // If not verified
                                {
                                    connectionClosedButtons();
                                    serverSocket.Close();
                                    richTextBox1.AppendText("Signature can not be verified! Connection closed.\n");
                                }
                            }
                            else // If it is a negative acknowledgement
                            {
                                bool isSignatureVerified = verifyWithRSA(hmacMessage, 4096, ServerKey, signatureBytes);
                                if (isSignatureVerified) // If signature is verified
                                {
                                    richTextBox1.AppendText("Negative Acknowledgment from the server! Connection closed.\n");
                                }
                                else //If not verified
                                {
                                    richTextBox1.AppendText("Signature can not be verified! Connection closed.\n");
                                }
                                connectionClosedButtons();
                                serverSocket.Close();
                            }
                        }
                        catch
                        {
                            connectionClosedButtons();
                            serverSocket.Close();
                            richTextBox1.AppendText("Error during session key receiving!\n");
                        }
                    }
                }
                catch
                {
                    richTextBox1.AppendText("Error while receiving message or deserializing!\n");
                    connectionClosedButtons();
                    serverSocket.Close();
                }
            }

            //After authentication
            while (connected && authenticated)
            {
                try
                {
                    CommunicationMessage msg = receiveMessage(2048); // We may need to increase the size since it is a general recieve function

                    //Result of the upload request is here
                    if(msg.topic=="File Name")
                    {
                        string actualMessage = msg.message.Substring(0, msg.message.Length - 128);
                        string signature = msg.message.Substring(msg.message.Length - 128);
                        richTextBox1.AppendText("Signature received: " + signature + "\n");
                       
                        if (verifyHmac(signature, actualMessage))
                        {
                            if (msg.msgCode == MessageCodes.ErrorResponse) // If the upload is not successful for any reason
                            {
                                canContinue = false;
                                richTextBox1.AppendText("Server could not verify client's signature, upload stopped!\n");
                            }
                            else if (msg.msgCode == MessageCodes.SuccessfulResponse) // If the upload is successful
                            {
                                CommunicationMessage uploadmsg = JsonConvert.DeserializeObject<CommunicationMessage>(actualMessage);
                                string storedFileName = uploadmsg.message;
                                saveToKeys(tempFileName,storedFileName);
                                richTextBox1.AppendText("Upload Successful!\n");
                            }
                        }
                        else
                        {
                            richTextBox1.AppendText("Client could not verify server's signature, upload stopped!\n");
                        }
                    }
                    /*
                    else if(msg.msgCode == MessageCodes.ErrorResponse)
                    {
                        canContinue = false;
                        connectionClosedButtons();
                        serverSocket.Close();
                        connected = false;
                        richTextBox1.AppendText("Server could not verify client's signature, upload stopped!\n");
                    }
                    */
                    else if(msg.topic == "DownloadRequest")
                    {
                        if(msg.msgCode == MessageCodes.ErrorResponse)
                        {
                            string actualMessage = msg.message.Substring(0, msg.message.Length - 1024);
                            string signatureHex = msg.message.Substring(msg.message.Length - 1024);
                            byte[] signatureBytes = hexStringToByteArray(signatureHex);

                            bool isVerified = verifyWithRSA(actualMessage, 4096, ServerKey, signatureBytes);

                            if(isVerified)
                            {
                                CommunicationMessage inMsg = JsonConvert.DeserializeObject<CommunicationMessage>(actualMessage);
                                richTextBox1.AppendText("Message from server: " + inMsg.message + " Download request is cancelled.\n");
                            }
                            else
                            {
                                richTextBox1.AppendText("Could not verify the message from server!\n");
                            }
                        }
                        else if(msg.msgCode == MessageCodes.DownloadRequest) //If the requested file belongs to us
                        {
                            string actualMessage = msg.message.Substring(0, msg.message.Length - 1024);
                            string signatureHex = msg.message.Substring(msg.message.Length - 1024);
                            byte[] signatureBytes = hexStringToByteArray(signatureHex);

                            CommunicationMessage inMsg = JsonConvert.DeserializeObject<CommunicationMessage>(actualMessage);
                            string incomingData = inMsg.message;
                            UploadMessage inData = JsonConvert.DeserializeObject<UploadMessage>(incomingData);

                            bool isVerified = verifyWithRSA(actualMessage, 4096, ServerKey, signatureBytes);
                            if (!isVerified)
                            {
                                richTextBox1.AppendText("Could not verify the message from server!\n");
                            }
                            else
                            {
                                string ciphertextHex = inData.message;
                                byte[] ciphertextBytes = hexStringToByteArray(ciphertextHex);
                                string ciphertext = Encoding.Default.GetString(ciphertextBytes);
                                byte[] key = extractKeyFromFile();
                                byte[] IV = extractIVFromFile();
                                byte[] plaintextBytes = decryptWithAES256(ciphertext, key, IV);
                                string originalFileName = extractOriginalFileNameFromFile();
                                saveFile(originalFileName, Encoding.Default.GetString(plaintextBytes));
                            }
                        }
                    }
                    
                }
                catch
                {
                    if (!terminating)
                    {
                        richTextBox1.AppendText("Server disconnected!\n");
                    }
                    connectionClosedButtons();
                    serverSocket.Close();
                    connected = false;
                }
            }
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            if (serverSocket != null)
            {
                serverSocket.Close();
            }
            Environment.Exit(0);
        }


        /***** HELPER FUNCTIONS *****/
        // Button defaults when the connection is closed

        private void saveFile(string fileName, string data)
        {
            string path = downloadPath + "\\" + fileName;
            File.AppendAllText(path, data);
        }

        private string extractOriginalFileNameFromFile()
        {
            string filePath = keyLocationPath + "\\keys_" + textBoxRequestFileName.Text + ".txt";
            FileStream file = File.OpenRead(filePath);
            byte[] content = new byte[256];
            file.Read(content, 0, content.Length);
            string contentStr = Encoding.Default.GetString(content);
            string fileName = contentStr.Substring(0, contentStr.IndexOf('-'));
            richTextBox1.AppendText("Original file name: " + fileName + "\n");
            return fileName;
        }

        private byte[] extractKeyFromFile()
        {
            string filePath = keyLocationPath + "\\keys_" + textBoxRequestFileName.Text + ".txt";
            FileStream file = File.OpenRead(filePath);
            byte[] content = new byte[256];
            file.Read(content, 0, content.Length);
            string contentStr = Encoding.Default.GetString(content);
            string key = contentStr.Substring(contentStr.Length - 97, 64);
            richTextBox1.AppendText("AES key: " + key + "\n");
            return hexStringToByteArray(key);
        }

        private byte[] extractIVFromFile()
        {
            string filePath = keyLocationPath + "\\keys_" + textBoxRequestFileName.Text + ".txt";
            FileStream file = File.OpenRead(filePath);
            byte[] content = new byte[256];
            file.Read(content, 0, content.Length);
            string contentStr = Encoding.Default.GetString(content);
            string IV = contentStr.Substring(contentStr.Length - 32);
            richTextBox1.AppendText("AES IV: " + IV + "\n");
            return hexStringToByteArray(IV);
        }

        private void connectionClosedButtons()
        {
            authenticated = false;
            connected = false;
            button_send.Enabled = false;
            textBox_message.Enabled = false;
            button_Login.Enabled = false;
            textBox_Password.Enabled = false;
            button_disconnect.Enabled = false;
            button_connect.Enabled = true;
            browseKeylocation.Enabled = false;
            button_Upload.Enabled = false;
            textBox_Password.Text = "";
            UserPrivateKey = "";
            SessionKey = "";
        }

        public bool verifyHmac(string signature, string message)
        {
            string sessionKey = SessionKey;
            byte[] key_bytes = Encoding.Default.GetBytes(sessionKey);
            byte[] hmac_message = applyHMACwithSHA512(message, key_bytes);

            string hmac = generateHexStringFromByteArray(hmac_message);

            if (hmac == signature)
                return true;
            return false;

        }

        // Sends any kind of message to the server
        private void send_message(string message, string topic, MessageCodes code)
        {
            CommunicationMessage msg = new CommunicationMessage();
            msg.topic = topic;
            msg.message = message;
            msg.msgCode = code;
            string jsonObject = JsonConvert.SerializeObject(msg);
            byte[] buffer = Encoding.Default.GetBytes(jsonObject);
            serverSocket.Send(buffer);
        }

        //Receive messages and returning a message object
        private CommunicationMessage receiveMessage(int size)
        {
            Byte[] buffer = new Byte[size];
            serverSocket.Receive(buffer);
            string incomingMessage = Encoding.Default.GetString(buffer).Trim('\0');
            CommunicationMessage msg = JsonConvert.DeserializeObject<CommunicationMessage>(incomingMessage);
            return msg;
        }

        public string randomNumberGenerator(int length)
        {
            Byte[] bytesRandom = new Byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytesRandom);
            }
            string randomNumber = Encoding.Default.GetString(bytesRandom).Trim('\0');
            richTextBox1.AppendText((length * 8).ToString() + "-bit Random Number: " + generateHexStringFromByteArray(bytesRandom) + "\n"); // For debugging purposes
            return randomNumber;
        }

        public void saveToKeys(string originalFileName, string storedFileName)
        {
            string keyPath = keyLocationPath + "\\keys_" + storedFileName + ".txt";
            string line = originalFileName + "-" + tempHexaAES256Key + "-" + tempHexaAES256IV + "\n";
            File.AppendAllText(keyPath, line);
        }


        /***** GUI ELEMENTS *****/
        private void serverPubKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                serverPubText.Text = fileName.Substring(fileName.LastIndexOf('\\') + 1);

                try
                {
                    ServerKey = File.ReadAllText(fileName);
                    byte[] byteServerKey = Encoding.Default.GetBytes(ServerKey);
                    string hexaServerKey = generateHexStringFromByteArray(byteServerKey);
                    richTextBox1.AppendText("Server Public Key: " + hexaServerKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox1.AppendText("Error while getting server public key " + ex.Message);
                }
            }
        }

        private void clientPublicKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                clientPubText.Text = fileName.Substring(fileName.LastIndexOf('\\') + 1);

                try
                {
                    UserPublicKey = File.ReadAllText(fileName);
                    byte[] byteUserPubKey = Encoding.Default.GetBytes(UserPublicKey);
                    string hexaUserPubKey = generateHexStringFromByteArray(byteUserPubKey);
                    richTextBox1.AppendText("User Public Key: " + hexaUserPubKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox1.AppendText("Error while getting client public key " + ex.Message);
                }
            }
        }

        private void clientPrivateKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                clientPrivText.Text = fileName.Substring(fileName.LastIndexOf('\\') + 1);

                try
                {
                    UserEncryptedPrivateKey = File.ReadAllText(fileName);
                    richTextBox1.AppendText("User Encrypted Private Key: " + UserEncryptedPrivateKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox1.AppendText("Error while getting client encrypted private key " + ex.Message);
                }
            }
        }

        private void button_Upload_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                uploadPath = fileName;
                textBox_message.Text = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                tempFileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
            }
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string IP = textBox_IP_input.Text;
            int port;
            name = textBox_Username.Text;

            // Input checks for username
            if (name.IndexOf('_') != -1)
            {
                richTextBox1.AppendText("Username cannot contain '_' symbol!\n");
                return;
            }
            else if (name.Length == 0 || name.IndexOf(' ') != -1)
            {
                richTextBox1.AppendText("Username cannot be empty or contain whitespaces!\n");
                return;
            }

            //Get the local IP Address
            string currIP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    currIP = ip.ToString();                         //Since type of "ip" is System.Net.IPAddress, there must be conversions
            }

            if (Int32.TryParse(textBox_Port_input.Text, out port))
            {
                try
                {
                    if (IP == currIP)
                    {
                        serverSocket.Connect(IP, port);
                        send_message(name, "User name", MessageCodes.Request);      // send username to server and wait for uniqeness check
                        CommunicationMessage serverResponse = receiveMessage(128);   // Receive Uniqueness check
                        if (serverResponse.msgCode != MessageCodes.ErrorResponse)   // if unique
                        {
                            // Change button settings
                            button_connect.Enabled = false;
                            button_disconnect.Enabled = true;
                            button_Login.Enabled = true;
                            textBox_Password.Enabled = true;
                            connected = true;
                            richTextBox1.AppendText(serverResponse.message);

                            CommunicationMessage randomNumberMessage = receiveMessage(128);
                            randomNumber = randomNumberMessage.message;

                            Thread recieveThread = new Thread(Receive);
                            recieveThread.Start();
                        }
                        else // if username is already used 
                        {
                            richTextBox1.AppendText(serverResponse.message);
                            serverSocket.Close();
                            connected = false;
                        }
                    }
                    else
                    {
                        richTextBox1.AppendText("Could not connect to the server. Check IP!\n");
                    }
                }
                catch
                {
                    //Change button settings to initial
                    connectionClosedButtons();
                    richTextBox1.AppendText("Could not connect to the server.\n");
                }
            }
            else
            {
                richTextBox1.AppendText("Check the port number.\n");
            }
        }

        private void button_disconnect_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText("You disconnected.\n");
            connected = false;
            authenticated = false;
            terminating = true;
            connectionClosedButtons();
            string message = name + " disconnected.\n";
            send_message(message, "Disconnect", MessageCodes.DisconnectResponse);
            serverSocket.Close();
        }

        // Login protocol
        private void button_Login_Click(object sender, EventArgs e)
        {
            if (UserPublicKey == "" || UserEncryptedPrivateKey == "" || ServerKey == "")
            {
                richTextBox1.AppendText("You need to choose all the keys first!\n");
            }
            else
            {
                if (connected)
                {
                    byte[] randomNumberBytes = Encoding.Default.GetBytes(randomNumber);
                    richTextBox1.AppendText("Random Number:" + generateHexStringFromByteArray(randomNumberBytes) + "\n");
                    string pass = textBox_Password.Text;

                    //Password hashing and creating AES-256 Key and IV
                    byte[] hashedPassword = hashWithSHA384(pass);
                    Array.Copy(hashedPassword, 0, AES256Key, 0, 32);
                    Array.Copy(hashedPassword, 32, AES256IV, 0, 16);
                    string hexaDecimalAES256Key = generateHexStringFromByteArray(AES256Key);
                    string hexaDecimalAES256IV = generateHexStringFromByteArray(AES256IV);
                    try
                    {
                        //Decrypt the Private Key using AES-256 
                        byte[] decryptedPasswordBytes = decryptWithAES256HexVersion(UserEncryptedPrivateKey, AES256Key, AES256IV);
                        UserPrivateKey = Encoding.Default.GetString(decryptedPasswordBytes);
                        string hexaPrivateKey = generateHexStringFromByteArray(decryptedPasswordBytes);
                        richTextBox1.AppendText("AES 256 Key: " + hexaDecimalAES256Key + "\n");
                        richTextBox1.AppendText("AES 256 IV: " + hexaDecimalAES256IV + "\n");
                        richTextBox1.AppendText("User Private Key: " + UserPrivateKey + "\n");
                        try
                        {
                            //Sign the Random Number and Send it to the server
                            byte[] signedNonce = signWithRSA(randomNumber, 4096, UserPrivateKey);
                            string strNonce = Encoding.Default.GetString(signedNonce);
                            string hexaDecimalSignedNonce = generateHexStringFromByteArray(signedNonce);
                            send_message(hexaDecimalSignedNonce, "signedRN", MessageCodes.Request);
                            richTextBox1.AppendText("Signed Nonce: " + hexaDecimalSignedNonce + "\n");
                        }
                        catch
                        {
                            connectionClosedButtons();
                            serverSocket.Close();
                            richTextBox1.AppendText("Error during signing the nonce and sending to the server!\n");
                        }
                    }
                    catch
                    {
                        AES256Key = new byte[32];
                        AES256IV = new byte[16];
                        richTextBox1.AppendText("Wrong password. Please try again.\n");
                    }
                }
            }
        }

        // Will change!
        private void button_send_Click(object sender, EventArgs e)
        {
            if (authenticated)
            {
                using (var file = File.OpenRead(uploadPath)) //opening the file
                {

                    var fileSize = BitConverter.GetBytes((int)file.Length); //converting file's size 

                    var sendBuffer = new byte[2048];
                    var bytesLeftToTransmit = fileSize; //it is initially the whole file size, while sending buffers(sendBuffer) it will decrement.
                    int count = 1;


                    string key = randomNumberGenerator(32);
                    byte[] byteKey = Encoding.Default.GetBytes(key);
                    string IV = randomNumberGenerator(16);
                    byte[] byteIV = Encoding.Default.GetBytes(IV);
                    string StringSendBuffer = Encoding.Default.GetString(sendBuffer);

                    tempHexaAES256IV = generateHexStringFromByteArray(byteIV);
                    tempHexaAES256Key = generateHexStringFromByteArray(byteKey);


                    while (BitConverter.ToInt32(bytesLeftToTransmit, 0) > 0 && canContinue)
                    {

                        var dataToSend = file.Read(sendBuffer, 0, sendBuffer.Length); //read inside of the file(to sendBuffer)

                        

                        byte[] encryptedSendBuffer = encryptWithAES256(StringSendBuffer, byteKey, byteIV);
                        string encryptedData = generateHexStringFromByteArray(encryptedSendBuffer);
                        richTextBox1.AppendText("Encrypted Data:\n" + encryptedData + "\n");
                        UploadMessage umsg;

                        int i = BitConverter.ToInt32(bytesLeftToTransmit, 0);
                        int sub = i - dataToSend;
                        byte[] sum = BitConverter.GetBytes(sub);
                        bytesLeftToTransmit = sum;
                        richTextBox1.AppendText("bytes left: " + sub + "\n");
                        if (sub <= 0)
                        {
                            umsg = new UploadMessage { message = encryptedData, lastPacket = true };
                        }
                        else
                        {
                            umsg = new UploadMessage { message = encryptedData, lastPacket = false };
                        }

                        string jsonUpload = JsonConvert.SerializeObject(umsg);
                        byte[] byteKEY = Encoding.Default.GetBytes(SessionKey);
                        byte[] byteHMAC = applyHMACwithSHA512(jsonUpload, byteKEY);
                        string stringHMAC = generateHexStringFromByteArray(byteHMAC);
                        richTextBox1.AppendText("Sending packet "+count.ToString()+"\n");
                        richTextBox1.AppendText("Sent message size :" + (jsonUpload + stringHMAC).Length + "\n");
                        send_message(jsonUpload + stringHMAC, "Upload", MessageCodes.UploadRequest);
                        count++;
                        Thread.Sleep(1000);

                    }


                }
            }
            else
            {
                richTextBox1.AppendText("You are not authenticated.\n");
            }
        }

        private void browseKeylocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                keyLocationPath = fbd.SelectedPath;
                string onlyFolderName = keyLocationPath.Substring(keyLocationPath.LastIndexOf('\\') + 1);
                keyLocation_text.Text = onlyFolderName;
            }
        }
        private void buttonDownloadLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                downloadPath = fbd.SelectedPath;
                string onlyFolderName = downloadPath.Substring(downloadPath.LastIndexOf('\\') + 1);
                textBoxDownloadLocation.Text = onlyFolderName;
            }
        }
        private void buttonRequest_Click(object sender, EventArgs e)
        {
            string requestedFileName = textBoxRequestFileName.Text;
            byte[] requestedFileNameSignature = signWithRSA(requestedFileName, 4096, UserPrivateKey);
            string message = requestedFileName + Encoding.Default.GetString(requestedFileNameSignature);
            string messageHex = generateHexStringFromByteArray(Encoding.Default.GetBytes(message));
            richTextBox1.AppendText("Download Request: " + messageHex + "\n");
            richTextBox1.AppendText("Length of the message: " + messageHex.Length + "\n");
            send_message(messageHex, "DownloadRequest", MessageCodes.DownloadRequest);
        }

        /****** CRYPTOGRAPHIC HELPER FUNCTIONS *******/
        static string generateHexStringFromByteArray(byte[] input)
        {
            string hexString = BitConverter.ToString(input);
            return hexString.Replace("-", "");
        }

        public static byte[] hexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /*    HASH    */
        // hash function: SHA-256
        static byte[] hashWithSHA256(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha256Hasher.ComputeHash(byteInput);

            return result;
        }

        // hash function: SHA-384
        static byte[] hashWithSHA384(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA384CryptoServiceProvider sha384Hasher = new SHA384CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha384Hasher.ComputeHash(byteInput);

            return result;
        }

        // hash function: SHA-512
        static byte[] hashWithSHA512(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA512CryptoServiceProvider sha512Hasher = new SHA512CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha512Hasher.ComputeHash(byteInput);

            return result;
        }

        // HMAC with SHA-256
        static byte[] applyHMACwithSHA256(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA256 hmacSHA256 = new HMACSHA256(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA256.ComputeHash(byteInput);

            return result;
        }

        // HMAC with SHA-384
        static byte[] applyHMACwithSHA384(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA384 hmacSHA384 = new HMACSHA384(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA384.ComputeHash(byteInput);

            return result;
        }

        // HMAC with SHA-512
        static byte[] applyHMACwithSHA512(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA512 hmacSHA512 = new HMACSHA512(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA512.ComputeHash(byteInput);

            return result;
        }

        /*    SYMMETRIC CIPHERS     */
        // encryption with AES-256
        static byte[] encryptWithAES256(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-256
            aesObject.KeySize = 256;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            // RijndaelManaged Mode property doesn't support CFB and OFB modes. 
            //If you want to use one of those modes, you should use RijndaelManaged library instead of RijndaelManaged.
            aesObject.Mode = CipherMode.CBC;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform encryptor = aesObject.CreateEncryptor();
            byte[] result = null;

            try
            {
                result = encryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        // decryption with AES-256
        static byte[] decryptWithAES256(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-256
            aesObject.KeySize = 256;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform decryptor = aesObject.CreateDecryptor();
            byte[] result = null;

            try
            {
                result = decryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {

                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        static byte[] decryptWithAES256HexVersion(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = hexStringToByteArray(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-256
            aesObject.KeySize = 256;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform decryptor = aesObject.CreateDecryptor();
            byte[] result = null;

            try
            {
                result = decryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {

                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        /*    PUBLIC KEY CRYPTOGRAPHY    */
        // RSA encryption with varying bit length
        static byte[] encryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            byte[] result = null;

            try
            {
                //true flag is set to perform direct RSA encryption using OAEP padding
                result = rsaObject.Encrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // RSA decryption with varying bit length
        static byte[] decryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            byte[] result = null;

            try
            {
                result = rsaObject.Decrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // signing with RSA
        static byte[] signWithRSA(string input, int algoLength, string xmlString)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            byte[] result = null;

            try
            {
                result = rsaObject.SignData(byteInput, "SHA512");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // verifying with RSA
        static bool verifyWithRSA(string input, int algoLength, string xmlString, byte[] signature)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            bool result = false;

            try
            {
                result = rsaObject.VerifyData(byteInput, "SHA256", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        
    }
}
