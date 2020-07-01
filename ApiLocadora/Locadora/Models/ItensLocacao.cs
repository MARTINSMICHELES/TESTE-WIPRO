using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Locadora
{
    public class ItensLocacao
    {
        public int IdItem { get; set; } 
        public int IdLocacao { get; set; }
        public int Status { get; set; } //0 -> Pendente(Locado) 1-> Devolvido
        public Filmes Filmes = new Filmes();
        public int IdFilme { get; set; }

    }


    public class ItensLocacaoDal
    {
        public void Salvar(ItensLocacao iloc, string conString)
        {
            using (Conexao c = new Conexao(conString))
            {
                if (iloc.IdItem == 0)
                {
                    c.Query($"INSERT INTO ItensLocacao(IdLocacao, Status, IdFilme) VALUES({(iloc.IdLocacao)}, {(iloc.Status)}, {iloc.IdFilme})");
                    iloc.IdLocacao = c.InsertReturnId();
                }
                else
                {
                    c.Query($"Update ItensLocacao Set IdLocacao =  {(iloc.IdLocacao )},   Status = {(iloc.Status)}, IdFilme = {(iloc.IdFilme)} Where Id = {iloc.IdLocacao}");
                    c.ExecuteSql();
                }
            }
        }

        public List<ItensLocacao> Listar(ItensLocacao iloc, string conString)
        {
            List<ItensLocacao> lista = new List<ItensLocacao>();
            DataTable d;
            using (Conexao c = new Conexao(conString))
            {
                c.Query("Select IdItem, IdLocacao, Status, IdFilme From ItensLocacao");
                c.Query(iloc.IdItem > 0 ? c.WhereAnd($"IdItem = {iloc.IdItem}") : "");
                c.Query(!G1.Nada(iloc.IdLocacao) ? c.WhereAnd($"IdLocacao Like {(iloc.IdLocacao)}") : "");
                c.Query(!G1.Nada(iloc.Status) ? c.WhereAnd($"Status Like {(iloc.Status)}") : "");
                c.Query(!G1.Nada(iloc.IdFilme) ? c.WhereAnd($"IdFilme = {(iloc.IdFilme)}") : "");
                d = c.DtSql();
            }
            if (G1.DtOk(d)) lista = JsonConvert.DeserializeObject<List<ItensLocacao>>(G1.DtToJson(d), G1.CfJson());
            return lista;
        }

        public bool FilmeDisponivel(ItensLocacao film, string conString)
        {
            try
            {
                using (Conexao c = new Conexao(conString))
                {
                    //Status =0 (Disponível) 
                    //Status = 1 (Alugado)
                    // Se estiver alugado(indisponível=1) não deixa inserir na tabela ItensLocacao
                    return c.RegistroExiste("ItensLocacao", "IdFilme", film.IdFilme, "Status", 0);
                }
            }
            catch (Exception e)
            {
                throw
                new Exception("|Erro!\n" + e.Message);
            }

        }
    }

}