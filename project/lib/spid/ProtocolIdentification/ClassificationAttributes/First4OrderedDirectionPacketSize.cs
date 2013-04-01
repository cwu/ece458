using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First4OrderedDirectionPacketSize : IClassificationAttribute{

        private int largestExpectedPacketSize;
        private SortedList<ClassificationAttributeFingerprint.PacketDirection, int> directionOffset;
        private int packetOrderIncrement;
        private double packetSizeBinExponent;

        public First4OrderedDirectionPacketSize() {
            this.largestExpectedPacketSize=1500;
            this.directionOffset=new SortedList<ClassificationAttributeFingerprint.PacketDirection, int>(2);
            this.directionOffset[ClassificationAttributeFingerprint.PacketDirection.ClientToServer]=0;
            this.directionOffset[ClassificationAttributeFingerprint.PacketDirection.ServerToClient]=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.packetOrderIncrement=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/8;

            this.packetSizeBinExponent=Math.Log(packetOrderIncrement)/Math.Log(this.largestExpectedPacketSize);
        }

        private int GetBinOffsetNumber(int packetLength) {
            return Math.Min(packetOrderIncrement-1, (int)Math.Pow(packetLength, packetSizeBinExponent));
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First4OrderedDirectionPacketSize"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4 && packetDirection!=ClassificationAttributeFingerprint.PacketDirection.Unknown) {
                yield return this.directionOffset[packetDirection]+packetOrderNumberInSession*packetOrderIncrement+GetBinOffsetNumber(packetLength);
            }
        }

        #endregion
    }
}
