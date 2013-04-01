using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First2PacketsFirst16ByteHashCounts : IClassificationAttribute {
        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First2PacketsFirst16ByteHashCounts"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //The first FINGERPRINT_BITS-4 bits define the byteHash value
            //The last 4 bits defines the count (number of bytes) with the specified byteHash
            if(packetOrderNumberInSession<2) {
                int byteHashBinCount=1<<(ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS-4);
                byte[] byteHashBinCounters=new byte[byteHashBinCount];
                for(int i=0; i<16 && i+packetStartIndex<frameData.Length && i<packetLength; i++) {
                    int byteHash=ConvertHelper.ToHashValue((int)frameData[packetStartIndex+i], ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS-4);
                    byteHashBinCounters[byteHash]++;
                }

                //now return the values
                for(int i=0; i<byteHashBinCounters.Length; i++) {
                    yield return (i<<4)^byteHashBinCounters[i];
                }
            }
            //yield break;
        }

        #endregion
    }
}
