using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class ByteFrequencyAttribute : IClassificationAttribute{


        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "Byte Frequency in first 8 packets"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //it's enough with the first 8 packets.
            //We will otherwise get too much overweight of data from long sessions
            if(packetOrderNumberInSession<8) {
                for(int i=packetStartIndex; i<packetStartIndex+packetLength && i<frameData.Length; i++)
                    yield return (int)frameData[i]%ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH;
            }
        }

        #endregion
    }
}
