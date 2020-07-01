using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Locadora.Controllers
{
    public class LocacaoController : MasterController
    {
        public string Salvar(string locacao)//o parâmetro cliente, é uma string que virá na url do navegador, ou em uma requisição via ajax com javascript
        {
            try
            {
                Locacao l = JsonConvert.DeserializeObject<Locacao>(locacao, G1.CfJson());//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente
                bool jaExiste = new LocacaoDal().VerLocPendente(l, GetConString());
                if (jaExiste) throw new Exception("Esse Cliente tem Locações Pendentes!!!");
                new LocacaoDal().Salvar(l, GetConString());//salva os dados, e traz o Id que foi gerado dentro do objeto que foi passado como parâmetro, caso seja um novo cadastro
               // return JsonConvert.SerializeObject(l);//retorna o objeto, depois que foi salvo, com todos os dados
                return JsonConvert.SerializeObject("Locação Inserida!");
            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public string Devolver(string locacao)//o parâmetro cliente, é uma string que virá na url do navegador, ou em uma requisição via ajax com javascript
        {
            try
            {
                Locacao l = JsonConvert.DeserializeObject<Locacao>(locacao, G1.CfJson());//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente
                bool jaExiste = new LocacaoDal().ExisteLocacao(l, GetConString());
                if (jaExiste == false) throw new Exception("Esta Locação não está pendente!!!");
                bool emAtraso = new LocacaoDal().EmAtraso(l, GetConString());
                new LocacaoDal().Devolver(l, GetConString());
                return JsonConvert.SerializeObject(new { retorno = emAtraso ? "Devolvido com Atraso" : "Devolvido em Dia" });
            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }


        public async Task<string> Listar(string locacao = null)
        {
            try
            {
                Locacao l = !G1.Nada(locacao) ? JsonConvert.DeserializeObject<Locacao>(locacao, G1.CfJson()) : new Locacao();//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente



                List<Locacao> lista = await Task.FromResult(new LocacaoDal().Listar(l, GetConString()));
                return JsonConvert.SerializeObject(lista);
            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public ActionResult TesteApi() { return View("TesteApi"); }


    }
}