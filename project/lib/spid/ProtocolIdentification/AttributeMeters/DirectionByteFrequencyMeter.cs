//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class DirectionByteFrequencyMeter : IAttributeMeter{

        private int directionLength;
        private int clientToServerStartIndex;
        private int serverToClientStartIndex;
        private const int maxBytesToParse=100;

        public DirectionByteFrequencyMeter() {
            this.directionLength=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.clientToServerStartIndex=0;
            this.serverToClientStartIndex=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
        }

        #region IFlowAttribute Members
        public string AttributeName {
            get { return "DirectionByteFrequencyMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetOrderNumberInSession<8) {
                for(int i=packetStartIndex; i<packetStartIndex+packetLength && i<frameData.Length && i<packetStartIndex+maxBytesToParse; i++) {
                    if(packetDirection==AttributeFingerprintHandler.PacketDirection.ClientToServer)
                        yield return (int)frameData[i]%directionLength+clientToServerStartIndex;
                    else if(packetDirection==AttributeFingerprintHandler.PacketDirection.ServerToClient)
                        yield return (int)frameData[i]%directionLength+serverToClientStartIndex;
                }
            }
        }

        #endregion
    }
}
