// Copyright 2014 Serilog Contributors
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


// Copyright (c) 2014 Logentries

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using Serilog.Debugging;
using System.Threading.Tasks;

namespace Serilog.Sinks.Logentries
{
    class LeClient
    {
        public LeClient(bool useHttpPut, bool useSsl, string url, int port)
        {
            m_UseSsl = useSsl;
            _url = url;
            m_TcpPort = port;
        }

        bool m_UseSsl;
        private readonly string _url;
        int m_TcpPort;
        TcpClient m_Client;
        Stream m_Stream;
        SslStream m_SslStream;

        Stream ActiveStream
        {
            get
            {
                return m_UseSsl ? m_SslStream : m_Stream;
            }
        }

        public async Task ConnectAsync()
        {
            m_Client = new TcpClient()
            {
                NoDelay = true
            };
            await m_Client.ConnectAsync(_url, m_TcpPort).ConfigureAwait(false);

            m_Stream = m_Client.GetStream();

            if (m_UseSsl)
            {
                m_SslStream = new SslStream(m_Stream);
                await m_SslStream.AuthenticateAsClientAsync(_url).ConfigureAwait(false);
            }
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            await ActiveStream.WriteAsync(buffer, offset, count).ConfigureAwait(false);
        }

        public async Task FlushAsync()
        {
            await ActiveStream.FlushAsync().ConfigureAwait(false);
        }

        public void Close()
        {
            if (m_Client != null)
            {
                try
                {
#if NETSTANDARD1_3
                    m_Client.Dispose();
#else
                    m_Client.Close();
#endif
                }
                catch (Exception ex)
                {
                    SelfLog.WriteLine("Exception while closing client: {0}", ex);
                }
            }
        }
    }
}
