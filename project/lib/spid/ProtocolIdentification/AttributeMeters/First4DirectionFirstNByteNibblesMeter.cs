//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First4DirectionFirstNByteNibblesMeter : IAttributeMeter {
        private int nByteNibbles;
        private int serverToClientOffset;
        private int clientToServerOffset;

        public First4DirectionFirstNByteNibblesMeter() {
            this.serverToClientOffset=0;
            this.clientToServerOffset=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nByteNibbles=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/(2*16);
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First4DirectionFirstNByteNibblesMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                for(int i=0; i<nByteNibbles && i<packetLength &&  packetStartIndex+i<frameData.Length; i++) {
                    if(packetDirection== AttributeFingerprintHandler.PacketDirection.ClientToServer)
                        yield return this.clientToServerOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]);
                    else if(packetDirection== AttributeFingerprintHandler.PacketDirection.ServerToClient)
                        yield return this.serverToClientOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]);
                }
            }
        }

        #endregion
    }
}
