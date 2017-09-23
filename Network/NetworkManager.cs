using ShootCube.Dynamics.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Network
{
    public class NetworkManager
    {
        public Connector Client { get; private set; }
        public List<MainPlayer> ForeignPlayers { get; private set; }

        public NetworkManager(string ip, int port, string name, string chifre, string pass)
        {
            Client = new Connector(ip, port, name, chifre, pass);
            ForeignPlayers = new List<MainPlayer>();


            // GET ACTIVE PLAYERS
            Client.Send("REQUEST_ACTIVE_PLAYERS");
            int amount = Client.Next();

            for (int i = 0; i < amount; i++)
            {
                ForeignPlayers.Add(Client.Next());
            }
        }


        public void BindToPlayer(MainPlayer player)
        {
            player.Network = this;
            Client.Send("NEW_PLAYER");
            Client.Send("ID=" + player.Id);

        }
        
    }
}
