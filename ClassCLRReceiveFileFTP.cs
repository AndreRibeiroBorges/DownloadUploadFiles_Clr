using System;
using System.IO;
using System.Net;

public static class FTPDownloader
{
    public static void BaixarArquivoFTP(string ftp, int porta, string usuario, string senha, string pastaRemota, string caminhoLocal, bool excluirArquivoFTP)
    {
        string url = "ftp://" + ftp + ":" + porta.ToString() + "/" + pastaRemota + "/";
        FtpWebRequest requisicaoFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
        requisicaoFTP.Credentials = new NetworkCredential(usuario, senha);
        requisicaoFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

        try
        {
            using (WebResponse resposta = requisicaoFTP.GetResponse())
            {
                using (StreamReader leitor = new StreamReader(resposta.GetResponseStream()))
                {
                    string conteudo = leitor.ReadToEnd();
                    string[] linhas = conteudo.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string linha in linhas)
                    {
                        string[] campos = linha.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string nomeArquivo = campos[8];

                        if (nomeArquivo.EndsWith(".sql")|| nomeArquivo.EndsWith(".txt"))
                        {
                            url = "ftp://" + ftp + ":" + porta.ToString() + "/" + pastaRemota + "/" + nomeArquivo;
                            requisicaoFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                            requisicaoFTP.Credentials = new NetworkCredential(usuario, senha);
                            requisicaoFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                            requisicaoFTP.UseBinary = true;

                            using (FtpWebResponse respostaFTP = (FtpWebResponse)requisicaoFTP.GetResponse())
                            {
                                using (Stream stream = respostaFTP.GetResponseStream())
                                {
                                    using (FileStream arquivoLocal = new FileStream(Path.Combine(caminhoLocal, nomeArquivo), FileMode.Create))
                                    {
                                        byte[] buffer = new byte[10240];
                                        int bytesLidos = stream.Read(buffer, 0, buffer.Length);

                                        while (bytesLidos > 0)
                                        {
                                            arquivoLocal.Write(buffer, 0, bytesLidos);
                                            bytesLidos = stream.Read(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }

                            if (excluirArquivoFTP)
                            {
                                url = "ftp://" + ftp + ":" + porta.ToString() + "/" + pastaRemota + "/" + nomeArquivo;
                                requisicaoFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                                requisicaoFTP.Credentials = new NetworkCredential(usuario, senha);
                                requisicaoFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                                requisicaoFTP.UseBinary = true;

                                using (FtpWebResponse respostaFTP = (FtpWebResponse)requisicaoFTP.GetResponse())
                                {
                                    // Arquivo do FTP excluído com sucesso
                                }
                            }
                        }// Enf IF Para Filtro extensao .SQL
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao baixar os arquivos do servidor FTP: " + ex.Message);
        }
    }
}
