﻿using System;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace SyslogNet.Client.Transport
{
	public class SyslogEncryptedTcpSender : SyslogTcpSender
	{
		public Boolean IgnoreTLSChainErrors { get; private set; }

		protected MessageTransfer _messageTransfer;
		public MessageTransfer messageTransfer
		{
			get { return _messageTransfer; }
			set
			{
				if (!value.Equals(MessageTransfer.OctetCounting) && transportStream is SslStream)
				{
					throw new SyslogTransportException("Non-Transparent-Framing can not be used with TLS transport");
				}

				_messageTransfer = value;
			}
		}

		public SyslogEncryptedTcpSender(string hostname, int port, int timeout = Timeout.Infinite, bool ignoreChainErrors = false) : base(hostname, port)
		{
			IgnoreTLSChainErrors = ignoreChainErrors;
			startTLS(hostname, timeout);
		}

		private void startTLS(String hostname, int timeout)
		{
			transportStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate))
			{
				ReadTimeout = timeout,
				WriteTimeout = timeout
			};

			// According to RFC 5425 we MUST support TLS 1.2, but this protocol version only implemented in framework 4.5 and Windows Vista+...
			((SslStream)transportStream).AuthenticateAsClient(
				hostname,
				null,
				System.Security.Authentication.SslProtocols.Tls,
				false
			);

			if (!((SslStream)transportStream).IsEncrypted)
				throw new SecurityException("Could not establish an encrypted connection");

			messageTransfer = MessageTransfer.OctetCounting;
		}

		private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None || (IgnoreTLSChainErrors && sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors))
				return true;

			CertificateErrorHandler(String.Format("Certificate error: {0}", sslPolicyErrors));
			return false;
		}

		// Quick and nasty way to avoid logging framework dependency
		public static Action<string> CertificateErrorHandler = err => { };
	}
}
