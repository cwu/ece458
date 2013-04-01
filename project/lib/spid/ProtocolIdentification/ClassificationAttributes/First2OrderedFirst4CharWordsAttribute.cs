using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First2OrderedFirst4CharWordsAttribute : IClassificationAttribute {


        //public First2OrderedFirst4CharWordsAttribute() {}

        #region IClassificationAttribute Members

        string IClassificationAttribute.AttributeName {
            get { return "First2OrderedFirst4CharWordsAttribute"; }
        }

        IEnumerable<int> IClassificationAttribute.GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<2) {
                StringBuilder sb=new StringBuilder(4);

                for(int i=0; sb.Length<4 && i<packetLength && packetStartIndex+i<frameData.Length; i++)
                    sb.Append(frameData[packetStartIndex+i]);
                yield return ConvertHelper.ToHashValue((sb.ToString()+packetDirection.ToString()).GetHashCode(), ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS);
            }
        }

        #endregion
    }
}
