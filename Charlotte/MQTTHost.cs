namespace Charlotte
{
    class MqttHost
    {
        public string BrokerHostName { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        
        public MqttHost (string brokerHostName, int port, string username)
        {
            this.BrokerHostName = brokerHostName;
            this.Port = port;
            this.Username = username;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (!(obj is MqttHost))
                return false;

            var mobj = (MqttHost)obj;

            if (mobj.BrokerHostName != this.BrokerHostName)
                return false;
            if (mobj.Port != this.Port)
                return false;
            if (mobj.Username != this.Username)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (BrokerHostName + Port + Username).GetHashCode();
        }
    }
}
