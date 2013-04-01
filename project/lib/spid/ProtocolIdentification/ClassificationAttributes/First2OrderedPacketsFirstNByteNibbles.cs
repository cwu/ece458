using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First2OrderedPacketsFirstNByteNibbles : IClassificationAttribute{
        //private int[] packetOrderOffset;
        private int nBytesToParsePerPacket;
        private int packetOrderOffsetIncrement;

        public First2OrderedPacketsFirstNByteNibbles() {
            this.packetOrderOffsetIncrement=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nBytesToParsePerPacket=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/(2*16);
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First 2 Ordered Packets First "+nBytesToParsePerPacket+" Byte-Nibbles"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<2)//Only do work if it is first (0) or second (1) packet in the session
                for(int i=0; i<Math.Min(nBytesToParsePerPacket, packetLength) && packetStartIndex+i<frameData.Length; i++) {
                    yield return (packetOrderNumberInSession*this.packetOrderOffsetIncrement+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]))%ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH;
                }
        }

        #endregion
    }
}
