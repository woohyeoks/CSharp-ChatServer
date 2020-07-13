using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class Session
    {
        Socket m_socket;


        SocketAsyncEventArgs m_sendArgs = new SocketAsyncEventArgs();
        Queue<byte[]> m_sendQueue = new Queue<byte[]>();
        object m_lock = new object();
        bool m_pending = false;
        int m_disconnected = 0;

        public void Start(Socket _socket)
        {
            m_socket = _socket;

            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            m_sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted); 
            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuffer)
        {
            lock(m_lock)
            {
                m_sendQueue.Enqueue(sendBuffer);
                if(m_pending == false)
                {
                    RegisterSend();
                }
            }
        }

        public void Disconnect()
        {
            //_disconnected = 1; //멀티 스레드 환경에서 문제
            if (Interlocked.Exchange(ref m_disconnected, 1) == 1)
                return;

            m_socket.Shutdown(SocketShutdown.Both); // 우아하게 종료 시킨다.
            m_socket.Close();
        }

        #region 네트워크 통신
        private void RegisterSend()
        {
            m_pending = true;

            byte[] buff = m_sendQueue.Dequeue();
            m_sendArgs.SetBuffer(buff, 0, buff.Length);

            bool pending = m_socket.SendAsync(m_sendArgs);
            if (m_pending == false)
            {
                OnSendCompleted(null, m_sendArgs);
            }

        }

        private void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = m_socket.ReceiveAsync(args);
            if (pending == false)
            {
                OnRecvCompleted(null, args);
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                    RegisterRecv(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {

            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (m_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) //0 바이트 오는 경우 상대방 연결 끊었을 떄
                {
                    try
                    {
                        if (m_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                        else
                            m_pending = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }
        #endregion
    }
}
