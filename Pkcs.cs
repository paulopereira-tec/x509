using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace x509
{
    public class Pkcs
  {
    private X509Certificate2 _certificate { get; set; }

    public Pkcs(X509Certificate2 certificate)
    {
      _certificate = certificate;
    }

    public string Signer(string message)
    {
      
      if (_certificate == null)
          throw new Exception("Certificado não localizado.");                
      if (_certificate.PrivateKey == null)                
          throw new Exception("chave privada não localizada no certificado.");

      //convertendo certificado para objeto que o bouncycastle conhece
      var bouncyCastleKey = DotNetUtilities.GetKeyPair(_certificate.PrivateKey).Private;
      var x5091 = new X509Certificate(_certificate.RawData);
      var x509CertBouncyCastle = DotNetUtilities.FromX509Certificate(x5091);

      var generator = new CmsSignedDataGenerator();
      var signerInfoGeneratorBuilder = new SignerInfoGeneratorBuilder();
      generator.AddSignerInfoGenerator(
          signerInfoGeneratorBuilder.Build(new Asn1SignatureFactory("SHA256WithRSA", bouncyCastleKey),
              x509CertBouncyCastle));

      //criando certstore que o bouncycastle conhece
      IList certList = new ArrayList();
      certList.Add(x509CertBouncyCastle);
      var store509BouncyCastle = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(certList));
      generator.AddCertificates(store509BouncyCastle);

      var encoding = new UTF8Encoding();
      var messageBytes = encoding.GetBytes(message);

      var cmsdata = new CmsProcessableByteArray(messageBytes);
      
      //assina
      var signeddata = generator.Generate(cmsdata, true);
      var finalMessage = signeddata.GetEncoded();

      //converte para base64 que eh o formato que o serviço espera
      return Convert.ToBase64String(finalMessage);
    }
  }
}
