using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace x509
{
    public class Certificate
    {
        #region Propriedades
        public X509Certificate2 x509 { get; private set; }
        public string CertificateString { get; private set; }
        public string Subject { get; private set; }
        public string Issuer { get; private set; }
        public int Version { get; private set; }
        public DateTime ValidDate { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public string Thumbprint { get; private set; }
        public string SerialNumber { get; private set; }
        public string FriendlyName { get; private set; }
        public string PublicKeyFormat { get; private set; }
        public int RawDataLength { get; private set; }
        public string XMLString { get; private set; }
        public byte[] RawData { get; set; }
        public byte[] PrivateKey { get; set; }
        #endregion

        /// <summary>
        /// Recupera uma lista de certificados instalados no computador.
        /// </summary>
        /// <param name="type">Tipos de certificados diposniveis para consulta</param>
        public Certificate(ECertTypes type, string password)
        {
            string _type = type == ECertTypes.CNPJ ? "e-CNPJ" : "e-CPF";

            X509Store store = new X509Store("My");
            store.Open(OpenFlags.ReadOnly);
            foreach (var certificado in store.Certificates)
            {
                if (certificado.Subject.Contains(_type))
                {
                    string Certificate = certificado.Subject;

                    GetCertificateFromStore(Certificate, password);
                }
            }
            store.Close();
        }

        /// <summary>
        /// Recupera o certificado com base em um local no computador e uma senha
        /// </summary>
        /// <param name="path"></param>
        /// <param name="password"></param>
        public Certificate(string path, string password)
        {
            //x509 = new X509Certificate2(File.ReadAllBytes(path), password, X509KeyStorageFlags.MachineKeySet);
            try
            {
                X509Store store = new X509Store(StoreLocation.LocalMachine);
                x509 = new X509Certificate2(File.ReadAllBytes(path), password, X509KeyStorageFlags.Exportable);
                bool result = x509.Verify();
                var r2 = result;
            }
            catch (Exception ex)
            {
                //Console.Out.WriteLine("ErrorGettingCertificate in base:" + ex.Message);
            }
            fillProperties();
        }

        /// <summary>
        /// Recupera um certificado específico pelo CN
        /// </summary>
        /// <param name="certName">CN específico do certifificado</param>
        /// <returns></returns>
        public X509Certificate2 GetCertificateFromStore(string certName, string password)
        {
            // Get the certificate store for the current user.
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                // Place all certificates in an X509Certificate2Collection object.
                X509Certificate2Collection certCollection = store.Certificates;
                // If using a certificate with a trusted root you do not need to FindByTimeValid, instead:
                // currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, true);
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);
                if (signingCert.Count == 0) return null;

                x509 = new X509Certificate2(signingCert[0].RawData, password, X509KeyStorageFlags.Exportable);

                fillProperties();

                // Return the first certificate in the collection, has the right name and is current.
                return x509;
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Preenche as propriedades com base nos dados recuperados do certificado.
        /// </summary>
        private void fillProperties()
        {
            //Certificate = x509.Certificate;
            Subject = x509.Subject;
            Issuer = x509.Issuer;
            Version = x509.Version;
            ValidDate = x509.NotBefore;
            ExpiryDate = x509.NotAfter;
            Thumbprint = x509.Thumbprint;
            SerialNumber = x509.SerialNumber;
            FriendlyName = x509.PublicKey.Oid.FriendlyName;
            PublicKeyFormat = x509.PublicKey.EncodedKeyValue.Format(true);
            RawDataLength = x509.RawData.Length;
            XMLString = x509.PublicKey.Key.ToXmlString(false);
            RawData = x509.RawData;
        }

    }
}