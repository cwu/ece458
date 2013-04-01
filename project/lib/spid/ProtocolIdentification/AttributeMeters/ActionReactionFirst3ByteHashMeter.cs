//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.AttributeMeters {

    //Inspired by: "Automatic Handling of Protocol Dependencies
    //and Reaction to 0-Day Attacks with ScriptGen Based Honeypots"
    //
    //Corrado Leita, Marc Dacier, and Frederic Massicotte
    //http://www.eurecom.fr/util/publidownload.fr.htm?id=2023

    class ActionReactionFirst3ByteHashMeter : IAttributeMeter {

        private const int MAX_STATE_TRANSITIONS_TO_USE=5;

        private byte[] serverToClientFirstByteTriad;
        private byte[] clientToServerFirstByteTriad;
        private bool waitingForPacketFromClient;
        private bool waitingForPacketFromServer;
        private int stateTransitionCount;

        public ActionReactionFirst3ByteHashMeter() {
            this.serverToClientFirstByteTriad=new byte[0];
            this.clientToServerFirstByteTriad=new byte[0];
            this.waitingForPacketFromClient=true;
            this.waitingForPacketFromServer=true;
            this.stateTransitionCount=0;
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "ActionReactionFirst3ByteHashMeter"; }
        }

        public IEnumerable<int> GetMeasurements(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, AttributeFingerprintHandler.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(this.stateTransitionCount<MAX_STATE_TRANSITIONS_TO_USE){
                if(packetDirection == AttributeFingerprintHandler.PacketDirection.ClientToServer && waitingForPacketFromClient) {
                    int maxDataSize=Math.Min(frameData.Length-packetStartIndex, packetLength);
                    clientToServerFirstByteTriad=new byte[Math.Min(3, maxDataSize)];
                    //now fill the c->s-triad with data
                    Array.Copy(frameData, packetStartIndex, clientToServerFirstByteTriad, 0, clientToServerFirstByteTriad.Length);
                    waitingForPacketFromClient=false;
                    waitingForPacketFromServer=true;
                    this.stateTransitionCount++;

                    //now make a comparison!
                    yield return GetActionReactionPairHash(serverToClientFirstByteTriad, clientToServerFirstByteTriad);
                    //and two extra to provide some larger footprint
                    yield return GetActionReactionPairHash(serverToClientFirstByteTriad, clientToServerFirstByteTriad, "foo");
                    yield return GetActionReactionPairHash(serverToClientFirstByteTriad, clientToServerFirstByteTriad, "bar");
                }
                else if(packetDirection == AttributeFingerprintHandler.PacketDirection.ServerToClient && waitingForPacketFromServer) {
                    int maxDataSize=Math.Min(frameData.Length-packetStartIndex, packetLength);
                    serverToClientFirstByteTriad=new byte[Math.Min(3, maxDataSize)];
                    //now fill the s->c-triad with data
                    Array.Copy(frameData, packetStartIndex, serverToClientFirstByteTriad, 0, serverToClientFirstByteTriad.Length);
                    waitingForPacketFromServer=false;
                    waitingForPacketFromClient=true;
                    this.stateTransitionCount++;

                    //now make a comparison!
                    yield return GetActionReactionPairHash(clientToServerFirstByteTriad, serverToClientFirstByteTriad);
                    //and two extra to provide some larger footprint
                    yield return GetActionReactionPairHash(clientToServerFirstByteTriad, serverToClientFirstByteTriad, "something");
                    yield return GetActionReactionPairHash(clientToServerFirstByteTriad, serverToClientFirstByteTriad, "else");
                }
                else
                    yield break;
            }
            else
                yield break;
        }


        #endregion

        //this one is to slow to be used!
        /*
        private int GetActionReactionPairHash(byte[] action, byte[] reaction, string mutationPassword) {
            byte[] salt=new byte[8];
            Array.Copy(action, salt, action.Length);
            System.Security.Cryptography.Rfc2898DeriveBytes actionMutator=new System.Security.Cryptography.Rfc2898DeriveBytes(mutationPassword, salt);
            byte[] mutatedAction=actionMutator.GetBytes(3);
            
            Array.Copy(reaction, salt, reaction.Length);
            System.Security.Cryptography.Rfc2898DeriveBytes reactionMutator=new System.Security.Cryptography.Rfc2898DeriveBytes(mutationPassword, salt);
            byte[] mutatedReaction=actionMutator.GetBytes(3);
            
            return GetActionReactionPairHash(mutatedAction, mutatedReaction);
        }
         * */
        private int GetActionReactionPairHash(byte[] action, byte[] reaction, string mutationPassword) {
            int m=mutationPassword.GetHashCode();
            for(int i=0; i<action.Length; i++)
                action[i]=(byte)(action[i]+m+i);
            for(int i=0; i<reaction.Length; i++)
                reaction[i]=(byte)(reaction[i]-m-i);
            return GetActionReactionPairHash(action, reaction);
        }
        private int GetActionReactionPairHash(byte[] action, byte[] reaction) {
            byte[] actionReactionData=new byte[action.Length+reaction.Length];
            Array.Copy(action, 0, actionReactionData, 0, action.Length);
            Array.Copy(reaction, 0, actionReactionData, action.Length, reaction.Length);
            return ConvertHelper.ToHashValue(actionReactionData, AttributeFingerprintHandler.Fingerprint.FINGERPRINT_BITS);
        }
    }
}
