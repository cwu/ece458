using System;
using System.Collections.Generic;
using System.Text;

namespace SPID {
    class SessionHandler {

        public enum TransportProtocol{TCP, UDP}

        //60 seconds
        private static readonly TimeSpan SESSION_TIMEOUT=new TimeSpan(0, 1, 0);//60 seconds. Rossi et al uses 200s in their Skype paper

        public delegate void SessionProtocolModelCompletedEventHandler(ISession session, ProtocolIdentification.ProtocolModel protocolModel);
        public event SessionProtocolModelCompletedEventHandler SessionProtocolModelCompleted;

        //this protocol model is here just to save memory
        //sessions protocol models are replaced this one when
        //they have reached 100 frames
        public static ProtocolIdentification.ProtocolModel PlaceholderProtocolModel=new ProtocolIdentification.ProtocolModel("Placeholder Protocol Model", Configuration.GetInstance().ActiveAttributeMeters);
        public int SessionsCount { get { return this.establishedSessionsList.Count; } }

        //the purpose with having a separate list for not-yet established
        //sessions is to prevent SYN scans from pushing established sessions
        //out from the list
        private PacketParser.PopularityList<string, ISession> establishedSessionsList, upcomingSessionsList;
        private Configuration config;

        //10.000 simultaneous sessions might a good (high) value
        //each session will consume ~30kB
        public SessionHandler(int maxSimultaneousSessions, Configuration config) {
            this.upcomingSessionsList=new PacketParser.PopularityList<string, ISession>(maxSimultaneousSessions);
            this.establishedSessionsList=new PacketParser.PopularityList<string, ISession>(maxSimultaneousSessions);
            this.establishedSessionsList.PopularityLost+=new PacketParser.PopularityList<string, ISession>.PopularityLostEventHandler(establishedSessionsList_PopularityLost);
            this.config=config;
        }

        void establishedSessionsList_PopularityLost(string sessionIdentifierKey, ISession session) {
            //now, do something smart with the session
            if(this.SessionProtocolModelCompleted!=null)
                this.SessionProtocolModelCompleted(session, session.ApplicationProtocolModel);
        }


