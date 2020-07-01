using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Locadora
{
    public class Conexao : IDisposable
    {
        public bool State { get; set; }
        public SqlCommand Cmd = new SqlCommand();
        public SqlDataReader Dr;
        SqlConnection con;
        StringBuilder query;

        /// <summary>michele, 27/08/2017 - método para fazer a abertura da conexão</summary>
        void AbreConexao(string conString)
        {
            if (con == null || con.State == ConnectionState.Closed)
            {
                con = new SqlConnection(conString);
                Cmd.Connection = con;
                Cmd.CommandTimeout = 15;//tempo para conexão local
                try { con.Open(); }
                catch (Exception ex) { throw new Exception($"Falha ao Conectar ao Banco de dados: {ex.Message}"); }
            }
        }

        /// <summary>michele, 27/08/2017 - construtor. A abertura da conexão é feita no momento em que é instanciado</summary>
        public Conexao(string conString)
        {
            AbreConexao(conString);
        }

        /// <summary>michele, 25/06/2014 - Facilita na criação de parâmetros para montar as Querys
        /// O parâmetro "trechoQuery" é opcional, mas se informado, cria o parâmetro e o trecho da Query que irá utilizá-lo</summary>
        public void Param(string parametro, object valor, string trechoQuery = null)
        {
            try
            {
                if (Cmd.Parameters.IndexOf(parametro) >= 0) Cmd.Parameters.RemoveAt(parametro);//caso já exista, será removido para evitar erros
                if (valor == null) valor = (string)string.Empty;
                if (valor is bool) valor = Convert.ToInt16(valor);
                if (valor is DateTime)
                {
                    if (!G1.IsDate(valor)) valor = DBNull.Value;
                    else valor = new DateTime
                        (Convert.ToDateTime(valor).Year, Convert.ToDateTime(valor).Month, Convert.ToDateTime(valor).Day,
                        Convert.ToDateTime(valor).Hour, Convert.ToDateTime(valor).Minute, Convert.ToDateTime(valor).Second);
                }
                Cmd.Parameters.AddWithValue(parametro, valor);
                if (!G1.Nada(trechoQuery)) Query(trechoQuery);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>coloca '' para montar o parâmetro quando for varchar, facilitando a montagem da query</summary>
        public string Str(string valor = "", bool semAcento = false)
        {
            try
            {
                if (G1.Nada(valor)) return "NULL";
                return $"'{(semAcento ? G1.RemoveAcentos(valor) : valor.Replace("'", ""))}'";
            }
            catch { return "NULL"; }
        }

        public string StrLike(string valor, bool semAcento = false)
        {
            try
            {
                return $"'%{(semAcento ? G1.RemoveAcentos(valor) : valor.Replace("'", "")).Replace("%", "")}%'";
            }
            catch { return "NULL"; }
        }


        /// <summary>retorna a data no formato inglês americano, sem separadores, para utilizar em querys do sqlserver. Caso tenha qualwuer falha, retorna NULL</summary>
        public string Data(DateTime data)
        {
            try
            {
                if (!G1.IsDate(data)) return "NULL";
                StringBuilder s = new StringBuilder(Convert.ToString(data.Year));
                s.Append(G1.NumeroMes(data));
                s.Append(G1.DiaDoMes(data));
                s.Append(" ");
                s.Append(G1.HoraDoDia(data));
                return $"'{Convert.ToString(s)}'";
            }
            catch { return "NULL"; }
        }

        /// <summary>michele, 25/06/2014 - Monta o StringBuider(query) de acordo com o parametro trecho.
        /// Para criar uma nova instância, defina o parâmetro novo com true</summary>
        public void Query(string trecho = null, bool novo = false)
        {
            if (query == null || novo) query = new StringBuilder();
            if (!G1.Nada(trecho)) query.Append(trecho);
        }


        /// <summary>michele, 14/12/2014 - coloca where ou and na query</summary>
        public string WhereAnd(string trechoQuery = null)
        {
            StringBuilder whereAnd = new StringBuilder();
            if (Convert.ToString(query).ToUpper().IndexOf(" WHERE ") < 0) whereAnd.Append(" Where ");
            else whereAnd.Append(" And ");
            if (trechoQuery != null) whereAnd.Append(trechoQuery);//concatena o trecho da query
            whereAnd.Append(" ");//coloca espaço no final da expressão para evitar erros
            return Convert.ToString(whereAnd);
        }

        /// <summary>michele, 14/12/2014 - coloca where ou and na query, ignorando as ocorrencias do where que podem ter havido na subquery</summary>
        public string WhereAndSubQuery(string trechoQuery = null)
        {
            StringBuilder whereAnd = new StringBuilder();
            string queryCompleta = Convert.ToString(query);
            int indiceUltimaSubQuery = queryCompleta.LastIndexOf(")");
            string queryIni = G1.GetSubstringLeft(queryCompleta, indiceUltimaSubQuery);
            string queryFim = queryCompleta.Replace(queryIni, "");
            if (queryFim.ToUpper().IndexOf(" WHERE ") < 0) whereAnd.Append(" Where ");
            else whereAnd.Append(" And ");
            if (trechoQuery != null) whereAnd.Append(trechoQuery);//concatena o trecho da query
            whereAnd.Append(" ");//coloca espaço no final da expressão para evitar erros
            return Convert.ToString(whereAnd);
        }



        public bool RegistroExiste(string nomeTabela,
            string nomeColuna = null, object valor = null,
            string nomeColuna2 = null, object valor2 = null,
            string nomeColuna3 = null, object valor3 = null)
        {
            try
            {
                bool existe = false;
                Query("Select Count(*) as Existe From ", true);
                Query(nomeTabela);
                if (!G1.Nada(nomeColuna) && !G1.Nada(Convert.ToString(valor)))
                {
                    Param("@v", valor);
                    Query($" Where {nomeColuna} = @v ");
                    if (!G1.Nada(nomeColuna2) && !G1.Nada(Convert.ToString(valor2)))
                    {
                        Param("@v2", valor2);
                        Query($" And {(nomeColuna2)} = @v2 ");
                        if (!G1.Nada(nomeColuna3) && !G1.Nada(Convert.ToString(valor3)))
                        {
                            Param("@v3", valor3);
                            Query($" And {nomeColuna3} = @v3");
                        }
                    }
                }
                Cmd.CommandText = Convert.ToString(query);
                SqlDataReader dr = Cmd.ExecuteReader();
                if (dr.Read()) existe = G1.GetBool(dr["Existe"]);
                dr.Close();
                dr.Dispose();
                Cmd.CommandText = null;
                query.Clear();
                return existe;
            }
            catch (Exception e) { throw e; }
        }



        public void ExecuteSql(string nomeDoMetodo = null, bool zerarQuery = true)
        {
            try
            {
                if (G1.Nada(Cmd.CommandText)) Cmd.CommandText = Convert.ToString(query);
                Cmd.ExecuteNonQuery();
                Cmd.CommandText = null;
                if (query != null && zerarQuery) query.Clear();
            }
            catch (Exception e)
            {
                Cmd.CommandText = null;
                query = null;
                if (!G1.Nada(nomeDoMetodo) && nomeDoMetodo.IndexOf(".AtualizaTabela") < 0) throw new Exception(nomeDoMetodo + " - " + e.Message, e.InnerException);//exceção será disparada somente para os comandos DML (Insert,Update,Delete e Select)
            }
        }

        public DataTable DtSql(string nomeDoMetodo = null)
        {
            try
            {
                Cmd.CommandText = Convert.ToString(query);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(Cmd);
                DataTable d = new DataTable();
                d.BeginLoadData();
                sqlAdapter.Fill(d);
                d.EndLoadData();
                Cmd.CommandText = null;
                return d;
            }
            catch (Exception e)
            {
                Cmd.CommandText = null;
                throw new Exception(nomeDoMetodo + ", " + e.Message);
            }
        }

        public int InsertReturnId(string nomeDoMetodo = null)
        {
            try
            {
                query.Append(";Select SCOPE_IDENTITY()");
                Cmd.CommandText = Convert.ToString(query);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(Cmd);
                DataTable d = new DataTable();
                d.BeginLoadData();
                sqlAdapter.Fill(d);
                d.EndLoadData();
                Cmd.CommandText = null;
                query.Clear();
                return G1.GetInt(d.Rows[0][0]);
            }
            catch (Exception e)
            {
                Cmd.CommandText = null;
                query = null;
                throw new Exception(nomeDoMetodo + " - " + e.Message, e.InnerException);
            }
        }

        public void Dispose()
        {
            if (con.State == ConnectionState.Open) con.Close();
        }
    }
}