using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System;
using QRCoder;

namespace LathBotFront._2FA
{
    public static class GoogleAuthenticator
    {
        const int IntervalLength = 30;
        const int PinLength = 6;
        static readonly int PinModulo = (int)Math.Pow(10, PinLength);
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///   Number of intervals that have elapsed.
        /// </summary>
        static long CurrentInterval
            => ((long)Math.Floor((DateTime.UtcNow - UnixEpoch).TotalSeconds)) / IntervalLength;

        /// <summary>
        ///   Generates a QR code bitmap for provisioning.
        /// </summary>
        public static byte[] GenerateProvisioningImage(string identifier, byte[] key, int pixelsPerModule = 7)
        {
            var KeyString = Encoder.Base32Encode(key);
            var ProvisionUrl = string.Format("otpauth://totp/{0}?secret={1}&issuer=Example", identifier, KeyString);

            return new PngByteQRCode(new QRCodeGenerator().CreateQrCode(ProvisionUrl, QRCodeGenerator.ECCLevel.Q)).GetGraphic(pixelsPerModule);
        }

        /// <summary>
        ///   Generates a pin for the given key.
        /// </summary>
        public static string GeneratePin(byte[] key)
            => GeneratePin(key, CurrentInterval);

        /// <summary>
        ///   Generates a pin by hashing a key and counter.
        /// </summary>
        static string GeneratePin(byte[] key, long counter)
        {
            const int SizeOfInt32 = 4;

            var CounterBytes = BitConverter.GetBytes(counter);

            if (BitConverter.IsLittleEndian)
            {
                //spec requires bytes in big-endian order
                Array.Reverse(CounterBytes);
            }

            var Hash = new HMACSHA1(key).ComputeHash(CounterBytes);
            var Offset = Hash[^1] & 0xF;

            var SelectedBytes = new byte[SizeOfInt32];
            Buffer.BlockCopy(Hash, Offset, SelectedBytes, 0, SizeOfInt32);

            if (BitConverter.IsLittleEndian)
            {
                //spec interprets bytes in big-endian order
                Array.Reverse(SelectedBytes);
            }

            var SelectedInteger = BitConverter.ToInt32(SelectedBytes, 0);

            //remove the most significant bit for interoperability per spec
            var TruncatedHash = SelectedInteger & 0x7FFFFFFF;

            //generate number of digits for given pin length
            var Pin = TruncatedHash % PinModulo;

            return Pin.ToString(CultureInfo.InvariantCulture).PadLeft(PinLength, '0');
        }

        #region Nested type: Encoder

        static class Encoder
        {
            /// <summary>
            ///   Url Encoding (with upper-case hexadecimal per OATH specification)
            /// </summary>
            public static string UrlEncode(string value)
            {
                const string UrlEncodeAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

                var Builder = new StringBuilder();

                for (var i = 0; i < value.Length; i++)
                {
                    var Symbol = value[i];

                    if (UrlEncodeAlphabet.IndexOf(Symbol) != -1)
                    {
                        Builder.Append(Symbol);
                    }
                    else
                    {
                        Builder.Append('%');
                        Builder.Append(((int)Symbol).ToString("X2"));
                    }
                }

                return Builder.ToString();
            }

            /// <summary>
            ///   Base-32 Encoding
            /// </summary>
            public static string Base32Encode(byte[] data)
            {
                const int InByteSize = 8;
                const int OutByteSize = 5;
                const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

                int i = 0, index = 0;
                var Builder = new StringBuilder((data.Length + 7) * InByteSize / OutByteSize);

                while (i < data.Length)
                {
                    int CurrentByte = data[i];
                    int Digit;

                    //Is the current digit going to span a byte boundary?
                    if (index > (InByteSize - OutByteSize))
                    {
                        int NextByte;

                        if ((i + 1) < data.Length)
                        {
                            NextByte = data[i + 1];
                        }
                        else
                        {
                            NextByte = 0;
                        }

                        Digit = CurrentByte & (0xFF >> index);
                        index = (index + OutByteSize) % InByteSize;
                        Digit <<= index;
                        Digit |= NextByte >> (InByteSize - index);
                        i++;
                    }
                    else
                    {
                        Digit = (CurrentByte >> (InByteSize - (index + OutByteSize))) & 0x1F;
                        index = (index + OutByteSize) % InByteSize;

                        if (index == 0)
                        {
                            i++;
                        }
                    }

                    Builder.Append(Base32Alphabet[Digit]);
                }

                return Builder.ToString();
            }
        }

        #endregion
    }
}
