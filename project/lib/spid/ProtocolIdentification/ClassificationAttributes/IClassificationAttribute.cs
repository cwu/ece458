using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {

    //Flow Attributes almost as defined in:
    //http://www.crc.ca/files/crc/home/research/network/system_apps/network_systems/network_security/publications/ADeMontigny_CRCTN2005003.pdf
    //http://www.pamconf.org/2005/PDF/34310328.pdf
    //and
    //http://netgroup.polito.it/teaching/trc/Salgarelli%20-%20statistical%20traffic%20classification.pdf
    //The main difference is that I don't do the classification per flow,
    //I do it per session (i.e. 2 flows per classification attribute)

    public interface IClassificationAttribute {

        string AttributeName { get;}

        //IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, TimeSpan interPacketDelay, AttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession);
        IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession);

        
    }
}
