using System;
using System.Security.Cryptography.X509Certificates;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Infrastructure.Interfaces;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Infrastructure.OAuth.Signing;
using Xero.Api.Serialization;

namespace HelloXero
{
    class Program
    {
        const string _baseUrl = "https://api.xero.com";
        const string _consumerKey = "FWXXXXXXXXXXXXXXXXXXXXXXXXXXAT";
        const string _consumerSecret = "OBXXXXXXXXXXXXXXXXXXXXXXXXXXZV";
        const string _signingCertPath = "c:/public_privatekey.pfx";
        const string _signingCertPassword = "";

        static void Main(string[] args)
        {
            XeroCoreApi api = new XeroCoreApi(_baseUrl, new PrivateAuthenticator(_signingCertPath, _signingCertPassword), new Consumer(_consumerKey, _consumerSecret), null, new DefaultMapper(), new DefaultMapper());

            foreach (Contact contact in api.Contacts.Where("IsCustomer = true").FindAsync().Result)
            {
                Console.WriteLine(contact.Name);
            }
            Console.ReadKey();
        }
    }

    internal class PrivateAuthenticator : IAuthenticator
    {
        private readonly X509Certificate2 _certificate;

        public PrivateAuthenticator(string certificatePath, string certificatePassword)
        {
            _certificate = new X509Certificate2(certificatePath, certificatePassword);
        }

        public PrivateAuthenticator(X509Certificate2 certificate)
        {
            _certificate = certificate;
        }

        public string GetSignature(IConsumer consumer, IUser user, Uri uri, string verb, IConsumer consumer1)
        {
            var token = new Token
            {
                ConsumerKey = consumer.ConsumerKey,
                ConsumerSecret = consumer.ConsumerSecret,
                TokenKey = consumer.ConsumerKey
            };

            return new RsaSha1Signer().CreateSignature(_certificate, token, uri, verb);
        }

        public X509Certificate Certificate { get { return _certificate; } }

        public IToken GetToken(IConsumer consumer, IUser user)
        {
            return null;
        }

        public IUser User { get; set; }
    }
}
