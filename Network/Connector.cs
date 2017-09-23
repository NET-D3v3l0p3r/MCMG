using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotNETWork.Tcp;
using DotNETWork.Globals;
using DotNETWork.Tcp.Events;

namespace ShootCube.Network
{
    public class Connector
    {
        private DotTcpClient _mainClient;
        private bool _allowed;

        private Queue<dynamic> _queue = new Queue<dynamic>();

        public Connector(string wan, int port, string username, string certificate, string password)
        {
            _mainClient = new DotTcpClient(wan, port, "", username);
            _mainClient.OnConnected += new DotTcpClient.OnConnectedDelegate(() =>
            {
                _allowed = true;

                while (_mainClient.IsConnected)
                {
                    _queue.Enqueue(_mainClient.Receive());
                }
                
            });


            _mainClient.SetVerificationHash(certificate);
            _mainClient.StartSession(60, password);

        }


        public void Send(object data)
        {
            if (!_allowed)
                return;
        }

        public dynamic Next()
        {
            return _queue.Dequeue();
        }
        
    }
}
