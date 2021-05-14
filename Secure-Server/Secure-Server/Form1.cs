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
using System.Security.Cryptography;
using Newtonsoft.Json;
using Secure_Server.Models;
using System.IO;

namespace Secure_Server
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool listening = false;
        
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> socketList = new List<Socket>();
        List<string> usernames = new List<string>();

        string serverPublicKey = "";
        string serverPrivateKey = "";
        string mainRepositoryPath = "";
        string folderPath = "";

        Dictionary<string, string> userPubKeys = new Dictionary<string, string>();
        Dictionary<string, string> userHMACKeys = new Dictionary<string, string>(); // session keys
        Dictionary<string, int> userFileCount = new Dictionary<string, int>();

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
            string disconnectMessage = createCommunicationMessage(MessageCodes.DisconnectResponse, "Disconnect", "Server disconnected\n");
            byte[] msg = Encoding.Default.GetBytes(disconnectMessage);
            foreach(Socket c in socketList)
            {
                c.Send(msg);
            }
            Environment.Exit(0);
        }

        private bool ChallengeResponsePhase1(Socket client,string username)
        {
            string randomNumber = randomNumberGenerator(16);
            try
            {
                // Create a 128-bit random number and send to the client
                string randomNumberMessage = createCommunicationMessage(MessageCodes.Request, "RN", randomNumber);
                sendMessage(client, randomNumberMessage);

                try
                {
                    return ReceiveSignedRN(client, username, randomNumber);
                }
                catch
                {
                    richTextBox_ConsoleOut.AppendText("Error during signed random number receiving.\n");
                    return false;
                }
            }
            catch
            {
                richTextBox_ConsoleOut.AppendText("Error during creating random number and sending to a client.\n");
                return false;
            }
        }

        private bool sendSessionKey(Socket client, string username, string clientPubKey)
        {
            try
            {
                // Create a HMAC Key
                string hmacKey = randomNumberGenerator(32);

                // Encrypt the HMAC Key and serialize it
                byte[] encryptedHMAC = encryptWithRSA(hmacKey, 4096, clientPubKey);
                string hmacMessageJSON = createCommunicationMessage(MessageCodes.SuccessfulResponse, "Session Key", generateHexStringFromByteArray(encryptedHMAC));

                // Sign the message
                byte[] signedMessage = signWithRSA(hmacMessageJSON, 4096, serverPrivateKey);
                richTextBox_ConsoleOut.AppendText("Session key signature: " + generateHexStringFromByteArray(signedMessage)+"\n");

                // Merge
                string sessionKeyAgreement = hmacMessageJSON + generateHexStringFromByteArray(signedMessage);
                richTextBox_ConsoleOut.AppendText("Positive acknowledgement + session key + signature: " + sessionKeyAgreement + "\n");
                string finalMessage = createCommunicationMessage(MessageCodes.Request, "Session Key", sessionKeyAgreement);
                // Send the Session Key Agreement to the client
                sendMessage(client, finalMessage);

                // Add to dictionary
                userHMACKeys.Add(username, hmacKey);
                userFileCount.Add(username, -1);
                richTextBox_ConsoleOut.AppendText(username + " is authenticated\n");
                printOnlineUsers();
                return true;
            }
            catch
            {
                sendDisconnectMessage(client, username);
                richTextBox_ConsoleOut.AppendText("Error during session key generation or sending to the client.\n");
                return false;
            }
        }

        private bool interpretReceivedRN(Socket client,string username,string randomNumber, string signedRandom)
        {
            try
            {
                // Verify the signed number retrieved from the client
                string clientPubKey = userPubKeys[username];
                bool isVerified = verifyWithRSA(randomNumber, 4096, clientPubKey, hexStringToByteArray(signedRandom));
                if (!isVerified)    // Negative Acknowledgement
                {
                    richTextBox_ConsoleOut.AppendText("Signature is not verified!\n");
                    string negativeAckJSON = createCommunicationMessage(MessageCodes.ErrorResponse, "Session Key", "Negative Acknowledgement");
                    byte[] signedNegativeAck = signWithRSA(negativeAckJSON, 4096, serverPrivateKey);
                    string hexSignedNegativeAck = generateHexStringFromByteArray(signedNegativeAck);

                    string sessionKeyProblem = negativeAckJSON + hexSignedNegativeAck;
                    string finalMessage = createCommunicationMessage(MessageCodes.Request, "Session Key", sessionKeyProblem);
                    sendMessage(client, finalMessage);

                    // Close the connection
                    return false;
                }
                else    // Positive Acknowledgement
                {
                    richTextBox_ConsoleOut.AppendText("Signature Verified!\n");
                    return sendSessionKey(client, username, clientPubKey);
                }
            }
            catch
            {
                sendDisconnectMessage(client, username);
                richTextBox_ConsoleOut.AppendText("Error during interpreting received signed random number.\n");
                return false;
            }
        }

        private bool ReceiveSignedRN(Socket client,string username,string randomNumber)
        {
            try
            {
                // Get signed random number from the client
                CommunicationMessage msg = receiveMessage(client, 1088);

                if (msg.msgCode == MessageCodes.Request)
                {
                    string signedRandom = msg.message;
                    richTextBox_ConsoleOut.AppendText("Signed Random Number From Client: "+ signedRandom + "\n");
                    try
                    {
                        return interpretReceivedRN(client, username, randomNumber, signedRandom);
                    }
                    catch
                    {
                        richTextBox_ConsoleOut.AppendText("Error during verifying the signed random number.\n");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                richTextBox_ConsoleOut.AppendText("Error while receiving the random number.\n");
                return false;
            }
        }

        private void Receive(Socket s, string username) //username is send from accept thread, to be used to find files in public key repo. 
        {
            // Challenge-Response Phase 1
            bool connected = true;
            try
            {
                // If (for some reason) challenge-response protocol fails
                if (!ChallengeResponsePhase1(s, username))
                {
                    closeConnection(s, username);
                    connected = false;
                }
            }
            catch   // Challenge-response fails
            {
                richTextBox_ConsoleOut.AppendText("Error during challenge-response protocol\n");
                closeConnection(s, username);
                connected = false;
            }
            
            // Implement below after exchanging the session keys
            while (connected && !terminating)
            {

                try
                {
                    CommunicationMessage commMsg = receiveMessage(s, 4352);
                    
                    //Upload Request came
                    if(commMsg.msgCode == MessageCodes.UploadRequest)
                    {
                        int count = 1;
                        richTextBox_ConsoleOut.AppendText("Received packet " + count.ToString() +" from "+username+ "\n");
                        //Get the file number to give and hmac to use
                        int fileNumber = userFileCount[username] + 1;
                        byte[] HMACKey = Encoding.Default.GetBytes(userHMACKeys[username]);

                        //msg part of the communication message is uploadmessage + signature
                        string msg = commMsg.message;

                        //Split the message into upload message and signature
                        string encryptedData = msg.Substring(0, msg.Length - 128);
                        string signatureHexa = msg.Substring(msg.Length - 128);
                        richTextBox_ConsoleOut.AppendText("Signature " + signatureHexa + "\n");

                        UploadMessage uploadMsg = JsonConvert.DeserializeObject<UploadMessage>(encryptedData);
                        bool verified = true;
                        richTextBox_ConsoleOut.AppendText("Received Upload Message: " + uploadMsg.message + "\n");
                        richTextBox_ConsoleOut.AppendText("Is last packet: " + uploadMsg.lastPacket.ToString() + "\n");

                        //While this is not the last packet and verified
                        while (!uploadMsg.lastPacket && verified)
                        {
                            verified = handleUploadRequests(signatureHexa, encryptedData, uploadMsg.message, username, fileNumber, HMACKey, s); //Write to the file and handle verification
                            commMsg = receiveMessage(s, 4352); // Continue receiving
                            count++;
                            richTextBox_ConsoleOut.AppendText("Received packet " + count.ToString() + " from " + username + "\n");
                            msg = commMsg.message;
                            encryptedData = msg.Substring(0, msg.Length - 128);
                            signatureHexa = msg.Substring(msg.Length - 128);
                            uploadMsg = JsonConvert.DeserializeObject<UploadMessage>(encryptedData);
                            richTextBox_ConsoleOut.AppendText("Received Upload Message: " + uploadMsg.message + "\n");
                            richTextBox_ConsoleOut.AppendText("Is last packet: " + uploadMsg.lastPacket.ToString() + "\n");
                        }

                        if (verified) // If everything is verified and we are in the last packet
                        {
                            if (handleUploadRequests(signatureHexa, encryptedData, uploadMsg.message, username, fileNumber, HMACKey, s))
                            {
                                richTextBox_ConsoleOut.AppendText("I am the last packet");
                                string fileStream = username + "_" + fileNumber + ".txt";

                                string fileNameMsg = createCommunicationMessage(MessageCodes.SuccessfulResponse, "File Name", fileStream);
                                string fileSignature = generateHexStringFromByteArray(applyHMACwithSHA512(fileNameMsg, HMACKey));

                                string finalMessage = createCommunicationMessage(MessageCodes.SuccessfulResponse, "File Name", fileNameMsg + fileSignature);
                                byte[] finalBuffer = Encoding.Default.GetBytes(finalMessage);
                                userFileCount[username]++;
                                s.Send(finalBuffer);
                                
                            }
                        }

                    }

                    
                }
                catch
                {
                    closeConnection(s, username);
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
                    getUserName(newClient);
                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        richTextBox_ConsoleOut.AppendText("The socket stopped working.\n");
                    }
                }
            }
        }


        // Helper Functions
        public void getUserName(Socket newClient)
        {
            string username = "";
            try
            {
                CommunicationMessage msg = receiveMessage(newClient, 128);
                if (msg.msgCode == MessageCodes.Request)
                {
                    username = msg.message;
                }

                if (usernames.Contains(username))
                {
                    richTextBox_ConsoleOut.AppendText("This client already exists!\n");
                    string message = createCommunicationMessage(MessageCodes.ErrorResponse, "User name", "You are already connected!\n");
                    sendMessage(newClient, message);    //sends message to client
                    newClient.Close();                  // and closes the socket
                }
                else
                {
                    socketList.Add(newClient);
                    string message = createCommunicationMessage(MessageCodes.SuccessfulResponse, "User name", "You connected succesfully!\n");
                    sendMessage(newClient, message);
                    usernames.Add(username);
                    bool isClientExist = addClientPubKey(username);
                    if (isClientExist){
                        richTextBox_ConsoleOut.AppendText(username + " connected.\n");
                        Thread receiveThread = new Thread(() => Receive(newClient, username));
                        receiveThread.Start();  //Login protocol initiates
                    }
                    else
                    {
                        closeConnection(newClient, username);
                    }
                }
            }
            catch
            {
                closeConnection(newClient, username);
            }
        }
        
        public bool addClientPubKey (string userName)
        {
            string clientPublicKeyPath = mainRepositoryPath + "\\" + userName + "_pub.txt";
            try
            {
                string clientPubKey = File.ReadAllText(clientPublicKeyPath);
                userPubKeys.Add(userName, clientPubKey);
                return true;
            }
            catch
            {
                richTextBox_ConsoleOut.AppendText("There is no such user!\n");
                return false;
            }
        }

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

        public string createCommunicationMessage(MessageCodes msgCode, string topic, string message)
        {
            CommunicationMessage msg = new CommunicationMessage
            {
                msgCode = msgCode,
                topic = topic,
                message = message
            };
            string msgJSON = JsonConvert.SerializeObject(msg);
            return msgJSON;
        }

        public void sendMessage(Socket s, string m)
        {
            Byte[] buffer = Encoding.Default.GetBytes(m);
            s.Send(buffer);
        }

        CommunicationMessage receiveMessage(Socket s, int size)
        {
            Byte[] incomingByteArray = new Byte[size];
            s.Receive(incomingByteArray);
            string inMessage = Encoding.Default.GetString(incomingByteArray).Trim('\0');
            CommunicationMessage msg = JsonConvert.DeserializeObject<CommunicationMessage>(inMessage);
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
            richTextBox_ConsoleOut.AppendText((length*8).ToString() + "-bit Random Number: " + generateHexStringFromByteArray(bytesRandom) + "\n"); // For debugging purposes
            return randomNumber;
        }

        public void closeConnection(Socket client, string username)
        {
            client.Close();
            socketList.Remove(client);
            usernames.Remove(username);
            userHMACKeys.Remove(username);
            userPubKeys.Remove(username);
            printOnlineUsers();
            richTextBox_ConsoleOut.AppendText(username + " disconnected.\n");
        }

        public void sendDisconnectMessage(Socket client, string username)
        {
            string msg = createCommunicationMessage(MessageCodes.DisconnectResponse, "Disconnect", username + " disconnected");
            sendMessage(client, msg);
        }

        public void printOnlineUsers()
        {
            textBox_onlineClients.ResetText();
            foreach(string authenticatedUser in userHMACKeys.Keys)
            {
                textBox_onlineClients.AppendText(authenticatedUser + Environment.NewLine);
            }
        }

        public bool verifyHmac(string signature, string clientName, string message)
        {
            string sessionKey = userHMACKeys[clientName];
            byte[] key_bytes = Encoding.Default.GetBytes(sessionKey);
            byte[] hmac_message = applyHMACwithSHA512(message, key_bytes);

            string hmac = generateHexStringFromByteArray(hmac_message);

            if (hmac == signature)
                return true;
            return false;
            
        }

        public bool handleUploadRequests(string signatureHexa,string encryptedData, string uploadmessage, string username, int fileNumber, byte[] HMACKey, Socket s)
        {

            if (verifyHmac(signatureHexa, username, encryptedData))
            {
                richTextBox_ConsoleOut.AppendText("Signature Verified!\n");
                richTextBox_ConsoleOut.AppendText("Encrypted Data size " + Encoding.Default.GetBytes(encryptedData).Length +"\n");
                string fileStream = folderPath +"\\"+ username + "_" + fileNumber + ".txt";
                FileStream target_file = File.Open(fileStream, FileMode.Append);
                BinaryWriter bWrite = new BinaryWriter(target_file);
                bWrite.Write(Encoding.Default.GetBytes(uploadmessage), 0, uploadmessage.Length);
                //I tried to read but we can't read the file!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //string deneme = File.ReadAllText(fileStream);
                //richTextBox_ConsoleOut.AppendText("Deneme okuması: " + deneme + "\n");
                target_file.Close();
                return true;
            }
            else
            {
                string negativeAck = createCommunicationMessage(MessageCodes.ErrorResponse, "Signature Error", "Signature can't be verified during Upload");
                byte[] hmacBytes = applyHMACwithSHA512(negativeAck, HMACKey);
                string finalMsg = negativeAck + generateHexStringFromByteArray(hmacBytes);
                sendMessage(s, finalMsg);
                return false;
            }
        }


        /***** GUI Elements *****/
        private void button_listen_Click(object sender, EventArgs e)
        {
            int serverPort;

            if (serverPrivateKey == "" || serverPublicKey == "" || mainRepositoryPath == "")
            {
                richTextBox_ConsoleOut.AppendText("Please browse all files and folders first!\n");
            }
            else
            {
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
                    richTextBox_ConsoleOut.AppendText("Please check port number.\n");
                }
            }
        }

        private void ServerPublicKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                string onlyFileName = fileName.Substring(fileName.LastIndexOf('\\')+1);
                textBox_serverPub.Text = onlyFileName;

                try
                {
                    serverPublicKey = File.ReadAllText(fileName);
                    byte[] byteServerPubKey = Encoding.Default.GetBytes(serverPublicKey);
                    string hexaServerPubKey = generateHexStringFromByteArray(byteServerPubKey);
                    richTextBox_ConsoleOut.AppendText("Server Public Key: " + hexaServerPubKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox_ConsoleOut.AppendText("Error while getting client public key " + ex.Message + "\n");
                }
            }
        }

        private void ServerPrivateKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                string onlyFileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                textBox_serverPriv.Text = onlyFileName;

                try
                {
                    serverPrivateKey = File.ReadAllText(fileName);
                    byte[] byteServerPrvKey = Encoding.Default.GetBytes(serverPrivateKey);
                    string hexaServerPrvKey = generateHexStringFromByteArray(byteServerPrvKey);
                    richTextBox_ConsoleOut.AppendText("Server Private Key: " + hexaServerPrvKey + "\n");
                }
                catch (IOException ex)
                {
                    richTextBox_ConsoleOut.AppendText("Error while getting client public key " + ex.Message + "\n");
                }
            }
        }

        private void mainRepo_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                mainRepositoryPath = fbd.SelectedPath;
                string onlyFolderName = mainRepositoryPath.Substring(mainRepositoryPath.LastIndexOf('\\') + 1);
                textBox_mainRepo.Text = onlyFolderName;
            }
        }

        private void folderSelectBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                folderPath = fbd.SelectedPath;
                string onlyFolderName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
                folderBox_text.Text = onlyFolderName;
            }
        }


        /***** Cryptographic Helper Functions *****/
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
        // encryption with AES-128
        static byte[] encryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
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

        // encryption with AES-192
        static byte[] encryptWithAES192(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-192
            aesObject.KeySize = 192;
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
            aesObject.Mode = CipherMode.CFB;
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

        // encryption with AES-128
        static byte[] decryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            // aesObject.FeedbackSize = 128;
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

        // decryption with AES-192
        static byte[] decryptWithAES192(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-192
            aesObject.KeySize = 192;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            // aesObject.FeedbackSize = 128;
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
                result = rsaObject.SignData(byteInput, "SHA256");
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
                result = rsaObject.VerifyData(byteInput, "SHA512", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

   
    }
}
