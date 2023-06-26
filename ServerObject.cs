using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослуховування
        List<ClientObject> clients = new List<ClientObject>(); // всі підключення
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }

        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // якщо id клієнта не дорівнює id відправника
                {
                    clients[i].Stream.Write(data, 0, data.Length); // відправляємо повідомлення
                }
            }
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Server started. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop(); //зупиняємо сервер

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //відключаємо клієнтів
            }
            Environment.Exit(0); //завершуємо потік
        }
    }
