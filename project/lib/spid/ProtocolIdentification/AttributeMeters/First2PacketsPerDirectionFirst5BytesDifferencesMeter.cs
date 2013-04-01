//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {
    class First2PacketsPerDirectionFirst5BytesDifferencesMeter : IAttributeMeter {
        #region IClassificationAttribute Members

        private bool firstPacketFromServerReceived;
        private bool firstPacketFromClientReceived;
        private bool secondPacketFromServerReceived;
        private bool secondPacketFromClientReceived;

        byte[] firstFiveBytesFromServer;
        byte[] firstFiveBytesFromClient;

        public string AttributeName {
            get { return "First2PacketsPerDirectionFirst5BytesDifferencesMeter"; }
        }

        public First2PacketsPerDirectionFirst5BytesDifferencesMeter() {
            this.firstPacketFromClientReceived=false;
            this.firstPacketFromServerReceived=false;
            this.secondPacketFromClientReceived=false;
            this.secondPacketFromServerReceived=false;
            this.firstFiveBytesFromServer=new byte[5];
            this.firstFiveBytesFromClient=new byte[5];
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(packetDirection== AttributeFingerprintHandler.PacketDirection.ClientToServer) {
                if(!firstPacketFromClientReceived) {
                    firstPacketFromClientReceived=true;
                    Array.Copy(frameData, packetStartIndex, firstFiveBytesFromClient, 0, Math.Min(5, frameData.Length-packetStartIndex));
                }
                else if(!secondPacketFromClientReceived) {
                    secondPacketFromClientReceived=true;
                    for(int i=0; i<5 && frameData.Length>packetStartIndex+i; i++)
                        yield return CalculateDistance(firstFiveBytesFromClient[i], frameData[packetStartIndex+i]);
                }
            }
            else if(packetDirection== AttributeFingerprintHandler.PacketDirection.ServerToClient) {
                if(!firstPacketFromServerReceived) {
                    firstPacketFromServerReceived=true;
                    Array.Copy(frameData, packetStartIndex, firstFiveBytesFromServer, 0, Math.Min(5, frameData.Length-packetStartIndex));
                }
                else if(!secondPacketFromServerReceived) {
                    secondPacketFromServerReceived=true;
                    for(int i=0; i<5 && frameData.Length>packetStartIndex+i; i++)
                        yield return AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2+CalculateDistance(firstFiveBytesFromServer[i], frameData[packetStartIndex+i]);
                }
                  
            }
            
        }

        private int CalculateDistance(byte b1, byte b2){
            int x;
            if(b1>b2)
                x=b1-b2;
            else
                x=b2-b1;
            return Math.Min(x, (AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2)-1);
        }

        #endregion
    }
}
