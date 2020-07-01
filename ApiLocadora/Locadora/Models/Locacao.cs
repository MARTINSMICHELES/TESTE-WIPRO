using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Locadora
{
    public class Locacao
    {
        public int Id { get; set; }
        public int Status { get; set; } //0-> Pendente(Alugado)  1-> Devolvido
        public Cliente Cliente = new Cliente();
        public int IdCliente { get; set; }
        public DateTime DtLocacao { get; set; }
        public DateTime DtDevolucao { get; set; }


    }


    public class LocacaoDal
    {
        public void Salvar(Locacao loc, string conString)
        {
            using (Conexao c = new Conexao(conString))
            {
                if (loc.Id == 0)
                {
                    c.Query($"INSERT INTO Locacao(Status, IdCliente,DtLocacao, DtDevolucao) VALUES({(loc.Status)}, {loc.IdCliente},{c.Data(loc.DtLocacao)},{c.Data(loc.DtDevolucao)})");
                    loc.Id = c.InsertReturnId();
                }
                else
                {
                    c.Query($"Update Locacao Set Status = {(loc.Status)}, IdCliente = {(loc.IdCliente)}, DtLocacao = {c.Data(loc.DtLocacao)} ,DtDevolucao ={c.Data(loc.DtDevolucao)}  Where Id = {loc.Id}");
                    c.ExecuteSql();
                }
            }
        }


        public bool ExisteLocacao(Locacao loc, string conString)
        {
            try
            {
                using (Conexao c = new Conexao(conString))
                {
                    return c.RegistroExiste("Locacao", "Id", loc.Id, "Status", "0");
                }
            }
            catch (Exception e)
            {
                throw
                new Exception("Esta Locação não está Pendente!\n" + e.Message);
            }
        }
        public void Devolver(Locacao loc, string conString)
        {
            try
            {
                using (Conexao c = new Conexao(conString))
                {

                    c.Query($"Update Locacao Set Status = 1,DtDevolucao ={c.Data(loc.DtDevolucao)} Where Id = {loc.Id}");
                    c.ExecuteSql();
                    c.Query($"Update ItensLocacao Set Status = 1 Where IdLocacao = {loc.Id}");
                    c.ExecuteSql();

                }

            }
            catch (Exception)
            {

                throw
                new Exception("Locação não cadastrada!\n");
            }

        }



        public List<Locacao> Listar(Locacao loc, string conString)
        {
            List<Locacao> lista = new List<Locacao>();
            DataTable d;
            using (Conexao c = new Conexao(conString))
            {
                c.Query("Select Id, Status, IdCliente, DtLocacao, DtDevolucao From Locacao");
                c.Query(loc.Id > 0 ? c.WhereAnd($"Id = {loc.Id}") : "");
                c.Query(loc.Status > 0 ? c.WhereAnd($"Status = {loc.Status}") : "");
                c.Query(loc.IdCliente > 0 ? c.WhereAnd($"IdCliente = {loc.IdCliente}") : "");
                c.Query(G1.IsDate(loc.DtLocacao) ? c.WhereAnd($"DtLocacao = {c.Data(loc.DtLocacao)}") : "");
                c.Query(G1.IsDate(loc.DtDevolucao) ? c.WhereAnd($"DtDevolucao = {c.Data(loc.DtDevolucao)}") : "");

                d = c.DtSql();
            }
            if (G1.DtOk(d)) lista = JsonConvert.DeserializeObject<List<Locacao>>(G1.DtToJson(d), G1.CfJson());
            return lista;
        }


        //Veficia se tem Locacao em Aberta deste cliente
        public bool VerLocPendente(Locacao loc, string conString)
        {
            try
            {
                using (Conexao c = new Conexao(conString))
                {
                    return c.RegistroExiste("Locacao", "IdCliente", loc.IdCliente, "Status", "0");
                }
            }
            catch (Exception e)
            {
                throw
                new Exception("Cliente possui Locações Pendentes\n" + e.Message);
            }
        }
        
        //Verifica Atraso
        public bool EmAtraso(Locacao loc, string conString)
        {
            try
            {
                using (Conexao c = new Conexao(conString))
                {
                    c.Query($"Select count(*) From Locacao Where Id = {loc.Id} And Status = 0 And Convert(Date,DtDevolucao,120) < convert(date,getdate(),120)");
                    return G1.GetInt(c.DtSql().Rows[0][0]) > 0;
                }
            }
            catch (Exception e)
            {
                throw
                new Exception("Erro Locação em Atraso\n" + e.Message);
            }
        }

    }
}



