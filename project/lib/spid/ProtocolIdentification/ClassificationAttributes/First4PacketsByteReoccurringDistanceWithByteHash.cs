using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First4PacketsByteReoccurringDistanceWithByteHash : IClassificationAttribute{
        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First4PacketsByteReoccurringDistanceWithByteHash"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                //use 4 bits to define distances 1->16
                int byteHashBits=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS-4;
                int[] lastByteIndex=new int[256];//one box per byte value
                //initialize all values to something small
                for(int i=0; i<lastByteIndex.Length; i++)
                    lastByteIndex[i]=int.MinValue;
                for(int i=0; i<packetLength && packetStartIndex+i<frameData.Length && i<32; i++) {
                    int reoccurringDistance=packetStartIndex+i-lastByteIndex[frameData[packetStartIndex+i]];
                    if(reoccurringDistance>0 && reoccurringDistance<17) {
                        //calculate the byteHash
                        int byteHash=ConvertHelper.ToHashValue(frameData[packetStartIndex+i], byteHashBits);
                        yield return (byteHash<<4)+reoccurringDistance-1;
                    }
                    lastByteIndex[frameData[packetStartIndex+i]]=packetStartIndex+i;
                }
            }
        }

        #endregion
    }
}
