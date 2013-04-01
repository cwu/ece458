//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First4PacketsFirst16BytePairsMeter : IAttributeMeter {


        #region IAttributeMeter Members

        public string AttributeName {
            get { return "First4PacketsFirst16BytePairsMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                for(int i=1; packetStartIndex+i<frameData.Length && i<packetLength && i<17; i++) {
                    int pairData=(int)frameData[packetStartIndex+i-1];
                    pairData<<=8;
                    pairData+=frameData[packetStartIndex+i];
                    yield return ConvertHelper.ToHashValue(pairData, AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS);
                }
            }
        }

        #endregion
    }
}
