using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class ByteValueOffsetHashOfFirst32BytesInFirst4Packets : IClassificationAttribute{
        private int hashSizeInBits;

        public ByteValueOffsetHashOfFirst32BytesInFirst4Packets() {
            this.hashSizeInBits=(int)Math.Log(ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH, 2.0);
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "Byte Value+Offset Hash Of First 32 Bytes InFirst 4 Packets"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                for(int i=0; i<Math.Min(packetLength, 32) && packetStartIndex+i<frameData.Length; i++) {
                    yield return ConvertHelper.ToHashValue(frameData[packetStartIndex+i]+256*i, hashSizeInBits);
                }
            }
        }

        #endregion
    }
}
