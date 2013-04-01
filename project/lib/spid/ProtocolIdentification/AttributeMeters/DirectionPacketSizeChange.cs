using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {

    //The inspiration for this attribute meter comes from
    //Lin Y-D, et al. Application classification using packet size distribution and port association. J Network Comput Appl (2009), doi:10.1016/j.jnca.2009.03.001
    class DirectionPacketSizeChange : IAttributeMeter{
        private int previousClientToServerPacketSize;
        private int previousServerToClientPacketSize;

        #region IAttributeMeter Members

        string IAttributeMeter.AttributeName {
            get { return "DirectionPacketSizeChange"; }
        }

        IEnumerable<int> IAttributeMeter.GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            int diff=0;
            int directionOffset=0;

            if(packetDirection==AttributeFingerprintHandler.PacketDirection.ClientToServer) {
                //C->S
                diff=packetLength-previousClientToServerPacketSize;
                previousClientToServerPacketSize=packetLength;
            }
            else if(packetDirection==AttributeFingerprintHandler.PacketDirection.ServerToClient) {
                //S->C
                diff=packetLength-previousServerToClientPacketSize;
                previousServerToClientPacketSize=packetLength;
                //set the offset
                directionOffset=AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2;
            }
            if(diff<0)
                diff=-diff;
            diff=diff%(AttributeFingerprintHandler.Fingerprint.FINGERPRINT_LENGTH/2);
            yield return diff+directionOffset;

        }

        #endregion
    }
}
