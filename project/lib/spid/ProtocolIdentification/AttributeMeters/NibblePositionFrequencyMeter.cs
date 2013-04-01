using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {

    //Inspired by "KISS: Stochastic Packet Inspection" by Finamore, Mellia, Meo and Rossi
    //http://www.tlc-networks.polito.it/mellia/papers/KISS_tma.pdf
    class NibblePositionFrequencyMeter : IAttributeMeter{

        #region IAttributeMeter Members

        public string AttributeName {
            get { return "NibblePositionFrequencyMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<8) {
                for(int nibbleIndex=0; packetStartIndex+nibbleIndex/2<frameData.Length && nibbleIndex/2<packetLength && nibbleIndex<AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/16; nibbleIndex++) {
                    int byteIndex=packetStartIndex+nibbleIndex/2;
                    bool firstNibble=(nibbleIndex%2==0);
                    byte nibble=ConvertHelper.GetNibble(frameData[byteIndex], firstNibble);
                    yield return nibbleIndex*16+nibble;
                }
            }
        }

        #endregion
    }
}
