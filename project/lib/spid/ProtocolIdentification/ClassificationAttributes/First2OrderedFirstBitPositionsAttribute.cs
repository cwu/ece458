using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First2OrderedFirstBitPositionsAttribute : IClassificationAttribute{
        private int nBytesToCheck;
        private int zeroValueOffset;
        private int oneValueOffset;
        private int packetNumberIncrement;

        public First2OrderedFirstBitPositionsAttribute() {
            this.zeroValueOffset=0;
            this.oneValueOffset=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/4;
            this.packetNumberIncrement=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nBytesToCheck=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/32;//4*8
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "Counter bins for the bit values of the first "+nBytesToCheck+" bytes in the first 2 ordered packets"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<2) {
                byte[] packetBytes=new byte[Math.Min(Math.Min(packetLength, nBytesToCheck), frameData.Length-packetStartIndex)];
                Array.Copy(frameData, packetStartIndex, packetBytes, 0, packetBytes.Length);
                System.Collections.BitArray bitArray=new System.Collections.BitArray(packetBytes);
                for(int i=0; i<bitArray.Length; i++) {
                    if(bitArray[i])
                        yield return packetOrderNumberInSession*packetNumberIncrement+zeroValueOffset+i;
                    else
                        yield return packetOrderNumberInSession*packetNumberIncrement+oneValueOffset+i;
                }
            }
        }

        #endregion
    }
}
