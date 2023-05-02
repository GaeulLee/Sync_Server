using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Socket_Sync_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //------------------------------------------------- server

        private void Form1_Load(object sender, EventArgs e)
        {
            s_send_btn.Enabled = false;
            button2.Enabled = false;
        }

        // (1) 소켓 객체 생성 (TCP 소켓) (전화기 생성)
        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /*
            AddressFamily -> 네트워크 유형 지정 (일반 네트워크IP 통신에서는 AddressFamily.InterNetwork 값을 사용)
            SocketType -> 소켓 종류 지정 (stream 방식은 양방향 연결 필요, 데이터 보장 및 순서대로 받을 수 있음)
            ProtocolType -> 프로토콜 종류 지정
         */

        Socket clientSock = null; // 클라이언트 소켓 생성
        /*
            왜 생성하는가? 하나의 소켓으로 서버와 클라이언트가 데이터를 주고 받는게 아닌가..
            서버가 클라이언트의 요청을 받으면 통신을 위한 새로운 소켓 생성
            이 소켓은 클라이언트 소켓의 IP 주소와 포트번호 사용 => TCP 연결 세션이 생성됨
        */

        private void s_start_btn_Click(object sender, EventArgs e)
        {
            // (2) 포트에 바인드 (상대방에게 연결되게 전화번호를 부여)
            string serverIp = "192.168.172.11";
            int serverPort = Convert.ToInt32(s_port_input.Text);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, serverPort);
            sock.Bind(ep);
           // s_chatRoom.Items.Add("[Bind OK!]");

            // (3) 포트 Listening 시작 (전화를 통신망에 연결)
            sock.Listen(10);
            //s_chatRoom.Items.Add("[Listen OK!]");

            // (4) 연결을 받아들여 새 소켓 생성 (하나의 연결만 받아들임)
            clientSock = sock.Accept();
            //s_chatRoom.Items.Add("[Accept OK!]");
            // 요약:
            //     새로 만든 연결에 대한 새 System.Net.Sockets.Socket을 만듭니다.
            //
            // 반환 값:
            //     새로 만든 연결에 대한 System.Net.Sockets.Socket입니다.
            s_chatRoom.Items.Add("[Connected!]");

            // 버튼 속성 변경
            s_send_btn.Enabled = true;
            button2.Enabled = true;
            s_start_btn.Enabled = false;
        }

        private void s_send_btn_Click(object sender, EventArgs e)
        {
            // 수신 받은 데이터를 받을 바이트배열 변수 선언 -> 왜 바이트로 송수신 하는지 확인
            byte[] buff = new byte[8192];
            try
            {
                // (5) 소켓 수신
                int rec = clientSock.Receive(buff); // 클라이언트 소켓에서 전송한 데이터를 정수로 담음
                string recData = Encoding.UTF8.GetString(buff, 0, rec); // 정수로 담은 데이터를 문자열로 변환 (디코딩)
                s_chatRoom.Items.Add("[Client(상대)] -------> " + recData); // 채팅창에 표시

                // (6) 소켓 전송
                string m = s_msg_input.Text;
                /*
                    if (m.Equals("") || m == null) // 유효성 확인
                    {
                        MessageBox.Show("서버에 보낼 메세지를 전송하세요.");
                        return;
                    }
                */

                byte[] sendMsg = Encoding.UTF8.GetBytes(m);
                clientSock.Send(sendMsg, SocketFlags.None);
                s_chatRoom.Items.Add("[Server(나)] -------> " + m); // 채팅창에 표시
                s_msg_input.Text = String.Empty; // 전송 후 텍스트 상자 비움
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
                sock.Close(); // 서버 소켓
                clientSock.Close(); // 클라이언트와 통신을 위해 생성한 소켓
                s_chatRoom.Items.Add("[Server Closed]");

                s_start_btn.Enabled = true;
                s_send_btn.Enabled = false;
                button2.Enabled = false;
            }
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // (7) 소켓 닫기
            sock.Close(); // 서버 소켓
            clientSock.Close(); // 클라이언트와 통신을 위해 생성한 소켓
            s_chatRoom.Items.Add("[Server Closed]");
        }

        
    }
}
