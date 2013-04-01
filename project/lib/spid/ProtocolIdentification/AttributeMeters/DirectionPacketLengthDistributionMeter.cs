//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class DirectionPacketLengthDistributionMeter : IAttributeMeter{

        private int expectedMaxPacketLength;
        //private int highestBinNumber;
        private int clientToServerStartIndex;
        private int serverToClientStartIndex;
        private double exponent;

        public DirectionPacketLengthDistributionMeter() {
            this.expectedMaxPacketLength=1500;
            this.clientToServerStartIndex=0;
            this.serverToClientStartIndex=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            this.exponent=Math.Log(serverToClientStartIndex)/Math.Log(expectedMaxPacketLength);
        }

        private int GetPacketBinNumber(int packetLength) {
            //Annie De Montigny-Leboeuf is displaying a way to use bins for packet sizes in "Flow Attributes For Use In Traffic Characterization"
            //See page 10 of http://www.crc.ca/files/crc/home/research/network/system_apps/network_systems/network_security/publications/ADeMontigny_CRCTN2005003.pdf
            //I am however doing this in a more automatic manner by using a small (<1) exponent
            return Math.Min(serverToClientStartIndex-1, (int)Math.Pow(packetLength, exponent));
        }


        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "DirectionPacketLengthDistributionMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetDirection==AttributeFingerprintHandler.PacketDirection.ClientToServer)
                yield return this.clientToServerStartIndex+GetPacketBinNumber(packetLength);
            else if(packetDirection==AttributeFingerprintHandler.PacketDirection.ServerToClient)
                yield return this.serverToClientStartIndex+GetPacketBinNumber(packetLength);
        }

        #endregion
    }
}
