//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First4OrderedDirectionInterPacketDelayMeter : IAttributeMeter{

        private DateTime lastPacketTimestamp;
        private int smallestMicroSecondTimeValue;
        private int largestMicroSecondTimeValue;
        private SortedList<AttributeFingerprintHandler.PacketDirection, int> directionOffset;
        private int packetOrderIncrement;
        private double delayBinExponent;


        public First4OrderedDirectionInterPacketDelayMeter() {
            this.lastPacketTimestamp=DateTime.MinValue;
            this.smallestMicroSecondTimeValue=16;
            this.largestMicroSecondTimeValue=60000000;//1 minute

            this.directionOffset=new SortedList<AttributeFingerprintHandler.PacketDirection, int>(3);
            this.directionOffset[AttributeFingerprintHandler.PacketDirection.ClientToServer]=0;
            this.directionOffset[AttributeFingerprintHandler.PacketDirection.ServerToClient]=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.packetOrderIncrement=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/8;

            this.delayBinExponent=Math.Log(packetOrderIncrement)/Math.Log(this.largestMicroSecondTimeValue);
        }

        private int GetBinOffsetNumber(TimeSpan interPacketDelay) {
            return Math.Min(packetOrderIncrement-1, (int)Math.Pow(Math.Max(0.0, 10.0*interPacketDelay.Ticks-smallestMicroSecondTimeValue), delayBinExponent));
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First4OrderedDirectionInterPacketDelayMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            TimeSpan interPacketDelay=packetTimestamp.Subtract(lastPacketTimestamp);
            lastPacketTimestamp=packetTimestamp;

            if(packetOrderNumberInSession<4 && packetDirection!=AttributeFingerprintHandler.PacketDirection.Unknown)
                yield return directionOffset[packetDirection]+packetOrderNumberInSession*packetOrderIncrement+GetBinOffsetNumber(interPacketDelay);

        }

        #endregion
    }
}
