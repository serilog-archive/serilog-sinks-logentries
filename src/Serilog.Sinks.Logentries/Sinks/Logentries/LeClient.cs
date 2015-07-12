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
using Serilog.Debugging;
using Sockets.Plugin;
using System.Net.Http;

namespace Serilog.Sinks.Logentries
{
   internal  class LeClient
    {
        #region Constants

        // Logentries API server address. 
        const string    API_TCP_NOSSL       = "data.logentries.com";
        const string    API_TCP_SSL         = "api.logentries.com";
        // Port number for token logging on Logentries API server. 
        const int       API_TCP_NOSSL_PORT  = 10000; // TCP
        const int       API_TCP_SSL_PORT    = 20000; // TCP-SSL

        const string    API_HTTPUT_URL          = "http{0}://api.logentries.com/{1}/hosts/{2}/{3}?realtime=1";
        const int       API_HTTPUT_NOSSL_PORT   = 80;   // HTTP
        const int       API_HTTPUT_SSL_PORT     = 443;  // HTTOS
        #endregion Constants

        #region Member Variables
        bool _useSsl;
        bool _useHttpPut;
        int _tcpPort;
        string _url;
        TcpSocketClient _socketClient;
        HttpClient _httpClient;
        #endregion Member Variables

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LeClient" /> class.
        /// </summary>
        /// <param name="useHttpPut">if set to <c>true</c> [use HTTP put].</param>
        /// <param name="useSsl">if set to <c>true</c> [use SSL].</param>
        /// <param name="accountKey">The account key.</param>
        /// <param name="hostKey">The host key.</param>
        /// <param name="logKey">The log key.</param>
        public LeClient(bool useHttpPut, bool useSsl, string accountKey = null, string hostKey = null, string logKey = null)
        {
            _useSsl = useSsl;
            _useHttpPut = useHttpPut;

            if (_useHttpPut)
            {
                if (string.IsNullOrEmpty(accountKey)) throw new ArgumentNullException("accountKey", "Account Key required for HTTP PUT API");
                if (string.IsNullOrEmpty(logKey)) throw new ArgumentNullException("logKey", "Log Key required for HTTP PUT API");

                _tcpPort = _useSsl ? API_HTTPUT_SSL_PORT : API_HTTPUT_NOSSL_PORT;
                _url = string.Format(API_HTTPUT_URL, _useSsl ? "s" : "", accountKey, hostKey, logKey);
            }
            else
            {
                _tcpPort = _useSsl ? API_TCP_SSL_PORT : API_TCP_NOSSL_PORT;
                _url = _useSsl || _useHttpPut ? API_TCP_SSL : API_TCP_NOSSL;
            }

        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Connects to the Logentries API Server.
        /// </summary>
        public void Connect()
        {
            if (_useHttpPut)
            {
                _httpClient = new HttpClient();
            }
            else
            {
                _socketClient = new TcpSocketClient();

                var task = _socketClient.ConnectAsync(_url, _tcpPort, _useSsl);

                task.Wait(new TimeSpan(0, 0, 30));
            }
        }

        /// <summary>
        /// Writes the specified buffer to the API server.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (_useHttpPut)
            {                
                var response = _httpClient.PutAsync(_url, new ByteArrayContent(buffer, offset, count));

                response.Result.EnsureSuccessStatusCode();
            }
            else
            {
                _socketClient.WriteStream.Write(buffer, offset, count);
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(string data)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);

            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Flushes the buffer.
        /// </summary>
        public void Flush()
        {
            if (_socketClient != null && !_useHttpPut)
            {
                _socketClient.WriteStream.Flush();
            }
        }

        /// <summary>
        /// Closes the socket connection.
        /// </summary>
        public void Close()
        {
            if (_socketClient != null && !_useHttpPut)
            {
                try
                {
                    _socketClient.DisconnectAsync();
                }
                catch (Exception ex)
                {
                    SelfLog.WriteLine("Exception while closing client: {0}", ex);
                }
            }
        }
        #endregion Public Methods
    }
}