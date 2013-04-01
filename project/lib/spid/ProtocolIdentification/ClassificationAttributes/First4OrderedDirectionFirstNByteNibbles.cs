using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First4OrderedDirectionFirstNByteNibbles : IClassificationAttribute{
        int clientToServerOffset;
        int serverToClientOffset;
        int packetOrderOffsetIncrement;
        int nBytesToParsePerPacket;


        public First4OrderedDirectionFirstNByteNibbles() {
            this.clientToServerOffset=0;
            this.serverToClientOffset=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.packetOrderOffsetIncrement=serverToClientOffset/4;
            this.nBytesToParsePerPacket=packetOrderOffsetIncrement/16;
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First 4 Ordered Direction First "+nBytesToParsePerPacket+" Byte-Nibbles"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {//only care about the first 4 packets (value 0->3)
                int directionOffset;
                if(packetDirection== ClassificationAttributeFingerprint.PacketDirection.ClientToServer)
                    directionOffset=clientToServerOffset;
                else if(packetDirection== ClassificationAttributeFingerprint.PacketDirection.ServerToClient)
                    directionOffset=serverToClientOffset;
                else
                    yield break;
                for(int i=0; i<Math.Min(packetLength, nBytesToParsePerPacket); i++) {

                    yield return directionOffset+packetOrderOffsetIncrement*packetOrderNumberInSession+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]);
                }
            }
        }

        #endregion
    }
}
