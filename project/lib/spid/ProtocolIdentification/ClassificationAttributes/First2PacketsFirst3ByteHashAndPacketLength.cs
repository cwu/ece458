using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First2PacketsFirst3ByteHashAndPacketLength : IClassificationAttribute{
        private int expectedMaxPacketLength;
        private double exponent;


        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First2PacketsFirst3ByteHashAndPacketLength"; }
        }

        public First2PacketsFirst3ByteHashAndPacketLength() {
            this.expectedMaxPacketLength=1500;
            //This attribute uses 4 bits for the packet length, i.e. 16 bins
            this.exponent=Math.Log(16)/Math.Log(expectedMaxPacketLength);
        }

        private int GetLengthBinOffset(int packetLength) {
            return Math.Min(15, (int)Math.Pow(packetLength, exponent));
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<2) {
                int lengthBinOffset=GetLengthBinOffset(packetLength);
                for(int i=0; i<3 && packetStartIndex+i<frameData.Length && i<packetLength; i++) {
                    int byteHash=ConvertHelper.ToHashValue((int)frameData[packetStartIndex+i], ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS-4);
                    yield return (byteHash<<4)+lengthBinOffset;
                }
            }
        }

        #endregion
    }
}
