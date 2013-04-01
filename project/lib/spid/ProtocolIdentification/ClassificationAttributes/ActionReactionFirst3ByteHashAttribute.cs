using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification.ClassificationAttributes {

    //Inspired by: "Automatic Handling of Protocol Dependencies
    //and Reaction to 0-Day Attacks with ScriptGen Based Honeypots"
    //
    //Corrado Leita, Marc Dacier, and Frederic Massicotte
    //http://www.eurecom.fr/util/publidownload.fr.htm?id=2023

    class ActionReactionFirst3ByteHashAttribute : IClassificationAttribute {

        private const int MAX_STATE_TRANSITIONS_TO_USE=5;

        private byte[] serverToClientFirstByteTriad;
        private byte[] clientToServerFirstByteTriad;
        private bool waitingForPacketFromClient;
        private bool waitingForPacketFromServer;
        private int stateTransitionCount;

        public ActionReactionFirst3ByteHashAttribute() {
            this.serverToClientFirstByteTriad=new byte[0];
            this.clientToServerFirstByteTriad=new byte[0];
            this.waitingForPacketFromClient=true;
            this.waitingForPacketFromServer=true;
            this.stateTransitionCount=0;
        }

        #region IClassificationAttribute Members

        public string AttributeName {
            get { return "ActionReactionFirst3ByteHashAttribute"; }
        }

        public IEnumerable<int> GetFingerprintIndices(byte[] frameData, int packetStartIndex, int packetLength, DateTime packetTimestamp, ClassificationAttributeFingerprint.PacketDirection packetDirection, int packetOrderNumberInSession) {
            if(this.stateTransitionCount<MAX_STATE_TRANSITIONS_TO_USE){
                if(packetDirection == ClassificationAttributeFingerprint.PacketDirection.ClientToServer && waitingForPacketFromClient) {
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
                else if(packetDirection == ClassificationAttributeFingerprint.PacketDirection.ServerToClient && waitingForPacketFromServer) {
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

        private int GetActionReactionPairHash(byte[] action, byte[] reaction, string mutationPassword) {
            byte[] salt=new byte[8];
            Array.Copy(action, salt, action.Length);
            System.Security.Cryptography.Rfc2898DeriveBytes actionMutator=new System.Security.Cryptography.Rfc2898DeriveBytes(mutationPassword, salt);
            byte[] mutatedAction=actionMutator.GetBytes(3);
            
            Array.Copy(reaction, salt, reaction.Length);
            System.Security.Cryptography.Rfc2898DeriveBytes reactionMutator=new System.Security.Cryptography.Rfc2898DeriveBytes(mutationPassword, salt);
            byte[] mutatedReaction=actionMutator.GetBytes(3);
            
            /*
            for(int i=0; i<mutatedAction.Length; i++) {
                //mutatedAction[i]^=mutation;
                //I must do a non-linear mutation
                System.Security.Cryptography.HMAC
            }
            byte[] mutatedReaction=(byte[])reaction.Clone();
            for(int i=0; i<mutatedReaction.Length; i++) {
                //mutatedReaction[i]^=mutation;

            }
             * */
            return GetActionReactionPairHash(mutatedAction, mutatedReaction);
        }
        private int GetActionReactionPairHash(byte[] action, byte[] reaction) {
            byte[] actionReactionData=new byte[action.Length+reaction.Length];
            Array.Copy(action, 0, actionReactionData, 0, action.Length);
            Array.Copy(reaction, 0, actionReactionData, action.Length, reaction.Length);
            return ConvertHelper.ToHashValue(actionReactionData, ClassificationAttributeFingerprint.Fingerprint.FINGERPRINT_BITS);
        }
    }
}
