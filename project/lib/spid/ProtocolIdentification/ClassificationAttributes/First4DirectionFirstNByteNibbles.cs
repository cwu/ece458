using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First4DirectionFirstNByteNibbles : IClassificationAttribute {
        private int nByteNibbles;
        private int serverToClientOffset;
        private int clientToServerOffset;

        public First4DirectionFirstNByteNibbles() {
            this.serverToClientOffset=0;
            this.clientToServerOffset=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nByteNibbles=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/(2*16);
        }

        #region IClassificationAttribute Members

        string IClassificationAttribute.AttributeName {
            get { return "First 4 packets with Direction (non-ordered) First "+nByteNibbles+" Byte-Nibbles"; }
        }

        IEnumerable<int> IClassificationAttribute.GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                for(int i=0; i<nByteNibbles && i<packetLength &&  packetStartIndex+i<frameData.Length; i++) {
                    if(packetDirection== ClassificationAttributeFingerprint.PacketDirection.ClientToServer)
                        yield return this.clientToServerOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]);
                    else if(packetDirection== ClassificationAttributeFingerprint.PacketDirection.ServerToClient)
                        yield return this.serverToClientOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]);
                }
            }
        }

        #endregion
    }
}
