//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First2PacketsFirst3ByteHashAndPacketLengthMeter : IAttributeMeter{
        private int expectedMaxPacketLength;
        private double exponent;


        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First2PacketsFirst3ByteHashAndPacketLengthMeter"; }
        }

        public First2PacketsFirst3ByteHashAndPacketLengthMeter() {
            this.expectedMaxPacketLength=1500;
            //This attribute uses 4 bits for the packet length, i.e. 16 bins
            this.exponent=Math.Log(16)/Math.Log(expectedMaxPacketLength);
        }

        private int GetLengthBinOffset(int packetLength) {
            return Math.Min(15, (int)Math.Pow(packetLength, exponent));
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<2) {
                int lengthBinOffset=GetLengthBinOffset(packetLength);
                for(int i=0; i<3 && packetStartIndex+i<frameData.Length && i<packetLength; i++) {
                    int byteHash=ConvertHelper.ToHashValue((int)frameData[packetStartIndex+i], AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS-4);
                    yield return (byteHash<<4)+lengthBinOffset;
                }
            }
        }

        #endregion
    }
}
