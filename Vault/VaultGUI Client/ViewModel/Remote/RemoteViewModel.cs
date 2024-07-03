using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using VaultGUI_Client.Model;
using System.Collections.Generic;

namespace VaultGUI_Client.ViewModel.Remote
{
    public class RemoteViewModel
    {
        private static RemoteViewModel instance = null;
        private static readonly object padlock = new object();
        public static List<BackupJob> jobs;
        public static List<BackupJob> runningJobs;
        public static event EventHandler<List<BackupJob>> UpdateBackupJob;
        public static event EventHandler<List<BackupJob>> UpdateActiveBackupJob;


        public static Socket _socket { get; set; }
        public static IPAddress Ip { get; set; } // Default IP, update as necessary
        public static int Port { get; set; }// Default port, update as necessary

        // Constructor is private to prevent instantiation outside of the class.
        private RemoteViewModel()
        {
            ReceiveSeverData();
        }

   
        public void SetIpPort(IPAddress ip, int port)
        {
            Ip = ip;
            Port = port;
        }
        public static void OnUpdateBackupJob(List<BackupJob> backuplist)
        {
            UpdateBackupJob?.Invoke(new object(), backuplist);
        }
        public static void OnUpdateActiveBackupJob(List<BackupJob> backuplist)
        {
            UpdateActiveBackupJob?.Invoke(new object(), backuplist);
        }

        // Public static method to get the instance of the class.
        public static RemoteViewModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RemoteViewModel();
                    }
                    return instance;
                }
            }
        }

        public void ReceiveSeverData()
        {
            try
            {
                _socket = ConnectToServer();

                Task.Run(() =>
                {
                    while (true)
                    {
                        Receive(_socket);
                    }
                });

            }
            catch (Exception ex)
            {
                throw new Exception($"Communication failed: {ex.Message}, the application will now close");
            }
        }

        /* ------------------------ Client ------------------------ */

        public static Socket ConnectToServer()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Ip = IPAddress.Parse(MainWindow.ipServer);
                Port = MainWindow.portServer;
                socket.Connect(new IPEndPoint(Ip, Port));
                return socket;
            }
            catch (Exception ex)
            {
                throw new Exception($"Communication failed: {ex.Message}");
            }
        }

        public static void Receive(Socket socket)
        {
            try
            {
                byte[] buffer = new byte[8196];
                int received = socket.Receive(buffer, 0, buffer.Length, 0);
                Array.Resize(ref buffer, received);
                if (Encoding.Default.GetString(buffer).StartsWith("AllJobs"))
                {
                    buffer = Encoding.Default.GetBytes(Encoding.Default.GetString(buffer).Substring(7));
                    jobs = JsonSerializer.Deserialize<List<BackupJob>>(Encoding.Default.GetString(buffer));
                    OnUpdateBackupJob(jobs);

                }
                if (Encoding.Default.GetString(buffer).StartsWith("RunningJobs"))
                {
                    buffer = Encoding.Default.GetBytes(Encoding.Default.GetString(buffer).Substring(11));
                    Debug.WriteLine("Received: " + Encoding.Default.GetString(buffer));
                    List<BackupJob> runningJobs = new List<BackupJob>();
                    if (Encoding.Default.GetString(buffer) == ("[]RunningJobs[]"))
                    {

                    }
                    else
                    {
                        runningJobs = JsonSerializer.Deserialize<List<BackupJob>>(Encoding.Default.GetString(buffer));
                    }

                    OnUpdateActiveBackupJob(runningJobs);


                }
            }
            catch (Exception ex)
            {
                //throw new Exception($"Communication failed: {ex.Message}");
            }
        }


        public static List<BackupJob> GetRunningJobs(Socket socket)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("GetRunningJobs");
                socket.Send(buffer);

                buffer = new byte[1024];
                int received = socket.Receive(buffer, 0, buffer.Length, 0);
                Array.Resize(ref buffer, received);
                string test = Encoding.Default.GetString(buffer);
                Trace.WriteLine("Received: " + Encoding.Default.GetString(buffer));

                return new List<BackupJob>(received);
                }

            catch (Exception ex)
            {
                throw new Exception($"Communication failed: {ex.Message}");
            }
        }

        public static void RunJobs(Socket socket, string jobs)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("RunJobs" + jobs);
                socket.Send(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Communication failed: {ex.Message}");
            }
        }

        public static void PauseResume(Socket socket, int jobs)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("PauseResume" + jobs);
                socket.Send(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Communication failed: {ex.Message}");
            }
        }

        public static void Stop(Socket socket, int jobs)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("Stop" + jobs);
                socket.Send(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Communication failed: {ex.Message}");
            }
        }


        public static void Shutdown(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Shutdown failed: {ex.Message}");
            }
        }




    }
}
