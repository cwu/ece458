using System;
using System.Collections.Generic;
using System.Text;

namespace SPID {
    interface ISession {

        ProtocolIdentification.ProtocolModel ApplicationProtocolModel { get; }
        
        System.Net.IPAddress ServerIP{get;}
        System.Net.IPAddress ClientIP{get;}
        ushort ClientPort {get;}
        ushort ServerPort {get;}
        DateTime FirstPacketTimestamp { get;}

        SessionHandler.TransportProtocol TransportProtocol { get;}

        DateTime LastPacketTimestamp { get;}

        string Identifier{get;}

        ProtocolIdentification.AttributeFingerprintHandler.PacketDirection AddFrame(PacketParser.Frame frame);
        
    }
}
