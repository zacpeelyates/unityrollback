using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
    /// <summary>
    /// Provides methods for copying and encoding Unicode text.
    /// </summary>
    [BurstCompatible]
    public static unsafe class UTF8ArrayUnsafeUtility
    {

        /// <summary>
        /// Copies a buffer of UCS-2 text. The copy is encoded as UTF-8.
        /// </summary>
        /// <remarks>Assumes the source data is valid UCS-2.</remarks>
        /// <param name="src">The source buffer for reading UCS-2.</param>
        /// <param name="srcLength">The number of chars to read from the source.</param>
        /// <param name="dest">The destination buffer for writing UTF-8.</param>
        /// <param name="destLength">Outputs the number of bytes written to the destination.</param>
        /// <param name="destUTF8MaxLengthInBytes">The max number of bytes that will be written to the destination buffer.</param>
        /// <returns><see cref="CopyError.None"/> if the copy fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Copy(byte *dest, out int destLength, int destUTF8MaxLengthInBytes, char *src, int srcLength)
        {
            var error = Unicode.Utf16ToUtf8(src, srcLength, dest, out destLength, destUTF8MaxLengthInBytes);
            if (error == ConversionError.None)
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Copies a buffer of UCS-2 text. The copy is encoded as UTF-8.
        /// </summary>
        /// <remarks>Assumes the source data is valid UCS-2.</remarks>
        /// <param name="src">The source buffer for reading UCS-2.</param>
        /// <param name="srcLength">The number of chars to read from the source.</param>
        /// <param name="dest">The destination buffer for writing UTF-8.</param>
        /// <param name="destLength">Outputs the number of bytes written to the destination.</param>
        /// <param name="destUTF8MaxLengthInBytes">The max number of bytes that will be written to the destination buffer.</param>
        /// <returns><see cref="CopyError.None"/> if the copy fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Copy(byte *dest, out ushort destLength, ushort destUTF8MaxLengthInBytes, char *src, int srcLength)
        {
            var error = Unicode.Utf16ToUtf8(src, srcLength, dest, out var temp, destUTF8MaxLengthInBytes);
            destLength = (ushort)temp;
            if (error == ConversionError.None)
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Copies a buffer of UCS-8 text.
        /// </summary>
        /// <remarks>Assumes the source data is valid UTF-8.</remarks>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcLength">The number of chars to read from the source.</param>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="destLength">Outputs the number of bytes written to the destination.</param>
        /// <param name="destUTF8MaxLengthInBytes">The max number of bytes that will be written to the destination buffer.</param>
        /// <returns><see cref="CopyError.None"/> if the copy fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Copy(byte *dest, out int destLength, int destUTF8MaxLengthInBytes, byte *src, int srcLength)
        {
            destLength = srcLength > destUTF8MaxLengthInBytes ? destUTF8MaxLengthInBytes : srcLength;
            UnsafeUtility.MemCpy(dest, src, destLength);
            return destLength == srcLength ? CopyError.None : CopyError.Truncation;
        }

        /// <summary>
        /// Copies a buffer of UCS-8 text.
        /// </summary>
        /// <remarks>Assumes the source data is valid UTF-8.</remarks>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcLength">The number of chars to read from the source.</param>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="destLength">Outputs the number of bytes written to the destination.</param>
        /// <param name="destUTF8MaxLengthInBytes">The max number of bytes that will be written to the destination buffer.</param>
        /// <returns><see cref="CopyError.None"/> if the copy fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Copy(byte *dest, out ushort destLength, ushort destUTF8MaxLengthInBytes, byte *src, ushort srcLength)
        {
            var error = Unicode.Utf8ToUtf8(src, srcLength, dest, out var temp, destUTF8MaxLengthInBytes);
            destLength = (ushort)temp;
            if (error == ConversionError.None)
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Copies a buffer of UTF-8 text. The copy is encoded as UCS-2.
        /// </summary>
        /// <remarks>Assumes the source data is valid UTF-8.</remarks>
        /// <param name="src">The source buffer for reading UTF-8.</param>
        /// <param name="srcLength">The number of bytes to read from the source.</param>
        /// <param name="dest">The destination buffer for writing UCS-2.</param>
        /// <param name="destLength">Outputs the number of chars written to the destination.</param>
        /// <param name="destUCS2MaxLengthInChars">The max number of chars that will be written to the destination buffer.</param>
        /// <returns><see cref="CopyError.None"/> if the copy fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Copy(char *dest, out int destLength, int destUCS2MaxLengthInChars, byte *src, int srcLength)
        {
            if (ConversionError.None == Unicode.Utf8ToUtf16(src, srcLength, dest, out destLength, destUCS2MaxLengthInChars))
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Copies a buffer of UTF-8 text. The copy is encoded as UCS-2.
        /// </summary>
        /// <remarks>Assumes the source data is valid UTF-8.</remarks>
        /// <param name="src">The source buffer for reading UTF-8.</param>
        /// <param name="srcLength">The number of bytes to read from the source.</param>
        /// <param name="dest">The destination buffer for writing UCS-2.</param>
        /// <param name="destLength">Outputs the number of chars written to the destination.</param>
        /// <param name="destUCS2MaxLengthInChars">The max number of chars that will be written to the destination buffer.</param>
        /// <returns><see cref="CopyError.None"/> if the copy fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Copy(char *dest, out ushort destLength, ushort destUCS2MaxLengthInChars, byte *src, ushort srcLength)
        {
            var error = Unicode.Utf8ToUtf16(src, srcLength, dest, out var temp, destUCS2MaxLengthInChars);
            destLength = (ushort)temp;
            if (error == ConversionError.None)
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Appends UTF-8 text to a buffer.
        /// </summary>
        /// <remarks>Assumes the source data is valid UTF-8.
        ///
        /// No data will be copied if the destination has insufficient capacity for the full append, *i.e.* if `srcLength > (destCapacity - destLength)`.
        /// </remarks>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcLength">The number of bytes to read from the source.</param>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="destLength">Reference to the destination buffer's length in bytes *before* the append. Will be assigned the new length *after* the append.</param>
        /// <param name="destCapacity">The destination buffer capacity in bytes.</param>
        /// <returns><see cref="FormatError.None"/> if the append fully completes. Otherwise, returns <see cref="FormatError.Overflow"/>.</returns>
        public static FormatError AppendUTF8Bytes(byte* dest, ref int destLength, int destCapacity, byte* src, int srcLength)
        {
            if (destLength + srcLength > destCapacity)
                return FormatError.Overflow;
            UnsafeUtility.MemCpy(dest + destLength, src, srcLength);
            destLength += srcLength;
            return FormatError.None;
        }

        /// <summary>
        /// Appends UTF-8 text to a buffer.
        /// </summary>
        /// <remarks>Assumes the source data is valid UTF-8.</remarks>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcLength">The number of bytes to read from the source.</param>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="destLength">Reference to the destination buffer's length in bytes *before* the append. Will be assigned the number of bytes appended.</param>
        /// <param name="destUTF8MaxLengthInBytes">The destination buffer's length in bytes. Data will not be appended past this length.</param>
        /// <returns><see cref="CopyError.None"/> if the append fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Append(byte *dest, ref ushort destLength, ushort destUTF8MaxLengthInBytes, byte *src, ushort srcLength)
        {
            var error = Unicode.Utf8ToUtf8(src, srcLength, dest + destLength, out var temp, destUTF8MaxLengthInBytes - destLength);
            destLength += (ushort)temp;
            if (error == ConversionError.None)
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Appends UCS-2 text to a buffer, encoded as UTF-8.
        /// </summary>
        /// <remarks>Assumes the source data is valid UCS-2.</remarks>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcLength">The number of chars to read from the source.</param>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="destLength">Reference to the destination buffer's length in bytes *before* the append. Will be assigned the number of bytes appended.</param>
        /// <param name="destUTF8MaxLengthInBytes">The destination buffer's length in bytes. Data will not be appended past this length.</param>
        /// <returns><see cref="CopyError.None"/> if the append fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Append(byte *dest, ref ushort destLength, ushort destUTF8MaxLengthInBytes, char *src, int srcLength)
        {
            var error = Unicode.Utf16ToUtf8(src, srcLength, dest + destLength, out var temp, destUTF8MaxLengthInBytes - destLength);
            destLength += (ushort)temp;
            if (error == ConversionError.None)
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Appends UTF-8 text to a buffer, encoded as UCS-2.
        /// </summary>
        /// <remarks>Assumes the source data is valid UTF-8.</remarks>
        /// <param name="src">The source buffer.</param>
        /// <param name="srcLength">The number of bytes to read from the source.</param>
        /// <param name="dest">The destination buffer.</param>
        /// <param name="destLength">Reference to the destination buffer's length in chars *before* the append. Will be assigned the number of chars appended.</param>
        /// <param name="destUCS2MaxLengthInChars">The destination buffer's length in chars. Data will not be appended past this length.</param>
        /// <returns><see cref="CopyError.None"/> if the append fully completes. Otherwise, returns <see cref="CopyError.Truncation"/>.</returns>
        public static CopyError Append(char *dest, ref ushort destLength, ushort destUCS2MaxLengthInChars, byte *src, ushort srcLength)
        {
            var error = Unicode.Utf8ToUtf16(src, srcLength, dest + destLength, out var temp, destUCS2MaxLengthInChars - destLength);
            destLength += (ushort)temp;
            if (error == ConversionError.None)
                return CopyError.None;
            return CopyError.Truncation;
        }

        /// <summary>
        /// Returns true if two UTF-8 buffers have the same length and content.
        /// </summary>
        /// <param name="aBytes">The first buffer of UTF-8 text.</param>
        /// <param name="aLength">The length in bytes of the first buffer.</param>
        /// <param name="bBytes">The second buffer of UTF-8 text.</param>
        /// <param name="bLength">The length in bytes of the second buffer.</param>
        /// <returns></returns>
        public static bool EqualsUTF8Bytes(byte* aBytes, int aLength, byte* bBytes, int bLength)
        {
            if (aLength != bLength)
                return false;

            return UnsafeUtility.MemCmp(aBytes, bBytes, aLength) == 0;
        }
    }
}
