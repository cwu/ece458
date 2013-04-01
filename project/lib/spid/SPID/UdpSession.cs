using System;
using System.Collections.Generic;
using System.Text;

namespace SPID {
    class UdpSession : ISession{

        //definition of an UDP session:
        //server and client IP and UDP port is the same
        //at least one packet is sent in each direction (to avoid classifying UDP port scannings as sessions)
        //the host that sends the first packet is the client
        //a session is terminated when no packet is sent in TIMEOUT (currently 60 seconds)

        public delegate void SessionEstablishedEventHandler(UdpSession session);
        public event SessionEstablishedEventHandler SessionEstablished;

        public delegate void ProtocolModelCompletedEventHandler(UdpSession session, ProtocolIdentification.ProtocolModel protocolModel);
        public event ProtocolModelCompletedEventHandler ProtocolModelCompleted;


        private ProtocolIdentification.ProtocolModel applicationProtocolModel;
        private System.Net.IPAddress serverIp, clientIp;
        private ushort serverPort, clientPort;
        private DateTime firstPacketTimestamp, lastPacketTimestamp;
        bool usePlaceholderProtocolModel;

        private bool clientToServerPacketReceived;
        private bool serverToClientPacketReceived;

        private Configuration config;

        private int frameCount;


        public ProtocolIdentification.ProtocolModel ApplicationProtocolModel {
            get { return this.applicationProtocolModel; }
        }

        public System.Net.IPAddress ServerIP {
            get { return this.serverIp; }
        }

        public System.Net.IPAddress ClientIP {
            get { return this.clientIp; }
        }

        public ushort ClientPort {
            get { return this.clientPort; }
        }

        public ushort ServerPort {
            get { return this.serverPort; }
        }

        public DateTime FirstPacketTimestamp {
            get { return this.firstPacketTimestamp; }
        }

        public SessionHandler.TransportProtocol TransportProtocol {
            get { return SessionHandler.TransportProtocol.UDP; }
        }

        public DateTime LastPacketTimestamp {
            get { return this.lastPacketTimestamp; }
        }

        public string Identifier {
            get {
                return SessionHandler.GetSessionIdentifier(clientIp, clientPort, serverIp, serverPort, this.TransportProtocol);
            }
        }

        public UdpSession(Configuration config, ProtocolIdentification.ProtocolModel applicationProtocolModel)
            : this(config) {
            this.applicationProtocolModel=applicationProtocolModel;
        }
        public UdpSession(Configuration config, System.Net.IPAddress clientIp, ushort clientPort, System.Net.IPAddress serverIp, ushort serverPort)
            : this(config) {
            this.clientIp=clientIp;
            this.clientPort=clientPort;
            this.serverIp=serverIp;
            this.serverPort=serverPort;
        }

        public UdpSession(Configuration config) {
            this.config=config;
            this.applicationProtocolModel=null;//protocol model is not initialized until the session is established
            this.firstPacketTimestamp=DateTime.MinValue;
            this.serverIp=null;
            this.clientIp=null;
            this.usePlaceholderProtocolModel=false;
            this.serverToClientPacketReceived=false;
            this.clientToServerPacketReceived=false;
        }

