using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class DirectionByteFrequencyAttribute : IClassificationAttribute{

        private int directionLength;
        private int clientToServerStartIndex;
        private int serverToClientStartIndex;

        public DirectionByteFrequencyAttribute() {
            this.directionLength=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.clientToServerStartIndex=0;
            this.serverToClientStartIndex=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
        }

        #region IFlowAttribute Members
        public string AttributeName {
            get { return "Byte Frequency with Packet Direction"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {

            for(int i=packetStartIndex; i<packetStartIndex+packetLength && i<frameData.Length; i++) {
                if(packetDirection==ClassificationAttributeFingerprint.PacketDirection.ClientToServer)
                    yield return (int)frameData[i]%directionLength+clientToServerStartIndex;
                else if(packetDirection==ClassificationAttributeFingerprint.PacketDirection.ServerToClient)
                    yield return (int)frameData[i]%directionLength+serverToClientStartIndex;
            }
        }

        #endregion
    }
}
