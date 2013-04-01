//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First2OrderedPacketsFirstNByteNibblesMeter : IAttributeMeter{
        //private int[] packetOrderOffset;
        private int nBytesToParsePerPacket;
        private int packetOrderOffsetIncrement;

        public First2OrderedPacketsFirstNByteNibblesMeter() {
            this.packetOrderOffsetIncrement=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nBytesToParsePerPacket=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/(2*16);
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First2OrderedPacketsFirstNByteNibblesMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<2)//Only do work if it is first (0) or second (1) packet in the session
                for(int i=0; i<Math.Min(nBytesToParsePerPacket, packetLength) && packetStartIndex+i<frameData.Length; i++) {
                    yield return (packetOrderNumberInSession*this.packetOrderOffsetIncrement+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]))%AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH;
                }
        }

        #endregion
    }
}
