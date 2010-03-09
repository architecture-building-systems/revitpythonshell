//---------------------------------------------------------------------------------
//IPEWrapper.cs - version 2.7.6.0r
//BY DOWNLOADING AND USING, YOU AGREE TO THE FOLLOWING TERMS:
//Copyright (c) 2006-2008 by Joseph P. Socoloski III
//LICENSE
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//the MIT License, given here: <http://www.opensource.org/licenses/mit-license.php> 
//---------------------------------------------------------------------------------
using System;
using System.Text;
using System.IO;

namespace IronTextBox
{
    public delegate void Response(string text);

    public class IpeStreamWrapper : Stream 
    {
        MemoryStream _stream = new MemoryStream();
        readonly Response _response;

        public IpeStreamWrapper(Response response)
        {
            _response = response;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }

        public override void Flush()
        {
            _stream.Flush();

            _stream.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(_stream, Encoding.ASCII);
            _response(sr.ReadToEnd());
            sr.Close();
            _stream = new MemoryStream();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public static StringBuilder Output = new StringBuilder();

        /// <summary>
        /// Method that allows capture of IronPython output
        /// </summary>
        /// <param name="text"></param>
        public static void EngineResponse(string text)
        {
            
            if (!string.IsNullOrEmpty(text.Trim()))
            {
                //Output.Remove(0, Output.Length);        //Clear
                text = text.Replace("\\n", "\r\n");         //to support newline for textbox use
                Output.Append(text + Environment.NewLine);
            }
        }        
    }
}