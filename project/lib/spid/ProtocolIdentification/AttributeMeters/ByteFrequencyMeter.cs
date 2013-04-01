//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class ByteFrequencyMeter : IAttributeMeter{

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "ByteFrequencyMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //it's enough with the first 5 packets.
            //We will otherwise get too much overweight of data from long sessions
            if(packetOrderNumberInSession<5) {
                for(int i=packetStartIndex; i<packetStartIndex+packetLength && i<frameData.Length; i++)
                    yield return (int)frameData[i]%AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH;
            }
        }

        #endregion
    }
}
