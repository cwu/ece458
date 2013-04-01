//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class ByteValueOffsetHashOfFirst32BytesInFirst4PacketsMeter : IAttributeMeter{
        private int hashSizeInBits;

        public ByteValueOffsetHashOfFirst32BytesInFirst4PacketsMeter() {
            this.hashSizeInBits=(int)Math.Log(AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH, 2.0);
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "ByteValueOffsetHashOfFirst32BytesInFirst4PacketsMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                for(int i=0; i<Math.Min(packetLength, 32) && packetStartIndex+i<frameData.Length; i++) {
                    yield return ConvertHelper.ToHashValue(frameData[packetStartIndex+i]+256*i, hashSizeInBits);
                }
            }
        }

        #endregion
    }
}
