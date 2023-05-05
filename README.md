# DownloadUploadFiles_Clr
Classe C# em formato CLR para mandar/receber arquivos de uma pasta em FTP

Fugindo um pouco das limitações do SQL Server, às vezes precisamos "recriar a roda". Tem diversas ferramentas no mercado pra fazer isso, Mas por falta de orçamento ou habilidade em outra ferramenta temos que resolver problemas com o que temos. Ou, seja, o bom e velho SQL. 

Usando a opção nativa do SQL Server de integrar-se a codigos .net, temos a opção de escrever em C#(exemplo apenas, poderia ser outra compativel microsoft), um código como esse que envia e recebe arquivos de um ftp. 

O motivo desse código foi a necessidade de criar um "sincronizador" que atualizasse alguns objetos em bancos de algumas filiais de uma empresa que prestei serviço. 

Então, quando se atualizava a procedure Usp_getClient na matriz, por exemplo, gostariam de poder enviar essa versao para as filiais e então manter todas na mesma versão. 
Depois de criar a classe CLR, foi só ajustar um codigo que iria executar nossa sp e baixar os arquivos, e depois rodá-los na base através de um job. 

Espero que seja útil. 

Até mais. 
