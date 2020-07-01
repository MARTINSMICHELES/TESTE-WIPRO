using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Locadora
{
    public class Filmes
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public int Status { get; set; }

        //Disponível
        //Indisponível
    }



    public class FilmesDal
    {
        public void Salvar(Filmes film, string conString)
        {
            using (Conexao c = new Conexao(conString))
            {
                if (film.Id == 0)
                {
                    c.Query($"INSERT INTO Filmes(Titulo,Genero, Status) VALUES({c.Str(film.Titulo)},{c.Str(film.Genero)},{film.Status})");
                    film.Id = c.InsertReturnId();
                }
                else
                {
                    c.Query($"Update Filmes Set Titulo = {c.Str(film.Titulo)}, Genero = {c.Str(film.Genero)}, Status = {(film.Status)} Where Id = {film.Id}");
                    c.ExecuteSql();
                }
            }
        }

        public List<Filmes> Listar(Filmes film, string conString)
        {
            List<Filmes> lista = new List<Filmes>();
            DataTable d;
            using (Conexao c = new Conexao(conString))
            {
                c.Query("Select Id,Titulo,Genero, Status From Filmes");
                c.Query(film.Id > 0 ? c.WhereAnd($"Id = {film.Id}") : "");
                c.Query(!G1.Nada(film.Titulo) ? c.WhereAnd($"Titulo Like {c.StrLike(film.Titulo)}") : "");
                c.Query(!G1.Nada(film.Genero) ? c.WhereAnd($"Genero = {c.Str(film.Genero)}") : "");
                c.Query(!G1.Nada(film.Status) ? c.WhereAnd($"Status = {film.Status}") : "");

                d = c.DtSql();
            }
            if (G1.DtOk(d)) lista = JsonConvert.DeserializeObject<List<Filmes>>(G1.DtToJson(d), G1.CfJson());
            return lista;
        }

        
    }
}