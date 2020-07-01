using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Locadora
{
    public class G1
    {
        
        /// <summary>verifica se a data informada é válida</summary>
        public static bool IsDate(object data = null)
        {
            if (data == null) return false;
            DateTime dataIncorreta1 = Convert.ToDateTime("1753/01/01");
            DateTime dataIncorreta2 = Convert.ToDateTime("9999/12/31");
            DateTime date;
            bool isDate;
            try
            {
                date = DateTime.Parse(data.ToString());
                if (date >= dataIncorreta1 && date <= dataIncorreta2) isDate = true;
                else isDate = false;
            }
            catch (Exception) { isDate = false; }
            return isDate;
        }

        /// <summary>verifica se uma string é uma data válida, e caso seja, converte e atribui a uma variável</summary>
        public static DateTime ToDate(object data)
        {
            try
            {
                if (Len(data) >= 8 && Convert.ToString(data).IndexOf(" ") >= 0 && Convert.ToString(data).IndexOf("/") < 0)
                {
                    DateTime dataRetorno = new DateTime();
                    string strData = Convert.ToString(data).Replace(" ", "").Replace(":", "");
                    int ano = GetInt(strData.Substring(0, 4));
                    int mes = GetInt(strData.Substring(4, 2));
                    int dia = GetInt(strData.Substring(6, 2));
                    int hora = 0;
                    int minuto = 0;
                    if (Len(strData) >= 12)
                    {
                        hora = GetInt(strData.Substring(8, 2));
                        minuto = GetInt(strData.Substring(10, 2));
                    }
                    return dataRetorno;
                }
                else return Convert.ToDateTime(data);//caso tenha data e hora
            }
            catch (Exception) { return new DateTime(); }//caso haja qualquer tipo de falha de conversão, não fará nada
        }

        /// <summary>verifica e retorna a quantidade de caracteres de uma string</summary>
        public static int Len(object texto)
        {
            if (Nada(Convert.ToString(texto))) return 0;
            else return Convert.ToString(texto).Length;
        }

        /// <summary>retorna true se a string estiver vazia, ou false se tiver algum valor</summary>
        public static bool Nada(object expressao = null)
        {
            if (expressao == null || Convert.ToString(expressao) == "''") return true;
            if (Convert.ToString(expressao) == "\n") return false;
            return string.IsNullOrEmpty(Convert.ToString(expressao).Trim());
        }

        /// <summary>michele, 01/06/2014 - retorna um número inteiro</summary>
        public static int GetInt(object valor, bool isNullRetorna1Negativo = false)
        {
            try
            {
                if (G1.Nada(valor)) return isNullRetorna1Negativo ? -1 : 0;
                int numero = 0;
                var tipo = valor.GetType().ToString().ToLower().Replace("system.", "");
                if (tipo == "dbnull") return 0;
                if (valor is bool || tipo == "int16") numero = Convert.ToInt16(valor);
                else if (tipo == "int32" || tipo == "int64" || tipo == "double" || tipo == "decimal" || tipo == "float") numero = Convert.ToInt32(valor);
                else if (tipo == "string")
                {
                    string v = Convert.ToString(valor).ToLower();
                    if (Nada(v))
                    {
                        if (isNullRetorna1Negativo) return -1;
                        else return 0;
                    }
                    if (v == "sim" || v == "s" || v == "ativo" || v == "true") return 1;
                    else if (v == "não" || v == "n" || v == "inativo" || v == "false") return 0;
                    else numero = int.Parse(valor.ToString());
                }
                //if (numero  is int) numero = int.Parse(valor.ToString());
                if (numero >= -32768 && numero <= 32767) return Convert.ToInt16(numero);
                else return Convert.ToInt32(numero);
                //return numero;
            }
            catch (Exception)
            {
                return 0;//retorna 0 se algo der errado na conversão
            }
        }


        /// <summary>michele, 01/06/2014 - pega um substring à esquerda da palavra</summary>
        public static string GetSubstringLeft(object palavra, int fim)
        {
            string word = Convert.ToString(palavra);
            if (Len(word) >= fim) word = word.Substring(0, fim);
            if (Nada(word)) word = "";
            return word;
        }

        /// <summary>michele, 01/06/2014 - retorna true ou false</summary>
        public static bool GetBool(object valor)
        {
            bool retorno = false;
            try
            {
                if (valor is bool) retorno = Convert.ToBoolean(valor);
                else
                {
                    var val = Convert.ToString(valor).ToLower();
                    if (val == "true" || val == "sim" || val == "s" || GetInt(val) > 0) retorno = true;
                    if (val == "false" || val == "não" || val == "n" || GetInt(val) < 1) retorno = false;
                }
            }
            catch (Exception)
            {
                retorno = false;//se o parâmetro estiver vazio ou ocorrer algum erro na conversão
            }
            return retorno;
        }

        
        /// <summary>michele, 01/06/2014 - exclui um arquivo especificado</summary>
        public static bool DeleteFile(string arquivo)
        {
            try
            {
                new FileInfo(arquivo).Delete();
                return true;
            }
            catch (Exception) { return false; }
        }



        //retorna o número do mês de uma data
        public static string NumeroMes(DateTime data)
        {
            StringBuilder numMes = new StringBuilder();
            if (Len(data.Month) == 1) numMes.Append("0");
            numMes.Append(Convert.ToString(data.Month));
            return Convert.ToString(numMes);
        }

        //retorna o dia de uma data
        public static string DiaDoMes(DateTime data)
        {
            StringBuilder numDia = new StringBuilder();
            if (Len(data.Day) == 1) numDia.Append("0");
            numDia.Append(Convert.ToString(data.Day));
            return Convert.ToString(numDia);
        }

        //retorna hora, minuto e segundos de uma data
        public static string HoraDoDia(DateTime data, bool considerarSegundos = false)
        {
            StringBuilder s = new StringBuilder();
            if (Len(data.Hour) == 1) s.Append("0");
            s.Append(Convert.ToString(data.Hour));
            s.Append(":");
            if (Len(data.Minute) == 1) s.Append("0");
            s.Append(Convert.ToString(data.Minute));
            s.Append(":");
            if (!considerarSegundos) s.Append("00");
            else
            {
                if (Len(data.Second) == 1) s.Append("0");
                s.Append(Convert.ToString(data.Second));
            }
            return Convert.ToString(s);
        }

        /// <summary>michele, 01/06/2014 - verifica se o datatable possui dados</summary>
        public static bool DtOk(DataTable dt) { return dt != null && dt.Rows.Count > 0; }

        /// <summary>michele, 18/08/2015 - retorna o texto sem acentos</summary>
        public static string RemoveAcentos(string text = null)
        {
            if (G1.Nada(text)) return "";
            StringBuilder s = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark) s.Append(letter);
            }
            return Convert.ToString(s).Replace("’", "").Replace("'", "").Replace("–", "").Replace("'", "").Replace("Ç", "C").Replace("ç", "c").Replace("à", "a").Replace("À", "A").Replace("º", "").Replace("ª", "").Replace(" - ", "").Replace("-", "").Trim();
        }



        /// <summary>método para resolver problemas de data em formato incorreto e campos que não existirem no objeto que está sendo convertido para Json</summary>
        public static JsonSerializerSettings CfJson()
        {
            JsonSerializerSettings j = new JsonSerializerSettings();
            j.NullValueHandling = NullValueHandling.Ignore;
            j.MissingMemberHandling = MissingMemberHandling.Ignore;
            return j;
        }


        public static string DtToJson(DataTable d)
        {
            try { return JsonConvert.SerializeObject(d, CfJson()); }
            catch (Exception e) { throw new Exception(e.Message); }
        }
    }
}
