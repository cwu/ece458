using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class FirstBitPositionsAttribute : IClassificationAttribute{
        private int nBytesToCheck;
        private int zeroValueOffset;
        private int oneValueOffset;

        public FirstBitPositionsAttribute(){
            this.zeroValueOffset=0;
            this.oneValueOffset=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nBytesToCheck=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/16;//2*8
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "Counter bins for the bit values of the first "+nBytesToCheck+" bytes"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            byte[] packetBytes=new byte[Math.Min(Math.Min(packetLength, nBytesToCheck), frameData.Length-packetStartIndex)];
            Array.Copy(frameData, packetStartIndex, packetBytes, 0, packetBytes.Length);
            System.Collections.BitArray bitArray=new System.Collections.BitArray(packetBytes);
            for(int i=0; i<bitArray.Length; i++){
                if(bitArray[i])
                    yield return zeroValueOffset+i;
                else
                    yield return oneValueOffset+i;
            }
        }

        #endregion
    }
}
