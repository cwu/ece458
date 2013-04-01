using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {
    class FirstPacketPerDirectionFirstNByteNibbles : IClassificationAttribute {

        private int nBytesToParsePerPacket;
        private int clientToServerOffset;
        private int serverToClientOffset;
        private bool clientToServerPacketReceived;
        private bool serverToClientPacketReceived;


        public FirstPacketPerDirectionFirstNByteNibbles() {
            this.clientToServerOffset=0;
            this.serverToClientOffset=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/2;
            this.nBytesToParsePerPacket=ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH/(2*16);
            this.clientToServerPacketReceived=false;
            this.serverToClientPacketReceived=false;
        }
        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "First Packet Per Direction (C->S or S->C) First "+nBytesToParsePerPacket+" Byte-Nibbles"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            //C->S
            //Only do work if it is first packet for the direction in the session
            if(packetDirection==ClassificationAttributeFingerprint.PacketDirection.ClientToServer && !clientToServerPacketReceived){
                this.clientToServerPacketReceived=true;
                for(int i=0; i<Math.Min(nBytesToParsePerPacket, packetLength) && packetStartIndex+i<frameData.Length; i++) {
                    yield return (clientToServerOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]))%ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH;
                }
            }
            //S->C
            else if(packetDirection==ClassificationAttributeFingerprint.PacketDirection.ServerToClient && !serverToClientPacketReceived){
                this.serverToClientPacketReceived=true;
                for(int i=0; i<Math.Min(nBytesToParsePerPacket, packetLength) && packetStartIndex+i<frameData.Length; i++) {
                    yield return (serverToClientOffset+i*16+ConvertHelper.ToByteNibble(frameData[packetStartIndex+i]))%ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_LENGTH;
                }
            }
        }

        #endregion
    }
}
