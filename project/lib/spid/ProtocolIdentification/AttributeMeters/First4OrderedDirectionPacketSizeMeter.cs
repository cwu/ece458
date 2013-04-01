//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First4OrderedDirectionPacketSizeMeter : IAttributeMeter{

        private int largestExpectedPacketSize;
        private SortedList<AttributeFingerprintHandler.PacketDirection, int> directionOffset;
        private int packetOrderIncrement;
        private double packetSizeBinExponent;

        public First4OrderedDirectionPacketSizeMeter() {
            this.largestExpectedPacketSize=1500;
            this.directionOffset=new SortedList<AttributeFingerprintHandler.PacketDirection, int>(2);
            this.directionOffset[AttributeFingerprintHandler.PacketDirection.ClientToServer]=0;
            this.directionOffset[AttributeFingerprintHandler.PacketDirection.ServerToClient]=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.packetOrderIncrement=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/8;

            this.packetSizeBinExponent=Math.Log(packetOrderIncrement)/Math.Log(this.largestExpectedPacketSize);
        }

        private int GetBinOffsetNumber(int packetLength) {
            return Math.Min(packetOrderIncrement-1, (int)Math.Pow(packetLength, packetSizeBinExponent));
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First4OrderedDirectionPacketSizeMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4 && packetDirection!=AttributeFingerprintHandler.PacketDirection.Unknown) {
                yield return this.directionOffset[packetDirection]+packetOrderNumberInSession*packetOrderIncrement+GetBinOffsetNumber(packetLength);
            }
        }

        #endregion
    }
}
