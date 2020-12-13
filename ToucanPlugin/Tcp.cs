﻿using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToucanPlugin
{
    public class Tcp
    {
        public static Socket S { get; set; } = null;
        readonly static List<String> messageQueue = new List<string>();
        static public Stopwatch topicUpdateTimer;
        private bool connecting = false;
        public int MaxMessageLenght = 3000;
        public void Main()
        {
            if (connecting) return;
            connecting = true;
            try
            {
                // Define those variables to be evaluated in the next for loop and
                // then used to connect to the server. These variables are defined
                // outside the for loop to make them accessible there after.
                IPEndPoint hostEndPoint;
                IPAddress hostAddress = null;
                int conPort = 173; //Get it?

                // Get DNS host information.
                if (ToucanPlugin.Instance.Config.ToucanServerIP == "") { Log.Warn($"No Toucan Server ip set!"); return; };
                IPHostEntry hostInfo = Dns.GetHostEntry(ToucanPlugin.Instance.Config.ToucanServerIP);
                // Get the DNS IP addresses associated with the host.
                IPAddress[] IPaddresses = hostInfo.AddressList;

                // Evaluate the socket and receiving host IPAddress and IPEndPoint.
                for (int index = 0; index < IPaddresses.Length; index++)
                {
                    hostAddress = IPaddresses[index];
                    hostEndPoint = new IPEndPoint(hostAddress, conPort);


                    // Creates the Socket to Send data over a Tcp connection.
                    S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                    // Connect to the host using its IPEndPoint.
                    S.Connect(hostEndPoint);

                    if (!IsConnected())
                    {
                        connecting = false;
                        // Connection failed, try next IPaddress.
                        Log.Error("Connection Failed");
                        S = null;
                        continue;
                        //return;
                    }
                    else
                    {
                        connecting = false;
                        Log.Info("Connected To Toucan Server.");
                        while (S.Connected)
                        {
                            try
                            {
                                byte[] bytes = new byte[MaxMessageLenght];
                                int i = S.Receive(bytes);
                                MessageResponder mr = new MessageResponder();
                                mr.Respond(Encoding.UTF8.GetString(bytes));
                            }
                            catch (Exception e)
                            {
                                Log.Error($"Message Responder Failed/Reciving messages Failed, {e}");
                            }
                        }
                    }
                }
            }
            catch
            {
                Log.Error("Connecting failed");
                connecting = false;
            }
        }
        public static bool IsConnected()
        {
            if (S == null)
                return false;

            try
            {
                return !((S.Poll(1000, SelectMode.SelectRead) && (S.Available == 0)) || !S.Connected);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
        private bool SendShit(string data)
        {
            if (data == null) return false;
            try
            {
                if (S.Connected || S != null)
                {
                    // Process the data sent by the client.
                    byte[] niceData = Encoding.ASCII.GetBytes(data);
                    S.Send(niceData);
                    Log.Debug($"Sent: {data}");
                    return true;
                }
                else
                {
                    Log.Debug($"Not Connected?");
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Debug($"Failed to send data: {e}");
                return false;
            }
        }
        public void Send(string data) =>
            messageQueue.Add(data);
        public void SendLog(string log) =>
    Send($"log [{DateTime.Now}] {log}");

        public void SendQueue()
        {
            if (!IsConnected()) return;
            if (topicUpdateTimer.ElapsedMilliseconds >= 10000)
            {
                topicUpdateTimer.Reset();
                topicUpdateTimer.Start();
            }
            for (int i = 0; i < messageQueue.Count; i++)
            {
                if (SendShit(messageQueue[i]))
                {
                    messageQueue.RemoveAt(i);
                    i--;
                    Thread.Sleep(100);
                }
            }
            if (messageQueue.Count != 0)
                Log.Error("Could not send all messages.");
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (IsConnected())
                            SendQueue();
                        else
                            Task.Factory.StartNew(() => Main());
                        Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Could not connect: {e}");
                    }
                }
            });
        }
    }
}