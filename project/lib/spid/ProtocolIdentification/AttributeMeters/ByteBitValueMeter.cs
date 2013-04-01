//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf
//
// This attributeMeter is an addition made after the release of
// SPID proof-of-concept v.0.1
//
// This attributeMeter is created in order to better identify encrypted protocols,
// such as the Message Stream Encryption (MSE) used by BitTorrent. The attributeMeter
// is however deviced to provide use also for other non-encrypted protocols.

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class ByteBitValueMeter : IAttributeMeter {
        private int nBytesToParsePerPacket;
        private int nPacketsToParsePerSession;

        private int counterValueModulus;

        public ByteBitValueMeter(){
            //I will use the 32 first bytes of the 8 first packets
            this.nBytesToParsePerPacket=32;
            this.nPacketsToParsePerSession=8;

            //1 byte = direction
            //3 bytes = bit position in the byte
            //x bytes = counter value of the number of zeroes
            this.counterValueModulus=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/16;
        }

        #region IAttributeMeter Members

        public string AttributeName {
            get { return "ByteBitValueMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {

            if(packetOrderNumberInSession<nPacketsToParsePerSession) {

                int[] zeroCounters=new int[8];//good thing all these ints will be initialized to zero
                //int[] oneCounters=new int[8];

                byte[] packetFirstBytes=new byte[Math.Min(Math.Min(packetLength, nBytesToParsePerPacket), frameData.Length-packetStartIndex)];
                if(packetFirstBytes.Length==0)
                    yield break;
                Array.Copy(frameData, packetStartIndex, packetFirstBytes, 0, packetFirstBytes.Length);
                System.Collections.BitArray bitArray=new System.Collections.BitArray(packetFirstBytes);

                for(int i=0; i<bitArray.Length; i++) {
                    if(!bitArray[i])
                        zeroCounters[i%8]++;
                }

                int packetDirectionOffset;
                if(packetDirection==AttributeFingerprintHandler.PacketDirection.ClientToServer)
                    packetDirectionOffset=0;
                else if(packetDirection==AttributeFingerprintHandler.PacketDirection.ServerToClient)
                    packetDirectionOffset=8;
                else
                    yield break;

                for(int i=0; i<8; i++) {
                    yield return this.counterValueModulus*(packetDirectionOffset+i)+((zeroCounters[i]*(this.counterValueModulus-1))/packetFirstBytes.Length);
                }
            }
        }

        #endregion
    }
}
