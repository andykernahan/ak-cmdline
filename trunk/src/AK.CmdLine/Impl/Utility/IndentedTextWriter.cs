// Copyright 2011 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AK.CmdLine.Impl.Utility
{
    /// <summary>
    /// Provides support for indenting output.
    /// </summary>
    public class IndentedTextWriter : TextWriter
    {
        #region Public Interface.

        /// <summary>
        /// Defines the default indent String. This field is constant.
        /// </summary>
        public const string DefaultIndentString = " ";

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.Impl.Utility.IndentedTextWriter"/>
        /// class.
        /// </summary>
        /// <param name="inner">The inner <see cref="System.IO.TextWriter"/></param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="inner"/> is <see langword="null"/>.
        /// </exception>
        public IndentedTextWriter(TextWriter inner)
            : base(CultureInfo.CurrentCulture)
        {
            Guard.NotNull(inner, "inner");

            Inner = inner;
            IsIndentPending = true;
            IndentString = DefaultIndentString;
        }

        /// <summary>
        /// Pushes the specified <paramref name="indent"/>.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>An <see cref="System.IDisposable"/> which, when disposed of, pops the specified
        /// <paramref name="indent"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="indent"/> is negative.
        /// </exception>
        public IDisposable PushIndent(int indent)
        {
            Guard.InRange(indent > 0, "indent");

            return new Indent(this, indent);
        }

        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        public override void Close()
        {
            Inner.Close();
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            Inner.Flush();
        }

        /// <summary>
        /// Writes the text representation of a Boolean value to the text stream.
        /// </summary>
        /// <param name="value">The Boolean to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(bool value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes a character to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(char value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes a character array to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(char[] buffer)
        {
            WriteIndent();
            Inner.Write(buffer);
        }

        /// <summary>
        /// Writes a subarray of characters to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">Starting index in the buffer.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>. </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer"/> parameter is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> or <paramref name="count"/> is negative. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(char[] buffer, int index, int count)
        {
            WriteIndent();
            Inner.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes the text representation of a decimal value to the text stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(decimal value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(double value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(float value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(int value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(long value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an object to the text stream by calling ToString on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(object value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting String.</param>
        /// <param name="arg0">An object to write into the formatted String.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="format"/> is null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to the number of provided objects to be formatted. </exception>
        public override void Write(string format, object arg0)
        {
            WriteIndent();
            Inner.Write(format, arg0);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting String.</param>
        /// <param name="arg0">An object to write into the formatted String.</param>
        /// <param name="arg1">An object to write into the formatted String.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="format"/> is null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to the number of provided objects to be formatted. </exception>
        public override void Write(string format, object arg0, object arg1)
        {
            WriteIndent();
            Inner.Write(format, arg0, arg1);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting String.</param>
        /// <param name="arg0">An object to write into the formatted String.</param>
        /// <param name="arg1">An object to write into the formatted String.</param>
        /// <param name="arg2">An object to write into the formatted String.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="format"/> is null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to the number of provided objects to be formatted. </exception>
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            WriteIndent();
            Inner.Write(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting String.</param>
        /// <param name="arg">The object array to write into the formatted String.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="format"/> or <paramref name="arg"/> is null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to <paramref name="arg"/>. Length. </exception>
        public override void Write(string format, params object[] arg)
        {
            WriteIndent();
            Inner.Write(format, arg);
        }

        /// <summary>
        /// Writes a string to the text stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(string value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        [CLSCompliant(false)]
        public override void Write(uint value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        [CLSCompliant(false)]
        public override void Write(ulong value)
        {
            WriteIndent();
            Inner.Write(value);
        }

        /// <summary>
        /// Writes a line terminator to the text stream.
        /// </summary>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine()
        {
            Inner.WriteLine();
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of a Boolean followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The Boolean to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(bool value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes a character followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(char value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes an array of characters followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(char[] buffer)
        {
            WriteIndent();
            Inner.WriteLine(buffer);
            PostLineWritten();
        }

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <param name="index">The index into <paramref name="buffer"/> at which to begin reading.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>. </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer"/> parameter is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> or <paramref name="count"/> is negative. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            WriteIndent();
            Inner.WriteLine(buffer, index, count);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of a decimal value followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(decimal value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(double value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(float value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(int value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(long value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of an object by calling ToString on this object, followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The object to write. If <paramref name="value"/> is null, only the line termination characters are written.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(object value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatted String.</param>
        /// <param name="arg0">The object to write into the formatted String.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="format"/> is null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to the number of provided objects to be formatted. </exception>
        public override void WriteLine(string format, object arg0)
        {
            WriteIndent();
            Inner.WriteLine(format, arg0);
            PostLineWritten();
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting String.</param>
        /// <param name="arg0">The object to write into the format String.</param>
        /// <param name="arg1">The object to write into the format String.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="format"/> is null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to the number of provided objects to be formatted. </exception>
        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteIndent();
            Inner.WriteLine(format, arg0, arg1);
            PostLineWritten();
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting String.</param>
        /// <param name="arg0">The object to write into the format String.</param>
        /// <param name="arg1">The object to write into the format String.</param>
        /// <param name="arg2">The object to write into the format String.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="format"/> is null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to the number of provided objects to be formatted. </exception>
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteIndent();
            Inner.WriteLine(format, arg0, arg1, arg2);
            PostLineWritten();
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)"/>.
        /// </summary>
        /// <param name="format">The formatting String.</param>
        /// <param name="arg">The object array to write into format String.</param>
        /// <exception cref="T:System.ArgumentNullException">A string or object is passed in as null. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.FormatException">The format specification in format is invalid.-or- The number indicating an argument to be formatted is less than zero, or larger than or equal to arg.Length. </exception>
        public override void WriteLine(string format, params object[] arg)
        {
            WriteIndent();
            Inner.WriteLine(format, arg);
            PostLineWritten();
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value"/> is null, only the line termination characters are written.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void WriteLine(string value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        [CLSCompliant(false)]
        public override void WriteLine(uint value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        [CLSCompliant(false)]
        public override void WriteLine(ulong value)
        {
            WriteIndent();
            Inner.WriteLine(value);
            PostLineWritten();
        }

        /// <summary>
        /// When overridden in a derived class, returns the <see cref="T:System.Text.Encoding"/> in which the output is written.
        /// </summary>
        /// <value></value>
        /// <returns>The Encoding in which the output is written.</returns>
        public override Encoding Encoding
        {
            get { return Inner.Encoding; }
        }

        /// <summary>
        /// Gets an object that controls formatting.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.IFormatProvider"/> object for a specific culture, or the formatting of the current culture if no other culture is specified.</returns>
        public override IFormatProvider FormatProvider
        {
            get { return Inner.FormatProvider; }
        }

        /// <summary>
        /// Gets the current indent level.
        /// </summary>
        public int IndentLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the indent String.
        /// </summary>
        public string IndentString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the length of the indent string given the current indent level.
        /// </summary>
        public int IndentLength
        {
            get { return String.IsNullOrEmpty(IndentString) ? 0 : IndentLevel * IndentString.Length; }
        }

        /// <summary>
        /// Gets or sets the line terminator string used by the current TextWriter.
        /// </summary>
        /// <value></value>
        /// <returns>The line terminator string for the current TextWriter.</returns>
        public override string NewLine
        {
            get { return Inner.NewLine; }
            set { Inner.NewLine = value; }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Inner.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Writes the indent, if one is pending.
        /// </summary>
        protected void WriteIndent()
        {
            if (IsIndentPending)
            {
                for (int i = 0; i < IndentLevel; ++i)
                {
                    Inner.Write(IndentString);
                }
                IsIndentPending = false;
            }
        }

        /// <summary>
        /// Peforms any actions required after a line is written.
        /// </summary>
        protected void PostLineWritten()
        {
            IsIndentPending = true;
        }

        /// <summary>
        /// Gets the inner <see cref="System.IO.TextWriter"/>.
        /// </summary>
        protected TextWriter Inner
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating if an indent is pending.
        /// </summary>
        protected bool IsIndentPending
        {
            get;
            set;
        }

        #endregion

        #region Private Impl.

        private sealed class Indent : IDisposable
        {
            private bool _isDisposed;
            private readonly int _indent;
            private readonly IndentedTextWriter _writer;

            public Indent(IndentedTextWriter writer, int indent)
            {
                _writer = writer;
                _indent = indent;
                _writer.IndentLevel += _indent;
            }

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _writer.IndentLevel -= _indent;
                }
            }
        }

        #endregion
    }
}