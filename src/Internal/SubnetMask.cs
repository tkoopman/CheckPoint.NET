// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Net;

namespace Koopman.CheckPoint.Internal
{
    internal static class SubnetMask
    {
        #region Methods

        internal static IPAddress MaskLengthToSubnetMask(int maskLength)
        {
            if (maskLength < 0 || maskLength > 32)
                throw new ArgumentException("Valid values 0 - 32", nameof(maskLength));

            Byte[] binaryMask = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                if (i * 8 + 8 <= maskLength)
                    binaryMask[i] = (byte)255;
                else if (i * 8 > maskLength)
                    binaryMask[i] = (byte)0;
                else
                {
                    int oneLength = maskLength - i * 8;
                    string binaryDigit =
                        String.Empty.PadLeft(oneLength, '1').PadRight(8, '0');
                    binaryMask[i] = Convert.ToByte(binaryDigit, 2);
                }
            }
            return new IPAddress(binaryMask);
        }

        internal static int SubnetMaskToMaskLength(IPAddress subnetMask)
        {
            try
            {
                Byte[] ipbytes = subnetMask.GetAddressBytes();

                uint subnet = 16777216 * Convert.ToUInt32(ipbytes[0]) +
                    65536 * Convert.ToUInt32(ipbytes[1]) + 256 * Convert.ToUInt32(ipbytes[2]) + Convert.ToUInt32(ipbytes[3]);

                string test = Convert.ToString(subnet, 2);
                if (subnet == 0 || subnet >= 0x80000000)
                {
                    int count = test.IndexOf('0');

                    count = (count == -1) ? 32 : count;

                    bool valid = test.IndexOf('1', count) == -1;

                    if (valid) { return count; }
                }
            }
            catch { }

            throw new ArgumentException("Invalid subnet mask.", nameof(subnetMask));
        }

        #endregion Methods
    }
}