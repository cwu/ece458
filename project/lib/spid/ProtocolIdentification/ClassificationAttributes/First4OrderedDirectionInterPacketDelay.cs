using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class First4OrderedDirectionInterPacketDelay : IClassificationAttribute{

        private DateTime lastPacketTimestamp;
        private int smallestMicroSecondTimeValue;
        private int largestMicroSecondTimeValue;
        private SortedList<ClassificationAttributeFingerprint.PacketDirection, int> directionOffset;
        private int packetOrderIncrement;
        private double delayBinExponent;


        public First4OrderedDirectionInterPacketDelay() {
            this.lastPacketTimestamp=DateTime.MinValue;
            this.smallestMicroSecondTimeValue=16;
            this.largestMicroSecondTimeValue=60000000;//1 minute

            this.directionOffset=new SortedList<ClassificationAttributeFingerprint.PacketDirection, int>(3);
            this.directionOffset[ClassificationAttributeFingerprint.PacketDirection.ClientToServer]=0;
            this.directionOffset[ClassificationAttributeFingerprint.PacketDirection.ServerToClient]=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.packetOrderIncrement=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/8;

            this.delayBinExponent=Math.Log(packetOrderIncrement)/Math.Log(this.largestMicroSecondTimeValue);
        }

        private int GetBinOffsetNumber(TimeSpan interPacketDelay) {
            return Math.Min(packetOrderIncrement-1, (int)Math.Pow(Math.Max(0.0, 10.0*interPacketDelay.Ticks-smallestMicroSecondTimeValue), delayBinExponent));
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "Inter-packet-delay of first 4 packets with order-number and direction"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            TimeSpan interPacketDelay=packetTimestamp.Subtract(lastPacketTimestamp);
            lastPacketTimestamp=packetTimestamp;

            if(packetOrderNumberInSession<4 && packetDirection!=ClassificationAttributeFingerprint.PacketDirection.Unknown)
                yield return directionOffset[packetDirection]+packetOrderNumberInSession*packetOrderIncrement+GetBinOffsetNumber(interPacketDelay);

        }

        #endregion
    }
}
