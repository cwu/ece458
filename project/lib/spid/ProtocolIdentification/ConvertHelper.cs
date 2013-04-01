//  Statistical Protocol IDentification (SPID) Algorithm Proof-of-Concept
//  Copyright: Erik Hjelmvik <hjelmvik@users.sourceforge.net>
//
//  http://sourceforge.net/projects/spid
//  http://www.iis.se/docs/The_SPID_Algorithm_-_Statistical_Protocol_IDentification.pdf

using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolIdentification {
    class ConvertHelper {
        public static byte ToByteNibble(byte byteValue) {
            return (byte)((byteValue^(byteValue>>4))&0x0f);//first nibble XOR:ed with last nibble
        }

        public static byte ToByteNibblePair(byte firstByte, byte secondByte) {
            return (byte)((ToByteNibble(firstByte)<<4)^ToByteNibble(secondByte));
        }

        public static byte GetNibble(byte byteValue, bool mostSignificantNibble) {
            if(mostSignificantNibble)
                return (byte)(byteValue>>4);
            else
                return (byte)(byteValue&0x0f);
        }

        public static int GetMaskedValue(int data, int nBitsFromStartToKeep, int nBitsFromEndToKeep) {
            int mask=0;
            int startMask=0;
            int endMask=0;
            for(int i=0; i<nBitsFromStartToKeep; i++) {
                startMask|=1<<(31-i);
            }
            for(int i=0; i<nBitsFromEndToKeep; i++) {
                endMask|=1<<i;
            }
            mask=startMask|endMask;
            return data&mask;
        }

        public static int ToHashValue(byte[] data, int hashBitCount) {
            int[] intData=new int[data.Length];
            for(int i=0; i<data.Length; i++)
                intData[i]=(int)data[i];
            return ToHashValue(intData, hashBitCount);
        }

        public static int ToHashValue(int[] data, int hashBitCount) {
            int mergedData=0;
            for(int i=0; i<data.Length; i++) {
                int shiftOffset=(i*3)%32;//3 and 32 have 1 as greatest common divisor (GCD)
                mergedData^=(data[i]<<shiftOffset)^(GetMaskedValue(data[i], shiftOffset, 0)>>(32-i));
            }
            return ToHashValue(mergedData, hashBitCount);
        }

        public static int ToHashValue(int data, int hashBitCount) {
            return ToHashValue(data, hashBitCount, true);
        }
        public static int ToHashValue(int data, int hashBitCount, bool usePrimeModulo) {
            int hash;
            int moduloValue=1<<hashBitCount;

            if(usePrimeModulo) {
                //this is probably the neatest way to do this...
                hash=data%GetLargestPrimeValue(hashBitCount);
            }
            else {    
                hash=data;
                for(int i=1; data>>(i*hashBitCount) > 0 && i*hashBitCount<32; i++)
                    hash^=data>>(i*hashBitCount);
                hash%=moduloValue;
            } 
            while(hash<0)
                hash+=moduloValue;
            return hash;
        }

        private static int GetLargestPrimeValue(int maxBitsIntPrimeNumber) {
            if(maxBitsIntPrimeNumber==1)
                return 1;//not really a prime though...
            else if(maxBitsIntPrimeNumber==2)
                return 3;
            else if(maxBitsIntPrimeNumber==3)
                return 7;
            else if(maxBitsIntPrimeNumber==4)
                return 13;
            else if(maxBitsIntPrimeNumber==5)
                return 31;
            else if(maxBitsIntPrimeNumber==6)
                return 61;
            else if(maxBitsIntPrimeNumber==7)
                return 127;
            else if(maxBitsIntPrimeNumber==8)
                return 251;
            else if(maxBitsIntPrimeNumber==9)
                return 509;
            else if(maxBitsIntPrimeNumber==10)
                return 1021;
            else if(maxBitsIntPrimeNumber==11)
                return 2039;
            else if(maxBitsIntPrimeNumber==12)
                return 4093;
            else if(maxBitsIntPrimeNumber==13)
                return 8191;
            else if(maxBitsIntPrimeNumber==14)
                return 16381;
            else if(maxBitsIntPrimeNumber==15)
                return 32749;
            else if(maxBitsIntPrimeNumber==16)
                return 65521;
            else
                throw new Exception("This algorithm does not hold a precalculated prime number of "+maxBitsIntPrimeNumber+" bits");
        }
    }
}
