using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using Microsoft.SqlServer.Server;

public static class FTPUploader
{
    [SqlProcedure]
    public static void UploadToFTP(SqlString ftp, SqlInt32 porta, SqlString usuario, SqlString senha, SqlString pastaRemota,SqlString pastaLocal )
    {
        string retorno;
        try
        {
            // Cria a conexão FTP
            string ftpUrl = "ftp://" + ftp.Value + ":" + porta.Value + "/";
            FtpWebRequest requisicaoFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpUrl));
            requisicaoFTP.UseBinary = true;
            requisicaoFTP.UsePassive = true;
            requisicaoFTP.Credentials = new NetworkCredential(usuario.Value, senha.Value);
            requisicaoFTP.Method = WebRequestMethods.Ftp.ListDirectory;
            
            // Lista os arquivos na pasta remota
            string[] listaArquivosRemotos;
            using (WebResponse resposta = requisicaoFTP.GetResponse())
            {
                using (StreamReader leitor = new StreamReader(resposta.GetResponseStream()))
                {
                    string conteudo = leitor.ReadToEnd();
                    listaArquivosRemotos = conteudo.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            // Faz o upload de todos os arquivos da pasta local
            string[] listaArquivosLocais = Directory.GetFiles(pastaLocal.Value);
            foreach (string arquivoLocal in listaArquivosLocais)
            {
                string nomeArquivo = Path.GetFileName(arquivoLocal);
                string caminhoRemoto = pastaRemota.Value + "/" + nomeArquivo;

                // Verifica se o arquivo já existe na pasta remota
                if (Array.Exists(listaArquivosRemotos, arquivo => arquivo.Equals(nomeArquivo, StringComparison.OrdinalIgnoreCase)))
                {
                    retorno=("O arquivo '" + nomeArquivo + "' já existe na pasta remota.");
                    continue;
                }

                // Realiza o upload do arquivo
                using (Stream streamLocal = File.OpenRead(arquivoLocal))
                {
                    requisicaoFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpUrl + caminhoRemoto));
                    requisicaoFTP.Credentials = new NetworkCredential(usuario.Value, senha.Value);
                    requisicaoFTP.UseBinary = true;
                    requisicaoFTP.UsePassive = true;
                    requisicaoFTP.Method = WebRequestMethods.Ftp.UploadFile;
                    using (Stream streamRemoto = requisicaoFTP.GetRequestStream())
                    {
                        streamLocal.CopyTo(streamRemoto);
                    }
                }

                retorno=("Arquivo '" + nomeArquivo + "' enviado com sucesso para a pasta remota.");
            }
        }
        catch (Exception ex)
        {
            retorno=("Erro ao enviar arquivos para a pasta remota: " + ex.Message);
        }
    }
}