        public ProtocolIdentification.AttributeFingerprintHandler.PacketDirection AddFrame(PacketParser.Frame frame) {
            //ProtocolIdentification.AttributeFingerprintHandler.PacketDirection packetDirection=ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.Unknown;
            //let's get the IP and TCP packets from the frame
            PacketParser.Packets.AbstractPacket ipPacket;
            PacketParser.Packets.UdpPacket udpPacket;
            if(SessionHandler.TryGetIpAndUdpPackets(frame, out ipPacket, out udpPacket)) {
                //we now have the IP and TCP packets
                System.Net.IPAddress sourceIp=System.Net.IPAddress.None;
                System.Net.IPAddress destinationIp=System.Net.IPAddress.None;
                int applicationLayerProtocolLength=0;

                if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv4Packet)) {
                    PacketParser.Packets.IPv4Packet ipv4Packet=(PacketParser.Packets.IPv4Packet)ipPacket;
                    sourceIp=ipv4Packet.SourceIPAddress;
                    destinationIp=ipv4Packet.DestinationIPAddress;
                    applicationLayerProtocolLength=ipv4Packet.TotalLength-ipv4Packet.HeaderLength-udpPacket.DataOffsetByteCount;
                }
                else if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv6Packet)) {
                    PacketParser.Packets.IPv6Packet ipv6Packet=(PacketParser.Packets.IPv6Packet)ipPacket;
                    sourceIp=ipv6Packet.SourceIP;
                    destinationIp=ipv6Packet.DestinationIP;
                    applicationLayerProtocolLength=ipv6Packet.PayloadLength-udpPacket.DataOffsetByteCount;
                }

                //Check if the client and server have been defined
                if(this.serverIp==null || this.clientIp==null) {
                    //the first host to send a packet will be assumed to be the client
                    this.clientIp=sourceIp;
                    this.clientPort=udpPacket.SourcePort;
                    this.serverIp=destinationIp;
                    this.serverPort=udpPacket.DestinationPort;
                }

                //identify the direction
                if(sourceIp.Equals(this.clientIp) && destinationIp.Equals(this.serverIp) && udpPacket.SourcePort==this.clientPort && udpPacket.DestinationPort==this.serverPort) {
                    //AddFrame(ipPacket, tcpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer);
                    AddFrame(udpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer, applicationLayerProtocolLength);
                    return ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer;
                }
                else if(sourceIp.Equals(this.serverIp) && destinationIp.Equals(this.clientIp) && udpPacket.SourcePort==this.serverPort && udpPacket.DestinationPort==this.clientPort) {
                    AddFrame(udpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient, applicationLayerProtocolLength);
                    return ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient;
                }
                else
                    throw new Exception("IP's and ports do not match those belonging to this session\nSession server: "+this.serverIp.ToString()+":"+this.ServerPort.ToString()+"\nSession client: "+this.clientIp.ToString()+":"+this.clientPort.ToString()+"\nPacket source: "+sourceIp.ToString()+":"+udpPacket.SourcePort.ToString()+"\nPacket destination: "+destinationIp+":"+udpPacket.DestinationPort);
            }
            else
                return ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.Unknown;


        }


        private void AddFrame(PacketParser.Packets.UdpPacket udpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection direction, int protocolPacketLength) {
            frameCount++;
            this.lastPacketTimestamp=udpPacket.ParentFrame.Timestamp;

            if(this.firstPacketTimestamp==DateTime.MinValue)
                this.firstPacketTimestamp=udpPacket.ParentFrame.Timestamp;

            //see if the session has just been established
            if(direction == ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer && !this.clientToServerPacketReceived) {
                this.clientToServerPacketReceived=true;
                if(this.serverToClientPacketReceived && SessionEstablished!=null)
                    this.SessionEstablished(this);
            }
            else if(direction == ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient && !this.serverToClientPacketReceived) {
                this.serverToClientPacketReceived=true;
                if(this.clientToServerPacketReceived && SessionEstablished!=null)
                    this.SessionEstablished(this);
            }

            if(this.frameCount==config.MaxFramesToInspectPerSession+1 && this.applicationProtocolModel!=SessionHandler.PlaceholderProtocolModel) {
                if(this.ProtocolModelCompleted!=null)
                    this.ProtocolModelCompleted(this, this.applicationProtocolModel);
                if(usePlaceholderProtocolModel)
                    this.applicationProtocolModel=SessionHandler.PlaceholderProtocolModel;
            }
            if(this.frameCount<=config.MaxFramesToInspectPerSession) {
                if(protocolPacketLength>0) {
                    if(this.applicationProtocolModel==null)
                        this.applicationProtocolModel=new ProtocolIdentification.ProtocolModel(this.Identifier, config.ActiveAttributeMeters);
                    int protocolPacketStartIndex=udpPacket.PacketStartIndex+udpPacket.DataOffsetByteCount;
                    this.applicationProtocolModel.AddObservation(udpPacket.ParentFrame.Data, protocolPacketStartIndex, protocolPacketLength, udpPacket.ParentFrame.Timestamp, direction);
                }
            }
        }


    }
}
