using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        private Socket m_listensocket;
        private Action<Socket> m_onAcceptHandler;


        public Listener()
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ipaddress, 8000);
            m_listensocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_listensocket.Bind(endPoint);
        }

        public void Start(Action<Socket> onAcceptHandler)
        {
            m_listensocket.Listen(100);

            m_onAcceptHandler += onAcceptHandler;
            // 동기 처리
            //  m_socket.Accept();

            // 비동기 처리
            SocketAsyncEventArgs saeAcceptArgs = new SocketAsyncEventArgs();
            saeAcceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

            RegisterAccept(saeAcceptArgs);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // 이벤트 재 사용시 기존 잔재 없애야 한다.
            bool pending = m_listensocket.AcceptAsync(args);

            if (pending == false) // 바로 완료 (동기 처리)
            {
                OnAcceptCompleted(null, args);
            }
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 소켓 에러 체크
            if (args.SocketError == SocketError.Success)
            {
                // TODO 컨텐츠로 요청
                m_onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }
            RegisterAccept(args);
        }
    }
}
