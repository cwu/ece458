//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First4OrderedDirectionFirstNByteNibblesMeter : IAttributeMeter{
        int clientToServerOffset;
        int serverToClientOffset;
        int packetOrderOffsetIncrement;
        int nBytesToParsePerPacket;


        public First4OrderedDirectionFirstNByteNibblesMeter() {
            this.clientToServerOffset=0;
            this.serverToClientOffset=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.packetOrderOffsetIncrement=serverToClientOffset/4;
            this.nBytesToParsePerPacket=packetOrderOffsetIncrement/16;
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First4OrderedDirectionFirstNByteNibblesMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<4) {
                int directionOffset;
                if(packetDirection== AttributeFingerprintHandler.PacketDirection.ClientToServer)
                    directionOffset=clientToServerOffset;
                else if(packetDirection== AttributeFingerprintHandler.PacketDirection.ServerToClient)
                    directionOffset=serverToClientOffset;
                else
                    yield break;
                for(int i=0; i<packetLength && i<nBytesToParsePerPacket && packetStartIndex+i<frameData.Length; i++) {

                    yield return directionOffset+packetOrderOffsetIncrement*packetOrderNumberInSession+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]);
                }
            }
        }

        #endregion
    }
}
