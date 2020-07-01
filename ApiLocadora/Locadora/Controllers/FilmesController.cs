using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Locadora.Controllers
{
    public class FilmesController : MasterController
    {
        public string Salvar(string filmes)//o parâmetro filmes, é uma string que virá na url do navegador, ou em uma requisição via ajax com javascript
        {
            try
            {
                Filmes f = JsonConvert.DeserializeObject<Filmes>(filmes, G1.CfJson());//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente
                new FilmesDal().Salvar(f, GetConString());//salva os dados, e traz o Id que foi gerado dentro do objeto que foi passado como parâmetro, caso seja um novo cadastro
                //return JsonConvert.SerializeObject(f);//retorna o objeto, depois que foi salvo, com todos os dados
                return JsonConvert.SerializeObject("Filme Cadastrado");

            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public async Task<string> Listar(string filmes = null)
        {
            try
            {
                Filmes f = !G1.Nada(filmes) ? JsonConvert.DeserializeObject<Filmes>(filmes, G1.CfJson()) : new Filmes();//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente
                List<Filmes> lista = await Task.FromResult(new FilmesDal().Listar(f, GetConString()));
                return JsonConvert.SerializeObject(lista);
            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public ActionResult TesteApi() { return View("TesteApi"); }

    }

}