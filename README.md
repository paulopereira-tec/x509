# Descrição
O objetivo desse projeto é prover ferramentas e métodos para facilitar o manuseio de certificados digitais (e-CPF e e-CNPJ). Atualmente está disponível a utilização de certificados no formato de arquivos "*.pfx". - Formato conhecido como A1.

Também é possível utilizar os recursos dos certificados instalados no computador.

Este projeto ainda implementa a encriptação de mensagens (textos) utilizando o padrão PKCS#7.

O próximo objetivo será a assinatura de documentos PDF e outros tipos de arquivos.

Em breve, também há espectativa para a implementação de uso de certificados conhecidos como A3 - nuvem, token ou cartão.

# Exemplo de implementação
## Para utilização de certificados em formato de arquivo PFX
    Certificate cert = new Certificate("localDoCertificadoPFX", "SenhaDoCertificado");


## Para utilização de certificados instalados no computador
    // e-CPF
    Certificate cert = new Certificate(ECertTypes.CPF, "SenhaDoCertificado");

    // e-CNPJ
    Certificate cert = new Certificate(ECertTypes.CNPJ, "SenhaDoCertificado");

## Propriedades disponíveis para acesso
- X509Certificate2 x509 /* instância do certificado digital */
- string CertificateString 
- string Subject 
- string Issuer 
- int Version 
- DateTime ValidDate 
- DateTime ExpiryDate 
- string Thumbprint 
- string SerialNumber 
- string FriendlyName 
- string PublicKeyFormat 
- int RawDataLength 
- string XMLString 
- byte[] RawData { get; set; }
- byte[] PrivateKey { get; set; }

# Assinatura de mensagens
Para assinar uma _string_ utilizando o padrão PKCS#7 (normalmente utilizados por bancos e e-mails), utilize os exemplos abaixo:

## Criando a instância PKCS
    // utilize a instância do certificado digital (propriedade x502 da classe Certificate)
    Pkcs pkcs = new Pkcs(certificateInstance)

## Executando a assinatura
    // passe a mensagem (string) a ser criptografada por parâmetro
    string message = "Messagem a ser criptografada";

    // Conteúdo criptografado
    string content = pkcs.Signer(message);