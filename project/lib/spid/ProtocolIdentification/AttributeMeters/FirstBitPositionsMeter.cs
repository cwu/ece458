//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class FirstBitPositionsMeter : IAttributeMeter{
        private int nBytesToCheck;
        private int oneValueOffset;
        private int zeroValueOffset;

        public FirstBitPositionsMeter(){
            this.oneValueOffset=0;
            this.zeroValueOffset=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nBytesToCheck=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/16;//2*8
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "FirstBitPositionsMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<8) {
                byte[] packetBytes=new byte[Math.Min(Math.Min(packetLength, nBytesToCheck), frameData.Length-packetStartIndex)];
                Array.Copy(frameData, packetStartIndex, packetBytes, 0, packetBytes.Length);
                System.Collections.BitArray bitArray=new System.Collections.BitArray(packetBytes);
                for(int i=0; i<bitArray.Length; i++) {
                    if(bitArray[i])
                        yield return oneValueOffset+i;
                    else
                        yield return zeroValueOffset+i;
                }
            }
        }

        #endregion
    }
}
