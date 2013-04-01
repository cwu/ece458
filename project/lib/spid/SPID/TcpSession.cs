using System;
using System.Collections.Generic;
using System.Text;

namespace SPID {

    /// <summary>
    /// The definition of a session is:
    /// * SYN and SYN+ACK is received
    /// * FIN+ACK not received
    /// * RST not received
    /// </summary>
    class TcpSession : ISession{

        public enum TCPState { NONE, SYN, SYN_ACK, ESTABLISHED, FIN, CLOSED }

        //public const int PROTOCOL_MODEL_MAX_FRAMES=100;

        public SessionHandler.TransportProtocol TransportProtocol { get { return SessionHandler.TransportProtocol.TCP; } }

        public delegate void SessionEstablishedEventHandler(TcpSession session);
        public event SessionEstablishedEventHandler SessionEstablished;

        public delegate void SessionClosedEventHandler(TcpSession session, ProtocolIdentification.ProtocolModel protocolModel);
        public event SessionClosedEventHandler SessionClosed;

        public delegate void ProtocolModelCompletedEventHandler(TcpSession session, ProtocolIdentification.ProtocolModel protocolModel);
        public event ProtocolModelCompletedEventHandler ProtocolModelCompleted;

        //each protocol model consumes 30*256*4 = 30720 bytes (30kB)
        private ProtocolIdentification.ProtocolModel applicationProtocolModel;
        private TCPState state;
        //private System.Net.NetworkInformation.PhysicalAddress serverMac, clientMac;
        private System.Net.IPAddress serverIp, clientIp;
        private ushort serverPort, clientPort;
        private DateTime firstPacketTimestamp, lastPacketTimestamp;
        private int frameCount;
        private bool usePlaceholderProtocolModel;//used in order to save memory
        private Configuration config;

        public System.Net.IPAddress ServerIP {
            get { return this.serverIp; }
            set { this.serverIp=value; }
        }
        public System.Net.IPAddress ClientIP {
            get { return this.clientIp; }
            set { this.clientIp=value; }
        }
        public ushort ClientPort {
            get { return this.clientPort; }
            set { this.clientPort=value; }
        }
        public ushort ServerPort {
            get { return this.serverPort; }
            set { this.serverPort=value; }
        }
        public DateTime FirstPacketTimestamp { get { return this.firstPacketTimestamp; } }

        public int FrameCount { get { return this.frameCount; } }

        public ProtocolIdentification.ProtocolModel ApplicationProtocolModel { get { return this.applicationProtocolModel; } }

        public TCPState State {
            get { return this.state; }
            set { this.state=value; }
        }

        public string Identifier {
            get {
                return SessionHandler.GetSessionIdentifier(clientIp, clientPort, serverIp, serverPort, this.TransportProtocol);
            }
        }

        /// <summary>
        /// The session's application protocol model can be replaced with a
        /// "place holder" protocol model when the session or model is complete
        /// in order to save memory.
        /// 
        /// Default value is: false
        /// </summary>
        public bool UsePlaceholderProtocolModel {
            get { return this.usePlaceholderProtocolModel; }
            set { this.usePlaceholderProtocolModel=value; }
        }

        public DateTime LastPacketTimestamp { get { return this.lastPacketTimestamp; } }

