/*1. Com base no modelo acima, escreva um comando SQL que liste a quantidade de registros por
Status com sua descrição.
*/

--Modo 1
select qtt_Registros = count(*), Descricao=(select dsStatus from tb_Status ts where ts.idStatus = tb_Processo.idStatus  ) from tb_Processo group by idStatus    
--Modo 2
select qtt_Registros = count(*), Descricao = ts.dsStatus 
 from tb_Processo tb left outer join tb_Status ts 
 on ts.idStatus = tb.idStatus   group by ts.dsStatus   


/*2. Com base no modelo acima, construa um comando SQL que liste a maior data de andamento
por número de processo, com processos encerrados no ano de 2013
*/

select tp.nroProcesso, Maior_dt_andamento = max(ta.dtAndamento)  from tb_Processo tp left outer join tb_Andamento ta on
tp.IdProcesso = ta.idProcesso 
 where year(tp.DtEncerramento) = 2013 
 group by tp.nroProcesso



 /*3 Com base no modelo acima, construa um comando SQL que liste a quantidade de Data de
Encerramento agrupada por ela mesma com a quantidade da contagem seja maior que 5.
*/

SELECT DtEncerramento,  Count(DtEncerramento ) AS tb_Processo
FROM tb_Processo
GROUP BY  DtEncerramento
having Count(*)>5


/*4. Possuímos um número de identificação do processo, onde o mesmo contem 12 caracteres
com zero à esquerda, contudo nosso modelo e dados ele é apresentado como bigint. Como
fazer para apresenta-lo com 12 caracteres considerando os zeros a esquerda?
*/

SELECT nroProcesso=REPLICATE('0',12-LEN(RTRIM(nroProcesso))) + RTRIM(nroProcesso) from tb_Processo


