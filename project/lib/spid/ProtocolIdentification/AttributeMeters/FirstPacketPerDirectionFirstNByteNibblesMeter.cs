//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class FirstPacketPerDirectionFirstNByteNibblesMeter : IAttributeMeter {

        private int nBytesToParsePerPacket;
        private int clientToServerOffset;
        private int serverToClientOffset;
        private bool clientToServerPacketReceived;
        private bool serverToClientPacketReceived;


        public FirstPacketPerDirectionFirstNByteNibblesMeter() {
            this.clientToServerOffset=0;
            this.serverToClientOffset=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nBytesToParsePerPacket=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/(2*16);
            this.clientToServerPacketReceived=false;
            this.serverToClientPacketReceived=false;
        }
        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "FirstPacketPerDirectionFirstNByteNibblesMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //C->S
            if(packetDirection==AttributeFingerprintHandler.PacketDirection.ClientToServer && !clientToServerPacketReceived){
                this.clientToServerPacketReceived=true;
                for(int i=0; i<Math.Min(nBytesToParsePerPacket, packetLength) && packetStartIndex+i<frameData.Length; i++) {
                    yield return (clientToServerOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]))%AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH;
                }
            }
            //S->C
            else if(packetDirection==AttributeFingerprintHandler.PacketDirection.ServerToClient && !serverToClientPacketReceived){
                this.serverToClientPacketReceived=true;
                for(int i=0; i<Math.Min(nBytesToParsePerPacket, packetLength) && packetStartIndex+i<frameData.Length; i++) {
                    yield return (serverToClientOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]))%AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH;
                }
            }
        }

        #endregion
    }
}