        public TcpSession(Configuration config, ProtocolIdentification.ProtocolModel applicationProtocolModel)
            : this(config) {
            this.applicationProtocolModel=applicationProtocolModel;
        }
        public TcpSession(Configuration config, System.Net.IPAddress clientIp, ushort clientPort, System.Net.IPAddress serverIp, ushort serverPort)
            : this(config) {
            this.clientIp=clientIp;
            this.clientPort=clientPort;
            this.serverIp=serverIp;
            this.serverPort=serverPort;
        }
        public TcpSession(Configuration config) {
            this.config=config;
            this.applicationProtocolModel=null;//protocol model is not initialized until the session is established
            this.state=TCPState.NONE;
            this.firstPacketTimestamp=DateTime.MinValue;
            this.serverIp=null;
            this.clientIp=null;
            this.usePlaceholderProtocolModel=false;

        }

        
        public ProtocolIdentification.AttributeFingerprintHandler.PacketDirection AddFrame(PacketParser.Frame frame) {
            //ProtocolIdentification.AttributeFingerprintHandler.PacketDirection packetDirection=ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.Unknown;
            //let's get the IP and TCP packets from the frame
            PacketParser.Packets.AbstractPacket ipPacket;
            PacketParser.Packets.TcpPacket tcpPacket;
            if(SessionHandler.TryGetIpAndTcpPackets(frame, out ipPacket, out tcpPacket)) {
                //we now have the IP and TCP packets
                System.Net.IPAddress sourceIp=System.Net.IPAddress.None;
                System.Net.IPAddress destinationIp=System.Net.IPAddress.None;
                int applicationLayerProtocolLength=0;

                if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv4Packet)) {
                    PacketParser.Packets.IPv4Packet ipv4Packet=(PacketParser.Packets.IPv4Packet)ipPacket;
                    sourceIp=ipv4Packet.SourceIPAddress;
                    destinationIp=ipv4Packet.DestinationIPAddress;
                    applicationLayerProtocolLength=ipv4Packet.TotalLength-ipv4Packet.HeaderLength-tcpPacket.DataOffsetByteCount;
                }
                else if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv6Packet)) {
                    PacketParser.Packets.IPv6Packet ipv6Packet=(PacketParser.Packets.IPv6Packet)ipPacket;
                    sourceIp=ipv6Packet.SourceIP;
                    destinationIp=ipv6Packet.DestinationIP;
                    applicationLayerProtocolLength=ipv6Packet.PayloadLength-tcpPacket.DataOffsetByteCount;
                }

                //Check if the client and server have been defined
                if(this.serverIp==null || this.clientIp==null) {
                    //find out which host is server and which is client
                    if(tcpPacket.FlagBits.Synchronize && !tcpPacket.FlagBits.Acknowledgement) {
                        //source is client
                        this.clientIp=sourceIp;
                        this.clientPort=tcpPacket.SourcePort;
                        this.serverIp=destinationIp;
                        this.serverPort=tcpPacket.DestinationPort;
                    }
                    else if(tcpPacket.FlagBits.Synchronize && tcpPacket.FlagBits.Acknowledgement) {
                        //source is server
                        this.serverIp=sourceIp;
                        this.serverPort=tcpPacket.SourcePort;
                        this.clientIp=destinationIp;
                        this.clientPort=tcpPacket.DestinationPort;
                    }
                    else
                        throw new Exception("Session does not start with a SYN or SYN+ACK packet");
                }

                //identify the direction
                if(sourceIp.Equals(this.clientIp) && destinationIp.Equals(this.serverIp) && tcpPacket.SourcePort==this.clientPort && tcpPacket.DestinationPort==this.serverPort) {
                    //AddFrame(ipPacket, tcpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer);
                    AddFrame(tcpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer, applicationLayerProtocolLength);
                    return ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer;
                }
                else if(sourceIp.Equals(this.serverIp) && destinationIp.Equals(this.clientIp) && tcpPacket.SourcePort==this.serverPort && tcpPacket.DestinationPort==this.clientPort) {
                    AddFrame(tcpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient, applicationLayerProtocolLength);
                    return ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient;
                }
                else
                    throw new Exception("IP's and ports do not match those belonging to this session\nSession server: "+this.serverIp.ToString()+":"+this.ServerPort.ToString()+"\nSession client: "+this.clientIp.ToString()+":"+this.clientPort.ToString()+"\nPacket source: "+sourceIp.ToString()+":"+tcpPacket.SourcePort.ToString()+"\nPacket destination: "+destinationIp+":"+tcpPacket.DestinationPort);
            }
            else
                return ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.Unknown;

     
            
        }

        public void AddFrame(PacketParser.Packets.TcpPacket tcpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection direction, int protocolPacketLength) {    

            this.frameCount++;
            this.lastPacketTimestamp=tcpPacket.ParentFrame.Timestamp;

            if(this.firstPacketTimestamp==DateTime.MinValue)
                this.firstPacketTimestamp=tcpPacket.ParentFrame.Timestamp;

            if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.Unknown)
                throw new Exception("A valid direction must be supplied");

            if(this.frameCount==config.MaxFramesToInspectPerSession+1 && this.applicationProtocolModel!=SessionHandler.PlaceholderProtocolModel) {
                if(this.ProtocolModelCompleted!=null)
                    this.ProtocolModelCompleted(this, this.applicationProtocolModel);
                if(usePlaceholderProtocolModel)
                    this.applicationProtocolModel=SessionHandler.PlaceholderProtocolModel;
            }

            if(this.state==TCPState.CLOSED) {
                //do nothing
                //throw new Exception("Cannot add a frame to a closed session");
            }
            else if(this.state==TCPState.NONE) {
                //Expected: client->server SYN
                //or unidirectional server->client SYN+ACK
                if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer && tcpPacket.FlagBits.Synchronize)
                    this.state=TCPState.SYN;
                else if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient && tcpPacket.FlagBits.Synchronize && tcpPacket.FlagBits.Acknowledgement)
                    this.state=TCPState.SYN_ACK;
            }
            else if(this.state==TCPState.SYN) {
                //Expected: server->client SYN+ACK
                //or unidirectional client->server ACK
                if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient && tcpPacket.FlagBits.Synchronize && tcpPacket.FlagBits.Acknowledgement)
                    this.state=TCPState.SYN_ACK;
                else if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer && !tcpPacket.FlagBits.Synchronize && tcpPacket.FlagBits.Acknowledgement)
                    this.state=TCPState.ESTABLISHED;
            }
            else if(this.state==TCPState.SYN_ACK) {
                //Expected: client->server ACK
                //or unidirectional first data packet server->client
                if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ClientToServer && tcpPacket.FlagBits.Acknowledgement && !tcpPacket.FlagBits.Synchronize) {
                    this.state=TCPState.ESTABLISHED;
                    //generate a SessionEstablishedEvent
                    if(SessionEstablished!=null)
                        SessionEstablished(this);
                }
                //else if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient && (ipPacket.TotalLength-(tcpPacket.PacketStartIndex-ipPacket.PacketStartIndex)-tcpPacket.DataOffsetByteCount)>0) {
                else if(direction==ProtocolIdentification.AttributeFingerprintHandler.PacketDirection.ServerToClient && protocolPacketLength>0) {
                    this.state=TCPState.ESTABLISHED;
                    if(SessionEstablished!=null)
                        SessionEstablished(this);
                    //AddFrameToOpenSession(ipPacket, tcpPacket, direction);
                    AddFrameToOpenSession(tcpPacket, direction, protocolPacketLength);
                }

            }
            else if(this.state==TCPState.ESTABLISHED || this.state==TCPState.FIN) {
                //AddFrameToOpenSession(ipPacket, tcpPacket, direction);
                AddFrameToOpenSession(tcpPacket, direction, protocolPacketLength);
            }
        }


        private void AddFrameToOpenSession(PacketParser.Packets.TcpPacket tcpPacket, ProtocolIdentification.AttributeFingerprintHandler.PacketDirection direction, int protocolPacketLength) {    
            if(this.state!=TCPState.ESTABLISHED && this.state!=TCPState.FIN)
                throw new Exception("Session is not open!");
            else{
                if(this.state==TCPState.ESTABLISHED && tcpPacket.FlagBits.Fin && !tcpPacket.FlagBits.Acknowledgement)
                    this.state=TCPState.FIN;
                else if(tcpPacket.FlagBits.Fin && tcpPacket.FlagBits.Acknowledgement) {
                    this.state=TCPState.CLOSED;
                    if(this.SessionClosed!=null)
                        this.SessionClosed(this, this.applicationProtocolModel);
                    if(usePlaceholderProtocolModel)
                        this.applicationProtocolModel=SessionHandler.PlaceholderProtocolModel;//to save memory
                }
                else if(tcpPacket.FlagBits.Reset) {
                    this.state=TCPState.CLOSED;
                    if(this.SessionClosed!=null)
                        this.SessionClosed(this, this.applicationProtocolModel);
                    if(usePlaceholderProtocolModel)
                        this.applicationProtocolModel=SessionHandler.PlaceholderProtocolModel;//to save memory
                }
                else {
                    if(this.applicationProtocolModel==null)
                        this.applicationProtocolModel=new ProtocolIdentification.ProtocolModel(this.Identifier, config.ActiveAttributeMeters);

                    if(this.frameCount<=config.MaxFramesToInspectPerSession) {
                        //at last we can add the observation to the model
                        //int protocolPacketStartIndex=tcpPacket.PacketStartIndex+tcpPacket.DataOffsetByteCount;
                        //int protocolPacketLength=ipPacket.TotalLength-(tcpPacket.PacketStartIndex-ipPacket.PacketStartIndex)-tcpPacket.DataOffsetByteCount;
                        if(protocolPacketLength>0) {
                            int protocolPacketStartIndex=tcpPacket.PacketStartIndex+tcpPacket.DataOffsetByteCount;
                            this.applicationProtocolModel.AddObservation(tcpPacket.ParentFrame.Data, protocolPacketStartIndex, protocolPacketLength, tcpPacket.ParentFrame.Timestamp, direction);
                        }
                    }
                }
            }
        }

    }
}