        public bool TryGetSession(PacketParser.Frame frame, out ISession session) {
            //start with getting the IP adresses and port numbers
            PacketParser.Packets.AbstractPacket ipPacket;
            PacketParser.Packets.TcpPacket tcpPacket;
            PacketParser.Packets.UdpPacket udpPacket;
            session=null;

            System.Net.IPAddress sourceIp=System.Net.IPAddress.None;
            System.Net.IPAddress destinationIp=System.Net.IPAddress.None;

            if(TryGetIpAndTcpPackets(frame, out ipPacket, out tcpPacket)) {
                if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv4Packet)) {
                    PacketParser.Packets.IPv4Packet ipv4Packet=(PacketParser.Packets.IPv4Packet)ipPacket;
                    sourceIp=ipv4Packet.SourceIPAddress;
                    destinationIp=ipv4Packet.DestinationIPAddress;
                }
                else if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv6Packet)) {
                    PacketParser.Packets.IPv6Packet ipv6Packet=(PacketParser.Packets.IPv6Packet)ipPacket;
                    sourceIp=ipv6Packet.SourceIP;
                    destinationIp=ipv6Packet.DestinationIP;
                }
                //we now have the IP addresses
                TcpSession tcpSession;
                if(TryGetSession(sourceIp, destinationIp, tcpPacket, out tcpSession)) {
                    session=tcpSession;
                    return true;
                }
                else
                    return false;
            }
            else if(TryGetIpAndUdpPackets(frame, out ipPacket, out udpPacket)) {
                if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv4Packet)) {
                    PacketParser.Packets.IPv4Packet ipv4Packet=(PacketParser.Packets.IPv4Packet)ipPacket;
                    sourceIp=ipv4Packet.SourceIPAddress;
                    destinationIp=ipv4Packet.DestinationIPAddress;
                }
                else if(ipPacket.GetType()==typeof(PacketParser.Packets.IPv6Packet)) {
                    PacketParser.Packets.IPv6Packet ipv6Packet=(PacketParser.Packets.IPv6Packet)ipPacket;
                    sourceIp=ipv6Packet.SourceIP;
                    destinationIp=ipv6Packet.DestinationIP;
                }
                //we now have the IP addresses
                UdpSession udpSession;
                if(TryGetSession(sourceIp, destinationIp, udpPacket, out udpSession)) {
                    session=udpSession;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;//no session found
        }

        public bool TryGetSession(System.Net.IPAddress sourceIp, System.Net.IPAddress destinationIp, PacketParser.Packets.UdpPacket udpPacket, out UdpSession session) {
            session=null;
            string clientToServerIdentifier=GetSessionIdentifier(sourceIp, udpPacket.SourcePort, destinationIp, udpPacket.DestinationPort, TransportProtocol.UDP);
            string serverToClientIdentifier=GetSessionIdentifier(destinationIp, udpPacket.DestinationPort, sourceIp, udpPacket.SourcePort, TransportProtocol.UDP);
            if(this.upcomingSessionsList.ContainsKey(clientToServerIdentifier))
                session=(UdpSession)this.upcomingSessionsList[clientToServerIdentifier];
            else if(this.establishedSessionsList.ContainsKey(clientToServerIdentifier))
                session=(UdpSession)this.establishedSessionsList[clientToServerIdentifier];
            else if(this.upcomingSessionsList.ContainsKey(serverToClientIdentifier))
                session=(UdpSession)this.upcomingSessionsList[serverToClientIdentifier];
            else if(this.establishedSessionsList.ContainsKey(serverToClientIdentifier))
                session=(UdpSession)this.establishedSessionsList[serverToClientIdentifier];
            
            //see if the session has timed out
            if(session!=null && session.LastPacketTimestamp.Add(SESSION_TIMEOUT)<udpPacket.ParentFrame.Timestamp) {
                //session has timed out
                if(this.establishedSessionsList.ContainsKey(session.Identifier)) {
                    this.establishedSessionsList.Remove(session.Identifier);
                    if(this.SessionProtocolModelCompleted!=null)
                        this.SessionProtocolModelCompleted(session, session.ApplicationProtocolModel);
                }

                session=null;
            }

            if(session==null) {
                //create a new session, source is client (the UDP client is defined as the host that send the first packet)
                session=new UdpSession(config, sourceIp, udpPacket.SourcePort, destinationIp, udpPacket.DestinationPort);
                this.upcomingSessionsList.Add(session.Identifier, session);
                session.SessionEstablished+=new UdpSession.SessionEstablishedEventHandler(session_SessionEstablished);
                session.ProtocolModelCompleted+=new UdpSession.ProtocolModelCompletedEventHandler(session_ProtocolModelCompleted);
            }
            if(session!=null)
                return true;
            else
                return false;
        }

        public bool TryGetSession(System.Net.IPAddress sourceIp, System.Net.IPAddress destinationIp, PacketParser.Packets.TcpPacket tcpPacket, out TcpSession session) {
            session=null;

            string clientToServerIdentifier=GetSessionIdentifier(sourceIp, tcpPacket.SourcePort, destinationIp, tcpPacket.DestinationPort, TransportProtocol.TCP);
            string serverToClientIdentifier=GetSessionIdentifier(destinationIp, tcpPacket.DestinationPort, sourceIp, tcpPacket.SourcePort, TransportProtocol.TCP);
            if(this.upcomingSessionsList.ContainsKey(clientToServerIdentifier))
                session=(TcpSession)this.upcomingSessionsList[clientToServerIdentifier];
            else if(this.establishedSessionsList.ContainsKey(clientToServerIdentifier))
                session=(TcpSession)this.establishedSessionsList[clientToServerIdentifier];
            else if(this.upcomingSessionsList.ContainsKey(serverToClientIdentifier))
                session=(TcpSession)this.upcomingSessionsList[serverToClientIdentifier];
            else if(this.establishedSessionsList.ContainsKey(serverToClientIdentifier))
                session=(TcpSession)this.establishedSessionsList[serverToClientIdentifier];

            //see if the session has timed out
            if(session!=null && session.LastPacketTimestamp.Add(SESSION_TIMEOUT)<tcpPacket.ParentFrame.Timestamp) {
                //session has timed out
                if(this.upcomingSessionsList.ContainsKey(session.Identifier))
                    this.upcomingSessionsList.Remove(session.Identifier);
                else if(this.establishedSessionsList.ContainsKey(session.Identifier)) {
                    this.establishedSessionsList.Remove(session.Identifier);
                    if(this.SessionProtocolModelCompleted!=null)
                        this.SessionProtocolModelCompleted(session, session.ApplicationProtocolModel);
                }

                session=null;
            }
            
            if(session==null) {
                //try to create a new session
                if(tcpPacket.FlagBits.Synchronize && !tcpPacket.FlagBits.Acknowledgement) {
                    //the first SYN packet - source is client
                    session=new TcpSession(config, sourceIp, tcpPacket.SourcePort, destinationIp, tcpPacket.DestinationPort);
                }
                else if(tcpPacket.FlagBits.Synchronize && tcpPacket.FlagBits.Acknowledgement) {
                    //we've missed the first SYN but caught the SYN+ACK - destination is client
                    session=new TcpSession(config, destinationIp, tcpPacket.DestinationPort, sourceIp, tcpPacket.SourcePort);
                    session.State=TcpSession.TCPState.SYN;//I will pretend that the SYN has already been observed
                }

                if(session!=null) {
                    this.upcomingSessionsList.Add(session.Identifier, session);
                    session.UsePlaceholderProtocolModel=true;//in order to save memory, but I will need to detect when the protocol model is completed instead
                    session.SessionEstablished+=new TcpSession.SessionEstablishedEventHandler(session_SessionEstablished);
                    session.ProtocolModelCompleted+=new TcpSession.ProtocolModelCompletedEventHandler(session_ProtocolModelCompleted);
                    session.SessionClosed+=new TcpSession.SessionClosedEventHandler(session_SessionClosed);
                }
            }
            if(session!=null)
                return true;
            else
                return false;

        
        }

        void session_SessionClosed(TcpSession session, ProtocolIdentification.ProtocolModel protocolModel) {
            //first remove the session from the list
            if(this.upcomingSessionsList.ContainsKey(session.Identifier))
                this.upcomingSessionsList.Remove(session.Identifier);
            else if(this.establishedSessionsList.ContainsKey(session.Identifier))
                this.establishedSessionsList.Remove(session.Identifier);

            if(protocolModel!=null && protocolModel!=PlaceholderProtocolModel && this.SessionProtocolModelCompleted!=null)
                this.SessionProtocolModelCompleted(session, protocolModel);
        }

        void session_ProtocolModelCompleted(ISession session, ProtocolIdentification.ProtocolModel protocolModel) {
            if(protocolModel!=null && protocolModel!=PlaceholderProtocolModel && this.SessionProtocolModelCompleted!=null)
                this.SessionProtocolModelCompleted(session, protocolModel);
        }

        void session_SessionEstablished(ISession session) {
            //see if the session is in the non-established list
            if(upcomingSessionsList.ContainsKey(session.Identifier))
                upcomingSessionsList.Remove(session.Identifier);
            //now move it to the established list instead
            if(!establishedSessionsList.ContainsKey(session.Identifier))
                establishedSessionsList.Add(session.Identifier, session);
        }

        public static string GetSessionIdentifier(System.Net.IPAddress clientIp, ushort clientPort, System.Net.IPAddress serverIp, ushort serverPort, TransportProtocol transportProtocol) {
            return transportProtocol.ToString()+" "+clientIp.ToString()+":"+clientPort.ToString()+" -> "+serverIp.ToString()+":"+serverPort.ToString();
        }

        public static bool TryGetEthernetPacket(PacketParser.Frame frame, out PacketParser.Packets.Ethernet2Packet ethernetPacket) {
            ethernetPacket=null;
            foreach(PacketParser.Packets.AbstractPacket p in frame.PacketList) {
                if(p.GetType()==typeof(PacketParser.Packets.Ethernet2Packet)){
                    ethernetPacket=(PacketParser.Packets.Ethernet2Packet)p;
                    return true;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.RawPacket))
                    return false;
                else if(p.GetType()==typeof(PacketParser.Packets.IPv4Packet))
                    return false;
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="ipPacket">IPv4 or IPv6 packet in frame</param>
        /// <param name="tcpPacket">TCP packet in frame</param>
        /// <returns></returns>
        public static bool TryGetIpAndTcpPackets(PacketParser.Frame frame, out PacketParser.Packets.AbstractPacket ipPacket, out PacketParser.Packets.TcpPacket tcpPacket){

            ipPacket=null;
            //sourceIp=System.Net.IPAddress.None;
            //destinationIp=System.Net.IPAddress.None;
            tcpPacket=null;
            foreach(PacketParser.Packets.AbstractPacket p in frame.PacketList) {
                if(p.GetType()==typeof(PacketParser.Packets.RawPacket)) {
                    return false;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.UdpPacket)) {
                    return false;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.IPv4Packet)) {
                    ipPacket=p;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.IPv6Packet)) {
                    ipPacket=p;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.TcpPacket)) {
                    tcpPacket=(PacketParser.Packets.TcpPacket)p;
                    //there is no point in enumarating further than the TCP packet
                    if(ipPacket!=null)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public static bool TryGetIpAndUdpPackets(PacketParser.Frame frame, out PacketParser.Packets.AbstractPacket ipPacket, out PacketParser.Packets.UdpPacket udpPacket) {

            ipPacket=null;
            udpPacket=null;
            foreach(PacketParser.Packets.AbstractPacket p in frame.PacketList) {
                if(p.GetType()==typeof(PacketParser.Packets.RawPacket)) {
                    return false;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.TcpPacket)) {
                    return false;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.IPv4Packet)) {
                    ipPacket=p;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.IPv6Packet)) {
                    ipPacket=p;
                }
                else if(p.GetType()==typeof(PacketParser.Packets.UdpPacket)) {
                    udpPacket=(PacketParser.Packets.UdpPacket)p;
                    //there is no point in enumarating further than the UDP packet
                    if(ipPacket!=null)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// To get the remaining sessions whose protocol models have not yet been fully completed
        /// </summary>
        /// <returns>Sessions</returns>
        public IEnumerable<ISession> GetSessionsWithoutCompletedProtocolModels() {
            foreach(ISession s in this.establishedSessionsList.GetValueEnumerator())
                if(s.ApplicationProtocolModel!=null && s.ApplicationProtocolModel!=PlaceholderProtocolModel)
                    yield return s;
        }



    }
}
