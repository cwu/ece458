//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First4PacketsByteFrequencyMeter : IAttributeMeter {
        private const int maxBytesToParse=100;

        #region IAttributeMeter Members

        public string AttributeName {
            get { return "First4PacketsByteFrequencyMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                for(int i=packetStartIndex; i<packetStartIndex+packetLength && i<frameData.Length && i<maxBytesToParse; i++)
                    yield return (int)((frameData[i]%(AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/4))+packetOrderNumberInSession*AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/4);
            }
        }

        #endregion
    }
}
