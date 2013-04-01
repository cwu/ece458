using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First4PacketsFirst16BytePairsAttribute : IClassificationAttribute {
        #region IClassificationAttribute Members

        string IClassificationAttribute.AttributeName {
            get { return "First4PacketsFirst16BytePairsAttribute"; }
        }

        IEnumerable<int> IClassificationAttribute.GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                for(int i=1; packetStartIndex+i<frameData.Length && i<packetLength && i<17; i++) {
                    int pairData=(int)frameData[packetStartIndex+i-1];
                    pairData<<=8;
                    pairData+=frameData[packetStartIndex+i];
                    yield return ConvertHelper.ToHashValue(pairData, ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS);
                }
            }
        }

        #endregion
    }
}
