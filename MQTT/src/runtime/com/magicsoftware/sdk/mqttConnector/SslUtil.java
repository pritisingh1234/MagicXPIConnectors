package com.magicsoftware.sdk.mqttConnector;

import org.bouncycastle.cert.X509CertificateHolder;
import org.bouncycastle.cert.jcajce.JcaX509CertificateConverter;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.openssl.PEMParser;
import javax.net.ssl.KeyManagerFactory;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSocketFactory;
import javax.net.ssl.TrustManagerFactory;
import java.io.FileInputStream;
import java.io.FileReader;
import java.security.KeyStore;
import java.security.Security;
import java.security.cert.X509Certificate;

public class SslUtil {

    public static SSLSocketFactory getSocketFactory(final String caCertificateFilePEM, final String pkcs12CrtFile,final String pkcs12Password,final String securityProtocol) {
        try {

            /**
             * Adds BouncyCastle as a Security Provider
             */
            Security.addProvider(new BouncyCastleProvider());

            JcaX509CertificateConverter certificateConverter = new JcaX509CertificateConverter().setProvider("BC");

            /**
             * Loads Certificate Authority (CA) certificate
             */
            PEMParser reader = new PEMParser(new FileReader(caCertificateFilePEM));
            X509CertificateHolder caCertHolder = (X509CertificateHolder) reader.readObject();
            reader.close();
            
            

            X509Certificate caCert = certificateConverter.getCertificate(caCertHolder);
            
            
            /**
             * CA certificate is used to authenticate the server
             */
            KeyStore caKeyStore = KeyStore.getInstance(KeyStore.getDefaultType());
            caKeyStore.load(null, null);
            caKeyStore.setCertificateEntry("broker-ca-certificate", caCert);

            TrustManagerFactory trustManagerFactory = TrustManagerFactory.getInstance(
                    TrustManagerFactory.getDefaultAlgorithm());
            trustManagerFactory.init(caKeyStore);

            /**
             * Client key and certificates are sent to server so it can authenticate the client
             */
            KeyManagerFactory keyManagerFactory=null;
            if(pkcs12CrtFile!=null)
            {
            KeyStore p12 = KeyStore.getInstance("pkcs12");
            p12.load(new FileInputStream(pkcs12CrtFile), pkcs12Password.toCharArray());
            keyManagerFactory = KeyManagerFactory.getInstance(
                    KeyManagerFactory.getDefaultAlgorithm());
            keyManagerFactory.init(p12, pkcs12Password.toCharArray());
            }
           

            /**
             * Create SSL socket factory
             */
           // SSLContext context = SSLContext.getInstance("TLSv1.2");
            SSLContext context = SSLContext.getInstance(securityProtocol);
            
            if(keyManagerFactory!=null)
            {
            context.init(keyManagerFactory.getKeyManagers(), trustManagerFactory.getTrustManagers(), null);
            }
            else
            	{
            context.init(null, trustManagerFactory.getTrustManagers(), null);
            	}

            /**
             * Return the newly created socket factory object
             */
           
            
            return context.getSocketFactory();

        } catch (Exception e) {
            e.printStackTrace();
        }

        return null;
    }
}

