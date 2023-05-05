



/* -- Esse Comando é executado no cmd e é o responsavel por gerar uma dll a partir do arquivo C#.

C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc /nologo /target:library /out:ClassCLRReceiveFileFTP.dll ClassCLRReceiveFileFTP.cs
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc /nologo /target:library /out:ClassCLRSendFileFTP.dll ClassCLRSendFileFTP.cs
*/



use db_teste
go
--------------------------------------------------
-- Registrando CLRs
--------------------------------------------------
-- Ativar trustworthy database (requisito para CLRs unsafe e external_access sem assinatura)
Alter Database db_teste set trustworthy on
 
-- Ativar CLRs no SQL
Execute sp_configure 'show advanced options',1
reconfigure
execute sp_configure 'clr enabled', 1
reconfigure

/* :::::::::::::::::::::::::::::::::::::::::::::::: DownLoad de Arquivo :::::::::::::::::::::::::::::::::::::::::::::::: */
Drop Procedure if exists Usp_BaixarArquivoFTP

-- Registrar a DLL (assembly) no SQL:
If exists (select * from sys.assemblies where name = 'ClassCLRReceiveFileFTP') 
begin
	drop assembly ClassCLRReceiveFileFTP
End
Create assembly ClassCLRReceiveFileFTP from 'c:\Temp\ClassCLRReceiveFileFTP.dll' with permission_set = unsafe
Go

CREATE PROCEDURE Usp_BaixarArquivoFTP (@ftp nVARCHAR(100), @porta int, @usuario nVARCHAR(200), @senha nVARCHAR(200),@pastaRemota nvarchar(2000),@caminhoLocal nvarchar(2000),@excluirArquivoFTP bit=0)
AS EXTERNAL NAME ClassCLRReceiveFileFTP.[FTPDownloader].BaixarArquivoFTP;
GO

/* Baixar todos os arquivos que existem no diretorio informado */
EXEC Usp_BaixarArquivoFTP 
	 @ftp ='ftpExemplo.com'
	,@porta ='21'
	,@usuario='SeuUsuario' 
	,@senha='SuaSenha'
	,@pastaRemota = 'ArquivosTransferencia'
	,@caminhoLocal ='C:\Temp\BaixadosFTP\'
	,@excluirArquivoFTP=0

/* :::::::::::::::::::::::::::::::::::::::::::::::: Upload de Arquivo :::::::::::::::::::::::::::::::::::::::::::::::: */
-- Registrar a DLL (assembly) no SQL:
Drop PROCEDURE if exists Usp_EnviarArquivoFTP

IF exists (select 1 from sys.assemblies where name = 'ClassCLRSendFileFTP') 
Begin
	Drop assembly ClassCLRSendFileFTP
End

Create assembly ClassCLRSendFileFTP from 'c:\Temp\ClassCLRSendFileFTP.dll' with permission_set = unsafe
Go

CREATE PROCEDURE Usp_EnviarArquivoFTP (@ftp nVARCHAR(100), @porta int, @usuario nVARCHAR(200), @senha nVARCHAR(200),@pastaRemota nvarchar(2000),@caminhoLocal nvarchar(2000))
AS EXTERNAL NAME ClassCLRSendFileFTP.[FTPUploader].UploadToFTP;
GO

EXEC Usp_EnviarArquivoFTP 
	@ftp ='ftpExemplo.com'
	,@porta ='21'
	,@usuario='SeuUsuario' 
	,@senha='SuaSenha'
	,@caminhoLocal ='C:\Temp\BaixadosFTP\'
	,@pastaRemota = 'ArquivosTransferencia'


