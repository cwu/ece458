//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class ByteFrequencyOfFirstPacketBytesMeter : IAttributeMeter{

        private const int maxBytesToParse=100;

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "ByteFrequencyOfFirstPacketBytesMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //it's enough with the first 8 packets.
            //We will otherwise get too much overweight of data from long sessions
            if(packetOrderNumberInSession<8) {
                for(int i=packetStartIndex; i<packetStartIndex+packetLength && i<frameData.Length && i<maxBytesToParse; i++)
                    yield return (int)frameData[i]%AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH;
            }
        }

        #endregion
    }
}
