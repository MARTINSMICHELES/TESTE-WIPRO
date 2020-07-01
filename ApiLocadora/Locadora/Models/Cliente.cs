using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Locadora
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
    }

    public class ClienteDal
    {
        public void Salvar(Cliente cli, string conString)
        {
            using (Conexao c = new Conexao(conString))
            {
                if (cli.Id == 0)
                {
                    c.Query($"INSERT INTO Cliente(Nome,Cpf) VALUES({c.Str(cli.Nome)},{c.Str(cli.Cpf)})");
                    cli.Id = c.InsertReturnId();
                }
                else
                {
                    c.Query($"Update Cliente Set Nome = {c.Str(cli.Nome)}, Cpf = {c.Str(cli.Cpf)} Where Id = {cli.Id}");
                    c.ExecuteSql();
                }
            }
        }


        public List<Cliente> Listar(Cliente cli, string conString)
        {
            List<Cliente> lista = new List<Cliente>();
            DataTable d;
            using (Conexao c = new Conexao(conString))
            {
                c.Query("Select Id,Nome,Cpf From Cliente");
                c.Query(cli.Id > 0 ? c.WhereAnd($"Id = {cli.Id}") : "");
                c.Query(!G1.Nada(cli.Nome) ? c.WhereAnd($"Nome Like {c.StrLike(cli.Nome)}") : "");
                c.Query(!G1.Nada(cli.Cpf) ? c.WhereAnd($"Cpf = {c.Str(cli.Cpf)}") : "");
                d = c.DtSql();
            }
            if (G1.DtOk(d)) lista = JsonConvert.DeserializeObject<List<Cliente>>(G1.DtToJson(d), G1.CfJson());
            return lista;
        }

        public bool CPFCadastrado(Cliente cli, string conString)
        {
            try
            {
                using (Conexao c = new Conexao(conString))
                {
                    return c.RegistroExiste("Cliente", "Cpf", cli.Cpf);
                }
            }
            catch (Exception e)
            {
                throw
                new Exception("Este CPF já está cadastrado!\n" + e.Message);
            }

        }

    }



}