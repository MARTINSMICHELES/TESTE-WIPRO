--*******************************CLIENTE***************************************************************************
--para chamar o List de cliente
http://localhost:50040/Cliente/Listar?cliente={"Id":"0","Nome":"","Cpf":""}

--para chamar o Salvar de cliente
http://localhost:50040/Cliente/Salvar?cliente={"Id":"0","Nome":"Lorena","Cpf":"32021608899"}

http://localhost:50040/Cliente/Salvar?cliente={"Id":"0","Nome":"Michele","Cpf":"34149935870"}

-- Erro esperado
--"F#CPF já está cadastrado ou vazio!!!"
http://localhost:50040/Cliente/Salvar?cliente={"Id":"0","Nome":"Mi","Cpf":""}

http://localhost:50040/Cliente/Salvar?cliente={"Id":"0","Nome":"Mi","Cpf":"11122233311"}

use BDLocadora 
Select * from Cliente
--*************************************************************************************************************
--******************************FILMES*************************************************************************
--para chamar o List de filmes
http://localhost:50040/Filmes/Listar?filmes={"Id":"0","Titulo":"","Genero":"", "Status":""}
--para chamar o Salvar de filmes
--Retorno esperado => "Filme Cadastrado"
http://localhost:50040/Filmes/Salvar?filmes={"Id":"0","Titulo":"Transformers","Genero":"Acao","Status":"0"}
http://localhost:50040/Filmes/Salvar?filmes={"Id":"0","Titulo":"Procurando Dory","Genero":"Aventura","Status":"0"}
http://localhost:50040/Filmes/Salvar?filmes={"Id":"0","Titulo":"Toc Toc","Genero":"Comedia","Status":"0"}
http://localhost:50040/Filmes/Salvar?filmes={"Id":"0","Titulo":"Resgate","Genero":"Suspense","Status":"0"}

Select * from Filmes

--****************************************************************************************************************
--*****************************LOCAÇÃO / ITENS DA LOCAÇÃO ********************************************************
--AAAA-MM-DD
--para chamar o List de Locacao************************************************************************************
http://localhost:50040/Locacao/Listar?Locacao={"Id":"0","Status":"","IdCliente":"","DtLocacao":"","DtDevolucao":""}

--para chamar o Salvar de Locacao**********************************************************************************

--Atenção: Salvar Locação e após inserir os itens da Locação, pois um cliente pode alugar mais de 1 filme
--Retorno esperado =>"Locação Inserida!"
http://localhost:50040/Locacao/Salvar?Locacao={"Id":"0","Status":"0","IdCliente":"2","DtLocacao":"2020-06-30","DtDevolucao":"2020-07-02"}

--Itens da Locação
--Retorno Esperado => "Itens da Locação Inseridos"
http://localhost:50040/ItensLocacao/Salvar?ItensLocacao={"IdItem":"0","IdLocacao":"1","Status":"0","IdFilme":"2"}
http://localhost:50040/ItensLocacao/Salvar?ItensLocacao={"IdItem":"0","IdLocacao":"1","Status":"0","IdFilme":"3"}
--******************************************************************************************************************

-- Locação
--Retorno esperado =>"Locação Inserida!"
http://localhost:50040/Locacao/Salvar?Locacao={"Id":"0","Status":"0","IdCliente":"1","DtLocacao":"2020-06-30","DtDevolucao":"2020-07-02"}
--Itens da Locacao
--Retorno Esperado => "Itens da Locação Inseridos"
http://localhost:50040/ItensLocacao/Salvar?ItensLocacao={"IdItem":"0","IdLocacao":"2","Status":"0","IdFilme":"1"}



select * from Locacao
select * from ItensLocacao 

--para chamar o List de ItensLocacao
http://localhost:50040/ItensLocacao/Listar?ItensLocacao={"IdItem":"0","IdLocacao":"0","Status":"","IdFilme":""}

--****************************************************************************************************************

--*************************************TESTE DE BLOQUEIO/ALERTA************************************************************
--Inserir locação para Cliente com Pendencia
--Mensagem esperada => "F#Esse Cliente tem Locações Pendentes!!!"
http://localhost:50040/Locacao/Salvar?Locacao={"Id":"0","Status":"0","IdCliente":"1","DtLocacao":"2020-06-30","DtDevolucao":"2020-07-02"}


--***************************************************************************************************************
--Devolver uma Locacao
-- Mensagem esperada => {"retorno":"Devolvido em Dia"}
-- Status Locacao 0 (Pendente) 1 (Devolvido)
-- Status ItensLocacao 0 (Pendente) 1 (Devolvido)
http://localhost:50040/Locacao/Devolver?Locacao={"Id":"1","DtDevolucao":"2020-06-30"}
select Status from Locacao where id = 1
select Status from ItensLocacao where IdLocacao  = 1 

--**************************************************************************************************************
--Devolver uma Locacao em Atraso
--Mensagem esperada => {"retorno":"Devolvido com Atraso"}
update Locacao set status = 0, DtDevolucao = {d'2020-06-29'} where id = 1
update ItensLocacao set status =0 where IdLocacao = 1
http://localhost:50040/Locacao/Devolver?Locacao={"Id":"1","DtDevolucao":"2020-06-30"}


--**************************************************************************************************************
-- Teste Filme Indisponível/Locado
-- Locação
select * from Locacao 
select * from ItensLocacao 
--Loacao Inserida
http://localhost:50040/Locacao/Salvar?Locacao={"Id":"0","Status":"0","IdCliente":"3","DtLocacao":"2020-06-30","DtDevolucao":"2020-07-02"}
--Itens da Locacao
--Mensagem esperada=> "Itens da Locação Inseridos"
http://localhost:50040/ItensLocacao/Salvar?ItensLocacao={"IdItem":"0","IdLocacao":"3","Status":"0","IdFilme":"4"}
--MENSAGEM ESPERADA-> "F#Filme indisponível!!!"
http://localhost:50040/ItensLocacao/Salvar?ItensLocacao={"IdItem":"0","IdLocacao":"3","Status":"0","IdFilme":"1"}
--MENSAGEM ESPERADA-> ""Itens da Locação Inseridos"
http://localhost:50040/ItensLocacao/Salvar?ItensLocacao={"IdItem":"0","IdLocacao":"3","Status":"0","IdFilme":"2"}







